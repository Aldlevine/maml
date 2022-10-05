using Maml.Events;
using Maml.Math;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;

using static Maml.Utils.Bits;
using static Windows.Win32.PInvoke;

namespace Maml;

public partial class Window
{
	#region Abstract
	public override RenderTarget? RenderTarget
	{
		get => ImmediateMode switch
		{
			true => immediateRenderTarget,
			false => syncRenderTarget,
		};
	}

	protected override Vector2 GetSize()
	{
		GetClientRect(hWnd, out var rc);
		return new Vector2(rc.right - rc.left, rc.bottom - rc.top);
	}

	public override double DpiRatio => GetDpiForWindow(hWnd) * stdDpiInv;

	public override event EventHandler<ResizeEvent>? Resize;
	public override event EventHandler<PointerEvent>? PointerMove;
	public override event EventHandler<PointerEvent>? PointerDown;
	public override event EventHandler<PointerEvent>? PointerUp;
	public override event EventHandler<WheelEvent>? Wheel;
	public override event EventHandler<KeyEvent>? KeyDown;
	public override event EventHandler<KeyEvent>? KeyUp;
	public override event EventHandler<FocusEvent>? Focus;
	public override event EventHandler<FocusEvent>? Blur;
	public override event EventHandler<DrawEvent>? Draw;
	#endregion

	#region Internal
	private const string className = "MamlWindow";
	private const string windowName = "Maml";
	private const int stdDpi = 96;
	private const double stdDpiInv = 1.0 / (double)stdDpi;

	internal HWND hWnd;
	private Rect clientRect;

	internal bool ImmediateMode = false;
	private RenderTarget? syncRenderTarget;
	private RenderTarget? immediateRenderTarget;

	unsafe internal static void RegisterWindowClass()
	{
		fixed (char* pClassName = className)
		{
			PCWSTR szNull = default;
			PCWSTR szClassName = new(pClassName);
			PCWSTR szCursorName = new((char*)IDC_ARROW);
			PCWSTR szIconName = new((char*)IDI_APPLICATION);
			WNDCLASSEXW wcex = new()
			{
				style = WNDCLASS_STYLES.CS_HREDRAW | WNDCLASS_STYLES.CS_VREDRAW,
				lpfnWndProc = (delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, LRESULT>)Marshal.GetFunctionPointerForDelegate(WndProcDelegate),
				cbClsExtra = 0,
				cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
				cbWndExtra = 0,
				hInstance = GetModuleHandle(szNull),
				hbrBackground = new HBRUSH(new IntPtr(6)),
				lpszMenuName = szNull,
				lpszClassName = szClassName,
			};

			if (RegisterClassEx(wcex) == 0)
			{
				var err = Marshal.GetLastWin32Error();
				throw new Exception("RegisterClass Failed: " + err);
			}
		}
	}

	unsafe internal Window() : base()
	{
		hWnd = CreateWindowEx(
			0,
			className,
			windowName,
			WINDOW_STYLE.WS_BORDER | WINDOW_STYLE.WS_CAPTION | WINDOW_STYLE.WS_SYSMENU | WINDOW_STYLE.WS_MAXIMIZEBOX | WINDOW_STYLE.WS_MINIMIZEBOX | WINDOW_STYLE.WS_SIZEBOX,
			CW_USEDEFAULT,
			CW_USEDEFAULT,
			CW_USEDEFAULT,
			CW_USEDEFAULT,
			default,
			default,
			default,
			null);

		// BOOL dm = true;
		// DwmSetWindowAttribute(hWnd, Windows.Win32.Graphics.Dwm.DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, &dm, (uint)sizeof(BOOL));
		// COLORREF borderColor = new(0xffffffff);
		// DwmSetWindowAttribute(hWnd, Windows.Win32.Graphics.Dwm.DWMWINDOWATTRIBUTE.DWMWA_BORDER_COLOR, &borderColor, (uint)sizeof(COLORREF));
		// uint borderThickness = 2;
		// DwmSetWindowAttribute(hWnd, Windows.Win32.Graphics.Dwm.DWMWINDOWATTRIBUTE.DWMWA_VISIBLE_FRAME_BORDER_THICKNESS, &borderThickness, (uint)sizeof(uint));

		if (hWnd == HWND.Null)
		{
			var err = Marshal.GetLastWin32Error();
			throw new Exception("CreateWindow Failed " + err);
		}

		SetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWLP_USERDATA, windowID);

		SetWindowPos(
			hWnd,
			HWND.Null,
			0,
			0,
			(int)System.Math.Ceiling(1440.0),
			(int)System.Math.Ceiling(810.0),
			SET_WINDOW_POS_FLAGS.SWP_NOMOVE);
		ShowWindow(hWnd, SHOW_WINDOW_CMD.SW_NORMAL);
		UpdateWindow(hWnd);
	}

	unsafe private void CreateRenderTarget(ID2D1HwndRenderTarget** ppRenderTarget, D2D1_PRESENT_OPTIONS presentMode)
	{
		var dpi = GetDpiForWindow(hWnd);
		D2D1_RENDER_TARGET_PROPERTIES renderTargetProps = new()
		{
			dpiX = dpi,
			dpiY = dpi,
		};

		D2D1_HWND_RENDER_TARGET_PROPERTIES hWndRenderTargetProps = new()
		{
			hwnd = hWnd,
			pixelSize = Size.ToD2DSizeU(),
			presentOptions = presentMode,
		};

		Engine.Singleton.pD2DFactory->CreateHwndRenderTarget(
			in renderTargetProps,
			in hWndRenderTargetProps,
			ppRenderTarget).ThrowOnFailure();
	}

	unsafe private void CreateDeviceResources()
	{
		if (immediateRenderTarget == null)
		{
			ID2D1HwndRenderTarget* pRenderTarget;
			CreateRenderTarget(&pRenderTarget, D2D1_PRESENT_OPTIONS.D2D1_PRESENT_OPTIONS_IMMEDIATELY);
			immediateRenderTarget = new RenderTarget((ID2D1RenderTarget*)pRenderTarget);
		}

		if (syncRenderTarget == null)
		{
			ID2D1HwndRenderTarget* pRenderTarget;
			CreateRenderTarget(&pRenderTarget, D2D1_PRESENT_OPTIONS.D2D1_PRESENT_OPTIONS_NONE);
			syncRenderTarget = new RenderTarget((ID2D1RenderTarget*)pRenderTarget);
		}
	}

	private void DiscardDeviceResources()
	{
		immediateRenderTarget?.Dispose();
		syncRenderTarget?.Dispose();

		// Somehow need to notify Geometries and Brushes to release their stuff...
	}

	// TODO: Need to abstract out the window update stuff
	internal void Redraw(bool forceUpdate)
	{
		InvalidateRect(hWnd, (RECT?)null, false);
		if (forceUpdate)
		{
			UpdateWindow(hWnd);
		}
	}


	unsafe internal void HandleDraw()
	{
		lock (Engine.Singleton.EventMutex)
		{
			CreateDeviceResources();

			RenderTarget!.pRenderTarget->BeginDraw();

			SceneTree.Draw(RenderTarget);

			var hr = RenderTarget!.pRenderTarget->EndDraw();
			if (hr == HRESULT.D2DERR_RECREATE_TARGET)
			{
				DiscardDeviceResources();
			}
			else
			{
				hr.ThrowOnFailure();
			}
		}
	}

	unsafe internal void HandleResize()
	{
		lock (Engine.Singleton.EventMutex)
		{
			float dpi = GetDpiForWindow(hWnd);
			if (syncRenderTarget != null)
			{
				((ID2D1HwndRenderTarget*)syncRenderTarget.pRenderTarget)->Resize(Size.ToD2DSizeU()).ThrowOnFailure();
				((ID2D1HwndRenderTarget*)syncRenderTarget.pRenderTarget)->SetDpi(dpi, dpi);
			}
			if (immediateRenderTarget != null)
			{
				((ID2D1HwndRenderTarget*)immediateRenderTarget.pRenderTarget)->Resize(Size.ToD2DSizeU()).ThrowOnFailure();
				((ID2D1HwndRenderTarget*)immediateRenderTarget.pRenderTarget)->SetDpi(dpi, dpi);
			}

			Resize?.Invoke(this, new ResizeEvent { Size = Size });
			SizeProperty[this].SetDirty();
		}
	}

	private (LRESULT result, bool wasHandled) HandleMessage(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam)
	{
		LRESULT result = new(0);
		bool wasHandled = false;

		switch (msg)
		{
			case WM_SETCURSOR:
				{
					if (LoWord(lParam) == HTCLIENT)
					{
						SetCursor(LoadCursor(default, IDI_APPLICATION));
						// SetCursor(LoadCursor(default(HINSTANCE), default));
						wasHandled = true;
					}
				}
				break;

			case WM_DPICHANGED:
				{
					HandleResize();
				}
				wasHandled = true;
				break;

			case WM_WINDOWPOSCHANGED:
				{
					WINDOWINFO pwi = default;
					if (GetWindowInfo(hWnd, ref pwi))
					{
						var oldRect = clientRect;
						clientRect = new Rect(pwi.rcClient);
						if (oldRect.Size != clientRect.Size)
						{
							HandleResize();
						}
					}
				}
				wasHandled = true;
				break;

			case WM_ENTERSIZEMOVE:
				{
					Engine.Singleton.Animator.StartTicker();
				}
				wasHandled = true;
				break;

			case WM_EXITSIZEMOVE:
				{
					Engine.Singleton.Animator.StopTicker();
				}
				wasHandled = true;
				break;

			case WM_DISPLAYCHANGE:
				{
					InvalidateRect(hWnd, (RECT?)null, false);
				}
				wasHandled = true;
				break;

			case WM_ERASEBKGND:
				wasHandled = true;
				result = new(1);
				break;

			case WM_PAINT:
				{
					HandleDraw();
					ValidateRect(hWnd, (RECT?)null);
				}
				wasHandled = true;
				break;

			case WM_DESTROY:
				{
					PostQuitMessage(0);
				}
				result = new(1);
				wasHandled = true;
				break;

			case WM_POINTERUPDATE:
			case WM_POINTERDOWN:
			case WM_POINTERUP:
				{
					ImmediateMode = true;
					HandlePointer(wParam, lParam);
					Redraw(true);
					ImmediateMode = false;
				}
				wasHandled = true;
				break;
		}

		return (result, wasHandled);
	}

	private delegate LRESULT WNDPROC(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam);
	private static readonly WNDPROC WndProcDelegate = WndProc;

	private static LRESULT WndProc(
		HWND hWnd,
		uint msg,
		WPARAM wParam,
		LPARAM lParam)
	{
		if (msg == WM_CREATE)
		{
			return new(1);
		}

		var windowId = GetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWLP_USERDATA);
		var window = GetWindow(windowId);

		(LRESULT result, bool wasHandled) = window?.HandleMessage(hWnd, msg, wParam, lParam) ?? (new(0), false);

		if (!wasHandled)
		{
			result = DefWindowProc(hWnd, msg, wParam, lParam);
		}

		return result;
	}

	#endregion
}

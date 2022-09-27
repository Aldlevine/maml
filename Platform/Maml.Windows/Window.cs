using Maml.Events;
using Maml.Graphics;
using Maml.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;

using static Maml.Utils.Bits;
using static Windows.Win32.PInvoke;

namespace Maml;

public class Window : WindowBase<RenderTarget>
{
	#region Abstract
	public override RenderTarget RenderTarget { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }

	public override Vector2 Size
	{
		get
		{
			GetClientRect(hWnd, out var rc);
			return new(rc.right - rc.left, rc.bottom - rc.top);
		}
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
	#endregion

	#region Internal
	private const string className = "MamlMainWindow";
	private const int stdDpi = 96;
	private const double stdDpiInv = 1.0 / (double)stdDpi;

	private static int CurrentWindowID { get; set; } = 0;
	private static readonly Dictionary<int, Window> Windows = new();
	private static Window GetWindow(int id) => Windows[id];
	private static void RegisterWindow(Window window) => Windows[window.id] = window;

	internal HWND hWnd;
	private Rect clientRect;
	private int id = CurrentWindowID++;

	unsafe internal static void RegisterWindowClass()
	{
		fixed (char* pClassName = className)
		{
			PCWSTR szNull = default;
			PCWSTR szClassName = new(pClassName);
			// PCWSTR szWindowName = new(pWindowName);
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
				// hCursor = LoadCursor(GetModuleHandle(szNull), IDI_APPLICATION),
				lpszClassName = szClassName,
			};

			if (RegisterClassEx(wcex) == 0)
			{
				var err = Marshal.GetLastWin32Error();
				throw new Exception("RegisterClass Failed: " + err);
			}
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
					// Viewport.HandleResize();
					RenderTarget.HandleResize();
				}
				wasHandled = true;
				break;

			case WM_WINDOWPOSCHANGED:
				{
					WINDOWINFO pwi = default;
					if (GetWindowInfo(hWnd, ref pwi))
					{
						// windowPosition = new(pwi.rcClient.left, pwi.rcClient.top);
						clientRect = new Rect(pwi.rcClient);
					}
					// Viewport.HandleResize();
					RenderTarget.HandleResize();
				}
				wasHandled = true;
				break;

			case WM_ENTERSIZEMOVE:
				{
					// Animator.StartTicker();
				}
				wasHandled = true;
				break;

			case WM_EXITSIZEMOVE:
				{
					// Animator.StopTicker();
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
					// Viewport.HandleDraw();
					RenderTarget.HandleDraw();
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
					// Viewport.ImmediateMode = true;
					RenderTarget.ImmediateMode = true;
					Input.HandlePointer(wParam, lParam);
					// Viewport.Redraw(true);
					RenderTarget.Redraw(true);
					// Viewport.ImmediateMode = false;
					RenderTarget.ImmediateMode = false;
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


public class RenderTarget : RenderTargetBase
{
	#region Abstract
	public override event EventHandler<DrawEvent>? Draw;

	public override void Clear(Color color) => throw new NotImplementedException();
	public override void DrawGeometry(Geometry geometry, Fill fill) => throw new NotImplementedException();
	public override void DrawGeometry(Geometry geometry, Stroke stroke) => throw new NotImplementedException();
	public override Transform GetTransform() => throw new NotImplementedException();
	public override void SetTransform(Transform transform) => throw new NotImplementedException();
	#endregion

	#region Internal
	internal bool ImmediateMode = false;

	internal void Redraw(bool forceUpdate)
	{

	}

	internal void HandleDraw()
	{

	}

	internal void HandleResize()
	{

	}
	#endregion
}

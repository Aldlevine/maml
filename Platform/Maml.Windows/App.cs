﻿using Maml.Graphics;
using Maml.Math;
using Maml.UserInput;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;
using static Maml.Utils.Bits;
using static Windows.Win32.PInvoke;

[assembly: MetadataUpdateHandler(typeof(Maml.App))]

namespace Maml;

unsafe internal class App
{
	#region public

	public int ID;
	public HWND hWnd;

	public App()
	{
		ID = RegisterApp(this);
		Init();
	}

	~App()
	{
		UnregisterApp(this);

		if (pD2DFactory != null)
		{
			pD2DFactory->Release();
			pD2DFactory = null;
		}

	}

	public void RunMessageLoop()
	{
		BOOL result;
		while (result = GetMessage(out MSG msg, default, 0, 0))
		{
			if (result == -1)
			{
				throw new Exception("GetMessage Failed " + Marshal.GetLastWin32Error());
			}
			else
			{
				TranslateMessage(in msg);
				DispatchMessage(in msg);
			}
		}
	}

	public void UpdateApplication()
	{
		// DiscardDeviceResources();
		InvalidateRect(hWnd, (RECT?)null, true);
	}

	#endregion public

	#region private

	private ID2D1Factory* pD2DFactory;

	private const string className = "MamlWindow";
	private const string windowName = "Maml";

	internal Vector2 windowPosition = Vector2.Zero;
	public Viewport Viewport { get; private set; }

	private void Init()
	{
		CreateDeviceIndependentResources();
		int err = 0;

		SetProcessDpiAwareness(Windows.Win32.UI.HiDpi.PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE);

		fixed (char* pClassName = className, pWindowName = windowName)
		{
			PCWSTR szNull = default;
			PCWSTR szClassName = new(pClassName);
			PCWSTR szWindowName = new(pWindowName);
			PCWSTR szCursorName = new((char*)IDC_ARROW);
			PCWSTR szIconName = new((char*)IDI_APPLICATION);
			WNDCLASSEXW wcex = new()
			{
				style = WNDCLASS_STYLES.CS_HREDRAW | WNDCLASS_STYLES.CS_VREDRAW,
				// lpfnWndProc = &WndProcDelegate,
				lpfnWndProc = (delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, LRESULT>)Marshal.GetFunctionPointerForDelegate(WndProcDelegate),
				cbClsExtra = 0,
				cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
				cbWndExtra = 0,
				hInstance = GetModuleHandle(szNull),
				hbrBackground = new HBRUSH(new IntPtr(6)),
				lpszMenuName = szNull,
				hCursor = LoadCursor(GetModuleHandle(szNull), IDI_APPLICATION),
				lpszClassName = szClassName,
			};

			if (RegisterClassEx(wcex) == 0)
			{
				err = Marshal.GetLastWin32Error();
				throw new Exception("RegisterClass Failed: " + err);
			}

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

			if (hWnd == HWND.Null)
			{
				err = Marshal.GetLastWin32Error();
				throw new Exception("CreateWindow Failed " + err);
			}


			Viewport = new()
			{
				hWnd = hWnd,
				pD2DFactory = pD2DFactory,
			};

			SetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWLP_USERDATA, ID);

			SetWindowPos(
				hWnd,
				HWND.Null,
				0,
				0,
				(int)System.Math.Ceiling(640 * Viewport.DpiRatio),
				(int)System.Math.Ceiling(480 * Viewport.DpiRatio),
				SET_WINDOW_POS_FLAGS.SWP_NOMOVE);
			ShowWindow(hWnd, SHOW_WINDOW_CMD.SW_NORMAL);
			UpdateWindow(hWnd);
		}

	}
	private void CreateDeviceIndependentResources()
	{
		D2D1CreateFactory(D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_MULTI_THREADED, Marshal.GenerateGuidForType(typeof(ID2D1Factory)), null, out var obj).ThrowOnFailure();
		pD2DFactory = (ID2D1Factory*)obj;
	}

	private void OnMove(int x, int y)
	{
		windowPosition = new(x, y);
	}

	#endregion private
	#region private static

	private delegate LRESULT WNDPROC(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam);
	private static readonly WNDPROC WndProcDelegate = WndProc;

	private static int appId = 0;
	private static readonly Dictionary<int, WeakReference<App>> appRegistry = new();

	private static int RegisterApp(App app)
	{
		appRegistry.Add(appId, new WeakReference<App>(app));
		return appId++;
	}

	private static void UnregisterApp(App app)
	{
		appRegistry.Remove(app.ID);
	}

	private static App GetApp(int appId)
	{
		var wApp = appRegistry[appId];
		if (wApp.TryGetTarget(out var app))
		{
			return app;
		}
		throw new Exception("GetApp Failed");
	}

	internal Vector2 pointerPosition = new Vector2();

	private static LRESULT WndProc(
		HWND hWnd,
		uint msg,
		WPARAM wParam,
		LPARAM lParam)
	{
		LRESULT result = new(0);

		if (msg == WM_CREATE)
		{
			result = new(1);
		}
		else
		{
			var appId = GetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWLP_USERDATA);
			var app = GetApp(appId);

			bool wasHandled = false;

			if (app != null)
			{
				switch (msg)
				{
					case WM_SIZE:
						{
							int width = LoWord(lParam);
							int height = HiWord(lParam);
							app.Viewport.HandleResize(width, height);
						}
						wasHandled = true;
						break;

					case WM_MOVE:
						{
							int x = LoWord(lParam);
							int y = HiWord(lParam);
							app.OnMove(x, y);
						}
						wasHandled = true;
						break;

					case WM_DISPLAYCHANGE:
						{
							InvalidateRect(hWnd, (RECT?)null, false);
						}
						wasHandled = true;
						break;

					case WM_PAINT:
						{
							app.Viewport.HandleDraw();
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
						{
							Input.HandlePointerMove(wParam, lParam);
							app.Viewport.Redraw();
						}
						result = new(0);
						wasHandled = true;
						break;
				}
			}

			if (!wasHandled)
			{
				result = DefWindowProc(hWnd, msg, wParam, lParam);
			}
		}

		return result;
	}


	#endregion private static
}

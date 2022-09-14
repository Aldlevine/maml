using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Direct2D.Common;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;

[assembly: MetadataUpdateHandler(typeof(Maml.HotReloadManager))]

namespace Maml;

internal static class HotReloadManager
{
    public static void ClearCache(Type[]? types)
    {
        _ = types;
    }

    public static void UpdateApplication(Type[]? types)
    {
        _ = types;
        Program.App?.HotReload();
        Console.WriteLine("Update Application");
    }
}

unsafe internal class App
{
    #region public

    public int ID;
    public HWND hWnd;

    public App()
    {
        ID = RegisterApp(this);
        Initialize();
    }

    ~App()
    {
        UnregisterApp(this);

        if (pD2DFactory != null)
        {
            pD2DFactory->Release();
            pD2DFactory = null;
        }

        DiscardDeviceResources();

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

    public void HotReload()
    {
        DiscardDeviceResources();
        InvalidateRect(hWnd, (RECT?)null, true);
    }

    #endregion public

    #region private

    private ID2D1Factory* pD2DFactory;
    private ID2D1HwndRenderTarget* pRenderTarget;
    private ID2D1SolidColorBrush* pLightSlateGrayBrush;
    private ID2D1SolidColorBrush* pCornflowerBlueBrush;

    private const string className = "MamlWindow";
    private const string windowName = "Maml";

    private void Initialize()
    {
        CreateDeviceIndependentResources();
        int err = 0;

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

            SetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWLP_USERDATA, ID);

            double dpi = GetDpiForWindow(hWnd);

            SetWindowPos(
                hWnd,
                HWND.Null,
                0,
                0,
                (int)Math.Ceiling(640 * dpi / 96),
                (int)Math.Ceiling(480 * dpi / 96),
                SET_WINDOW_POS_FLAGS.SWP_NOMOVE);
            ShowWindow(hWnd, SHOW_WINDOW_CMD.SW_NORMAL);
            UpdateWindow(hWnd);
        }

    }


    private void CreateDeviceIndependentResources()
    {
        D2D1CreateFactory(D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_SINGLE_THREADED, Marshal.GenerateGuidForType(typeof(ID2D1Factory)), null, out var obj).ThrowOnFailure();
        pD2DFactory = (ID2D1Factory*)obj;
    }

    private void CreateDeviceResources()
    {
        if (pRenderTarget == null)
        {
            GetClientRect(hWnd, out var rc);
            D2D_SIZE_U size = new()
            {
                width = (uint)(rc.right - rc.left),
                height = (uint)(rc.bottom - rc.top),
            };

            D2D1_RENDER_TARGET_PROPERTIES renderTargetProps = new() { };

            D2D1_HWND_RENDER_TARGET_PROPERTIES hWndRenderTargetProps = new()
            {
                hwnd = hWnd,
                pixelSize = size,
            };

            fixed (ID2D1HwndRenderTarget** ppRenderTarget = &pRenderTarget)
            {
                pD2DFactory->CreateHwndRenderTarget(
                    in renderTargetProps,
                    in hWndRenderTargetProps,
                    ppRenderTarget).ThrowOnFailure();
            }

            fixed (ID2D1SolidColorBrush** ppLightSlateGrayBrush = &pLightSlateGrayBrush)
            fixed (ID2D1SolidColorBrush** ppCornflowerBlueBrush = &pCornflowerBlueBrush)
            {
                var color = Colors.LightSlateGray.ToD2DColorF();
                pRenderTarget->CreateSolidColorBrush(in color, null, ppLightSlateGrayBrush).ThrowOnFailure();
                color = (Colors.CornflowerBlue with { A = 0.75f }).ToD2DColorF();
                pRenderTarget->CreateSolidColorBrush(in color, null, ppCornflowerBlueBrush).ThrowOnFailure();
            }
        }
    }

    private void DiscardDeviceResources()
    {
        if (pRenderTarget != null)
        {
            pRenderTarget->Release();
            pRenderTarget = null;
        }

        if (pLightSlateGrayBrush != null)
        {
            pLightSlateGrayBrush->Release();
            pLightSlateGrayBrush = null;
        }

        if (pCornflowerBlueBrush != null)
        {
            pCornflowerBlueBrush->Release();
            pCornflowerBlueBrush = null;
        }
    }

    private void OnRender()
    {
        CreateDeviceResources();

        var xform = Transform.PixelIdentity.ToD2DMatrix3X2F();

        pRenderTarget->BeginDraw();
        pRenderTarget->SetTransform(in xform);

        var color = Colors.RebeccaPurple.ToD2DColorF();
        pRenderTarget->Clear(&color);

        GetClientRect(hWnd, out var rc);
        D2D_SIZE_U size = new()
        {
            width = (uint)(rc.right - rc.left),
            height = (uint)(rc.bottom - rc.top),
        };

        for (int x = 0; x < size.width; x += 10)
        {
            pRenderTarget->DrawLine(
                new D2D_POINT_2F { x = x, y = 0 },
                new D2D_POINT_2F { x = x, y = size.height },
                (ID2D1Brush*)pLightSlateGrayBrush,
                1f,
                null);
        }

        for (int y = 0; y < size.height; y += 10)
        {
            pRenderTarget->DrawLine(
                new D2D_POINT_2F { x = 0, y = y },
                new D2D_POINT_2F { x = size.width, y = y },
                (ID2D1Brush*)pLightSlateGrayBrush,
                1f,
                null);
        }

        // D2D_RECT_F rect1 = new()
        // {
        //     left = size.width / 2 - 50f,
        //     right = size.width / 2 + 50f,
        //     top = size.height / 2 - 50f,
        //     bottom = size.height / 2 + 50f,
        // };

        D2D_RECT_F rect1 = new()
        {
            left = pointerPosition.X - 50f,
            right = pointerPosition.X + 50f,
            top = pointerPosition.Y - 50f,
            bottom = pointerPosition.Y + 50f,
        };

        // D2D_RECT_F rect2 = new()
        // {
        //     left = size.width / 2 - 100f,
        //     right = size.width / 2 + 100f,
        //     top = size.height / 2 - 100f,
        //     bottom = size.height / 2 + 100f,
        // };

        D2D_RECT_F rect2 = new()
        {
            left = pointerPosition.X - 100f,
            right = pointerPosition.X + 100f,
            top = pointerPosition.Y - 100f,
            bottom = pointerPosition.Y + 100f,
        };

        pRenderTarget->FillRectangle(in rect1, (ID2D1Brush*)pCornflowerBlueBrush);
        pRenderTarget->DrawRectangle(in rect2, (ID2D1Brush*)pCornflowerBlueBrush, 1, default);

        var hr = pRenderTarget->EndDraw();
        if (hr == HRESULT.D2DERR_RECREATE_TARGET)
        {
            DiscardDeviceResources();
        }
        else
        {
            hr.ThrowOnFailure();
        }
    }

    private void OnResize(int width, int height)
    {
        if (pRenderTarget != null)
        {
            var size = new D2D_SIZE_U
            {
                width = (uint)width,
                height = (uint)height,
            };
            pRenderTarget->Resize(in size);
        }
    }

    #endregion private

    #region private static

    private static short LoWord(int value) => (short)(value & (0xffff));
    private static short LoWord(LPARAM value) => LoWord((int)value.Value);
    private static short LoWord(WPARAM value) => LoWord((int)value.Value);
    private static short HiWord(int value) => (short)((value >> 16) & (0xffff));
    private static short HiWord(LPARAM value) => HiWord((int)value.Value);
    private static short HiWord(WPARAM value) => HiWord((int)value.Value);

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

    private Vector2 pointerPosition = new Vector2();

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
                            app.OnResize(width, height);
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
                            app.OnRender();
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

                    case WM_MOUSEMOVE:
                        {
                            app.pointerPosition.X = LoWord(lParam);
                            app.pointerPosition.Y = HiWord(lParam);
                            InvalidateRect(hWnd, (RECT?)null, false);
                        }
                        result = new(1);
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

internal static class Program
{
    public static App? App;
    static int Main(string[] args)
    {
        if (args.Length > 0)
        {
            bool consoleMode = Boolean.Parse(args[0]);
            if (consoleMode)
            {
                if (!AttachConsole(unchecked((uint)-1)))
                {
                    AllocConsole();
                }
            }
        }

        App = new();
        App.RunMessageLoop();

        return 0;
    }
}

// class App : Box
// {
//     public App()
//     {
//         Name = "App";
//         Content =
//         new VBox
//         {
//             Content =
//             {
//                 new Box
//                 {
//                     Name = "Header",
//                     Content = "Hello world!"
//                 },
//                 new Box
//                 {
//                     Name = "Body",
//                     Content = { "Goodnight", "Moon", }
//                 }
//             }
//         };
//     }
// }
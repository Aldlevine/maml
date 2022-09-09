using System;
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
    }
}

internal class App
{
    #region public

    public App()
    {
    }

    ~App()
    {
        if (hWnd != null)
        {
            
        }
    }

    public HRESULT Initialize()
    {
        var hr = CreateDeviceIndependentResources();

        if (hr.Succeeded)
        {
            unsafe
            {

                fixed (char* pStr = "MamlWindow")
                {
                    WNDCLASSEXW wcex = new WNDCLASSEXW()
                    {
                        style = WNDCLASS_STYLES.CS_HREDRAW | WNDCLASS_STYLES.CS_VREDRAW,
                        lpfnWndProc = WndProc,
                        cbClsExtra = 0,
                        cbWndExtra = 0,
                        hInstance = GetModuleHandle(default(PCWSTR)),
                        hbrBackground = default(HBRUSH),
                        lpszMenuName = null,
                        hCursor = LoadCursor(HINSTANCE.Null, IDI_APPLICATION),
                        lpszClassName = new PCWSTR(pStr),
                    };
                    RegisterClassEx(in wcex);
                }

                GCHandle app = GCHandle.Alloc(this);
                IntPtr appPtr = GCHandle.ToIntPtr(app);
                hWnd = CreateWindowEx(
                    WINDOW_EX_STYLE.WS_EX_APPWINDOW,
                    "MamlWindow",
                    "Maml",
                    WINDOW_STYLE.WS_OVERLAPPEDWINDOW,
                    CW_USEDEFAULT,
                    CW_USEDEFAULT,
                    CW_USEDEFAULT,
                    CW_USEDEFAULT,
                    HWND.Null,
                    null,
                    GetModuleHandle(default(string)),
                    (void*)appPtr);

                if (hWnd != HWND.Null)
                {
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
        }

        return hr;
    }

    public void RunMessageLoop()
    {
        while (GetMessage(out MSG msg, hWnd, 0, 0))
        {
            TranslateMessage(in msg);
            DispatchMessage(in msg);
        }
    }

    #endregion public

    #region private

    HWND hWnd;
    ID2D1Factory d2DFactory;
    ID2D1HwndRenderTarget? renderTarget;
    ID2D1SolidColorBrush? lightSlateGrayBrush;
    ID2D1SolidColorBrush? cornflowerBlueBrush;

    private HRESULT CreateDeviceIndependentResources()
    {
        var hr = D2D1CreateFactory(D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_SINGLE_THREADED, Guid.Empty, null, out var obj);
        d2DFactory = (ID2D1Factory)obj;
        return hr;
    }

    private void CreateDeviceResources()
    {
        if (renderTarget == null)
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

            unsafe
            {
                D2D1_RENDER_TARGET_PROPERTIES* rtp = &renderTargetProps;
                D2D1_HWND_RENDER_TARGET_PROPERTIES* hrtp = &hWndRenderTargetProps;
                {
                    d2DFactory.CreateHwndRenderTarget(
                        rtp,
                        hrtp,
                        out renderTarget);
                }
            }

            unsafe
            {
                var color = Colors.LightSlategray.To2D2ColorF();
                renderTarget.CreateSolidColorBrush(&color, null, out lightSlateGrayBrush);
                color = Colors.CornflowerBlue.To2D2ColorF();
                renderTarget.CreateSolidColorBrush(&color, null, out cornflowerBlueBrush);
            }
        }
    }

    private HRESULT OnRender()
    {

    }

    private void OnResize()
    {

    }

    private static LRESULT WndProc(
        HWND hWnd,
        uint msg,
        WPARAM wParam,
        LPARAM lParam)
    {
        LRESULT result = new(0);

        switch (msg)
        {
            case WM_CREATE:
                // CREATESTRUCTW pcs = Marcgal
                break;

        }
    }

    #endregion private
}

internal static class Program
{
    static int Main()
    {
        App app = new();
        if (app.Initialize().Succeeded)
        {
            app.RunMessageLoop();
        }

        return 0;
    }
}

// internal class Program
// {
//     public static Size WindowSize { get; private set; } = new Size();
//     public static HWND HWnd { get; private set; }
// 
//     private const string WindowClassName = "Maml";
//     private static readonly WNDPROC WndProcDelegate = WndProc;
// 
//     private static void Main()
//     {
//         RegisterWindowClass();
//         InitInstance();
// 
//         while (GetMessage(out MSG msg, default, 0, 0) > 0)
//         {
//             TranslateMessage(msg);
//             DispatchMessage(msg);
//         }
//     }
// 
//     private static void RegisterWindowClass()
//     {
//         unsafe
//         {
//             fixed (char* szClassName = WindowClassName)
//             {
//                 var wcex = default(WNDCLASSEXW);
//                 PCWSTR szNull = default;
//                 PCWSTR szCursorName = new((char*)IDC_ARROW);
//                 PCWSTR szIconName = new((char*)IDI_APPLICATION);
//                 wcex.cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>();
//                 wcex.lpfnWndProc = WndProcDelegate;
//                 wcex.cbClsExtra = 0;
//                 wcex.hInstance = GetModuleHandle(szNull);
//                 wcex.hCursor = LoadCursor(wcex.hInstance, szCursorName);
//                 wcex.hbrBackground = new HBRUSH(new IntPtr(6));
//                 wcex.lpszClassName = szClassName;
//                 RegisterClassEx(wcex);
//             }
//         }
//     }
// 
//     private static void InitInstance()
//     {
//         unsafe
//         {
//             HWnd = CreateWindowEx(
//                 0,
//                 WindowClassName,
//                 "Maml",
//                 WINDOW_STYLE.WS_BORDER | WINDOW_STYLE.WS_CAPTION | WINDOW_STYLE.WS_SYSMENU | WINDOW_STYLE.WS_MAXIMIZEBOX | WINDOW_STYLE.WS_MINIMIZEBOX | WINDOW_STYLE.WS_SIZEBOX,
//                 CW_USEDEFAULT,
//                 CW_USEDEFAULT,
//                 CW_USEDEFAULT,
//                 CW_USEDEFAULT,
//                 default,
//                 default,
//                 default,
//                 null);
//         }
// 
//         ShowWindow(HWnd, SHOW_WINDOW_CMD.SW_NORMAL);
//         UpdateWindow(HWnd);
//     }
// 
//     private static short LoWord(int value) => (short)(value & (0xffff));
//     private static short LoWord(LPARAM value) => LoWord((int)value.Value);
//     private static short LoWord(WPARAM value) => LoWord((int)value.Value);
//     private static short HiWord(int value) => (short)((value >> 16) & (0xffff));
//     private static short HiWord(LPARAM value) => HiWord((int)value.Value);
//     private static short HiWord(WPARAM value) => HiWord((int)value.Value);
// 
//     private static Point pointerPosition;
// 
//     private static LRESULT WndProc(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam)
//     {
//         switch (msg)
//         {
//             case WM_CLOSE:
//                 {
//                     DestroyWindow(hWnd);
//                     break;
//                 }
//             case WM_DESTROY:
//                 {
//                     PostQuitMessage(0);
//                     break;
//                 }
//             case WM_SETCURSOR:
//                 unsafe
//                 {
//                     SetCursor(LoadCursor(HINSTANCE.Null, new PWSTR((char*)IDC_ARROW)));
//                     break;
//                 }
//             case WM_SIZE:
//                 {
//                     int width = Math.Max((int)LoWord(lParam), 0);
//                     int height = Math.Max((int)HiWord(lParam), 0);
//                     SetWindowText(hWnd, $"Maml ({width}x{height})");
//                     WindowSize = new Size(width, height);
//                     InvalidateRect(hWnd, (RECT?)null, true);
//                     break;
//                 }
//             case WM_MOVE:
//                 {
//         HWND hWnd;
//                     // InvalidateRect(hWnd, (RECT?)null, true);
//                     // PaintWindow(hWnd);
//                     break;
//                 }
//             case WM_MOUSEMOVE:
//                 {
//                     var oldPointerPosition = pointerPosition;
// 
//                     int x = LoWord(lParam);
//                     int y = HiWord(lParam);
//                     pointerPosition = new Point(x, y);
// 
//                     Rectangle rect = new(0, 0, 200, 200);
//                     Region rgn = new Region(rect with { Location = oldPointerPosition - rect.Size / 2 });
//                     rgn.Union(rect with { Location = pointerPosition - rect.Size / 2 });
//                     var hRgn = (HRGN)rgn.GetHrgn(Graphics.FromHwnd(hWnd));
//                     InvalidateRgn(hWnd, hRgn, false);
//                     // UpdateWindow(hWnd);
//                     rgn.ReleaseHrgn(hRgn.Value);
//                     break;
//                 }
//             case WM_LBUTTONDOWN:
//                 {
//                     Console.WriteLine("L BUTTON DOWN");
//                     break;
//                 }
//             case WM_LBUTTONUP:
//                 {
//                     Console.WriteLine("L BUTTON UP");
//                     break;
//                 }
//             case WM_RBUTTONDOWN:
//                 {
//                     Console.WriteLine("R BUTTON DOWN");
//                     break;
//                 }
//             case WM_RBUTTONUP:
//                 {
//                     Console.WriteLine("R BUTTON UP");
//                     break;
//                 }
//             case WM_MBUTTONDOWN:
//                 {
//                     Console.WriteLine("M BUTTON DOWN");
//                     break;
//                 }
//             case WM_MBUTTONUP:
//                 {
//                     Console.WriteLine("M BUTTON UP");
//                     break;
//                 }
//             case WM_MOUSEWHEEL:
//                 {
//                     double delta = HiWord(wParam) / 120d;
//                     Console.WriteLine("MOUSE WHEEL {0}", delta);
//                     break;
//                 }
//             case WM_KEYDOWN:
//             case WM_SYSKEYDOWN:
//                 {
//                     VirtualKey vKey = (VirtualKey)wParam.Value;
//                     if (vKey == VirtualKey.Control || vKey == VirtualKey.Menu || vKey == VirtualKey.Shift)
//                     {
//                         // NOOP
//                     }
//                     else
//                     {
//                         // Non-Modifier
//                         Console.Write("KEYDOWN: ");
//                         if ((GetAsyncKeyState((int)VirtualKey.Control) & 0xf000) > 0)
//                         {
//                             Console.Write(VirtualKey.Control + " + ");
//                         }
//                         if ((GetAsyncKeyState((int)VirtualKey.Menu) & 0xf000) > 0)
//                         {
//                             Console.Write("Alt" + " + ");
//                             // HACK: Should move this out of the message pump
//                             if (vKey == VirtualKey.F4)
//                             {
//                                 PostQuitMessage(0);
//                             }
//                         }
//                         if ((GetAsyncKeyState((int)VirtualKey.Shift) & 0xf000) > 0)
//                         {
//                             Console.Write(VirtualKey.Shift + " + ");
//                         }
//                         Console.WriteLine(vKey);
//                     }
//                     return new LRESULT(0);
//                 }
//             case WM_KEYUP:
//             case WM_SYSKEYUP:
//                 {
//                     VirtualKey vKey = (VirtualKey)wParam.Value;
//                     if (vKey == VirtualKey.Control || vKey == VirtualKey.Menu || vKey == VirtualKey.Shift)
//                     {
//                         // NOOP
//                     }
//                     else
//                     {
//                         // Non-Modifier
//                         Console.Write("KEYUP: ");
//                         if ((GetAsyncKeyState((int)VirtualKey.Control) & 0xf000) > 0)
//                         {
//                             Console.Write(VirtualKey.Control + " + ");
//                         }
//                         if ((GetAsyncKeyState((int)VirtualKey.Menu) & 0xf000) > 0)
//                         {
//                             Console.Write("Alt" + " + ");
//                         }
//                         if ((GetAsyncKeyState((int)VirtualKey.Shift) & 0xf000) > 0)
//                         {
//                             Console.Write(VirtualKey.Shift + " + ");
//                         }
//                         Console.WriteLine(vKey);
//                     }
//                     return new LRESULT(0);
//                 }
//             case WM_ERASEBKGND:
//                 {
//                     return new LRESULT(1);
//                 }
//             case WM_PAINT:
//                 PaintWindowBuffered(hWnd);
//                 // PaintWindow(hWnd);
//                 return new LRESULT(0);
//         }
// 
//         return DefWindowProc(hWnd, msg, wParam, lParam);
//     }
// 
//     private static void PaintWindow(HWND hWnd)
//     {
//         // Get the window hdc
//         HDC windowHdc = BeginPaint(hWnd, out PAINTSTRUCT ps);
//         if (windowHdc == HDC.Null)
//         {
//             throw new Exception("BeginPaint() Failed");
//         }
// 
//         var graphics = Graphics.FromHdc(windowHdc.Value);
//         graphics.TranslateTransform(-ps.rcPaint.X, -ps.rcPaint.Y);
// 
//         // PaintWindow(hWnd,bufferHdc, ps);
//         PaintGraphics(graphics);
// 
//         EndPaint(hWnd, in ps);
//     }
// 
//     private static void PaintWindowBuffered(HWND hWnd)
//     {
//         unsafe
//         {
//             // Get the window hdc
//             HDC windowHdc = BeginPaint(hWnd, out PAINTSTRUCT ps);
//             if (windowHdc == HDC.Null)
//             {
//                 throw new Exception("BeginPaint() Failed");
//             }
// 
//             // Get a buffer hdc compatible with the window hdc
//             HDC bufferHdc = (HDC)CreateCompatibleDC(windowHdc).Value;
//             if (bufferHdc == HDC.Null)
//             {
//                 throw new Exception("CreateCompatibleDC() Failed ");
//             }
// 
//             // Get a bitmap for the buffer hdc compatible with the window hdc
//             // HBITMAP bufferBmp = CreateCompatibleBitmap(windowHdc, WindowSize.Width, WindowSize.Height);
//             HBITMAP bufferBmp = CreateCompatibleBitmap(windowHdc, ps.rcPaint.Width, ps.rcPaint.Height);
//             if (bufferBmp == HBITMAP.Null)
//             {
//                 throw new Exception("CreateCompatibleBitmap() Failed");
//             }
// 
//             // Swap to buffer bitmap and store the window bitmap
//             HBITMAP windowBmp = new(SelectObject(bufferHdc, (HGDIOBJ)bufferBmp.Value).Value);
// 
//             var graphics = Graphics.FromHdc(bufferHdc.Value);
//             graphics.TranslateTransform(-ps.rcPaint.X, -ps.rcPaint.Y);
// 
//             // PaintWindow(hWnd,bufferHdc, ps);
//             PaintGraphics(graphics);
// 
//             // Blit the buffer bitmap to the window bitmap
//             if (BitBlt(windowHdc, ps.rcPaint.X, ps.rcPaint.Y, ps.rcPaint.Width, ps.rcPaint.Height, bufferHdc, 0, 0, ROP_CODE.SRCCOPY) == 0)
//             {
//                 throw new Exception("BitBlt() Failed");
//             }
// 
//             EndPaint(hWnd, in ps);
// 
//             // Swap back to window bitmap
//             SelectObject(bufferHdc, (HGDIOBJ)windowBmp.Value);
// 
//             // Delete buffer bitmap
//             if (DeleteObject((HGDIOBJ)bufferBmp.Value) == 0)
//             {
//                 throw new Exception("DeleteObject() Failed");
//             }
//             // Delete buffer hdc
//             if (DeleteDC(new CreatedHDC(bufferHdc.Value)) == 0)
//             {
//                 throw new Exception("DeleteDC() Failed");
//             }
// 
//         }
//     }
// 
//     private static readonly Brush bgBrush = new SolidBrush(ColorExtensions.FromArgb(0xff2e2e2e));
//     private static readonly Brush barBrush = new SolidBrush(ColorExtensions.FromArgb(0xff1f1f1f));
//     private static readonly Brush textBrush = new SolidBrush(ColorExtensions.FromArgb(0x88ffffff));
//     private static readonly Font defaultFont = new Font("Segoe UI", 10);
//     private static void PaintGraphics(Graphics graphics)
//     {
//         graphics.SmoothingMode = SmoothingMode.HighQuality;
//         graphics.CompositingQuality = CompositingQuality.GammaCorrected;
//         graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
// 
//         var windowRect = new Rectangle(Point.Empty, WindowSize);
//         graphics.FillRectangle(barBrush, windowRect with { Height = 24 });
//         graphics.FillRectangle(bgBrush, windowRect with { Y = 24 , Height = windowRect.Height - 24 });
// 
//         var text = $"Hello World: {pointerPosition}";
//         var textRect = windowRect with { X = 4, Y = (24 - defaultFont.Height) / 2, Height = 24 };
// 
//         graphics.DrawString(text, defaultFont, textBrush, textRect);
//         for (int i = 0; i < 10; i++)
//         {
//             textRect = textRect with { Y = textRect.Y + textRect.Height };
//             graphics.DrawString(text, defaultFont, textBrush, textRect);
//         }
// 
//         var state = graphics.Save();
//         // graphics.TranslateTransform(WindowSize.Width - 110, WindowSize.Height - 110);
//         graphics.TranslateTransform(pointerPosition.X - 50, pointerPosition.Y - 50);
//         // graphics.RotateTransform(45);
// 
//         var cornerRadius = 10;
//         var rect = new Rectangle(0, 0, 100, 100);
//         var outline = new Pen(Color.FromArgb(64, Color.White), 4)
//         {
//             DashPattern = new[] {3f, 0.5f, 1f, 0.5f},
//             DashCap = DashCap.Round,
//             DashOffset = (float)(DateTime.Now.Ticks / (10_000.0 * 500.0) % 5.0),
//             LineJoin = LineJoin.Bevel,
//             StartCap = LineCap.Round,
//             EndCap = LineCap.Round,
//         };
//         var fill = new SolidBrush(Color.FromArgb(64, Color.White));
//         // var roundedRect = CreateRoundedRectanglePath(rect, cornerRadius);
//         var roundedRect = graphics.GetRoundRectanglePath(rect, cornerRadius);
//         // graphics.FillPath(fill, roundedRect);
//         // graphics.DrawPath(outline, roundedRect);
//         graphics.DrawRoundedRectangle(rect, cornerRadius, outline);
// 
//         graphics.Restore(state);
//     }
// 
//     /*
//     unsafe private static void PaintWindow(HWND hWnd, HDC hdc, PAINTSTRUCT ps)
//     {
//         HBRUSH bgBrush = CreateSolidBrush(new COLORREF(0x002e2e2e));
//         HBRUSH barBrush = CreateSolidBrush(new COLORREF(0x001f1f1f));
// 
//         RECT rect = new(0, 0, WindowSize.Width, WindowSize.Height);
//         if (FillRect(hdc, &rect, bgBrush) == 0)
//         {
//             throw new Exception("FillRect() Failed");
//         }
// 
//         rect = new(0, 0, WindowSize.Width, 24);
//         if (FillRect(hdc, &rect, barBrush) == 0)
//         {
//             throw new Exception("FillRect() Failed");
//         }
// 
//         string text = $"Hello World: {pointerPosition}";
//         SelectObject(hdc, FontCache.GetFont(new()
//         {
//             Name = "Segoe UI",
//             Size = 16,
//             Weight = 400,
//             Style = FontStyle.Normal
//         }).ToHGDIOBJ());
// 
//         PWSTR pStr = text.ToPWSTR();
//         SIZE textSize;
//         GetTextExtentPoint32W(hdc, pStr, text.Length, &textSize);
//         RECT textRect = new(4 - ps.rcPaint.X, 4 - ps.rcPaint.Y, textSize.Width + 4, textSize.Height + 4);
// 
//         SetTextColor(hdc, new COLORREF(0x00ffffff));
// 
//         if (DrawText(hdc, pStr, text.Length, &textRect, DRAW_TEXT_FORMAT.DT_TOP | DRAW_TEXT_FORMAT.DT_LEFT) == 0)
//         {
//             throw new Exception("DrawText() Failed");
//         }
// 
//         for (int i = 0; i < 0; i++)
//         {
//             textRect = textRect with { top = textRect.top + 24, bottom = textRect.bottom + 24 };
//             if (DrawText(hdc, pStr, text.Length, &textRect, DRAW_TEXT_FORMAT.DT_TOP | DRAW_TEXT_FORMAT.DT_LEFT) == 0)
//             {
//                 throw new Exception("DrawText() Failed");
//             }
//         }
// 
// 
//         DeleteObject((HGDIOBJ)bgBrush.Value);
//         DeleteObject((HGDIOBJ)barBrush.Value);
//     }
//     */
// }
// 
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
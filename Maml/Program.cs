using System;
using System.Drawing;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using Windows.Globalization.DateTimeFormatting;
using Windows.System;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;

[assembly: MetadataUpdateHandler(typeof(Maml.HotReloadManager))]

namespace Maml;

internal static class HotReloadManager
{
    public static void ClearCache(Type[]? types)
    {
    }

    public static void UpdateApplication(Type[]? types)
    {
        InvalidateRect(Program.HWnd, (RECT?)null, true);
    }
}


internal sealed class ReleaseDCSafeHandle : DeleteDCSafeHandle
{
    public HWND HWnd { get; }
    public HDC HDC { get; }

    public ReleaseDCSafeHandle(HWND hWnd, HDC hDC) : base(hDC.Value)
    {
        HWnd = hWnd;
        HDC = hDC;
    }

    protected override bool ReleaseHandle() => ReleaseDC(HWnd, HDC) > 0;
}


internal class Program
{
    private const string WindowClassName = "Maml";
    public static Size WindowSize { get; private set; } = new Size();
    public static HWND HWnd { get; private set; }

    private static readonly HFONT DefaultFont = GetDefaultFont();
    private static readonly WNDPROC WndProcDelegate = WndProc;

    private static void Main()
    {
        RegisterWindowClass();
        InitInstance();

        while (GetMessage(out MSG msg, default, 0, 0) > 0)
        {
            TranslateMessage(msg);
            DispatchMessage(msg);
        }
    }

    private static void RegisterWindowClass()
    {
        unsafe
        {
            fixed (char* szClassName = WindowClassName)
            {
                var wcex = default(WNDCLASSEXW);
                PCWSTR szNull = default;
                PCWSTR szCursorName = new((char*)IDC_ARROW);
                PCWSTR szIconName = new((char*)IDI_APPLICATION);
                wcex.cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>();
                wcex.lpfnWndProc = WndProcDelegate;
                wcex.cbClsExtra = 0;
                wcex.hInstance = GetModuleHandle(szNull);
                wcex.hCursor = LoadCursor(wcex.hInstance, szCursorName);
                wcex.hbrBackground = new HBRUSH(new IntPtr(6));
                wcex.lpszClassName = szClassName;
                RegisterClassEx(wcex);
            }
        }
    }

    private static void InitInstance()
    {
        unsafe
        {
            HWnd = CreateWindowEx(
                0,
                WindowClassName,
                "Maml",
                WINDOW_STYLE.WS_BORDER | WINDOW_STYLE.WS_CAPTION | WINDOW_STYLE.WS_SYSMENU | WINDOW_STYLE.WS_MAXIMIZEBOX | WINDOW_STYLE.WS_MINIMIZEBOX | WINDOW_STYLE.WS_SIZEBOX,
                CW_USEDEFAULT,
                CW_USEDEFAULT,
                CW_USEDEFAULT,
                CW_USEDEFAULT,
                default,
                default,
                default,
                null);
        }

        ShowWindow(HWnd, SHOW_WINDOW_CMD.SW_NORMAL);
        UpdateWindow(HWnd);
    }

    private static short LoWord(int value) => (short)(value & (0xffff));
    private static short LoWord(LPARAM value) => LoWord((int)value.Value);
    private static short LoWord(WPARAM value) => LoWord((int)value.Value);
    private static short HiWord(int value) => (short)((value >> 16) & (0xffff));
    private static short HiWord(LPARAM value) => HiWord((int)value.Value);
    private static short HiWord(WPARAM value) => HiWord((int)value.Value);

    private static Point pointerPosition;

    private static LRESULT WndProc(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam)
    {
        switch (msg)
        {
            case WM_CLOSE:
                {
                    DestroyWindow(hWnd);
                    break;
                }
            case WM_DESTROY:
                {
                    PostQuitMessage(0);
                    break;
                }
            case WM_SETCURSOR:
                unsafe
                {
                    SetCursor(LoadCursor(HINSTANCE.Null, new PWSTR((char*)IDC_ARROW)));
                    break;
                }
            case WM_SIZE:
                {
                    int width = Math.Max((int)LoWord(lParam), 0);
                    int height = Math.Max((int)HiWord(lParam), 0);
                    SetWindowText(hWnd, $"Maml ({width}x{height})");
                    WindowSize = new Size(width, height);
                    InvalidateRect(hWnd, (RECT?)null, true);
                    break;
                }
            case WM_MOVE:
                {
                    InvalidateRect(hWnd, (RECT?)null, true);
                    break;
                }
            case WM_MOUSEMOVE:
                {
                    int x = LoWord(lParam);
                    int y = HiWord(lParam);
                    pointerPosition = new Point(x, y);
                    InvalidateRect(hWnd, (RECT?)null, true);
                    break;
                }
            case WM_LBUTTONDOWN:
                {
                    Console.WriteLine("L BUTTON DOWN");
                    break;
                }
            case WM_LBUTTONUP:
                {
                    Console.WriteLine("L BUTTON UP");
                    break;
                }
            case WM_RBUTTONDOWN:
                {
                    Console.WriteLine("R BUTTON DOWN");
                    break;
                }
            case WM_RBUTTONUP:
                {
                    Console.WriteLine("R BUTTON UP");
                    break;
                }
            case WM_MBUTTONDOWN:
                {
                    Console.WriteLine("M BUTTON DOWN");
                    break;
                }
            case WM_MBUTTONUP:
                {
                    Console.WriteLine("M BUTTON UP");
                    break;
                }
            case WM_MOUSEWHEEL:
                {
                    double delta = HiWord(wParam) / 120d;
                    Console.WriteLine("MOUSE WHEEL {0}", delta);
                    break;
                }
            case WM_KEYDOWN:
            case WM_SYSKEYDOWN:
                {
                    VirtualKey vKey = (VirtualKey)wParam.Value;
                    if (vKey == VirtualKey.Control || vKey == VirtualKey.Menu || vKey == VirtualKey.Shift)
                    {
                        // NOOP
                    }
                    else
                    {
                        // Non-Modifier
                        Console.Write("KEYDOWN: ");
                        if ((GetAsyncKeyState((int)VirtualKey.Control) & 0xf000) > 0)
                        {
                            Console.Write(VirtualKey.Control + " + ");
                        }
                        if ((GetAsyncKeyState((int)VirtualKey.Menu) & 0xf000) > 0)
                        {
                            Console.Write("Alt" + " + ");
                            // HACK: Should move this out of the message pump
                            if (vKey == VirtualKey.F4)
                            {
                                PostQuitMessage(0);
                            }
                        }
                        if ((GetAsyncKeyState((int)VirtualKey.Shift) & 0xf000) > 0)
                        {
                            Console.Write(VirtualKey.Shift + " + ");
                        }
                        Console.WriteLine(vKey);
                    }
                    return new LRESULT(0);
                }
            case WM_KEYUP:
            case WM_SYSKEYUP:
                {
                    VirtualKey vKey = (VirtualKey)wParam.Value;
                    if (vKey == VirtualKey.Control || vKey == VirtualKey.Menu || vKey == VirtualKey.Shift)
                    {
                        // NOOP
                    }
                    else
                    {
                        // Non-Modifier
                        Console.Write("KEYUP: ");
                        if ((GetAsyncKeyState((int)VirtualKey.Control) & 0xf000) > 0)
                        {
                            Console.Write(VirtualKey.Control + " + ");
                        }
                        if ((GetAsyncKeyState((int)VirtualKey.Menu) & 0xf000) > 0)
                        {
                            Console.Write("Alt" + " + ");
                        }
                        if ((GetAsyncKeyState((int)VirtualKey.Shift) & 0xf000) > 0)
                        {
                            Console.Write(VirtualKey.Shift + " + ");
                        }
                        Console.WriteLine(vKey);
                    }
                    return new LRESULT(0);
                }
            case WM_ERASEBKGND:
                {
                    return new LRESULT(1);
                }
            case WM_PAINT:
                unsafe
                {
                    // Get the window hdc
                    HDC windowHdc = BeginPaint(hWnd, out PAINTSTRUCT ps);
                    if (windowHdc == HDC.Null)
                    {
                        throw new Exception("BeginPaint() Failed");
                    }

                    // Get a buffer hdc compatible with the window
                    HDC bufferHdc = (HDC)CreateCompatibleDC(windowHdc).Value;
                    if (bufferHdc == HDC.Null)
                    {
                        throw new Exception("CreateCompatibleDC() Failed ");
                    }

                    // Get a bitmap for the buffer hdc same size as the window
                    HBITMAP bufferBmp = CreateCompatibleBitmap(windowHdc, WindowSize.Width, WindowSize.Height);
                    if (bufferBmp == HBITMAP.Null)
                    {
                        throw new Exception("CreateCompatibleBitmap() Failed");
                    }

                    // Swap to buffer bitmap and store the window bitmap
                    HBITMAP windowBmp = new(SelectObject(bufferHdc, (HGDIOBJ)bufferBmp.Value).Value);

                    // Paint the window
                    SetBkMode(bufferHdc, BACKGROUND_MODE.TRANSPARENT);
                    PaintWindow(hWnd,bufferHdc, ps);

                    // Blit the buffer bitmap to the window bitmap
                    if (BitBlt(windowHdc, ps.rcPaint.X, ps.rcPaint.Y, ps.rcPaint.Width, ps.rcPaint.Height, bufferHdc, 0, 0, ROP_CODE.SRCCOPY) == 0)
                    {
                        throw new Exception("BitBlt() Failed");
                    }

                    EndPaint(hWnd, in ps);

                    // Swap back to window bitmap
                    SelectObject(bufferHdc, (HGDIOBJ)windowBmp.Value);

                    // Delete buffer bitmap
                    if (DeleteObject((HGDIOBJ)bufferBmp.Value) == 0)
                    {
                        throw new Exception("DeleteObject() Failed");
                    }
                    // Delete buffer hdc
                    if (DeleteDC(new CreatedHDC(bufferHdc.Value)) == 0)
                    {
                        throw new Exception("DeleteDC() Failed");
                    }

                    return new LRESULT(0);
                }
        }

        return DefWindowProc(hWnd, msg, wParam, lParam);
    }

    unsafe private static void PaintWindow(HWND hWnd, HDC hdc, PAINTSTRUCT ps)
    {
        HBRUSH bgBrush = CreateSolidBrush(Color.DarkSlateBlue.ToColorRef());
        HBRUSH barBrush = CreateSolidBrush(Color.FromArgb(unchecked((int)0xff333333)).ToColorRef());

        RECT rect = new(0, 0, WindowSize.Width, WindowSize.Height);
        if (FillRect(hdc, &rect, bgBrush) == 0)
        {
            throw new Exception("FillRect() Failed");
        }

        rect = new(0, 0, WindowSize.Width, 24);
        if (FillRect(hdc, &rect, barBrush) == 0)
        {
            throw new Exception("FillRect() Failed");
        }

        string text = $"Hello World: {pointerPosition}";
        fixed (char* pText = text)
        {
            SelectObject(hdc, (HGDIOBJ)DefaultFont.Value);

            PWSTR pStr = new(pText);
            SIZE textSize;
            GetTextExtentPoint32W(hdc, pStr, text.Length, &textSize);
            RECT textRect = new(4 - ps.rcPaint.X, 4 - ps.rcPaint.Y, textSize.Width + 4, textSize.Height + 4);

            SetTextColor(hdc, Color.White.ToColorRef());

            if (DrawText(hdc, pStr, text.Length, &textRect, DRAW_TEXT_FORMAT.DT_TOP | DRAW_TEXT_FORMAT.DT_LEFT) == 0)
            {
                throw new Exception("DrawText() Failed");
            }

            for (int i = 0; i < 10; i++)
            {
                textRect = textRect with { top = textRect.top + 24, bottom = textRect.bottom + 24 };
                if (DrawText(hdc, pStr, text.Length, &textRect, DRAW_TEXT_FORMAT.DT_TOP | DRAW_TEXT_FORMAT.DT_LEFT) == 0)
                {
                    throw new Exception("DrawText() Failed");
                }
            }
        }

        DeleteObject((HGDIOBJ)bgBrush.Value);
        DeleteObject((HGDIOBJ)barBrush.Value);
    }

    unsafe private static HFONT GetDefaultFont()
    {
        string fontName = "Segoe UI";
        fixed (char* pFontName = fontName)
        {
            PWSTR fontNameStr = new(pFontName);
            return CreateFont(
                16, 0,
                0, 0,
                400, 0, 0, 0,
                (uint)FONT_CHARSET.DEFAULT_CHARSET,
                FONT_OUTPUT_PRECISION.OUT_DEFAULT_PRECIS,
                FONT_CLIP_PRECISION.CLIP_DEFAULT_PRECIS,
                FONT_QUALITY.CLEARTYPE_QUALITY,
                FONT_PITCH_AND_FAMILY.FF_DONTCARE,
                fontNameStr);
        }
    }
}

class App : Box
{
    public App()
    {
        Name = "App";
        Content =
        new VBox
        {
            Content =
            {
                new Box
                {
                    Name = "Header",
                    Content = "Hello world!"
                },
                new Box
                {
                    Name = "Body",
                    Content = { "Goodnight", "Moon", }
                }
            }
        };
    }
}
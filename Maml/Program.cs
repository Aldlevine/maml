// See https://aka.ms/new-console-template for more information

using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.System;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;

namespace Maml;

internal class NoReleaseSafeHandle : SafeHandle
{
    public NoReleaseSafeHandle(int value): base(IntPtr.Zero, true)
    {
        SetHandle(new IntPtr(value));
    }

    public NoReleaseSafeHandle(IntPtr value) : base(value, true) { }

    public override bool IsInvalid => throw new NotImplementedException();

    protected override bool ReleaseHandle()
    {
        return true;
    }
}


internal class Program
{
    private const string WindowClassName = "Maml";

    public static Size WindowSize { get; private set; } = new Size();

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
                wcex.lpfnWndProc = WndProc;
                wcex.cbClsExtra = 0;
                wcex.hInstance = GetModuleHandle(szNull);
                wcex.hCursor = LoadCursor(wcex.hInstance, szIconName);
                wcex.hbrBackground = new HBRUSH(new IntPtr(6));
                wcex.lpszClassName = szClassName;
                RegisterClassEx(wcex);
            }
        }
    }

    private static void InitInstance()
    {
        HWND hWnd;
        unsafe
        {
            hWnd = CreateWindowEx(
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
                null );
        }

        ShowWindow(hWnd, SHOW_WINDOW_CMD.SW_NORMAL);
        UpdateWindow(hWnd);
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
            case WM_SIZING:
            case WM_SIZE:
                {
                    int width = LoWord(lParam);
                    int height = HiWord(lParam);
                    SetWindowText(hWnd, $"Maml ({width}x{height})");
                    WindowSize = new Size(width, height);
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
                // Console.WriteLine("PAINT");
                unsafe
                {

                    HDC mainHdc = BeginPaint(hWnd, out PAINTSTRUCT ps);
                    if (mainHdc == HDC.Null)
                    {
                        throw new Exception();
                    }

                    HDC hdc = (HDC)CreateCompatibleDC(mainHdc).Value;
                    HBITMAP bmp = CreateBitmap(WindowSize.Width, WindowSize.Height, 1, 32);
                    HBITMAP mainBmp = new(SelectObject(hdc, (HGDIOBJ)bmp.Value).Value);

                    HBRUSH bgBrush = CreateSolidBrush(Color.DarkSlateBlue.ToColorRef());
                    if (FillRect(hdc, &ps.rcPaint, bgBrush) == 0)
                    {
                        Console.Error.WriteLine("FillRect() Failed");
                    }

                    string text = $"Hello World: {pointerPosition}";
                    fixed(char* pText = text)
                    {
                        SelectObject(hdc, (HGDIOBJ)DefaultFont().Value);

                        PWSTR pStr = new(pText);
                        SIZE textSize;
                        GetTextExtentPoint32W(hdc, pStr, text.Length, &textSize);
                        RECT textRect = new(4, 4, textSize.Width + 4, textSize.Height + 4);
                        
                        SetBkColor(hdc, Color.DarkSlateBlue.ToColorRef());
                        SetTextColor(hdc, Color.White.ToColorRef());
                        
                        if (DrawText(hdc, pStr, text.Length, &textRect, DRAW_TEXT_FORMAT.DT_TOP | DRAW_TEXT_FORMAT.DT_LEFT) == 0)
                        {
                            Console.Error.WriteLine("DrawText() Failed");
                        }
                    }

                    if (BitBlt(mainHdc, ps.rcPaint.X, ps.rcPaint.Y, ps.rcPaint.Width, ps.rcPaint.Height, hdc, 0, 0, ROP_CODE.SRCCOPY) == 0)
                    {
                        Console.Error.WriteLine("BitBlt() Failed");
                    }

                    EndPaint(hWnd, in ps);
                    ReleaseDC(hWnd, hdc);
                    DeleteObject((HGDIOBJ)bgBrush.Value);
                    SelectObject(hdc, (HGDIOBJ)mainBmp.Value);
                    DeleteObject((HGDIOBJ)bmp.Value);
                    return new LRESULT(0);
                }
        }

        return DefWindowProc(hWnd, msg, wParam, lParam);
    }

    unsafe private static HFONT DefaultFont()
    {
        string fontName = "Consolas";
        fixed(char* pFontName = fontName)
        {
            PWSTR fontNameStr = new PWSTR(pFontName);
            return CreateFont(
                0, 0,
                0, 0,
                400, 0, 0, 0,
                (uint)FONT_CHARSET.DEFAULT_CHARSET,
                FONT_OUTPUT_PRECISION.OUT_DEFAULT_PRECIS,
                FONT_CLIP_PRECISION.CLIP_DEFAULT_PRECIS,
                FONT_QUALITY.DEFAULT_QUALITY,
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
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
    private static short HiWord(int value) => (short)((value >> 16) & (0xffff));

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
            case WM_SIZE:
                {
                    int width = LoWord((int)lParam);
                    int height = HiWord((int)lParam);
                    SetWindowText(hWnd, $"Maml ({width}x{height})");
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
                    double delta = HiWord((int)wParam.Value) / 100;
                    Console.WriteLine("MOUSE WHEEL {0}", delta);
                    break;
                }
            case WM_KEYDOWN:
            case WM_SYSKEYDOWN:
                {
                    VirtualKey vKey = (VirtualKey)wParam.Value;
                    if ((vKey & ~(VirtualKey.Control | VirtualKey.Menu | VirtualKey.Shift)) > 0)
                    {
                        // Non-Modifier
                        Console.Write("KEYDOWN: ");
                        if ((GetAsyncKeyState((int)VirtualKey.Control) & 0xf000) > 0)
                        {
                            Console.Write(VirtualKey.Control + " + ");
                        }
                        if ((GetAsyncKeyState((int)VirtualKey.Menu) & 0xf000) > 0)
                        {
                            Console.Write(VirtualKey.Menu + " + ");
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
                    if ((vKey & ~(VirtualKey.Control | VirtualKey.Menu | VirtualKey.Shift)) > 0)
                    {
                        // Non-Modifier
                        Console.Write("KEYUP: ");
                        if ((GetAsyncKeyState((int)VirtualKey.Control) & 0xf000) > 0)
                        {
                            Console.Write(VirtualKey.Control + " + ");
                        }
                        if ((GetAsyncKeyState((int)VirtualKey.Menu) & 0xf000) > 0)
                        {
                            Console.Write(VirtualKey.Menu + " + ");
                        }
                        if ((GetAsyncKeyState((int)VirtualKey.Shift) & 0xf000) > 0)
                        {
                            Console.Write(VirtualKey.Shift + " + ");
                        }
                        Console.WriteLine(vKey);
                    }
                    return new LRESULT(0);
                }
            case WM_PAINT:
                unsafe
                {
                    PAINTSTRUCT ps;
                    HDC hdc = BeginPaint(hWnd, out ps);
                    if (hdc == HDC.Null)
                    {
                        throw new Exception();
                    }

                    HBRUSH bgBrush = CreateSolidBrush(ColorToRef(Color.DarkSlateBlue));

                    FillRect(hdc, &ps.rcPaint, bgBrush);

                    EndPaint(hWnd, in ps);
                    return new LRESULT(0);
                }
        }

        return DefWindowProc(hWnd, msg, wParam, lParam);
    }

    static COLORREF ColorToRef(Color color)
    {
        return new COLORREF((uint)((color.B << 16) | (color.G << 8) | (color.R << 0)));
    }
}

/*
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

unsafe class Window : IDisposable
{
    public IntPtr hWnd { get; private set; }

    private bool disposed;
    private WndProc wndProcDelegate;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
            }

            if (hWnd != IntPtr.Zero)
            {
                DestroyWindow(hWnd);
                hWnd = IntPtr.Zero;
            }
            disposed = true;
        }
    }

    public Window()
    {
        string className = "My Class Name";
        fixed (char* classNameC = className)
        {
            wndProcDelegate = GetWndProc;

            WNDCLASS wndClass = new();
            wndClass.lpszClassName = classNameC;
            wndClass.lpfnWndProc = wndProcDelegate;
            ushort classAtom = RegisterClass(ref wndClass);
            int lastError = Marshal.GetLastWin32Error();

            if (classAtom == 0 && lastError != (int)Win32ErrorCode.ERROR_CLASS_ALREADY_EXISTS)
            {
                throw new Exception("Could not register window class");
            }

            hWnd = CreateWindowEx(
                WindowStylesEx.WS_EX_APPWINDOW,
                className,
                String.Empty,
                WindowStyles.WS_BORDER | WindowStyles.WS_CAPTION | WindowStyles.WS_SYSMENU | WindowStyles.WS_MAXIMIZEBOX | WindowStyles.WS_MINIMIZEBOX | WindowStyles.WS_SIZEFRAME,
                100,
                100,
                400,
                300,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                (void*)IntPtr.Zero);

            ShowWindow(hWnd, WindowShowStyle.SW_SHOW);

        }
    }

    ~Window() => Dispose(false);

    private static short LoWord(int value) => (short)(value & (0xffff));
    private static short HiWord(int value) => (short)((value >> 16) & (0xffff));

    private static IntPtr GetWndProc(IntPtr hWnd, WindowMessage msg, void* wParam, void* lParam)
    {
        switch (msg)
        {
            case WindowMessage.WM_SIZE:
                {
                    int width = LoWord((int)lParam);
                    int height = HiWord((int)lParam);
                    SetWindowText(hWnd, $"Maml ({width}x{height})");
                    break;
                }
            case WindowMessage.WM_LBUTTONDOWN:
                {
                    Console.WriteLine("L BUTTON DOWN");
                    break;
                }
            case WindowMessage.WM_LBUTTONUP:
                {
                    Console.WriteLine("L BUTTON UP");
                    break;
                }
            case WindowMessage.WM_RBUTTONDOWN:
                {
                    Console.WriteLine("R BUTTON DOWN");
                    break;
                }
            case WindowMessage.WM_RBUTTONUP:
                {
                    Console.WriteLine("R BUTTON UP");
                    break;
                }
            case WindowMessage.WM_MBUTTONDOWN:
                {
                    Console.WriteLine("M BUTTON DOWN");
                    break;
                }
            case WindowMessage.WM_MBUTTONUP:
                {
                    Console.WriteLine("M BUTTON UP");
                    break;
                }
            case WindowMessage.WM_MOUSEWHEEL:
                {
                    double delta = HiWord((int)wParam) / 100;
                    Console.WriteLine("MOUSE WHEEL {0}", delta);
                    break;
                }
            case WindowMessage.WM_KEYDOWN:
            case WindowMessage.WM_SYSKEYDOWN:
                {
                    VirtualKey vKey = (VirtualKey)(int)wParam;
                    if ((vKey & ~(VirtualKey.VK_CONTROL | VirtualKey.VK_MENU | VirtualKey.VK_SHIFT)) > 0)
                    {
                        // Non-Modifier
                        Console.Write("KEYDOWN: ");
                        if ((GetAsyncKeyState((int)VirtualKey.VK_CONTROL) & 0xf000) > 0)
                        {
                            Console.Write(VirtualKey.VK_CONTROL + " + ");
                        }
                        if ((GetAsyncKeyState((int)VirtualKey.VK_MENU) & 0xf000) > 0)
                        {
                            Console.Write(VirtualKey.VK_MENU + " + ");
                        }
                        if ((GetAsyncKeyState((int)VirtualKey.VK_SHIFT) & 0xf000) > 0)
                        {
                            Console.Write(VirtualKey.VK_SHIFT + " + ");
                        }
                        Console.WriteLine(vKey);
                    }
                    return IntPtr.Zero;
                }
            case WindowMessage.WM_KEYUP:
            case WindowMessage.WM_SYSKEYUP:
                {
                    VirtualKey vKey = (VirtualKey)(int)wParam;

                    if ((vKey & ~(VirtualKey.VK_CONTROL | VirtualKey.VK_MENU | VirtualKey.VK_SHIFT)) > 0)
                    {
                        // Non-Modifier
                        Console.Write("KEYUP: ");
                        if ((GetAsyncKeyState((int)VirtualKey.VK_CONTROL) & 0xf000) > 0)
                        {
                            Console.Write(VirtualKey.VK_CONTROL + " + ");
                        }
                        if ((GetAsyncKeyState((int)VirtualKey.VK_MENU) & 0xf000) > 0)
                        {
                            Console.Write(VirtualKey.VK_MENU + " + ");
                        }
                        if ((GetAsyncKeyState((int)VirtualKey.VK_SHIFT) & 0xf000) > 0)
                        {
                            Console.Write(VirtualKey.VK_SHIFT + " + ");
                        }
                        Console.WriteLine(vKey);
                    }

                    break;
                }
            case WindowMessage.WM_PAINT:
                {
                    PAINTSTRUCT ps;
                    SafeDCHandle hdc = BeginPaint(hWnd, &ps);

                    // FillRect(hdc, &ps.rcPaint, Color.White);

                    EndPaint(hWnd, &ps);
                    return IntPtr.Zero;
                }
        }

        return DefWindowProc(hWnd, msg, (IntPtr)wParam, (IntPtr)lParam);
    }
}

static class Program
{
    private static Window? window;

    static void Main()
    {
        var app = new App();
        Console.WriteLine(app.Tree(4));

        window = new Window();

        MSG msg = new();

        unsafe
        {
            while (GetMessage(&msg, window.hWnd, 0, 0) > 0)
            {
                TranslateMessage(&msg);
                DispatchMessage(&msg);
            }
        }

        // Console.ReadLine();
    }
}
*/
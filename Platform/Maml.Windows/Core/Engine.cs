using System.Runtime.InteropServices;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.UI.WindowsAndMessaging;

using static Windows.Win32.PInvoke;

namespace Maml;

public class Engine : EngineBase<Window, RenderTarget>
{
	private static Engine? singleton { get; set; }
	public static Engine Singleton => singleton ?? (singleton = new Engine());

	#region Abstract
	public override Window Window { get; protected set; } = null!;

	public override void Run()
	{
		Animator.Frame += (s, e) => Window.Redraw(false);

		while (GetMessage(out MSG msg, default, 0, 0))
		{
			TranslateMessage(in msg);
			DispatchMessage(in msg);
			Animator.Tick();
		}

		Animator.Dispose();
	}

	unsafe public override void Initialize()
	{
		// Because we only care about pointer events
		EnableMouseInPointer(true);

		D2D1CreateFactory(D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_MULTI_THREADED, Marshal.GenerateGuidForType(typeof(ID2D1Factory)), null, out var obj).ThrowOnFailure();
		pD2DFactory = (ID2D1Factory*)obj;

		SetProcessDpiAwareness(Windows.Win32.UI.HiDpi.PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE);

		Window.RegisterWindowClass();
		Window = new Window();

		base.Initialize();
	}
	#endregion

	#region Internal
	unsafe internal ID2D1Factory* pD2DFactory { get; set; }

	unsafe ~Engine()
	{
		if (pD2DFactory != null)
		{
			pD2DFactory->Release();
			pD2DFactory = null;
		}
	}
	#endregion
}


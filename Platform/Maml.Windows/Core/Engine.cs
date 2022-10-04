using Maml.Animation;
using System.Runtime.InteropServices;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.UI.WindowsAndMessaging;

using static Windows.Win32.PInvoke;

namespace Maml;

public partial class Engine
{
	#region Abstract

	public override void Run()
	{
		while (GetMessage(out MSG msg, default, 0, 0))
		{
			TranslateMessage(in msg);
			DispatchMessage(in msg);
			Animator.Tick();
		}
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

		Animator.Frame += Frame;

		base.Initialize();
	}

	// This should process the scene tree and check for changes before rerawing
	private void Frame(object? sender, FrameEvent evt)
	{
		Window.Redraw(false);
	}

	public override void Dispose()
	{
		Animator.Dispose();
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


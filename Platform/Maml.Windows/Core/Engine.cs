using Maml.Animation;
using Maml.Math;
using System.Runtime.InteropServices;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.DirectWrite;
using Windows.Win32.System.Com;
using Windows.Win32.UI.WindowsAndMessaging;

using static Windows.Win32.PInvoke;

namespace Maml;

public partial class Engine
{
	#region Abstract

	public override void Run()
	{
		Window.PushUpdateRect(new Rect { Size = Window.Size, });

		while (true)
		{
			while (PeekMessage(out MSG msg, default, 0, 0, PEEK_MESSAGE_REMOVE_TYPE.PM_REMOVE))
			{
				if (msg.message == WM_QUIT)
				{
					return;
				}

				TranslateMessage(in msg);
				DispatchMessage(in msg);
			}

			Animator.Tick();
		}
	}

	unsafe public override void Initialize()
	{
		// Because we only care about pointer events
		EnableMouseInPointer(true);

		D2D1CreateFactory(D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_MULTI_THREADED, Marshal.GenerateGuidForType(typeof(ID2D1Factory)), null, out var pD2DFactoryUnk).ThrowOnFailure();
		pD2DFactory = (ID2D1Factory*)pD2DFactoryUnk;

		IUnknown* pDWriteFactoryUnk;
		DWriteCreateFactory(Windows.Win32.Graphics.DirectWrite.DWRITE_FACTORY_TYPE.DWRITE_FACTORY_TYPE_SHARED, Marshal.GenerateGuidForType(typeof(IDWriteFactory)), &pDWriteFactoryUnk).ThrowOnFailure();
		pDWriteFactory = (IDWriteFactory*)pDWriteFactoryUnk;

		SetProcessDpiAwareness(Windows.Win32.UI.HiDpi.PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE);

		Window.RegisterWindowClass();
		Window = new Window();

		Animator.LateFrame += Frame;

		base.Initialize();
	}

	private void Frame(object? sender, FrameEvent evt)
	{
		ProcessDeferred();
		Window.ComputeSceneUpdateRegion();
		Window.Redraw();

		if (!Animator.IsTicking)
		{
			UpdateWindow(Window.hWnd);
		}
	}

	public override void Dispose()
	{
		Animator.Dispose();
	}
	#endregion

	#region Internal
	unsafe internal ID2D1Factory* pD2DFactory { get; set; }
	unsafe internal IDWriteFactory* pDWriteFactory { get; set; }

	unsafe ~Engine()
	{
		if (pD2DFactory != null)
		{
			pD2DFactory->Release();
			pD2DFactory = null;
		}

		if (pDWriteFactory != null)
		{
			pDWriteFactory->Release();
			pDWriteFactory = null;
		}
	}
	#endregion
}


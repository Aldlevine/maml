using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32.UI.WindowsAndMessaging;

using static Windows.Win32.PInvoke;

namespace Maml;

public class Engine : EngineBase<Window, RenderTarget>
{
	// public override WindowBase Window { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
	public override Window Window { get; init; }

	public Engine()
	{
		Window = new Window();
	}

	public override void Run()
	{
		// Animator.Frame += (s, e) => Window.RenderTarget.Redraw(false);

		// HandleResize();

		while (GetMessage(out MSG msg, default, 0, 0))
		{
			TranslateMessage(in msg);
			DispatchMessage(in msg);
			Animator.Tick();
		}

		Animator.Dispose();
	}
}

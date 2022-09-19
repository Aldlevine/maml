using Maml.Events;
using Maml.UserInput;
using System;
using System.Reflection.Metadata;
using static Windows.Win32.PInvoke;

[assembly: MetadataUpdateHandler(typeof(Maml.Program))]

namespace Maml;

internal static class Program
{
	public static App? App;

	private static int Main(string[] args)
	{
		EnableMouseInPointer(true);

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

		Input.PointerMove += (PointerEvent evt) =>
		{
			Console.WriteLine(evt);
		};

		App = new();
		App.RunMessageLoop();

		return 0;
	}
}

// class App : Box
// {
//	 public App()
//	 {
//		 Name = "App";
//		 Content =
//		 new VBox
//		 {
//			 Content =
//			 {
//				 new Box
//				 {
//					 Name = "Header",
//					 Content = "Hello world!"
//				 },
//				 new Box
//				 {
//					 Name = "Body",
//					 Content = { "Goodnight", "Moon", }
//				 }
//			 }
//		 };
//	 }
// }
using System;
using System.Reflection.Metadata;
using static Windows.Win32.PInvoke;

[assembly: MetadataUpdateHandler(typeof(Maml.Program))]

namespace Maml;

// internal static class HotReloadManager
// {
// 	public static void ClearCache(Type[]? types)
// 	{
// 		_ = types;
// 	}
//
//	public static void UpdateApplication(Type[]? types) {}
// 
// }

internal static class Program
{
	public static App? App;

	private static int Main(string[] args)
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
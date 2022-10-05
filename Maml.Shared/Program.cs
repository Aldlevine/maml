namespace Maml;

internal static class Program
{
	private static Engine Engine { get; } = Engine.Singleton;

	private static void Main()
	{
		Engine.Initialize();
		Engine.Window.SceneTree.Root = new TestScene1();

//#if MAML_WASM
//		Engine.Window.Resize += (s, e) => System.Console.WriteLine(e);
//		Engine.Window.PointerMove += (s, e) => System.Console.WriteLine("Move {0}", e);
//		Engine.Window.PointerDown += (s, e) => System.Console.WriteLine("Down {0}", e);
//		Engine.Window.PointerUp += (s, e) => System.Console.WriteLine("Up {0}", e);
//#endif

		Engine.Run();
		Engine.Dispose();
	}
}

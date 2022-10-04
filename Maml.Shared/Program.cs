namespace Maml;

public static class Program
{
	internal static void Main()
	{
		Engine.Singleton.Initialize();

		Engine.Singleton.Window.SceneTree.Root = new TestScene1();

		Engine.Singleton.Window.Resize += (s, e) => System.Console.WriteLine(e);
		Engine.Singleton.Window.PointerMove += (s, e) => System.Console.WriteLine("Move {0}", e);
		Engine.Singleton.Window.PointerDown += (s, e) => System.Console.WriteLine("Down {0}", e);
		Engine.Singleton.Window.PointerUp += (s, e) => System.Console.WriteLine("Up {0}", e);

		Engine.Singleton.Run();

		Engine.Singleton.Dispose();
	}
}

namespace Maml;

public static class Program
{
	internal static void Main()
	{
		Engine.Singleton.Initialize();

		Engine.Singleton.Window.SceneTree.Root = new TestScene1();

		Engine.Singleton.Run();

		Engine.Singleton.Dispose();
	}
}

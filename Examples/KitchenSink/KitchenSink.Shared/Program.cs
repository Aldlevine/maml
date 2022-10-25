using Maml;

namespace KitchenSink;

internal static class Program
{
	private static Engine Engine { get; } = Engine.Singleton;

	private static void Main()
	{
		Engine.Initialize();
		Engine.Window.SceneTree.Root = new TestScene1();
		Engine.Run();
		Engine.Dispose();
	}
}

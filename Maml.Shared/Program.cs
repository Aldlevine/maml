using Maml.Math;
using System;

namespace Maml;

public static class Program2
{
	internal static void Main()
	{
		Engine.Singleton.Initialize();

		Engine.Singleton.Window!.SceneTree.Root = new Scene.Node
		{
			Name = "Root",
			Children = GetNodes(),
			Transform = Transform.PixelIdentity,
		};

		Engine.Singleton.Run();
	}

	private static Scene.Node.NodeCollection GetNodes()
	{
		Scene.Node.NodeCollection result = new()
		{
			new LineGrid(),
		};
		for (int i = 0; i < 100; i++)
		{
			result.Add(new TwirlyNode
			{
				Transform = new Math.Transform
				{
					Origin = new(
						Random.Shared.Next((int)Engine.Singleton.Window.Size.X),
						Random.Shared.Next((int)Engine.Singleton.Window.Size.Y)),
				}
			});
		}

		return result;
	}
}

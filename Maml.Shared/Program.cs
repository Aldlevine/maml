using Maml.Graphics;
using Maml.Math;
using Maml.Scene;
using System;

namespace Maml;

public static class Program
{
	internal static void Main()
	{
		Engine.Singleton.Initialize();

		if (Engine.Singleton.Window == null)
		{
			throw new ApplicationException("Unable to create window!");
		}

		Engine.Singleton.Window.SceneTree.Root = new Node
		{
			Name = "Root",
			Children = GetNodes(),
			// Transform = Transform.PixelIdentity,
		};

		Engine.Singleton.Run();
	}

	private static Node.NodeCollection GetNodes()
	{
		Node.NodeCollection result = new()
		{
			new LineGrid
			{
				MinorSpacing = new(20, 20),
				MajorInterval = new(5, 5),
				LineDrawLayersMajor = new()
				{
					new Stroke(new ColorBrush { Color = Colors.BlueViolet with { A = 0.25f } }, 3),
					new Stroke(new ColorBrush { Color = Colors.Lime with { A = 0.25f } }, 1),
				},
				LineDrawLayersMinor = new()
				{
					new Stroke(new ColorBrush { Color = Colors.BlueViolet with { A = 0.25f } }, 1),
				},
				Transform = Transform.PixelIdentity,
				["Hello"] = "World",
			},
			// new LineGrid
			// {
			// 	MinorSpacing = new(40, 40),
			// 	MajorInterval = new(10, 10),
			// },
		};
		for (int i = 0; i < 100; i++)
		{
			result.Add(new TwirlyNode
			{
				Transform = new Transform
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

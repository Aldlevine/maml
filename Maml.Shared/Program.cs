using Maml.Graphics;
using Maml.Math;
using Maml.Observable;
using Maml.Scene;
using System;

namespace Maml;

public static class Program
{
	private static Node testNode = new();

	internal static void Main()
	{
		Engine.Singleton.Initialize();

		Engine.Singleton.Window.SceneTree.Root = new Node
		{
			Name = "Root",
			Children = GetNodes(),
		};

		var startTick = DateTime.Now;
		Engine.Singleton.Animator.Frame += (s, e) =>
		{
			testNode.Transform = Transform.Identity
				.Translated(Engine.Singleton.Window.Size / -2)
				.Rotated(-(startTick - e.Tick).TotalSeconds)
				.Translated(Engine.Singleton.Window.Size / 2);
		};

		Engine.Singleton.Run();

		Engine.Singleton.Dispose();
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
			},
			new LineGrid
			{
				MinorSpacing = new(40, 40),
				MajorInterval = new(10, 10),
				[Node.TransformProperty] = testNode[Node.TransformProperty],
			},
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

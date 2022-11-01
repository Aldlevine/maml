using Maml.Math;
using Maml.Observable;

namespace Maml.Scene;

public class Box : Node
{
	public static BasicProperty<Box, Vector2> SizeProperty = new(Vector2.Zero);
	public Vector2 Size
	{
		get => SizeProperty[this].Get();
		set => SizeProperty[this].Set(value);
	}

	public static ComputedProperty<Box, Rect> RectProperty = new()
	{
		Get = (self) => new()
		{
			Size = self.Size,
		},
		Dependencies = (self) => new[]
		{
			SizeProperty[self],
		},
	};
	public Rect Rect
	{
		get => RectProperty[this].Get();
		//set => RectProperty[this].Set(value);
	}
}

public abstract class Container : Box
{
	public abstract void LayoutChildren();

	public Container() : base()
	{
		TreeChanged += (s, e) =>
		{
			Engine.QueueDeferred(LayoutChildren);
			//LayoutChildren();
		};
	}
}

public class VBox : Container
{
	public override void LayoutChildren()
	{
		double offset = 0;
		//double size = Size.X;
		foreach (var child in Children)
		{
			if (child is not Box b) { continue; }
			//b.Origin = new(0, offset);
			b.Transform = b.Transform with { Origin = new(0, offset), };
			offset += b.Size.Y;
		}
	}
}

public class HBox : Container
{
	public override void LayoutChildren()
	{
		throw new System.NotImplementedException();
	}
}

using Maml.Animation;
using Maml.Math;
using Maml.Observable;

namespace Maml.Scene;

public partial class Node : ObservableObject
{
	protected static Engine Engine => Engine.Singleton;
	protected static Window Window => Engine.Singleton.Window;
	protected static Animator Animator => Engine.Singleton.Animator;

	public Node()
	{
		Children.ParentNode = this;
	}

	private Node? parent;
	public Node? Parent
	{
		get => parent;
		private set
		{
			if (parent == value) { return; }

			if (parent != null)
			{
				GlobalTransformProperty[this].UndependOn(GlobalTransformProperty[parent]);
			}
			parent = value;
			if (parent != null)
			{
				GlobalTransformProperty[this].DependOn(GlobalTransformProperty[parent]);
			}
		}
	}
	public string Name { get; set; } = string.Empty;

	// TODO: Notify tree when child is added
	private NodeCollection children = new();
	public NodeCollection Children
	{
		get => children;
		init
		{
			children = value;
			children.ParentNode = this;
		}
	}
	public override string? ToString() => $"{GetType().Name}#{Name}";

	#region Properties
	public static BasicProperty<Node, int> ZIndexProperty = new(0);
	public int ZIndex
	{
		get => ZIndexProperty[this].Get();
		set => ZIndexProperty[this].Set(value);
	}

	public static BasicProperty<Node, IShape?> HitShapeProperty = new(null);
	public IShape? HitShape
	{
		get => HitShapeProperty[this].Get();
		set => HitShapeProperty[this].Set(value);
	}

	// ******** Local ********
	public static BasicProperty<Node, Transform> TransformProperty = new(Transform.Identity);
	public Transform Transform
	{
		get => TransformProperty[this].Get();
		set => TransformProperty[this].Set(value);
	}

	// Origin
	public static ComputedProperty<Node, Vector2> OriginProperty = new()
	{
		Get = (Node self) => self.Transform.Origin,
		Set = (Node self, Vector2 value) => self.Transform = self.Transform with { Origin = value, },
		Dependencies = (Node self) => new[] { TransformProperty[self], },
	};
	public Vector2 Origin
	{
		get => OriginProperty[this].Get();
		set => OriginProperty[this].Set(value);
	}

	// Rotation
	public static ComputedProperty<Node, double> RotationProperty = new()
	{
		Get = (Node self) => self.Transform.Rotation,
		Set = (Node self, double value) => self.Transform = self.Transform with { Rotation = value, },
		Dependencies = (Node self) => new[] { TransformProperty[self], },
	};
	public double Rotation
	{
		get => RotationProperty[this].Get();
		set => RotationProperty[this].Set(value);
	}

	// Scale
	public static ComputedProperty<Node, Vector2> ScaleProperty = new()
	{
		Get = (Node self) => self.Transform.Scale,
		Set = (Node self, Vector2 value) => self.Transform = self.Transform with { Scale = value, },
		Dependencies = (Node self) => new[] { TransformProperty[self], },
	};
	public Vector2 Scale
	{
		get => ScaleProperty[this].Get();
		set => ScaleProperty[this].Set(value);
	}

	// ******** Global ********
	public static ComputedProperty<Node, Transform> GlobalTransformProperty = new()
	{
		Get = (Node self) => self.getGlobalTransform(),
		Set = (Node self, Transform value) => self.setGlobalTransform(value),
		Dependencies = (Node self) => new[] { TransformProperty[self], },
	};
	public Transform GlobalTransform
	{
		get => GlobalTransformProperty[this].Get();
		set => GlobalTransformProperty[this].Set(value);
	}
	private Transform getGlobalTransform() => Parent switch
	{
		null => Transform,
		_ => Parent.GlobalTransform * Transform,
	};
	private void setGlobalTransform(Transform transform) => Transform = Parent switch
	{
		null => transform,
		_ => Parent.GlobalTransform.Inverse() * transform,
	};

	// GLobal Origin
	public static ComputedProperty<Node, Vector2> GlobalOriginProperty = new()
	{
		Get = (Node self) => self.Transform.Origin,
		Set = (Node self, Vector2 value) => self.Transform = self.Transform with { Origin = value, },
		Dependencies = (Node self) => new[] { GlobalTransformProperty[self], },
	};
	public Vector2 GlobalOrigin
	{
		get => GlobalOriginProperty[this].Get();
		set => GlobalOriginProperty[this].Set(value);
	}

	// Global Rotation
	public static ComputedProperty<Node, double> GlobalRotationProperty = new()
	{
		Get = (Node self) => self.Transform.Rotation,
		Set = (Node self, double value) => self.Transform = self.Transform with { Rotation = value, },
		Dependencies = (Node self) => new[] { GlobalTransformProperty[self], },
	};
	public double GlobalRotation
	{
		get => GlobalRotationProperty[this].Get();
		set => GlobalRotationProperty[this].Set(value);
	}

	// Global Scale
	public static ComputedProperty<Node, Vector2> GlobalScaleProperty = new()
	{
		Get = (Node self) => self.Transform.Scale,
		Set = (Node self, Vector2 value) => self.Transform = self.Transform with { Scale = value, },
		Dependencies = (Node self) => new[] { GlobalTransformProperty[self], },
	};
	public Vector2 GlobalScale
	{
		get => GlobalScaleProperty[this].Get();
		set => GlobalScaleProperty[this].Set(value);
	}
	#endregion
}

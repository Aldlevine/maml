using Maml.Animation;
using Maml.Math;
using Maml.Observable;
using System;

namespace Maml.Scene;

public partial class Node : ObservableObject
{
	protected static Engine Engine => Engine.Singleton;
	protected static Window Window => Engine.Singleton.Window;
	protected static Animator Animator => Engine.Singleton.Animator;

	public Node()
	{
		Children.ParentNode = this;
		TransformProperty[this].Changed += GlobalTransformProperty[this].SetDirty;
		GlobalTransformProperty[this].Changed += (s, e) =>
		{
			foreach (var child in Children)
			{
				GlobalTransformProperty[child].SetDirty(s, e);
			}
		};
	}

	public Node? Parent { get; private set; }
	public string Name { get; set; } = string.Empty;
	public int ZIndex { get; set; } = 0;
	public IShape? HitShape { get; set; } = default;

	// TODO: Notify tree when child is added
	// Children
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

	public void CallDeferred(Action action) => Animator.NextFrame += (s, e) => action.Invoke();

	// Transform
	public static ObservableProperty<Node, Transform> TransformProperty = new(Transform.Identity);
	public Transform Transform
	{
		get => TransformProperty[this].Get();
		set => TransformProperty[this].Set(value);
	}

	// GlobalTransform
	public static ComputedProperty<Node, Transform> GlobalTransformProperty = new()
	{
		Get = (Node self) => self.getGlobalTransform(),
		Set = (Node self, Transform value) => self.setGlobalTransform(value),
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

	// Origin
	public static ComputedProperty<Node, Vector2> OriginProperty = new()
	{
		Get = (Node self) => self.Transform.Origin,
		Set = (Node self, Vector2 value) => self.Transform = self.Transform with { Origin = value, },
	};
	public Vector2 Origin
	{
		get => OriginProperty[this].Get();
		set => OriginProperty[this].Set(value);
	}

	public override string? ToString() => $"{GetType().Name}#{Name}";
}

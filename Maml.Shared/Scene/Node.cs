using Maml.Animation;
using Maml.Events;
using Maml.Math;
using System;
using System.Collections.Generic;

namespace Maml.Scene;

public partial class Node: IChanged
{
	public event EventHandler<ChangedEvent>? Changed;
	public void RaiseChanged(object? sender, ChangedEvent e) => Changed?.Invoke(sender, e);

	protected Engine Engine => Engine.Singleton;
	protected Window Window => Engine.Singleton.Window;
	protected Animator Animator => Engine.Singleton.Animator;

	public string this[string idx]
	{
		get => idx;
		init => Console.WriteLine(idx, value);
	}

	public string Name { get; set; } = string.Empty;
	// TODO: Make dirty when this changes

	private Transform transform = Transform.Identity;
	public Transform Transform
	{
		get => transform;
		set
		{
			if (transform != value)
			{
				transform = value;
				markGlobalTransformDirty();
			}
		}
	}

	private Transform globalTransform = Transform.Identity;
	public Transform GlobalTransform
	{
		get => isGlobalTransformDirty switch
		{
			true => (globalTransform = getGlobalTransform()),
			false => globalTransform,
		};
		set => setGlobalTransform(value);
	}


	// TODO: Put this stuff in subclasses
	// TODO: Make dirty when this changes
	public int ZIndex { get; set; } = 0;

	public IShape? HitShape { get; set; } = default;

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

	public Node? Parent { get; private set; }

	public override string? ToString() => $"{GetType().Name}#{Name}";

	private Transform getGlobalTransform() =>
		(Parent?.getGlobalTransform() * Transform) ?? Transform;

	private void setGlobalTransform(Transform value) =>
		Transform = (Parent?.GlobalTransform.Inverse() * value) ?? value;

	private bool isGlobalTransformDirty = true;
	private void markGlobalTransformDirty(bool propagate = true)
	{
		isGlobalTransformDirty = true;
		isDirty = true;
		if (propagate)
		{
			foreach (var child in Children)
			{
				child.markGlobalTransformDirty(propagate);
			}
		}
	}

	internal bool isDirty = false;
}

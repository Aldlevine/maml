using Maml.Events;
using Maml.Math;
using System.Collections.Generic;

namespace Maml.Scene;

public class Node
{
	public virtual string Name { get; set; } = string.Empty;
	// TODO: Make dirty when this changes
	public virtual Transform Transform { get; set; } = Transform.Identity;
	// TODO: Make dirty when this changes
	public virtual int ZIndex { get; set; } = 0;

	// TODO: Make dirty when this changes
	public virtual List<GraphicComponent> Graphics { get; init; } = new();
	public virtual List<InputComponent> Inputs { get; init; } = new();

	// TODO: Notify tree when child is added
	public virtual List<Node> Children { get; init; } = new();

	public override string? ToString() => $"{GetType().Name}#{Name}";

	// public virtual void Initialize(InitEvent? e = null)
	// {
	// 	foreach (var c in Graphics)
	// 	{
	// 		((IComponent)c).Initialize();
	// 	}

	// 	foreach (var c in Inputs)
	// 	{
	// 		((IComponent)c).Initialize();
	// 	}
	// }
}

using Maml.Graphics;
using Maml.Math;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Maml.Scene;

public class SceneTree
{
	public Node? Root { get; set; } = new() { Name = "Root" };

	/// <summary>
	/// Returns nodes flattened in tree order
	/// </summary>
	public IEnumerable<Node> Nodes
	{
		get
		{
			if (Root != null)
			{
				return GetNodes(Root);
			}

			return Array.Empty<Node>();
		}
	}

	public IEnumerable<Node> GetNodes(Node root)
	{
		yield return root;
		foreach (var child in root.Children)
		{
			foreach (var childNodes in GetNodes(child))
			{
				yield return childNodes;
			}
		}

	}
}

public record struct GraphicInstance(Graphic Graphic, Transform Transform)
{
	public static implicit operator GraphicInstance(Graphic graphic) => new(graphic, Transform.Identity);
}

public class Node
{
	public virtual string Name { get; set; } = string.Empty;
	// TODO: Make dirty when this changes
	public virtual Transform Transform { get; set; } = Transform.Identity;
	// TODO: Make dirty when this changes
	public virtual int ZIndex { get; set; } = 0;

	// TODO: Make dirty when this changes
	public List<GraphicInstance> Graphics { get; init; } = new();

	// TODO: Notify tree when child is added
	public List<Node> Children { get; init; } = new();

	public override string? ToString() => $"{GetType().Name}#{Name}";
}

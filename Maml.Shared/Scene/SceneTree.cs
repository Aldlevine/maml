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

public record struct NodeGraphic(Graphic Graphic)
{
	public Transform Transform { get; init; } = Transform.Identity;

	public static implicit operator NodeGraphic(Graphic graphic) => new(graphic);
}

public class Node
{
	public virtual string Name { get; set; } = string.Empty;
	// TODO: Make dirty when this changes
	public virtual Transform Transform { get; set; } = Transform.Identity;
	// TODO: Make dirty when this changes
	public virtual int ZIndex { get; set; } = 0;

	// TODO: Make dirty when this changes
	public List<NodeGraphic> Graphics { get; init; } = new();

	// TODO: Notify tree when child is added
	public List<Node> Children { get; init; } = new();

	public override string? ToString() => $"{GetType().Name}#{Name}";
}

// public class NodeGraphicCollection : List<NodeGraphic>
// {
// 	public void Add(Graphic graphic)
// 	{
// 		Add(new NodeGraphic(graphic));
// 	}
// }

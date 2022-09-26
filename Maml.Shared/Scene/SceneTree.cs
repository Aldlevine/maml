using Maml.Events;
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
	public event EventHandler<InitEvent>? Initialized;

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

using System;
using System.Collections.Generic;

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
			foreach (var childNode in GetNodes(child))
			{
				yield return childNode;
			}
		}
	}

	public void Draw(RenderTarget renderTarget)
	{
		//var start = DateTime.Now;
		foreach (var node in Nodes)
		{
			if (node is GraphicNode graphicNode && graphicNode.VisibleInTree)
			{
				graphicNode.Draw(renderTarget);
			}
		}
		//Console.WriteLine("SceneTree.Draw Took: {0}ms", (DateTime.Now - start).TotalMilliseconds);
	}
}

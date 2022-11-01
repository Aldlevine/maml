using Maml.Math;
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
				return GetNodes(Root, Root.Content);
			}

			return Array.Empty<Node>();
		}
	}

	public IEnumerable<Node> GetNodes(Node root, Node.NodeCollection children)
	{
		yield return root;
		//foreach (var child in root.Children)
		foreach (var child in children)
		{
			if (child is ChildrenPlaceholder cp)
			{
				foreach (var childNode in GetNodes(cp.Self, cp.Self.Children))
				{
					if (childNode == child) { continue; }
					yield return childNode;
				}
			}
			else
			{
				foreach (var childNode in GetNodes(child, child.Content))
				{
					yield return childNode;
				}
			}
		}
	}

	public void Draw(RenderTarget renderTarget, Rect updateRegion)
	//public void Draw(RenderTarget renderTarget, IEnumerable<Rect> updateRegion)
	{
		foreach (var node in Nodes)
		{
			if (node is IGraphicNode graphicNode)
			{
				if (node.VisibleInTree && graphicNode.GetBoundingRect().Intersects(updateRegion))
				{
					graphicNode.Draw(renderTarget);
					graphicNode.NeedsRedraw = false;
				}
			}
		}
	}
}

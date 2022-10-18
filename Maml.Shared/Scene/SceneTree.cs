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

	//internal Rect updateRegion = new Rect();
	//public void PushRect(Rect rect)
	//{
	//	if (rect.Size == Vector2.Zero)
	//	{
	//		return;
	//	}

	//	if (updateRegion.Size == Vector2.Zero)
	//	{
	//		updateRegion = rect;
	//	}
	//	else
	//	{
	//		updateRegion = updateRegion.MergedWith(rect);
	//	}
	//}

	//public Rect ComputeUpdateRegion()
	//{
	//	foreach (var node in Nodes)
	//	{
	//		if (node is GraphicNode graphicNode && graphicNode.NeedsRedraw)
	//		{
	//			PushRect(graphicNode.PreviousBoundingRect);
	//			PushRect(graphicNode.PreviousBoundingRect = graphicNode.GetBoundingRect());
	//		}
	//	}
	//	return updateRegion;
	//}

	public void Draw(RenderTarget renderTarget, Rect updateRegion)
	{
		foreach (var node in Nodes)
		{
			if (node is GraphicNode graphicNode && graphicNode.VisibleInTree)
			{
				if (graphicNode.GetBoundingRect().Intersects(updateRegion))
				{
					graphicNode.Draw(renderTarget);
					graphicNode.NeedsRedraw = false;
				}
			}
		}
		//updateRegion = new();
	}
}

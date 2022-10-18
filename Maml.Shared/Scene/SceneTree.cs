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

	internal Rect updateRegion = new Rect();
	public Rect ComputeUpdateRegion()
	{
		//updateRegion = new();
		foreach (var node in Nodes)
		{
			if (node is GraphicNode graphicNode && graphicNode.NeedsRedraw)
			{
				if (updateRegion.Size == Vector2.Zero)
				{
					updateRegion = graphicNode.PreviousBoundingRect;
				}
				else
				{
					updateRegion = updateRegion.MergedWith(graphicNode.PreviousBoundingRect);
				}

				if (updateRegion.Size == Vector2.Zero)
				{
					//updateRegion = previousBoundingRects[graphicNode] = graphicNode.GetBoundingRect();
					graphicNode.PreviousBoundingRect = updateRegion = graphicNode.GetBoundingRect();
				}
				else
				{
					//updateRegion = updateRegion.MergedWith(previousBoundingRects[graphicNode] = graphicNode.GetBoundingRect());
					updateRegion = updateRegion.MergedWith(graphicNode.PreviousBoundingRect = graphicNode.GetBoundingRect());
				}
			}
		}
		return updateRegion;
	}

	public void Draw(RenderTarget renderTarget)
	{
		//var start = DateTime.Now;
		foreach (var node in Nodes)
		{
			if (node is GraphicNode graphicNode && graphicNode.VisibleInTree)
			{
				if (graphicNode.GetBoundingRect().Intersects(updateRegion))
				{
					graphicNode.Draw(renderTarget);
					graphicNode.NeedsRedraw = false;
				}
				//if (graphicNode.NeedsRedraw)
				//{
				//graphicNode.Draw(renderTarget);
				//graphicNode.NeedsRedraw = false;
				//}
			}
		}
		updateRegion = new Rect();
		//Console.WriteLine("SceneTree.Draw Took: {0}ms", (DateTime.Now - start).TotalMilliseconds);
	}
}

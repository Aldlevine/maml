using Maml.Events;
using Maml.Math;
using System.Collections;
using System.Collections.Generic;

namespace Maml.Scene;

public partial class Node
{
	public string Name { get; set; } = string.Empty;
	// TODO: Make dirty when this changes
	public Transform Transform { get; set; } = Transform.Identity;
	public Transform GlobalTransform
	{
		get => getGlobalTransform();
		set => setGlobalTransform(value);
	}

	// TODO: Put this stuff in subclasses
	// TODO: Make dirty when this changes
	public int ZIndex { get; set; } = 0;
	public List<GraphicComponent> Graphics { get; init; } = new();

	public Rect HitRect { get; set; } = default;
	// public InputComponent Input { get; init; } = new();

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

	// TODO: CACHE THIS MFER
	private Transform getGlobalTransform()
	{
		return (Parent?.getGlobalTransform() * Transform) ?? Transform;
	}

	private void setGlobalTransform(Transform value)
	{
		Transform = (Parent?.GlobalTransform.Inverse() * value) ?? value;
	}
}

partial class Node
{
	public partial class NodeCollection : IList<Node>
	{
		private List<Node> list { get; init; } = new();
		private Node? parentNode { get; set; }

		private void ReparentNode(Node node)
		{
			node.Parent?.Children.Remove(node);
			node.Parent = ParentNode;
		}

		public Node? ParentNode
		{
			get => parentNode;
			set
			{
				if (parentNode != value)
				{
					parentNode = value;
					foreach (var node in list)
					{
						ReparentNode(node);
					}
				}
			}
		}

		public Node this[int index]
		{
			get => ((IList<Node>)list)[index];
			set
			{
				ReparentNode(value);
				((IList<Node>)list)[index] = value;
			}
		}

		public int Count => ((ICollection<Node>)list).Count;

		public bool IsReadOnly => ((ICollection<Node>)list).IsReadOnly;

		public void Add(Node item)
		{
			ReparentNode(item);
			((ICollection<Node>)list).Add(item);
		}

		public void Clear() => ((ICollection<Node>)list).Clear();
		public bool Contains(Node item) => ((ICollection<Node>)list).Contains(item);
		public void CopyTo(Node[] array, int arrayIndex) => ((ICollection<Node>)list).CopyTo(array, arrayIndex);
		public IEnumerator<Node> GetEnumerator() => ((IEnumerable<Node>)list).GetEnumerator();
		public int IndexOf(Node item) => ((IList<Node>)list).IndexOf(item);
		public void Insert(int index, Node item)
		{
			ReparentNode(item);
			((IList<Node>)list).Insert(index, item);
		}

		public bool Remove(Node item)
		{
			item.Parent = null;
			return ((ICollection<Node>)list).Remove(item);
		}

		public void RemoveAt(int index)
		{
			list[index].Parent = null;
			((IList<Node>)list).RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)list).GetEnumerator();
	}
}

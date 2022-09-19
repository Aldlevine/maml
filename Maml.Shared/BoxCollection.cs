using System.Collections;
using System.Collections.Generic;

namespace Maml;

public class BoxCollection : IEnumerable<Box>
{
	private List<Box> Boxes { get; set; } = new();
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public IEnumerator<Box> GetEnumerator()
	{
		return Boxes.GetEnumerator();
	}

	public void Add(Box child)
	{
		Boxes.Add(child);
	}

	public void Add(string str)
	{
		Boxes.Add(new TextBox { Text = str });
	}

	public int Count => Boxes.Count;

	public static implicit operator BoxCollection(Box box)
	{
		return new() { box };
	}

	public static implicit operator BoxCollection(string str)
	{
		return new() { str };
	}
}

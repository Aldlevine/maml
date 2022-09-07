// See https://aka.ms/new-console-template for more information

using System.Collections;
using System.Collections.Generic;

namespace Maml;

public class BoxCollection : IEnumerable<Box>
{
    private List<Box> Boxes { get; set; } = new();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<Box> GetEnumerator() => Boxes.GetEnumerator();

    public void Add(Box child) => Boxes.Add(child);
    public void Add(string str) => Boxes.Add(new TextBox { Text = str });
    public int Count => Boxes.Count;

    public static implicit operator BoxCollection(Box box) => new() { box };
    public static implicit operator BoxCollection(string str) => new() { str };
}

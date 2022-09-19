using System.Text;

namespace Maml;

public class Box
{
	public string Name { get; set; } = "";
	public BoxCollection Content { get; set; } = new();

	public static implicit operator Box(string str)
	{
		return new TextBox { Text = str };
	}

	public string Tree(int indentSize = 2, int indent = 0)
	{
		string indentString = new(' ', indent * indentSize);
		StringBuilder sb = new();
		_ = sb.AppendLine(indentString + GetType().Name);
		if (Content.Count > 0)
		{
			_ = sb.AppendLine(indentString + "{");
			foreach (Box box in Content)
			{
				_ = sb.AppendLine(box.Tree(indentSize, indent + 1));
			}
			_ = sb.AppendLine(indentString + "}");
		}

		return sb.ToString().TrimEnd();
	}
}

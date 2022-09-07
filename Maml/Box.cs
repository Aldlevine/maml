// See https://aka.ms/new-console-template for more information

using System.Text;

namespace Maml;

public class Box
{
    public string Name { get; set; } = "";
    public BoxCollection Content { get; set; } = new();

    public static implicit operator Box(string str) => new TextBox { Text = str };

    public string Tree(int indentSize = 2, int indent = 0)
    {
        var indentString = new string(' ', indent * indentSize);
        var sb = new StringBuilder();
        sb.AppendLine(indentString + GetType().Name);
        if (Content.Count > 0)
        {
            sb.AppendLine(indentString + "{");
            foreach (Box box in Content)
            {
                sb.AppendLine(box.Tree(indentSize, indent + 1));
            }
            sb.AppendLine(indentString + "}");
        }

        return sb.ToString().TrimEnd();
    }
}

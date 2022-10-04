using Maml.Graphics;
using Maml.Observable;

namespace Maml.Scene;

public class GraphicNode : Node
{
	public static BasicProperty<GraphicNode, Graphic?> GraphicProperty { get; } = new(null);
	public Graphic? Graphic
	{
		get => GraphicProperty[this].Get();
		set => GraphicProperty[this].Set(value);
	}
}

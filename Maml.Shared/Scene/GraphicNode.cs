using Maml.Graphics;

namespace Maml.Scene;

public class GraphicNode : Node
{
	private Graphic? graphic;
	public Graphic? Graphic
	{
		get => graphic;
		set
		{
			if (graphic == value) return;
			graphic = value;
		}
	}
}

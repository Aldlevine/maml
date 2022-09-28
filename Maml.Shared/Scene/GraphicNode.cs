using Maml.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maml.Scene;

public class GraphicNode: Node
{
	private Graphic? graphic;
	public Graphic? Graphic
	{
		get => graphic;
		set
		{
			if (graphic == value) return;
			if (graphic != null) { graphic.Changed -= RaiseChanged; }
			graphic = value;
			if (graphic != null) { graphic.Changed += RaiseChanged; }
		}
	}
}

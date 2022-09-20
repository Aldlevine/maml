using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maml.Math;
public partial struct Line
{
	public Vector2 Start { get; set; } = Vector2.Zero;
	public Vector2 End { get; set; } = Vector2.Zero;
	public Line() { }
}

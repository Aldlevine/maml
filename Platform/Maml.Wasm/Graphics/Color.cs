using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maml.Graphics;
public partial struct Color
{
	internal string ToCSSColor() => $"rgba({R * 255}, {G * 255}, {B * 255}, {A})";
}

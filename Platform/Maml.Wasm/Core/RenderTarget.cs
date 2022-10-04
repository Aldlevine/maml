using Maml.Graphics;
using Maml.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maml;
public partial class RenderTarget
{
	public override void Clear(Color color) => throw new NotImplementedException();
	public override void DrawGeometry(Geometry geometry, Fill fill) => throw new NotImplementedException();
	public override void DrawGeometry(Geometry geometry, Stroke stroke) => throw new NotImplementedException();
	public override Transform GetTransform() => throw new NotImplementedException();
	public override void SetTransform(Transform transform) => throw new NotImplementedException();
}

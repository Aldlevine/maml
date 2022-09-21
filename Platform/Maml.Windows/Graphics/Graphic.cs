using Maml.Math;

using Windows.Win32.Graphics.Direct2D;

namespace Maml.Graphics;

unsafe public abstract partial class Graphic
{
	internal abstract void Draw(ID2D1RenderTarget* pRenderTarget);
}

public partial class GeometryGraphic
{
	unsafe override internal void Draw(ID2D1RenderTarget* pRenderTarget)
	{
		pRenderTarget->GetTransform(out var curD2DXform);
		Transform curXform = new(curD2DXform);
		pRenderTarget->SetTransform(curXform.Transformed(Transform).ToD2DMatrix3X2F());

		foreach (var layer in DrawLayers)
		{
			switch (layer)
			{
				case Fill l:
					DrawLayer(pRenderTarget, l);
					break;
				case Stroke l:
					DrawLayer(pRenderTarget, l);
					break;
			}
		}
		pRenderTarget->SetTransform(curD2DXform);
	}

	unsafe private void DrawLayer(ID2D1RenderTarget* pRenderTarget, Fill fill)
	{
		switch (Geometry)
		{
			case RectGeometry g:
				pRenderTarget->FillRectangle(g.Rect.ToD2DRectF(), fill.Brush.GetResource(pRenderTarget));
				break;
			case EllipseGeometry g:
				pRenderTarget->FillEllipse(g.Ellipse.ToD2DEllipse(), fill.Brush.GetResource(pRenderTarget));
				break;
			case LineGeometry g:
				// We can't fill line geometry
				break;
		}
	}

	unsafe private void DrawLayer(ID2D1RenderTarget* pRenderTarget, Stroke stroke)
	{
		switch (Geometry)
		{
			case RectGeometry g:
				pRenderTarget->DrawRectangle(g.Rect.ToD2DRectF(), stroke.Brush.GetResource(pRenderTarget), stroke.Thickness, default);
				break;
			case EllipseGeometry g:
				pRenderTarget->DrawEllipse(g.Ellipse.ToD2DEllipse(), stroke.Brush.GetResource(pRenderTarget), stroke.Thickness, default);
				break;
			case LineGeometry g:
				pRenderTarget->DrawLine(g.Line.Start.ToD2DPoint2F(), g.Line.End.ToD2DPoint2F(), stroke.Brush.GetResource(pRenderTarget), stroke.Thickness, default);
				break;
		}
	}
}



using Maml.Graphics;
using Maml.Math;
using System;
using Windows.Win32.Graphics.Direct2D;

namespace Maml;

public partial class RenderTarget: IDisposable
{
	#region Abstract
	unsafe public override void Clear(Color color) => pRenderTarget->Clear(color.ToD2DColorF());

	unsafe public override void DrawGeometry(Geometry geometry, Fill fill)
	{
		switch (geometry)
		{
			case RectGeometry g:
				pRenderTarget->FillRectangle(g.Rect.ToD2DRectF(), fill.Brush.GetResource((ID2D1RenderTarget*)pRenderTarget));
				break;
			case EllipseGeometry g:
				pRenderTarget->FillEllipse(g.Ellipse.ToD2DEllipse(), fill.Brush.GetResource((ID2D1RenderTarget*)pRenderTarget));
				break;
			case LineGeometry g:
				// We can't fill line geometry
				break;
		}
	}

	unsafe public override void DrawGeometry(Geometry geometry, Stroke stroke)
	{
		switch (geometry)
		{
			case RectGeometry g:
				pRenderTarget->DrawRectangle(g.Rect.ToD2DRectF(), stroke.Brush.GetResource((ID2D1RenderTarget*)pRenderTarget), stroke.Thickness, default);
				break;
			case EllipseGeometry g:
				pRenderTarget->DrawEllipse(g.Ellipse.ToD2DEllipse(), stroke.Brush.GetResource((ID2D1RenderTarget*)pRenderTarget), stroke.Thickness, default);
				break;
			case LineGeometry g:
				pRenderTarget->DrawLine(g.Line.Start.ToD2DPoint2F(), g.Line.End.ToD2DPoint2F(), stroke.Brush.GetResource((ID2D1RenderTarget*)pRenderTarget), stroke.Thickness, default);
				break;
		}
	}
	unsafe public override Transform GetTransform()
	{
		pRenderTarget->GetTransform(out var matrix);
		return new(matrix);
	}
	unsafe public override void SetTransform(Transform transform) => pRenderTarget->SetTransform(transform.ToD2DMatrix3X2F());

	#endregion

	#region Internal
	unsafe internal ID2D1RenderTarget* pRenderTarget;
	private bool disposedValue;

	unsafe internal RenderTarget(ID2D1RenderTarget* pRenderTarget)
	{
		this.pRenderTarget = pRenderTarget;
	}

	unsafe protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				// TODO: dispose managed state (managed objects)
			}

			if (pRenderTarget != null)
			{
				pRenderTarget->Release();
			}
			disposedValue = true;
		}
	}

	~RenderTarget()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
	#endregion
}

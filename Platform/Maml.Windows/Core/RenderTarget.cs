using Maml.Graphics;
using Maml.Math;
using System;
using System.Collections.Generic;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Direct2D.Common;
using Windows.Win32.Graphics.DirectWrite;

namespace Maml;

public partial class RenderTarget : IDisposable
{
	#region Abstract
	public override void BeginDraw() { }
	public override void EndDraw() { }
	unsafe public override void Clear(Color color) => pRenderTarget->Clear(color.ToD2DColorF());
	unsafe public override void ClearRect(Rect rect, Color color)
	{
		Brush brush = new ColorBrush { Color = color, };
		pRenderTarget->FillRectangle(rect.ToD2DRectF(), brush.GetResource(pRenderTarget));
	}
	unsafe public override void SetTransform(Transform transform) => pRenderTarget->SetTransform(transform.ToD2DMatrix3X2F());
	unsafe public override void DrawGeometry(Geometry geometry, Fill fill)
	{
		switch (geometry)
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
			default:
				pRenderTarget->FillGeometry(geometry.GetResource(Engine.Singleton), fill.Brush.GetResource(pRenderTarget));
				break;
		}
	}

	// TODO: Support stroke style
	unsafe public override void DrawGeometry(Geometry geometry, Stroke stroke)
	{
		switch (geometry)
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
			default:
				pRenderTarget->DrawGeometry(geometry.GetResource(Engine.Singleton), stroke.Brush.GetResource(pRenderTarget), stroke.Thickness, default);
				break;
		}
	}

	unsafe public override void DrawText(Text text, Brush brush)
	{
		var resource = text.GetResource(Engine.Singleton);
		pRenderTarget->DrawTextLayout(default, resource, brush.GetResource(pRenderTarget), D2D1_DRAW_TEXT_OPTIONS.D2D1_DRAW_TEXT_OPTIONS_ENABLE_COLOR_FONT);
	}

	unsafe public override void PushClip(Rect rect)
	{
		pRenderTarget->PushAxisAlignedClip(rect.ToD2DRectF(), D2D1_ANTIALIAS_MODE.D2D1_ANTIALIAS_MODE_ALIASED);
	}


	unsafe public override void PopClip()
	{
		pRenderTarget->PopAxisAlignedClip();
	}

	public override unsafe void PushLayer(Rect[] rect)
	{
		ID2D1Layer* pLayer;
		pRenderTarget->CreateLayer((D2D_SIZE_F?)null, &pLayer);

		ID2D1Geometry*[] geometries = new ID2D1Geometry*[rect.Length];
		//foreach (var r in rect)
		for (int i = 0; i < rect.Length; i++)
		{
			ID2D1RectangleGeometry* geo;
			Engine.Singleton.pD2DFactory->CreateRectangleGeometry(rect[i].ToD2DRectF(), &geo).ThrowOnFailure();
			geometries[i] = (ID2D1Geometry*)geo;
		}
		ID2D1GeometryGroup* geometryGroup;
		fixed (ID2D1Geometry** pGeometries = geometries)
		{
			Engine.Singleton.pD2DFactory->CreateGeometryGroup(Windows.Win32.Graphics.Direct2D.Common.D2D1_FILL_MODE.D2D1_FILL_MODE_ALTERNATE, pGeometries, (uint)geometries.Length, &geometryGroup).ThrowOnFailure();
		}
		geometryGroup->GetBounds(Transform.Identity.ToD2DMatrix3X2F(), out var bounds);
		D2D1_LAYER_PARAMETERS layerParams = new()
		{
			geometricMask = (ID2D1Geometry*)geometryGroup,
			contentBounds = bounds,
			layerOptions = D2D1_LAYER_OPTIONS.D2D1_LAYER_OPTIONS_INITIALIZE_FOR_CLEARTYPE,
			maskAntialiasMode = D2D1_ANTIALIAS_MODE.D2D1_ANTIALIAS_MODE_ALIASED,
			maskTransform = Transform.Identity.ToD2DMatrix3X2F(),
			opacity = 1,
			opacityBrush = null,
		};
		pRenderTarget->PushLayer(layerParams, null);

		//pLayer->Release();
		geometryGroup->Release();
		foreach (var geometry in geometries)
		{
			geometry->Release();
		}
	}

	public override unsafe void PopLayer()
	{
		pRenderTarget->PopLayer();
	}

	#endregion

	#region Internal
	unsafe internal ID2D1RenderTarget* pRenderTarget;
	private bool disposedValue;

	unsafe internal RenderTarget(ID2D1RenderTarget* pRenderTarget)
	{
		this.pRenderTarget = pRenderTarget;

		// setup text rendering params
		IDWriteRenderingParams* pRenderingParams;
		Engine.Singleton.pDWriteFactory->CreateRenderingParams(&pRenderingParams);

		// TODO: Should we make rendering mode configurable, or just use defaults?
		//IDWriteRenderingParams* pRenderingParams;
		//IDWriteRenderingParams* pDefaultRenderingParams;
		//Engine.Singleton.pDWriteFactory->CreateRenderingParams(&pDefaultRenderingParams);
		//Engine.Singleton.pDWriteFactory->CreateCustomRenderingParams(
		//	pDefaultRenderingParams->GetGamma(),
		//	pDefaultRenderingParams->GetEnhancedContrast(),
		//	pDefaultRenderingParams->GetClearTypeLevel(),
		//	pDefaultRenderingParams->GetPixelGeometry(),
		//	//DWRITE_RENDERING_MODE.DWRITE_RENDERING_MODE_ALIASED,
		//	//DWRITE_RENDERING_MODE.DWRITE_RENDERING_MODE_CLEARTYPE_GDI_CLASSIC,
		//	//DWRITE_RENDERING_MODE.DWRITE_RENDERING_MODE_CLEARTYPE_GDI_NATURAL,
		//	//DWRITE_RENDERING_MODE.DWRITE_RENDERING_MODE_CLEARTYPE_NATURAL,
		//	//DWRITE_RENDERING_MODE.DWRITE_RENDERING_MODE_CLEARTYPE_NATURAL_SYMMETRIC,
		//	DWRITE_RENDERING_MODE.DWRITE_RENDERING_MODE_DEFAULT,
		//	//DWRITE_RENDERING_MODE.DWRITE_RENDERING_MODE_GDI_CLASSIC,
		//	//DWRITE_RENDERING_MODE.DWRITE_RENDERING_MODE_GDI_NATURAL,
		//	//DWRITE_RENDERING_MODE.DWRITE_RENDERING_MODE_NATURAL,
		//	//DWRITE_RENDERING_MODE.DWRITE_RENDERING_MODE_NATURAL_SYMMETRIC,
		//	//DWRITE_RENDERING_MODE.DWRITE_RENDERING_MODE_OUTLINE,
		//	&pRenderingParams);
		//pDefaultRenderingParams->Release();

		pRenderTarget->SetTextRenderingParams(pRenderingParams);
		pRenderingParams->Release();

		pRenderTarget->SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE.D2D1_TEXT_ANTIALIAS_MODE_CLEARTYPE);
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

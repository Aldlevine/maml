using Windows.Win32.Graphics.Direct2D;

namespace Maml.Graphics;

unsafe public abstract partial class Geometry : Resource
{
	internal ID2D1Geometry* pResource;
	// internal abstract void MakeResource(ID2D1Factory* pFactory);
	internal abstract void MakeResource(Engine engine);
	// internal ID2D1Geometry* GetResource(ID2D1Factory* pFactory, bool noCache = false)
	// internal ID2D1Geometry* GetResource(ID2D1Factory* pFactory, bool noCache = false)
	internal ID2D1Geometry* GetResource(Engine engine, bool noCache = false)
	{
		if (pResource == null)
		{
			MakeResource(engine);
		}
		else
		{
			if (noCache || IsDirty)
			{
				FreeResources();
				MakeResource(engine);
			}
		}
		return pResource;
	}
	protected override void FreeResources()
	{
		pResource->Release();
	}
}

unsafe public partial class RectGeometry : Geometry
{
	internal override void MakeResource(Engine engine)
	{
		fixed (ID2D1Geometry** ppResource = &pResource)
		{
			engine.pD2DFactory->CreateRectangleGeometry(Rect.ToD2DRectF(), (ID2D1RectangleGeometry**)ppResource);
		}
	}
}

unsafe public partial class EllipseGeometry : Geometry
{
	internal override void MakeResource(Engine engine)
	{
		fixed (ID2D1Geometry** ppResource = &pResource)
		{
			engine.pD2DFactory->CreateEllipseGeometry(Ellipse.ToD2DEllipse(), (ID2D1EllipseGeometry**)ppResource);
		}
	}
}

unsafe public partial class LineGeometry : Geometry
{
	internal override void MakeResource(Engine engine)
	{
		fixed (ID2D1Geometry** ppResource = &pResource)
		{
			engine.pD2DFactory->CreatePathGeometry((ID2D1PathGeometry**)ppResource);
		}
		ID2D1GeometrySink* pSink;
		((ID2D1PathGeometry*)pResource)->Open(&pSink);
		pSink->BeginFigure(Line.Start.ToD2DPoint2F(), Windows.Win32.Graphics.Direct2D.Common.D2D1_FIGURE_BEGIN.D2D1_FIGURE_BEGIN_HOLLOW);
		pSink->AddLine(Line.End.ToD2DPoint2F());
		pSink->EndFigure(Windows.Win32.Graphics.Direct2D.Common.D2D1_FIGURE_END.D2D1_FIGURE_END_OPEN);
		pSink->Close().ThrowOnFailure();
		pSink->Release();
	}
}

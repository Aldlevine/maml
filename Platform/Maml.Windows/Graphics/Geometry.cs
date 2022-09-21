using Windows.Win32.Graphics.Direct2D;

namespace Maml.Graphics;

unsafe public abstract partial class Geometry : Resource
{
	internal ID2D1Geometry* pResource;
	internal abstract void MakeResource(ID2D1Factory* pFactory);
	internal ID2D1Geometry* GetResource(ID2D1Factory* pFactory, bool noCache = false)
	{
		if (pResource == null)
		{
			MakeResource(pFactory);
		}
		else
		{
			if (noCache || IsDirty)
			{
				FreeResources();
				MakeResource(pFactory);
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
	internal override void MakeResource(ID2D1Factory* pFactory)
	{
		fixed (ID2D1Geometry** ppResource = &pResource)
		{
			pFactory->CreateRectangleGeometry(Rect.ToD2DRectF(), (ID2D1RectangleGeometry**)ppResource);
		}
	}
}

unsafe public partial class EllipseGeometry : Geometry
{
	internal override void MakeResource(ID2D1Factory* pFactory)
	{
		fixed (ID2D1Geometry** ppResource = &pResource)
		{
			pFactory->CreateEllipseGeometry(Ellipse.ToD2DEllipse(), (ID2D1EllipseGeometry**)ppResource);
		}
	}
}

unsafe public partial class LineGeometry : Geometry
{
	internal override void MakeResource(ID2D1Factory* pFactory)
	{
		fixed (ID2D1Geometry** ppResource = &pResource)
		{
			pFactory->CreatePathGeometry((ID2D1PathGeometry**)ppResource);
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

namespace Maml.Graphics;

unsafe public abstract partial class Geometry : Resource
{
	internal int? iResource = null;
	internal RenderTarget? lastRenderTarget;

	internal abstract void MakeResource(RenderTarget renderTarget);

	internal int GetResource(RenderTarget renderTarget, bool noCache = false)
	{
		if (iResource == null)
		{
			MakeResource(renderTarget);
		}
		else
		{
			if (noCache || IsDirty || lastRenderTarget != renderTarget)
			{
				FreeResources();
				MakeResource(renderTarget);
			}
		}
		IsDirty = false;
		lastRenderTarget = renderTarget;
		return (int)iResource!;
	}

	protected override void FreeResources()
	{
		if (iResource != null)
		{
			lastRenderTarget?.ReleaseGeometry((int)iResource);
		}
	}
}

public partial class RectGeometry : Geometry
{
	internal override void MakeResource(RenderTarget renderTarget)
	{
		iResource = renderTarget.MakeRectGeometry(Rect);
	}
}

public partial class EllipseGeometry : Geometry
{
	internal override void MakeResource(RenderTarget renderTarget)
	{
		iResource = renderTarget.MakeEllipseGeometry(Ellipse);
	}
}

public partial class LineGeometry : Geometry
{
	internal override void MakeResource(RenderTarget renderTarget)
	{
		iResource = renderTarget.MakeLineGeometry(Line);
	}
}

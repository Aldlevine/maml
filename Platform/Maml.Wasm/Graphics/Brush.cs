namespace Maml.Graphics;
public partial class Brush : Resource
{
	internal int? iResource = null;
	internal RenderTarget? lastRenderTarget;

	internal abstract void MakeResource(RenderTarget rt);

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
			lastRenderTarget?.ReleaseBrush((int)iResource);
		}
	}
}

public partial class ColorBrush : Brush
{
	internal override void MakeResource(RenderTarget renderTarget) => iResource = renderTarget.MakeColorBrush(Color);
}

using Maml.Math;

namespace Maml.Graphics;
public partial class Text : Resource
{
	internal int? iResource = null;
	internal RenderTarget? lastRenderTarget;

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

	internal void MakeResource(RenderTarget rt)
	{
		(iResource, LineCount, Vector2 size) = rt.MakeText(this);
		Size = Vector2.Clamp(size, Vector2.Zero, MaxSize);
		IsDirty = false;
	}

	protected override void FreeResources()
	{
		if (iResource != null)
		{
			lastRenderTarget?.ReleaseText((int)iResource);
		}
	}
}


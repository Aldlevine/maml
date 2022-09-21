using Windows.Win32.Graphics.Direct2D;

namespace Maml.Graphics;

unsafe public abstract partial class Brush : Resource
{
	internal ID2D1RenderTarget* pLastRenderTarget;
	internal ID2D1Brush* pResource;
	internal abstract void MakeResource(ID2D1RenderTarget* pRenderTarget);
	internal ID2D1Brush* GetResource(ID2D1RenderTarget* pRenderTarget, bool noCache = false)
	{
		if (pResource == null)
		{
			MakeResource(pRenderTarget);
		}
		else
		{
			if (noCache || IsDirty || pLastRenderTarget != pRenderTarget)
			{
				FreeResources();
				MakeResource(pRenderTarget);
			}
		}
		pLastRenderTarget = pRenderTarget;
		return pResource;
	}
	protected override void FreeResources()
	{
		pResource->Release();
	}
}

unsafe public partial class ColorBrush : Brush
{
	internal override void MakeResource(ID2D1RenderTarget* pRenderTarget)
	{
		fixed (ID2D1Brush** ppResource = &pResource)
		{
			pRenderTarget->CreateSolidColorBrush(Color.ToD2DColorF(), default, (ID2D1SolidColorBrush**)ppResource);
		}
	}
}

using Maml.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Direct2D.Common;
using Windows.Win32.Graphics.DirectWrite;

namespace Maml.Graphics;
unsafe public partial class Text : Resource
{
	internal IDWriteTextLayout* pResource;
	//internal abstract void MakeResource(Engine engine);
	internal IDWriteTextLayout* GetResource(Engine engine, bool noCache = false)
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

	internal void MakeResource(Engine engine)
	{
		IDWriteTextFormat* pTextFormat;
		engine.pDWriteFactory->CreateTextFormat(
			Font.Name,
			null,
			(DWRITE_FONT_WEIGHT)Font.Weight,
			(DWRITE_FONT_STYLE)Font.Style,
			DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_NORMAL,
			(float)Font.Size,
			"",
			&pTextFormat);
		fixed (IDWriteTextLayout** ppResource = &pResource)
		{
			engine.pDWriteFactory->CreateTextLayout(String, (uint)String.Length, pTextFormat, float.MaxValue, float.MaxValue, ppResource);
		}
	}
}

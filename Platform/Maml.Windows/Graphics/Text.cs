using Maml.Math;
using System;
using Windows.Win32.Graphics.DirectWrite;

namespace Maml.Graphics;
unsafe public partial class Text : Resource
{
	internal IDWriteTextLayout* pResource;
	//internal IDWriteTextLayout* GetResource(Engine engine, bool noCache = false)
	internal IDWriteTextLayout* GetResource(RenderTarget renderTarget, bool noCache = false)
	{
		if (pResource == null)
		{
			MakeResource(renderTarget);
		}
		else
		{
			if (noCache || IsDirty)
			{
				FreeResources();
				MakeResource(renderTarget);
			}
		}
		return pResource;
	}
	protected override void FreeResources()
	{
		pResource->Release();
	}

	//internal void MakeResource(Engine engine)
	internal void MakeResource(RenderTarget renderTarget)
	{
		Engine engine = Engine.Singleton;
		IDWriteTextFormat* pTextFormat;
		engine.pDWriteFactory->CreateTextFormat(
			Font.Name,
			null,
			(DWRITE_FONT_WEIGHT)Font.Weight,
			(DWRITE_FONT_STYLE)Font.Style,
			DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_NORMAL,
			(float)Font.Size,
			"",
			&pTextFormat).ThrowOnFailure();
		fixed (IDWriteTextLayout** ppResource = &pResource)
		{
			engine.pDWriteFactory->CreateTextLayout(
				String,
				(uint)String.Length,
				pTextFormat,
				(float)double.Max(MaxSize.X, 0),
				(float)double.Max(MaxSize.Y, 0),
				ppResource).ThrowOnFailure();
		}
		pTextFormat->Release();

		double lineHeight = LineHeight switch
		{
			LineHeight.Relative => LineHeight.Value * Font.Size,
			_ => LineHeight.Value,
		};
		double baseline = (lineHeight + (Font.Size / 2)) / 2;

		pResource->SetLineSpacing(DWRITE_LINE_SPACING_METHOD.DWRITE_LINE_SPACING_METHOD_UNIFORM, (float)lineHeight, (float)baseline).ThrowOnFailure();

		DWRITE_WORD_WRAPPING wrappingMode = WrappingMode switch
		{
			WrappingMode.None => DWRITE_WORD_WRAPPING.DWRITE_WORD_WRAPPING_NO_WRAP,
			WrappingMode.Character => DWRITE_WORD_WRAPPING.DWRITE_WORD_WRAPPING_CHARACTER,
			WrappingMode.Word => DWRITE_WORD_WRAPPING.DWRITE_WORD_WRAPPING_WHOLE_WORD,
			WrappingMode.Normal => DWRITE_WORD_WRAPPING.DWRITE_WORD_WRAPPING_WRAP,
			_ => DWRITE_WORD_WRAPPING.DWRITE_WORD_WRAPPING_WRAP,
		};
		pResource->SetWordWrapping(wrappingMode).ThrowOnFailure();

		pResource->GetMetrics(out var textMetrics).ThrowOnFailure();
		Size = Vector2.Clamp(new(textMetrics.width, textMetrics.height), Vector2.Zero, MaxSize);
		LineCount = textMetrics.lineCount;

		IsDirty = false;
	}
}

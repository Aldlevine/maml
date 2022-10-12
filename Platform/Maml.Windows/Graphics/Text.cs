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
public partial class TextGeometry : Geometry
{
	private readonly Brush brush = new ColorBrush { Color = Colors.Black };

	internal override unsafe void MakeResource(Engine engine)
	{
		IDWriteFontCollection* pSystemFontCollection;
		engine.pDWriteFactory->GetSystemFontCollection(&pSystemFontCollection, false).ThrowOnFailure();
		pSystemFontCollection->FindFamilyName(Font.Name, out var index, out var exists).ThrowOnFailure();
		if (!exists)
		{
			throw new ArgumentException($"{nameof(Font)}.{nameof(Font.Name)}");
		}
		IDWriteFontFamily* pFontFamily;
		pSystemFontCollection->GetFontFamily(index, &pFontFamily).ThrowOnFailure();
		IDWriteFont* pFont;
		pFontFamily->GetFirstMatchingFont(
			(DWRITE_FONT_WEIGHT)Font.Weight,
			DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_NORMAL,
			(DWRITE_FONT_STYLE)Font.Style,
			&pFont).ThrowOnFailure();
		IDWriteFontFace* pFontFace;
		pFont->CreateFontFace(&pFontFace).ThrowOnFailure();

		/**/
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
		IDWriteTextLayout* pTextLayout;
		engine.pDWriteFactory->CreateTextLayout(
			Text,
			(uint)Text.Length,
			pTextFormat,
			1000,
			1000,
			&pTextLayout).ThrowOnFailure();

		//fixed (ID2D1Geometry** ppResource = &pResource)
		//{
		//	engine.pD2DFactory->CreatePathGeometry((ID2D1PathGeometry**)ppResource);
		//}
		//ID2D1GeometrySink* pSink;
		//((ID2D1PathGeometry*)pResource)->Open(&pSink);

		fixed (ID2D1Geometry** ppResource = &pResource)
		{
			TextRenderer tr = new(ppResource);
			pTextLayout->Draw(null, tr, 0, 0).ThrowOnFailure();
		}
		//pSink->Close().ThrowOnFailure();
		//pSink->Release();

		//engine.Window.RenderTarget.pRenderTarget->DrawTextLayout(
		//	Vector2.Zero.ToD2DPoint2F(),
		//	pTextLayout,
		//	brush.GetResource(engine.Window.RenderTarget.pRenderTarget),
		//	D2D1_DRAW_TEXT_OPTIONS.D2D1_DRAW_TEXT_OPTIONS_NONE);
		//fixed (ID2D1Geometry** ppResource = &pResource)
		//{
		//	engine.pD2DFactory->CreateRectangleGeometry(new Rect { Position=new(0,0), Size=new(100,100), }.ToD2DRectF(), (ID2D1RectangleGeometry**)ppResource);
		//}
		/**/

		/** /
		List<uint> codePointsList = new();
		for (int i = 0; i < Text.Length; i += char.IsSurrogatePair(Text, i) ? 2 : 1)
		{
			codePointsList.Add((uint)char.ConvertToUtf32(Text, i));
		}
		uint[] codePoints = codePointsList.ToArray();
		ushort[] glyphIndices = new ushort[codePointsList.Count];

		pFontFace->GetGlyphIndices(codePoints, glyphIndices);

		IDWriteTextAnalyzer* pTextAnalyzer;
		engine.pDWriteFactory->CreateTextAnalyzer(&pTextAnalyzer);
		pTextAnalyzer->GetGlyphPlacements(Text, )

		fixed (ID2D1Geometry** ppResource = &pResource)
		{
			engine.pD2DFactory->CreatePathGeometry((ID2D1PathGeometry**)ppResource);
		}
		ID2D1GeometrySink* pSink;
		((ID2D1PathGeometry*)pResource)->Open(&pSink);

		fixed(ushort* pGlyphIndices = glyphIndices)
		{
			pFontFace->GetGlyphRunOutline((float)Font.Size, pGlyphIndices, null, null, (uint)glyphIndices.Length, false, FlowDirection == FlowDirection.RightToLeft, (ID2D1SimplifiedGeometrySink*)pSink);
		}
		pSink->Close().ThrowOnFailure();
		pSink->Release();
		/**/

		/** /
		IDWriteTextAnalyzer* pTextAnalyzer;
		engine.pDWriteFactory->CreateTextAnalyzer(&pTextAnalyzer).ThrowOnFailure();
		PCWSTR pText;
		fixed (char* pTextChArr = Text)
		{
			pText = new(pTextChArr);
		}

		uint maxGlyphCount = (uint)(3 * Text.Length / 2 + 16);
		ushort[] clusterMap = new ushort[maxGlyphCount];
		DWRITE_SHAPING_TEXT_PROPERTIES[] textProps = new DWRITE_SHAPING_TEXT_PROPERTIES[maxGlyphCount];
		ushort[] glyphIndices = new ushort[maxGlyphCount];
		DWRITE_SHAPING_GLYPH_PROPERTIES[] glyphProps = new DWRITE_SHAPING_GLYPH_PROPERTIES[maxGlyphCount];
		uint glyphCount;

		DWRITE_SCRIPT_ANALYSIS scriptAnalysis = default;

		fixed (ushort* pClusterMap = clusterMap)
		fixed (DWRITE_SHAPING_TEXT_PROPERTIES* pTextProps = textProps)
		fixed (ushort* pGlyphIndices = glyphIndices)
		fixed (DWRITE_SHAPING_GLYPH_PROPERTIES* pGlyphProps = glyphProps)
		{
			pTextAnalyzer->GetGlyphs(
				pText,
				(uint)Text.Length,
				pFontFace,
				false,
				FlowDirection == FlowDirection.RightToLeft,
				&scriptAnalysis,
				null,
				null,
				null,
				null,
				0,
				maxGlyphCount,
				pClusterMap,
				pTextProps,
				pGlyphIndices,
				pGlyphProps,
				&glyphCount).ThrowOnFailure();

			float[] glyphAdvances = new float[glyphCount];
			DWRITE_GLYPH_OFFSET[] glyphOffsets = new DWRITE_GLYPH_OFFSET[glyphCount];

			fixed (float* pGlyphAdvances = glyphAdvances)
			fixed (DWRITE_GLYPH_OFFSET* pGlyphOffsets = glyphOffsets)
			{
				pTextAnalyzer->GetGlyphPlacements(
					pText,
					pClusterMap,
					pTextProps,
					(uint)Text.Length,
					pGlyphIndices,
					pGlyphProps,
					glyphCount,
					pFontFace,
					(float)Font.Size,
					false,
					FlowDirection == FlowDirection.RightToLeft,
					&scriptAnalysis,
					null,
					null,
					null,
					0,
					pGlyphAdvances,
					pGlyphOffsets).ThrowOnFailure();


				fixed (ID2D1Geometry** ppResource = &pResource)
				{
					engine.pD2DFactory->CreatePathGeometry((ID2D1PathGeometry**)ppResource).ThrowOnFailure();
				}
				ID2D1GeometrySink* pSink;
				//ID2D1PathGeometry* pPathGeometry;
				//engine.pD2DFactory->CreatePathGeometry(&pPathGeometry);
				//pPathGeometry->Open(&pSink);
				((ID2D1PathGeometry*)pResource)->Open(&pSink).ThrowOnFailure();

				//DWRITE_GLYPH_METRICS[] glyphMetrics = new DWRITE_GLYPH_METRICS[glyphIndices.Length];
				//pFontFace->GetDesignGlyphMetrics(glyphIndices, glyphMetrics, false).ThrowOnFailure();
				//pFontFace->GetMetrics(out var fontFaceMetrics);

				//float penPosition = 0;
				//for (int i = 0; i < glyphCount; i++)
				//{
				//	glyphAdvances[i] = float.Floor(glyphAdvances[i]);
				//	glyphOffsets[i].advanceOffset = glyphMetrics[i].leftSideBearing / fontFaceMetrics.
				//	//glyphOffsets[i] = glyphOffsets[i] with
				//	//{
				//	//	advanceOffset = float.Floor(glyphOffsets[i].advanceOffset),
				//	//	ascenderOffset = float.Floor(glyphOffsets[i].ascenderOffset),
				//	//};
				//}

				pFontFace->GetGlyphRunOutline(
					(float)Font.Size,
					pGlyphIndices,
					pGlyphAdvances,
					pGlyphOffsets,
					glyphCount,
					false,
					FlowDirection == FlowDirection.RightToLeft,
					(ID2D1SimplifiedGeometrySink*)pSink).ThrowOnFailure();

				pSink->Close().ThrowOnFailure();
				pSink->Release();
			}
		}
		/**/
	}
}

//internal class TextRenderer : IDWriteTextRenderer
//{

//}

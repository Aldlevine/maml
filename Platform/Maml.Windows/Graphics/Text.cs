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
		pFontFamily->GetFirstMatchingFont((DWRITE_FONT_WEIGHT)Font.Weight, DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_NORMAL, (DWRITE_FONT_STYLE)Font.Style, &pFont);
		IDWriteFontFace* pFontFace;
		pFont->CreateFontFace(&pFontFace).ThrowOnFailure();

		/*
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
		*/

		IDWriteTextAnalyzer* pTextAnalyzer;
		engine.pDWriteFactory->CreateTextAnalyzer(&pTextAnalyzer);
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
				&glyphCount);

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
					pGlyphOffsets);


				fixed (ID2D1Geometry** ppResource = &pResource)
				{
					engine.pD2DFactory->CreatePathGeometry((ID2D1PathGeometry**)ppResource);
				}
				ID2D1GeometrySink* pSink;
				//ID2D1PathGeometry* pPathGeometry;
				//engine.pD2DFactory->CreatePathGeometry(&pPathGeometry);
				//pPathGeometry->Open(&pSink);
				((ID2D1PathGeometry*)pResource)->Open(&pSink);

				float penPosition = 0;
				for (int i = 0; i < glyphCount; i++)
				{
					glyphAdvances[i] = float.Floor(glyphAdvances[i]);
					//float glyphPosition = penPosition + glyphOffsets[i].advanceOffset;
					//if (i > 0)
					//{
					//	penPosition += glyphAdvances[i - 1];
					//}
					//float offset = float.Floor(glyphPosition) - glyphPosition;
					//glyphOffsets[i] = glyphOffsets[i] with { advanceOffset = offset, };
					//penPosition += glyphAdvances[i];
				}

				pFontFace->GetGlyphRunOutline((float)Font.Size, pGlyphIndices, pGlyphAdvances, pGlyphOffsets, glyphCount, false, FlowDirection == FlowDirection.RightToLeft, (ID2D1SimplifiedGeometrySink*)pSink);
				pSink->Close().ThrowOnFailure();
				pSink->Release();

				//Transform transform = Transform.PixelIdentity;

				//fixed (ID2D1Geometry** ppResource = &pResource)
				//{
				//	engine.pD2DFactory->CreateTransformedGeometry((ID2D1Geometry*)pPathGeometry, transform.ToD2DMatrix3X2F(), (ID2D1TransformedGeometry**)ppResource);
				//}
			}
		}
	}
}

//internal class TextRenderer : IDWriteTextRenderer
//{

//}

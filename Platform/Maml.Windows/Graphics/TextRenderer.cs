using Maml.Events;
using Maml.Graphics;
using Maml.Math;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Windows.Devices.PointOfService;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Direct2D.Common;
using Windows.Win32.Graphics.DirectWrite;
using Windows.Win32.System.Com;

namespace Maml.Graphics
{
	unsafe internal struct TextRenderer
	{
		private static Dictionary<IntPtr, List<DrawLayer>> instDrawLayers = new();

		public static implicit operator IDWriteTextRenderer*(TextRenderer self) => &self.IDWriteTextRenderer;
		private IDWriteTextRenderer IDWriteTextRenderer;

		//private ID2D1RenderTarget* pRT;

		// private ID2D1GeometrySink* pSink;
		private ID2D1Geometry** ppGeometry;

		//public List<DrawLayer> DrawLayers
		//{
		//	get
		//	{
		//		fixed (TextRenderer* pThis = &this)
		//		{
		//			return instDrawLayers[(IntPtr)pThis];
		//		}
		//	}
		//	set
		//	{
		//		fixed (TextRenderer* pThis = &this)
		//		{
		//			instDrawLayers[(IntPtr)pThis] = value;
		//		}
		//	}
		//}

		//public TextRenderer(ID2D1GeometrySink* pSink)
		public TextRenderer(ID2D1Geometry** ppGeometry)
		{
			//this.pRT = pRT;
			//this.pSink = pSink;
			this.ppGeometry = ppGeometry;
			fixed(TextRenderer* pThis = &this)
			{
				instDrawLayers[(IntPtr)pThis] = new();
				IDWriteTextRenderer = IDWriteTextRenderer.Create(pThis);
			}
		}

		public HRESULT IsPixelSnappingDisabled(void* pClientDrawingContext, BOOL* pIsDisabled)
		{
			*pIsDisabled = false;
			return HRESULT.S_OK;
		}

		public HRESULT GetCurrentTransform(void* pClientDrawingContext, DWRITE_MATRIX* matrix)
		{
			//return HRESULT.E_NOTIMPL;
			*matrix = new DWRITE_MATRIX
			{
				dx = 100.5f,
				dy = 100.5f,
				m11 = 1,
				m12 = 0,
				m21 = 0,
				m22 = 1,
			};
			return HRESULT.S_OK;
		}

		public HRESULT GetPixelsPerDip(void* pClientDrawingContext, float* pPixelsPerDip)
		{
			//*pPixelsPerDip = 1;
			Engine.Singleton.Window.RenderTarget.pRenderTarget->GetDpi(out var x, out var _);
			*pPixelsPerDip = x / 96;
			return HRESULT.S_OK;
		}

		public HRESULT DrawGlyphRun(
		void* pClientDrawingContext,
		float baselineOriginX,
		float baselineOriginY,
		DWRITE_MEASURING_MODE measuringMode,
		DWRITE_GLYPH_RUN* pGlyphRun,
		DWRITE_GLYPH_RUN_DESCRIPTION* pGlyphRunDescription,
		IUnknown* pClientDrawingEffect)
		{
			Console.WriteLine("DRAW!");
			ID2D1PathGeometry* pPath;
			ID2D1GeometrySink* pSink;
			Engine.Singleton.pD2DFactory->CreatePathGeometry(&pPath).ThrowOnFailure();
			pPath->Open(&pSink).ThrowOnFailure();

			//return HRESULT.E_NOTIMPL;
			pGlyphRun->fontFace->GetGlyphRunOutline(
				pGlyphRun->fontEmSize,
				pGlyphRun->glyphIndices,
				pGlyphRun->glyphAdvances,
				pGlyphRun->glyphOffsets,
				pGlyphRun->glyphCount,
				pGlyphRun->isSideways,
				pGlyphRun->bidiLevel % 2 == 1,
				(ID2D1SimplifiedGeometrySink*)pSink).ThrowOnFailure();

			pSink->Close().ThrowOnFailure();
			pSink->Release();
			Engine.Singleton.pD2DFactory->CreateTransformedGeometry(
				(ID2D1Geometry*)pPath,
				(Transform.Identity with { Origin = new(baselineOriginX, baselineOriginY) }).ToD2DMatrix3X2F(),
				(ID2D1TransformedGeometry**)ppGeometry);

			//pSink->Close().ThrowOnFailure();
			//pSink->Release();
			return HRESULT.S_OK;
		}

		public HRESULT DrawUnderline(
		void* pClientDrawingContext,
		float baselineOriginX,
		float baselineOriginY,
		DWRITE_UNDERLINE* pUnderline,
		IUnknown* pClientDrawingEffect)
		{
			//return HRESULT.E_NOTIMPL;
			return HRESULT.S_OK;
		}

		public HRESULT DrawStrikethrough(
		void* pClientDrawingContext,
		float baselineOriginX,
		float baselineOriginY,
		DWRITE_STRIKETHROUGH* pStrikethrough,
		IUnknown* pClientDrawingEffect)
		{
			//return HRESULT.E_NOTIMPL;
			return HRESULT.S_OK;
		}

		public HRESULT DrawInlineObject(
		void* pClientDrawingContext,
		float originX,
		float originY,
		IDWriteInlineObject* pInlineObject,
		BOOL isSideways,
		BOOL isRightToLeft,
		IUnknown* pClientDrawingEffect)
		{
			//return HRESULT.E_NOTIMPL;
			return HRESULT.S_OK;
		}
	}
}



namespace Windows.Win32.Graphics.DirectWrite
{
	unsafe struct IDWriteTextRenderer
	{
		private static Vtbl vtbl = new();
		private struct Vtbl
		{
			internal delegate *unmanaged [Stdcall]<IDWriteTextRenderer*,global::System.Guid* ,void**, HRESULT> QueryInterface_1 = &QueryInterface;
			internal delegate *unmanaged [Stdcall]<IDWriteTextRenderer*,uint> AddRef_2 = &AddRef;
			internal delegate *unmanaged [Stdcall]<IDWriteTextRenderer*,uint> Release_3 = &Release;
			internal delegate *unmanaged [Stdcall]<IDWriteTextRenderer*,void*, BOOL*, HRESULT> IsPixelSnappingDisabled_4 = &IsPixelSnappingDisabled;
			internal delegate *unmanaged [Stdcall]<IDWriteTextRenderer*,void*, DWRITE_MATRIX* ,HRESULT> GetCurrentTransform_5 = &GetCurrentTransform;
			internal delegate *unmanaged [Stdcall]<IDWriteTextRenderer*,void*, float*, HRESULT> GetPixelsPerDip_6 = &GetPixelsPerDip;
			internal delegate *unmanaged [Stdcall]<IDWriteTextRenderer*,void*, float, float, DWRITE_MEASURING_MODE, DWRITE_GLYPH_RUN*, DWRITE_GLYPH_RUN_DESCRIPTION*, IUnknown*, HRESULT> DrawGlyphRun_7 = &DrawGlyphRun;
			internal delegate *unmanaged [Stdcall]<IDWriteTextRenderer*,void*, float, float, DWRITE_UNDERLINE*, IUnknown*, HRESULT> DrawUnderline_8 = &DrawUnderline;
			internal delegate *unmanaged [Stdcall]<IDWriteTextRenderer*,void*, float, float, DWRITE_STRIKETHROUGH*, IUnknown*, HRESULT> DrawStrikethrough_9 = &DrawStrikethrough;
			internal delegate *unmanaged [Stdcall]<IDWriteTextRenderer*,void*, float, float, IDWriteInlineObject*, BOOL, BOOL, IUnknown*, HRESULT> DrawInlineObject_10 = &DrawInlineObject;
			public Vtbl() { }
		}

		private Vtbl* lpVtbl;
		private TextRenderer* pInstance;

		public static IDWriteTextRenderer Create(TextRenderer* pInstance)
		{
			IDWriteTextRenderer result = new();
			fixed (Vtbl* pVtbl = &vtbl)
			{
				result.lpVtbl = pVtbl;
			}
			result.pInstance = pInstance;
			return result;
		}


		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
		static HRESULT QueryInterface(IDWriteTextRenderer* pThis, Guid* pGuid, void** pp)
		{
			return HRESULT.E_NOTIMPL;
		}


		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
		static uint AddRef(IDWriteTextRenderer* pThis)
		{
			//return pThis->refCount += 1;
			return (uint)Marshal.AddRef((IntPtr)pThis);
		}


		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
		static uint Release(IDWriteTextRenderer* pThis)
		{
			uint result = (uint)Marshal.Release((IntPtr)pThis);
			if (result == 0)
			{
				// delete this!
			}
			return result;
		}


		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
		static HRESULT IsPixelSnappingDisabled(IDWriteTextRenderer* pThis, void* pClientDrawingContext, BOOL* pIsDisabled)
			=> pThis->pInstance->IsPixelSnappingDisabled(pClientDrawingContext, pIsDisabled);


		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
		static HRESULT GetCurrentTransform(IDWriteTextRenderer* pThis, void* pClientDrawingContext, DWRITE_MATRIX* matrix)
			=> pThis->pInstance->GetCurrentTransform(pClientDrawingContext, matrix);


		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
		static HRESULT GetPixelsPerDip(IDWriteTextRenderer* pThis, void* pClientDrawingContext, float* pPixelsPerDip)
			=> pThis->pInstance->GetPixelsPerDip(pClientDrawingContext, pPixelsPerDip);


		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
		static HRESULT DrawGlyphRun(
		IDWriteTextRenderer* pThis,
		void* pClientDrawingContext,
		float baselineOriginX,
		float baselineOriginY,
		DWRITE_MEASURING_MODE measuringMode,
		DWRITE_GLYPH_RUN* pGlyphRun,
		DWRITE_GLYPH_RUN_DESCRIPTION* pGlyphRunDescription,
		IUnknown* pClientDrawingEffect)
			=> pThis->pInstance->DrawGlyphRun(
				pClientDrawingContext,
				baselineOriginX,
				baselineOriginY,
				measuringMode,
				pGlyphRun,
				pGlyphRunDescription,
				pClientDrawingEffect);


		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
		static HRESULT DrawUnderline(
		IDWriteTextRenderer* pThis,
		void* pClientDrawingContext,
		float baselineOriginX,
		float baselineOriginY,
		DWRITE_UNDERLINE* pUnderline,
		IUnknown* pClientDrawingEffect)
			=> pThis->pInstance->DrawUnderline(
				pClientDrawingContext,
				baselineOriginX,
				baselineOriginY,
				pUnderline,
				pClientDrawingEffect);
			

		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
		static HRESULT DrawStrikethrough(
		IDWriteTextRenderer* pThis,
		void* pClientDrawingContext,
		float baselineOriginX,
		float baselineOriginY,
		DWRITE_STRIKETHROUGH* pStrikethrough,
		IUnknown* pClientDrawingEffect)
			=> pThis->pInstance->DrawStrikethrough(
				pClientDrawingContext,
				baselineOriginX,
				baselineOriginY,
				pStrikethrough,
				pClientDrawingEffect);


		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
		static HRESULT DrawInlineObject(
		IDWriteTextRenderer* pThis,
		void* pClientDrawingContext,
		float originX,
		float originY,
		IDWriteInlineObject* pInlineObject,
		BOOL isSideways,
		BOOL isRightToLeft,
		IUnknown* pClientDrawingEffect)
			=> pThis->pInstance->DrawInlineObject(
				pClientDrawingContext,
				originX,
				originY,
				pInlineObject,
				isSideways,
				isRightToLeft,
				pClientDrawingEffect);
	}
}

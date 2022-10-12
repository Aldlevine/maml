using Maml.Events;
using Maml.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
		public HRESULT IsPixelSnappingDisabled(void* pClientDrawingContext, BOOL* pIsDisabled)
		{
			*pIsDisabled = false;
			return HRESULT.S_OK;
		}

		public HRESULT GetCurrentTransform(void* pClientDrawingContext, DWRITE_MATRIX* matrix)
		{
			return HRESULT.E_NOTIMPL;
			//return HRESULT.S_OK;
		}

		public HRESULT GetPixelsPerDip(void* pClientDrawingContext, float* pPixelsPerDip)
		{
			*pPixelsPerDip = 1;
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
			return HRESULT.E_NOTIMPL;
			//return HRESULT.S_OK;
		}

		public HRESULT DrawUnderline(
		void* pClientDrawingContext,
		float baselineOriginX,
		float baselineOriginY,
		DWRITE_UNDERLINE* pUnderline,
		IUnknown* pClientDrawingEffect)
		{
			return HRESULT.E_NOTIMPL;
			//return HRESULT.S_OK;
		}

		public HRESULT DrawStrikethrough(
		void* pClientDrawingContext,
		float baselineOriginX,
		float baselineOriginY,
		DWRITE_STRIKETHROUGH* pStrikethrough,
		IUnknown* pClientDrawingEffect)
		{
			return HRESULT.E_NOTIMPL;
			//return HRESULT.S_OK;
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
			return HRESULT.E_NOTIMPL;
			//return HRESULT.S_OK;
		}
	}
}



namespace Windows.Win32.Graphics.DirectWrite
{
	unsafe struct IDWriteTextRenderer
	{
		private struct Vtbl
		{
			internal delegate *unmanaged [Stdcall]<IDWriteTextRenderer*,global::System.Guid* ,void**, HRESULT> QueryInterface_1;
			internal delegate *unmanaged [Stdcall]<IDWriteTextRenderer*,uint> AddRef_2;
			internal delegate *unmanaged [Stdcall]<IDWriteTextRenderer*,uint> Release_3;
			internal delegate *unmanaged [Stdcall]<IDWriteTextRenderer*,void*, BOOL*, HRESULT> IsPixelSnappingDisabled_4;
			internal delegate *unmanaged [Stdcall]<IDWriteTextRenderer*,void*, DWRITE_MATRIX* ,HRESULT> GetCurrentTransform_5;
			internal delegate *unmanaged [Stdcall]<IDWriteTextRenderer*,void*, float*, HRESULT> GetPixelsPerDip_6;
			internal delegate *unmanaged [Stdcall]<IDWriteTextRenderer*,void*, float, float, DWRITE_MEASURING_MODE, DWRITE_GLYPH_RUN*, DWRITE_GLYPH_RUN_DESCRIPTION*, IUnknown*, HRESULT> DrawGlyphRun_7;
			internal delegate *unmanaged [Stdcall]<IDWriteTextRenderer*,void*, float, float, DWRITE_UNDERLINE*, IUnknown*, HRESULT> DrawUnderline_8;
			internal delegate *unmanaged [Stdcall]<IDWriteTextRenderer*,void*, float, float, DWRITE_STRIKETHROUGH*, IUnknown*, HRESULT> DrawStrikethrough_9;
			internal delegate *unmanaged [Stdcall]<IDWriteTextRenderer*,void*, float, float, IDWriteInlineObject*, BOOL, BOOL, IUnknown*, HRESULT> DrawInlineObject_10;
		}
		private Vtbl* lpVtbl;
		private uint refCount;
		private TextRenderer* pInstance;

		public static IDWriteTextRenderer Create(TextRenderer* pInstance)
		{
			IDWriteTextRenderer result = new();
			result.pInstance = pInstance;
			result.lpVtbl->QueryInterface_1 = &QueryInterface;
			result.lpVtbl->AddRef_2 = &AddRef;
			result.lpVtbl->Release_3 = &Release;
			result.lpVtbl->IsPixelSnappingDisabled_4 = &IsPixelSnappingDisabled;
			result.lpVtbl->GetCurrentTransform_5 = &GetCurrentTransform;
			result.lpVtbl->GetPixelsPerDip_6 = &GetPixelsPerDip;
			result.lpVtbl->DrawGlyphRun_7 = &DrawGlyphRun;
			result.lpVtbl->DrawUnderline_8 = &DrawUnderline;
			result.lpVtbl->DrawStrikethrough_9 = &DrawStrikethrough;
			result.lpVtbl->DrawInlineObject_10 = &DrawInlineObject;
			return result;
		}


		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
		static HRESULT QueryInterface(IDWriteTextRenderer* pThis, Guid* pGuid, void** pp)
		{
			pp = (void**)Marshal.GetIUnknownForObject(*pThis);
			return HRESULT.S_OK;
		}


		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
		static uint AddRef(IDWriteTextRenderer* pThis)
		{
			return pThis->refCount += 1;
		}


		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
		static uint Release(IDWriteTextRenderer* pThis)
		{
			uint result = pThis->refCount -= 1;
			if (pThis->refCount == 0)
			{
				// delete this!
			}
			return pThis->refCount;
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

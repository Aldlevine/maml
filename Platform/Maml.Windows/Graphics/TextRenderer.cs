using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.PointOfService;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Direct2D.Common;
using Windows.Win32.Graphics.DirectWrite;

namespace Maml.Graphics
{
	unsafe internal class TextRenderer
	{
		[UnmanagedCallersOnly]
		static HRESULT IsPixelSnappingDisabled(void* pClientDrawingContext, BOOL* pIsDisabled)
		{
			*pIsDisabled = false;
			return new(0);
		}
	}
}



namespace Windows.Win32.Graphics.DirectWrite
{

	unsafe partial struct IDWriteTextRenderer
	{
		private uint refCount;

		public static IDWriteTextRenderer Create()
		{
			IDWriteTextRenderer result = new();
			result.lpVtbl->QueryInterface_1 = &QueryInterface;
			result.lpVtbl->AddRef_2 = &AddRef;
			result.lpVtbl->Release_3 = &Release;
			result.lpVtbl->IsPixelSnappingDisabled_4 = &IsPixelSnappingDisabled;
			return result;
		}

		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
		static HRESULT QueryInterface(IDWriteTextRenderer* pThis, Guid* pGuid, void** pp)
		{
			pp = (void**)Marshal.GetIUnknownForObject(*pThis);
			return new(0);
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

			if (result == 0)
			{
				// Dispose??
			}

			return result;
		}

		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
		static HRESULT IsPixelSnappingDisabled(IDWriteTextRenderer* @this, void* pClientDrawingContext, BOOL* pIsDisabled)
		{
			*pIsDisabled = false;
			return new(0);
		}
	}

}

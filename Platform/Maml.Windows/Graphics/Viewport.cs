using Maml.Math;
using System.Threading;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Direct2D.Common;
using static Windows.Win32.PInvoke;

namespace Maml.Graphics;

public unsafe partial class Viewport
{
	internal HWND hWnd;
	internal ID2D1Factory* pD2DFactory;

	internal bool ImmediateMode = false;
	internal ID2D1HwndRenderTarget* pRenderTarget => ImmediateMode ? pImmediateRenderTarget : pSyncRenderTarget;
	private ID2D1HwndRenderTarget* pImmediateRenderTarget;
	private ID2D1HwndRenderTarget* pSyncRenderTarget;

	private const int stdDpi = 96;
	private const double stdDpiInv = 1.0 / (double)stdDpi;

	~Viewport()
	{
		DiscardDeviceResources();
	}

	internal partial double GetDpiRatio() => GetDpiForWindow(hWnd) * stdDpiInv;

	internal partial Vector2 GetSize()
	{
		GetClientRect(hWnd, out var rc);
		return new(rc.right - rc.left, rc.bottom - rc.top);
	}

	private void CreateDeviceResources()
	{
		if (pImmediateRenderTarget == null)
		{
			D2D1_RENDER_TARGET_PROPERTIES renderTargetProps = new()
			{
				dpiX = stdDpi,
				dpiY = stdDpi,
			};

			D2D1_HWND_RENDER_TARGET_PROPERTIES hWndRenderTargetProps = new()
			{
				hwnd = hWnd,
				pixelSize = Size.ToD2DSizeU(),
				presentOptions = D2D1_PRESENT_OPTIONS.D2D1_PRESENT_OPTIONS_IMMEDIATELY,
			};

			fixed (ID2D1HwndRenderTarget** ppRenderTarget = &pImmediateRenderTarget)
			{
				pD2DFactory->CreateHwndRenderTarget(
					in renderTargetProps,
					in hWndRenderTargetProps,
					ppRenderTarget).ThrowOnFailure();
			}
		}

		if (pSyncRenderTarget == null)
		{
			D2D1_RENDER_TARGET_PROPERTIES renderTargetProps = new()
			{
				dpiX = stdDpi,
				dpiY = stdDpi,
			};

			D2D1_HWND_RENDER_TARGET_PROPERTIES hWndRenderTargetProps = new()
			{
				hwnd = hWnd,
				pixelSize = Size.ToD2DSizeU(),
				presentOptions = D2D1_PRESENT_OPTIONS.D2D1_PRESENT_OPTIONS_NONE,
			};

			fixed (ID2D1HwndRenderTarget** ppRenderTarget = &pSyncRenderTarget)
			{
				pD2DFactory->CreateHwndRenderTarget(
					in renderTargetProps,
					in hWndRenderTargetProps,
					ppRenderTarget).ThrowOnFailure();
			}
		}
	}

	private void DiscardDeviceResources()
	{
		if (pImmediateRenderTarget != null)
		{
			pImmediateRenderTarget->Release();
			pImmediateRenderTarget = null;
		}
		if (pSyncRenderTarget != null)
		{
			pSyncRenderTarget->Release();
			pSyncRenderTarget = null;
		}

		// Somehow need to notify Geometries and Brushes to release their stuff...
	}

	public partial void DrawGraphic(Graphic graphic, Transform transform) => graphic.Draw((ID2D1RenderTarget*)pRenderTarget, transform);
	public partial void Clear(Color color) => pRenderTarget->Clear(color.ToD2DColorF());
	public partial void SetTransform(Transform transform) => pRenderTarget->SetTransform(transform.ToD2DMatrix3X2F());

	private Mutex drawMutex = new();

	internal void Redraw(bool forceUpdate)
	{
		InvalidateRect(hWnd, (RECT?)null, false);
		if (forceUpdate)
		{
			UpdateWindow(hWnd);
		}
	}

	internal void HandleDraw()
	{
		drawMutex.WaitOne();

		CreateDeviceResources();
		pRenderTarget->BeginDraw();

		Draw?.Invoke(new() { Viewport = this });

		var hr = pRenderTarget->EndDraw();
		if (hr == HRESULT.D2DERR_RECREATE_TARGET)
		{
			DiscardDeviceResources();
		}
		else
		{
			hr.ThrowOnFailure();
		}

		drawMutex.ReleaseMutex();
	}

	internal void HandleResize()
	{
		if (pRenderTarget != null)
		{
			pImmediateRenderTarget->Resize(Size.ToD2DSizeU()).ThrowOnFailure();
			pSyncRenderTarget->Resize(Size.ToD2DSizeU()).ThrowOnFailure();
			float dpi = GetDpiForWindow(hWnd);
			pImmediateRenderTarget->SetDpi(dpi, dpi);
			pSyncRenderTarget->SetDpi(dpi, dpi);
		}
		Resize?.Invoke(new Events.ResizeEvent { Size = Size });
	}
}

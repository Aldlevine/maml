using Maml.Events;
using Maml.Math;
using System;
using System.Threading;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Direct2D.Common;
using static Windows.Win32.PInvoke;

namespace Maml.Graphics;

public unsafe class Viewport : ViewportBase
{
	#region Abstract

	public override event EventHandler<ResizeEvent>? Resize;
	public override event EventHandler<DrawEvent>? Draw;

	public override Vector2 Size
	{
		get
		{
			GetClientRect(hWnd, out var rc);
			return new(rc.right - rc.left, rc.bottom - rc.top);
		}
	}

	public override double DpiRatio => GetDpiForWindow(hWnd) * stdDpiInv;

	public override Transform GetTransform()
	{
		pRenderTarget->GetTransform(out var matrix);
		return new(matrix);
	}
	public override void SetTransform(Transform transform) => pRenderTarget->SetTransform(transform.ToD2DMatrix3X2F());

	public override void Clear(Color color) => pRenderTarget->Clear(color.ToD2DColorF());

	public override void DrawGeometry(Geometry geometry, Fill fill)
	{
		switch (geometry)
		{
			case RectGeometry g:
				pRenderTarget->FillRectangle(g.Rect.ToD2DRectF(), fill.Brush.GetResource((ID2D1RenderTarget*)pRenderTarget));
				break;
			case EllipseGeometry g:
				pRenderTarget->FillEllipse(g.Ellipse.ToD2DEllipse(), fill.Brush.GetResource((ID2D1RenderTarget*)pRenderTarget));
				break;
			case LineGeometry g:
				// We can't fill line geometry
				break;
		}
	}

	public override void DrawGeometry(Geometry geometry, Stroke stroke)
	{
		switch (geometry)
		{
			case RectGeometry g:
				pRenderTarget->DrawRectangle(g.Rect.ToD2DRectF(), stroke.Brush.GetResource((ID2D1RenderTarget*)pRenderTarget), stroke.Thickness, default);
				break;
			case EllipseGeometry g:
				pRenderTarget->DrawEllipse(g.Ellipse.ToD2DEllipse(), stroke.Brush.GetResource((ID2D1RenderTarget*)pRenderTarget), stroke.Thickness, default);
				break;
			case LineGeometry g:
				pRenderTarget->DrawLine(g.Line.Start.ToD2DPoint2F(), g.Line.End.ToD2DPoint2F(), stroke.Brush.GetResource((ID2D1RenderTarget*)pRenderTarget), stroke.Thickness, default);
				break;
		}
	}

	#endregion

	private const int stdDpi = 96;
	private const double stdDpiInv = 1.0 / (double)stdDpi;

	internal HWND hWnd;
	internal ID2D1Factory* pD2DFactory;
	internal bool ImmediateMode = false;
	internal ID2D1HwndRenderTarget* pRenderTarget => ImmediateMode ? pImmediateRenderTarget : pSyncRenderTarget;
	private ID2D1HwndRenderTarget* pImmediateRenderTarget;
	private ID2D1HwndRenderTarget* pSyncRenderTarget;

	~Viewport()
	{
		DiscardDeviceResources();
	}

	private void CreateRenderTarget(ID2D1HwndRenderTarget** ppRenderTarget, D2D1_PRESENT_OPTIONS presentMode)
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
			presentOptions = presentMode,
		};

		pD2DFactory->CreateHwndRenderTarget(
			in renderTargetProps,
			in hWndRenderTargetProps,
			ppRenderTarget).ThrowOnFailure();
	}

	private void CreateDeviceResources()
	{
		if (pImmediateRenderTarget == null)
		{
			fixed (ID2D1HwndRenderTarget** ppRenderTarget = &pImmediateRenderTarget)
			{
				CreateRenderTarget(ppRenderTarget, D2D1_PRESENT_OPTIONS.D2D1_PRESENT_OPTIONS_IMMEDIATELY);
			}
		}

		if (pSyncRenderTarget == null)
		{
			fixed (ID2D1HwndRenderTarget** ppRenderTarget = &pSyncRenderTarget)
			{
				CreateRenderTarget(ppRenderTarget, D2D1_PRESENT_OPTIONS.D2D1_PRESENT_OPTIONS_NONE);
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

	internal void Redraw(bool forceUpdate)
	{
		InvalidateRect(hWnd, (RECT?)null, false);
		if (forceUpdate)
		{
			UpdateWindow(hWnd);
		}
	}

	private Mutex drawMutex = new();
	internal void HandleDraw()
	{
		lock (drawMutex)
		{
			CreateDeviceResources();
			pRenderTarget->BeginDraw();

			Draw?.Invoke(this, new() { Viewport = this });

			var hr = pRenderTarget->EndDraw();
			if (hr == HRESULT.D2DERR_RECREATE_TARGET)
			{
				DiscardDeviceResources();
			}
			else
			{
				hr.ThrowOnFailure();
			}
		}
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
		Resize?.Invoke(this, new ResizeEvent { Size = Size });
	}
}

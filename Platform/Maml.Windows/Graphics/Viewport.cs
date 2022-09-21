using Maml.Math;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Direct2D.Common;
using static Windows.Win32.PInvoke;

namespace Maml.Graphics;

public unsafe partial class Viewport
{
	private const int stdDpi = 96;
	internal HWND hWnd;
	internal ID2D1Factory* pD2DFactory;
	internal ID2D1HwndRenderTarget* pRenderTarget;

	~Viewport()
	{
		DiscardDeviceResources();
	}

	internal partial double GetDpiRatio() => GetDpiForWindow(hWnd) / (double)stdDpi;

	internal partial Vector2 GetSize()
	{
		GetClientRect(hWnd, out var rc);
		return new(rc.right - rc.left, rc.bottom - rc.top);
	}

	private void CreateDeviceResources()
	{
		if (pRenderTarget == null)
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

			fixed (ID2D1HwndRenderTarget** ppRenderTarget = &pRenderTarget)
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
		if (pRenderTarget != null)
		{
			pRenderTarget->Release();
			pRenderTarget = null;
		}

		// Somehow need to notify Geometries and Brushes to release their stuff...
	}


	public partial void DrawGraphic(Graphic graphic) => graphic.Draw((ID2D1RenderTarget*)pRenderTarget);
	public partial void Clear(Color color) => pRenderTarget->Clear(color.ToD2DColorF());
	public partial void SetTransform(Transform transform) => pRenderTarget->SetTransform(transform.ToD2DMatrix3X2F());

	internal void HandleDraw()
	{
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
	}

	internal void HandleResize(int width, int height)
	{
		if (pRenderTarget != null)
		{
			var size = new D2D_SIZE_U
			{
				width = (uint)width,
				height = (uint)height,
			};
			pRenderTarget->Resize(in size);
		}
		Resize?.Invoke(new Events.ResizeEvent { Size = Size });
	}
}

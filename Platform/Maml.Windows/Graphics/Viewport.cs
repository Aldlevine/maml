using Maml.Geometry;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Direct2D.Common;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;

namespace Maml.Graphics;

public unsafe partial class Viewport
{
	public partial void BeginDraw() => pRenderTarget->BeginDraw();

	public partial void Clear(Color color) => pRenderTarget->Clear(color.ToD2DColorF());

	public partial void EndDraw() => pRenderTarget->EndDraw().ThrowOnFailure();
	public partial void FillPath(Path path) => throw new System.NotImplementedException();
	public partial void PopClip() => throw new System.NotImplementedException();
	public partial void PushClip(Path path) => throw new System.NotImplementedException();
	public partial void SetFillBrush(Brush brush) => throw new System.NotImplementedException();
	public partial void SetStrokeBrush(Brush brush) => throw new System.NotImplementedException();
	public partial void SetTransform(Transform transform) => throw new System.NotImplementedException();
	public partial void StrokePath(Path path) => throw new System.NotImplementedException();

	internal HWND hWnd;
	internal ID2D1Factory* pD2DFactory;
	internal ID2D1HwndRenderTarget* pRenderTarget;
	internal ID2D1SolidColorBrush* pLightSlateGrayBrush;
	internal ID2D1SolidColorBrush* pCornflowerBlueBrush;

	~Viewport()
	{
		DiscardDeviceResources();
	}

	private void CreateDeviceResources()
	{
		if (pRenderTarget == null)
		{
			GetClientRect(hWnd, out var rc);
			D2D_SIZE_U size = new()
			{
				width = (uint)(rc.right - rc.left),
				height = (uint)(rc.bottom - rc.top),
			};

			D2D1_RENDER_TARGET_PROPERTIES renderTargetProps = new() { };

			D2D1_HWND_RENDER_TARGET_PROPERTIES hWndRenderTargetProps = new()
			{
				hwnd = hWnd,
				pixelSize = size,
			};

			fixed (ID2D1HwndRenderTarget** ppRenderTarget = &pRenderTarget)
			{
				pD2DFactory->CreateHwndRenderTarget(
					in renderTargetProps,
					in hWndRenderTargetProps,
					ppRenderTarget).ThrowOnFailure();
			}

			fixed (ID2D1SolidColorBrush** ppLightSlateGrayBrush = &pLightSlateGrayBrush)
			fixed (ID2D1SolidColorBrush** ppCornflowerBlueBrush = &pCornflowerBlueBrush)
			{
				var color = Colors.LightSlateGray.ToD2DColorF();
				pRenderTarget->CreateSolidColorBrush(in color, null, ppLightSlateGrayBrush).ThrowOnFailure();
				color = (Colors.CornflowerBlue with { A = 0.75f }).ToD2DColorF();
				pRenderTarget->CreateSolidColorBrush(in color, null, ppCornflowerBlueBrush).ThrowOnFailure();
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

		if (pLightSlateGrayBrush != null)
		{
			pLightSlateGrayBrush->Release();
			pLightSlateGrayBrush = null;
		}

		if (pCornflowerBlueBrush != null)
		{
			pCornflowerBlueBrush->Release();
			pCornflowerBlueBrush = null;
		}
	}

	private RectShape rectShape = new()
	{
		Rect = new() { Position = new(10, 10), Size = new(20, 20) },
	};

	private EllipseShape ellipseShape = new()
	{
		Ellipse = new() { Center = new(50, 50), Radius = new(50, 25) },
	};

	private LineShape lineShape = new()
	{
		Line = new() { Start = new(100, 100), End = new(200, 200) },
	};

	internal void HandleDraw()
	{
		CreateDeviceResources();

		var xform = Transform.PixelIdentity.ToD2DMatrix3X2F();

		pRenderTarget->BeginDraw();
		pRenderTarget->SetTransform(in xform);

		var color = Colors.DarkSlateGray.ToD2DColorF();
		pRenderTarget->Clear(&color);

		GetClientRect(hWnd, out var rc);
		D2D_SIZE_U size = new()
		{
			width = (uint)(rc.right - rc.left),
			height = (uint)(rc.bottom - rc.top),
		};

		for (int x = 0; x < size.width; x += 10)
		{
			pRenderTarget->DrawLine(
				new D2D_POINT_2F { x = x, y = 0 },
				new D2D_POINT_2F { x = x, y = size.height },
				(ID2D1Brush*)pLightSlateGrayBrush,
				1f,
				null);
		}

		for (int y = 0; y < size.height; y += 10)
		{
			pRenderTarget->DrawLine(
				new D2D_POINT_2F { x = 0, y = y },
				new D2D_POINT_2F { x = size.width, y = y },
				(ID2D1Brush*)pLightSlateGrayBrush,
				1f,
				null);
		}

		if (Program.App != null)
		{
			var rect1 = new Rect
			{
				Position = new Vector2(Program.App.pointerPosition.X - 50, Program.App.pointerPosition.Y - 50),
				Size = new Vector2(100, 100),
			}.ToD2DRectF();

			var rect2 = new Rect
			{
				Position = new Vector2(Program.App.pointerPosition.X - 100, Program.App.pointerPosition.Y - 100),
				Size = new Vector2(200, 200),
			}.ToD2DRectF();

			pRenderTarget->FillRectangle(in rect1, (ID2D1Brush*)pCornflowerBlueBrush);
			pRenderTarget->DrawRectangle(in rect2, (ID2D1Brush*)pCornflowerBlueBrush, 1, default);

			var shapes = new Shape[] { rectShape, ellipseShape, lineShape };
			foreach (var shape in shapes)
			{
				switch (shape)
				{
					case RectShape s:
						pRenderTarget->DrawRectangle(s.Rect.ToD2DRectF(), (ID2D1Brush*)pCornflowerBlueBrush, 5, default);
						break;
					case EllipseShape s:
						pRenderTarget->DrawEllipse(s.Ellipse.ToD2DEllipse(), (ID2D1Brush*)pCornflowerBlueBrush, 5, default);
						break;
					case LineShape s:
						pRenderTarget->DrawLine(s.Line.Start.ToD2DPoint2F(), s.Line.End.ToD2DPoint2F(), (ID2D1Brush*)pCornflowerBlueBrush, 5, default);
						break;
					default:
						pRenderTarget->DrawGeometry(shape.GetResource(pD2DFactory), (ID2D1Brush*)pCornflowerBlueBrush, 5, default);
						break;
				}
			}

		}

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
		Size = new Vector2(width, height);
		Resize?.Invoke(new Events.ResizeEvent { Size = Size });
	}
}

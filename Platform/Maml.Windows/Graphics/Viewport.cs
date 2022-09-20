using Maml.Math;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using Windows.UI.Composition;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Direct2D.Common;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;

namespace Maml.Graphics;

public unsafe partial class Viewport
{
	internal HWND hWnd;
	internal ID2D1Factory* pD2DFactory;
	internal ID2D1HwndRenderTarget* pRenderTarget;

	~Viewport()
	{
		DiscardDeviceResources();
	}

	internal partial double GetDpi() => GetDpiForWindow(hWnd);
	internal partial Vector2 GetSize()
	{
		GetClientRect(hWnd, out var rc);
		return new(rc.right - rc.left, rc.bottom - rc.top);
	}

	private void CreateDeviceResources()
	{
		if (pRenderTarget == null)
		{
			D2D1_RENDER_TARGET_PROPERTIES renderTargetProps = new() {
				dpiX = 96f,
				dpiY = 96f,
			};

			D2D1_HWND_RENDER_TARGET_PROPERTIES hWndRenderTargetProps = new()
			{
				hwnd = hWnd,
				pixelSize = Size.ToD2DSizeU(),
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

	private GeometryGraphic ellipseGfx = new()
	{
		Transform = Transform.PixelIdentity,
		Geometry = new EllipseGeometry()
		{
			Ellipse = new() { Center = new(50, 50), Radius = new(50, 25) },
		},
		DrawLayers = new()
		{
			new Fill(new ColorBrush { Color = Colors.RebeccaPurple with { A = 0.25f } }),
			new Stroke(new ColorBrush { Color = Colors.Cyan }, 5),
			new Stroke(new ColorBrush { Color = Colors.Green }, 3),
		},
	};

	private GeometryGraphic rectGfx = new()
	{
		Transform = Transform.PixelIdentity,
		Geometry = new RectGeometry
		{
			Rect = new() { Position = new(-25, -25), Size = new(50, 50) },
		},
		DrawLayers = new()
		{
			new Fill(new ColorBrush { Color = Colors.RebeccaPurple with { A = 0.25f } }),
			new Stroke(new ColorBrush { Color = Colors.Cyan }, 5),
			new Stroke(new ColorBrush { Color = Colors.Green }, 3),
		},
	};

	private GeometryGraphic lineGfx = new()
	{
		Transform = Transform.PixelIdentity,
		Geometry = new LineGeometry () {
			Line = new() { Start = new(100, 100), End = new(200, 200) }
		},
		DrawLayers = new()
		{
			new Stroke(new ColorBrush { Color = Colors.LightPink }, 9),
			new Stroke(new ColorBrush { Color = Colors.HotPink }, 3),
			new Stroke(new ColorBrush { Color = Colors.Red }, 1),
		}
	};


	private GeometryGraphic lineGfxX = new()
	{
		Geometry = new LineGeometry
		{
			Line = new() { Start = new(0, 0), End = new(0, 0) },
		},
		DrawLayers = new()
		{
			new Stroke(new ColorBrush{Color = Colors.DarkBlue with { A = 0.25f } }, 5),
			new Stroke(new ColorBrush{Color = Colors.LightBlue with { A = 0.75f } }, 1),
		}
	};

	private GeometryGraphic lineGfxY = new()
	{
		Geometry = new LineGeometry
		{
			Line = new() { Start = new(0, 0), End = new(0, 0) },
		},
		DrawLayers = new()
		{
			new Stroke(new ColorBrush{Color = Colors.DarkBlue with { A = 0.25f } }, 5),
			new Stroke(new ColorBrush{Color = Colors.LightBlue with { A = 0.75f } }, 1),
		}
	};

	internal void HandleDraw()
	{
		CreateDeviceResources();

		pRenderTarget->BeginDraw();
		var d2dXform = Transform.PixelIdentity.Scaled(new Vector2(Dpi / 96, Dpi / 96)).ToD2DMatrix3X2F();
		pRenderTarget->SetTransform(in d2dXform);

		var color = Colors.DarkSlateGray.ToD2DColorF();
		pRenderTarget->Clear(&color);

		// Draw Vertical Lines
		((LineGeometry)lineGfxX.Geometry!).Line = new Line { Start = new(0, 0), End = new(0, Size.Y) };
		for (int x = 0; x < Size.X; x += 30)
		{
			lineGfxX.Transform = Transform.Identity.Translated(new Vector2(x, 0));
			lineGfxX.Draw((ID2D1RenderTarget*)pRenderTarget);
		}

		// Draw Horizontal Lines
		((LineGeometry)lineGfxY.Geometry!).Line = new Line { Start = new(0, 0), End = new(Size.X, 0) };
		for (int y = 0; y < Size.Y; y += 30)
		{
			lineGfxY.Transform = Transform.Identity.Translated(new Vector2(0, y));
			lineGfxY.Draw((ID2D1RenderTarget*)pRenderTarget);
		}

		// Transform Graphics
		if (Program.App != null)
		{
			rectGfx.Transform = Transform.Identity.Translated(Program.App.pointerPosition);
			ellipseGfx.Transform = Transform.Identity.Translated(Program.App.pointerPosition);
			lineGfx.Transform = Transform.Identity.Translated(Program.App.pointerPosition);
		}

		// Populate Array of Graphics
		Graphic[] graphics = new[]
		{
			ellipseGfx,
			lineGfx,
			rectGfx,
		};

		// Draw Graphics
		foreach (var gfx in graphics)
		{
			gfx.Draw((ID2D1RenderTarget*)pRenderTarget);
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
		Resize?.Invoke(new Events.ResizeEvent { Size = Size });
	}
}

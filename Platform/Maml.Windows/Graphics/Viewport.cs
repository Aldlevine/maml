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
	internal HWND hWnd;

	public partial void BeginDraw() => throw new System.NotImplementedException();
	public partial void ClearRect(Figure.Rect rect) => throw new System.NotImplementedException();
	public partial void EndDraw() => throw new System.NotImplementedException();
	public partial void FillPath(Path path) => throw new System.NotImplementedException();
	public partial void PopClip() => throw new System.NotImplementedException();
	public partial void PushClip(Path path) => throw new System.NotImplementedException();
	public partial void SetFillBrush(Brush brush) => throw new System.NotImplementedException();
	public partial void SetStrokeBrush(Brush brush) => throw new System.NotImplementedException();
	public partial void SetTransform(Transform transform) => throw new System.NotImplementedException();
	public partial void StrokePath(Path path) => throw new System.NotImplementedException();
}

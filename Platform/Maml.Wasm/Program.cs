using Maml.Graphics;
using Maml.Events;
using Maml.Math;
using Maml.Events;

namespace Maml.WasmBrowser;

public partial class Program
{
	public static void Main()
	{
		Viewport.Main.Resize += delegate (ResizeEvent evt)
		{
			Input.Report("WindowResize", $"Window Resize: {evt.Size}");
			Draw();
		};

		Input.PointerMove += delegate (PointerEvent evt)
		{
			Input.Report("PointerMove", $@"Pointer Move: {evt.Position}<br/>{evt.ButtonMask}");
			pointerPosition = evt.Position;
			Draw();
		};

		Input.PointerDown += delegate (PointerEvent evt)
		{
			Input.Report("PointerButton", $"Pointer Down: {evt.Button} @ {evt.Position}<br/>{evt.ButtonMask}");
			pointerPosition = evt.Position;
			Draw();
		};

		Input.PointerUp += delegate (PointerEvent evt)
		{
			Input.Report("PointerButton", $"Pointer Up: {evt.Button} @ {evt.Position}<br/>{evt.ButtonMask}");
		};

		Input.Wheel += delegate (WheelEvent evt)
		{
			Input.Report("Wheel", $"Wheel: {evt.Position}<br/>Delta={evt.Delta}");
		};

		Input.KeyDown += delegate (KeyEvent evt)
		{
			Input.Report("Keyboard", $"Key Down: key={evt.VirtualKey}, echo={evt.Echo}");
		};

		Input.KeyUp += delegate (KeyEvent evt)
		{
			Input.Report("Keyboard", $"Key Up: key={evt.VirtualKey}, echo={evt.Echo}");
		};

		Input.Focus += delegate (FocusEvent evt)
		{
			Input.Report("Focus", "Focus");
		};

		Input.Blur += delegate (FocusEvent evt)
		{
			Input.Report("Focus", "Blur");
		};

		Draw();
	}

	private static void Draw()
	{
		Viewport.Main.BeginDraw();
		Viewport.Main.ClearRect(new() { Origin = Vector2.Zero, Size = Viewport.Main.Size });
		Viewport.Main.SetTransform(new() { Origin = new Vector2(pointerPosition.X, pointerPosition.Y), X = Vector2.Right, Y = Vector2.Down });
		Viewport.Main.SetFillBrush(fill);
		Viewport.Main.SetStrokeBrush(stroke);
		Viewport.Main.FillPath(path);
		Viewport.Main.StrokePath(path);
		Viewport.Main.EndDraw();
	}

	static Vector2 pointerPosition = Vector2.Zero;

	static Path path = new Path()
	.Add(new Figure.Ellipse
	{
		Radii = new Vector2(100, 100),
		StartAngle = 0,
		EndAngle = double.Tau,
		Rotation = 0
	})
	.Add(new Figure.Rect
	{
		Size = new Vector2(100, 100),
	})
	.Add(new Figure.Rect
	{
		Origin = new Vector2(-100, -100),
		Size = new Vector2(100, 100),
	});

	static readonly Brush fill = new LinearGradientBrush
	{
		Start = new Vector2(-100, -100),
		End = new Vector2(100, 100),
		ColorStops =
		{
			new ColorStop(Colors.Coral, 0.0),
			new ColorStop(Colors.DarkOrchid, 1.0),
		}
	};

	static readonly Brush stroke = new ColorBrush
	{
		Color = Colors.Coral,
	};
}

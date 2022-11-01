using Maml.Graphics;
using Maml.Math;
using Maml.Observable;

namespace Maml.Scene;

public interface IGraphicNode
{
	bool NeedsRedraw { get; set; }
	void Draw(RenderTarget renderTarget);
	Rect PreviousBoundingRect { get; set; }
	Rect GetBoundingRect();
}

public class GraphicNode : Node, IGraphicNode
{
	public bool NeedsRedraw { get; set; } = true;
	public static BasicProperty<GraphicNode, Graphic?> GraphicProperty { get; } = new(null);
	public Graphic? Graphic
	{
		get => GraphicProperty[this].Get();
		set => GraphicProperty[this].Set(value);
	}

	public virtual void Draw(RenderTarget renderTarget) => Graphic?.Draw(renderTarget, GlobalTransform);

	public Rect PreviousBoundingRect { get; set; } = new();
	public virtual Rect GetBoundingRect() => GlobalTransform * Graphic?.GetBoundingRect() ?? new Rect();

	public GraphicNode()
	{
		Changed += (s, p) => NeedsRedraw = (p != Node.VisibleInTreeProperty) && VisibleInTree;
	}
}

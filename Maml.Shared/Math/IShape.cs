namespace Maml.Math;

public interface IShape
{
	bool HasPoint(in Vector2 point);

	Rect GetBoundingRect(in Transform transform);
	Rect GetBoundingRect() => GetBoundingRect(Transform.Identity);
}

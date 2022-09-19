using System;
using System.Collections.Generic;

namespace Maml.Geometry;

public class Path
{
	internal Guid ID = Guid.NewGuid();
	internal bool Dirty = true;

	public List<Figure> Figures { get; init; } = new();

	public Path Add(Figure segment)
	{
		Dirty = true;
		Figures.Add(segment);
		return this;
	}
}

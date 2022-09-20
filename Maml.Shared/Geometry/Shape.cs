using System;
using System.Collections.Generic;
using System.Text;

namespace Maml.Geometry;

public abstract partial class Shape: IDisposable
{
	internal bool IsDirty = true;
	private bool disposedValue;

	protected virtual partial void FreeResources();

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				// TODO: dispose managed state (managed objects)
			}

			// free unmanaged resources (unmanaged objects) and override finalizer
			FreeResources();
			// TODO: set large fields to null
			disposedValue = true;
		}
	}

	// override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
	~Shape()
	{
	    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
	    Dispose(disposing: false);
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}

public partial class RectShape: Shape
{
	private Rect rect;
	public Rect Rect
	{
		get => rect;
		set
		{
			rect = value;
			IsDirty = true;
		}
	}
}

public partial class EllipseShape: Shape
{
	private Ellipse ellipse;
	public Ellipse Ellipse
	{
		get => ellipse;
		set
		{
			ellipse = value;
			IsDirty = true;
		}
	}
}

public partial class LineShape: Shape
{
	private Line line;
	public Line Line
	{
		get => line;
		set
		{
			line = value;
			IsDirty = true;
		}
	}
}

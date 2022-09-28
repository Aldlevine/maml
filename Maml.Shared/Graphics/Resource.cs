using Maml.Events;
using System;

namespace Maml.Graphics;
public abstract partial class Resource : IDisposable, IChanged
{
	public event EventHandler<ChangedEvent>? Changed;
	public void RaiseChanged(object? sender, ChangedEvent e) => Changed?.Invoke(sender, e);

	public bool IsDirty { get; internal set; }

	private bool isDisposed;

	protected abstract void FreeResources();
	protected virtual void Dispose(bool disposing)
	{
		if (!isDisposed)
		{
			if (disposing)
			{
				// TODO: dispose managed state (managed objects)
			}

			// free unmanaged resources (unmanaged objects) and override finalizer
			FreeResources();
			// TODO: set large fields to null
			isDisposed = true;
		}
	}

	// override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
	~Resource()
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

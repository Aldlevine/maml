using System;
using System.Collections.Generic;

namespace Maml.Observable;

public class ObservableObject: IDisposable
{
	internal HashSet<Property> boundProperties { get; } = new();

	public Binding this[Property property]
	{
		get
		{
			dynamic p = property;
			return p.GetBinding(this);
		}
		init
		{
			dynamic p = property;
			Binding b = p.GetBinding(this);
			b.BindTo(value);
		}
	}


	private bool disposedValue;
	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				// TODO: dispose managed state (managed objects)
			}

			// foreach (var kv in bindings)
			foreach (var prop in boundProperties)
			{
				dynamic p = prop;
				p.RemoveBinding(this);
			}

			// TODO: free unmanaged resources (unmanaged objects) and override finalizer
			// TODO: set large fields to null
			disposedValue = true;
		}
	}

	// TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
	~ObservableObject()
	{
		Console.WriteLine("BYE!!");
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


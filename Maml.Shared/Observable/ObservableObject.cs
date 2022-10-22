using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Maml.Observable;

public class ObservableObject
{
	public event EventHandler<Property>? Changed;
	internal void EmitChanged(Property property)
	{
		foreach (var kv in dependentBindings)
		{
			if (kv.Value.TryGetTarget(out var b))
			{
				b.SetDirty();
			}
		}
		Changed?.Invoke(this, property);
	}

	internal static ulong currentId = 0;
	internal ulong id = currentId++;
	internal HashSet<Property> boundProperties { get; } = new();

	internal ConcurrentDictionary<int, WeakReference<Binding>> dependentBindings { get; } = new();
	internal void RemoveDependentBinding(Binding binding)
	{
		dependentBindings.Remove(binding.GetHashCode(), out var _);
	}

	internal void AddDependentBinding(Binding binding)
	{
		RemoveDependentBinding(binding);
		dependentBindings[binding.GetHashCode()] = new WeakReference<Binding>(binding);
	}

	public Binding this[Property property]
	{
		get => property.GetBinding(this);
		init => property.GetBinding(this).BindTo(value);
	}

	~ObservableObject()
	{
		foreach (var property in boundProperties)
		{
			property.RemoveBinding(this);
		}
	}
}


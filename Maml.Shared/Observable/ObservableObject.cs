using System;
using System.Collections.Generic;

namespace Maml.Observable;

public class ObservableObject
{
	public event EventHandler<Property>? Changed;
	internal void EmitChanged(Property property)
	{
		// foreach (Binding b in dependentBindings)
		foreach (var wr in dependentBindings)
		{
			if (wr.TryGetTarget(out var b))
			{
				b.SetDirty();
			}
		}
		Changed?.Invoke(this, property);
	}

	internal static ulong currentId = 0;
	internal ulong id = currentId++;
	// TODO: Make this a ConcurrentHashSet
	internal HashSet<Property> boundProperties { get; } = new();

	internal HashSet<WeakReference<Binding>> dependentBindings { get; } = new();
	internal void RemoveDependentBinding(Binding binding)
	{
		dependentBindings.RemoveWhere((wr) =>
		{
			if (wr.TryGetTarget(out var b))
			{
				return b == binding;
			}
			return true;
		});
	}

	internal void AddDependentBinding(Binding binding)
	{
		RemoveDependentBinding(binding);
		dependentBindings.Add(new WeakReference<Binding>(binding));
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


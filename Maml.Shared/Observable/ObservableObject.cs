using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Maml.Observable;

public class ObservableObject
{
	internal static ulong currentId = 0;
	internal ulong id = currentId++;
	// TODO: Make this a ConcurrentHashSet
	internal HashSet<Property> boundProperties { get; } = new();

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


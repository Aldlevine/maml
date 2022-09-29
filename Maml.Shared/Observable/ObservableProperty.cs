﻿using System.Collections.Generic;

namespace Maml.Observable;

public class ObservableProperty<O, T> : IProperty<O, T> where O : ObservableObject
{
	public T Default { get; init; }

	public ObservableProperty(T @default)
	{
		Default = @default;
	}

	public IBinding<O, T> this[O @object] => GetBinding(@object);

	private Dictionary<O, ObservableBinding<O, T>> bindings = new();
	public IBinding<O, T> GetBinding(O @object)
	{
		if (!bindings.TryGetValue(@object, out var binding))
		{
			bindings[@object] = binding = new ObservableBinding<O, T>(@object, this);
		}
		return binding;
	}

	public IBinding<O, T> GetBinding(ObservableObject @object) => GetBinding((O)@object);
}
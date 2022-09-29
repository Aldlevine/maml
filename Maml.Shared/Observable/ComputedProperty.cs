using System.Collections.Generic;

namespace Maml.Observable;

public delegate T ComputedPropertyGetter<O, T>(O @object) where O : ObservableObject;

public delegate void ComputedPropertySetter<O, T>(O @object, T value) where O : ObservableObject;

public class ComputedProperty<O, T> : IProperty<O, T> where O : ObservableObject
{
	public ComputedPropertyGetter<O, T>? Get;
	public ComputedPropertySetter<O, T>? Set;
	public bool Cached = true;

	public IBinding<O, T> this[O @object] => GetBinding(@object);

	private Dictionary<O, ComputedBinding<O, T>> bindings = new();
	public IBinding<O, T> GetBinding(O @object)
	{
		if (!bindings.TryGetValue(@object, out var binding))
		{
			bindings[@object] = binding = new ComputedBinding<O, T>(@object, this);
		}
		return binding;
	}

	public IBinding<O, T> GetBinding(ObservableObject @object) => GetBinding((O)@object);
}

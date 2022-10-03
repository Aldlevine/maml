using Maml.Math;
using System;
using System.Collections.Generic;

namespace Maml.Observable;

public abstract class Property { }
public abstract class Property<T>: Property { }
public abstract class Property<O, T>: Property<T> where O: ObservableObject
{
	protected abstract Binding<O, T> CreateBinding(O @object);

	public Binding<O, T> this[O @object] => GetBinding(@object);

	private Dictionary<O, Binding<O, T>> bindings = new();

	public Binding<O, T> GetBinding(ObservableObject @object) => GetBinding((O)@object);
	public Binding<O, T> GetBinding(O @object)
	{
		if (!bindings.TryGetValue(@object, out var binding))
		{
			binding = CreateBinding(@object);
			bindings[@object] = binding;
			// @object.bindings[this] = new(binding);
			@object.boundProperties.Add(this);
		}

		return binding;
	}

	public void RemoveBinding(ObservableObject @object) => RemoveBinding((O)@object);
	public void RemoveBinding(O @object) => bindings.Remove(@object);
}

public class BasicProperty<O, T> : Property<O, T> where O : ObservableObject
{
	public T Default { get; init; }
	public BasicProperty(T @default) { Default = @default; }
	protected override Binding<O, T> CreateBinding(O @object) => new BasicBinding<O, T>(@object, this);
}

public delegate T ComputedPropertyGetter<O, T>(O @object) where O : ObservableObject;
public delegate void ComputedPropertySetter<O, T>(O @object, T value) where O : ObservableObject;
public delegate Binding[] ComputedPropertyInitter<O, T>(O @object) where O : ObservableObject;
public class ComputedProperty<O, T> : Property<O, T> where O : ObservableObject
{
	public ComputedPropertyGetter<O, T>? Get;
	public ComputedPropertySetter<O, T>? Set;
	public ComputedPropertyInitter<O, T>? Dependencies;
	public bool Cached = true;
	protected override Binding<O, T> CreateBinding(O @object) => new ComputedBinding<O, T>(@object, this);
}

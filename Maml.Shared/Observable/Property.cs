using Maml.Math;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Maml.Observable;

#region Abstract
public abstract class Property
{
	public abstract Binding GetBinding(ObservableObject @object);
	public abstract void RemoveBinding(ObservableObject @object);
}

public abstract class Property<T>: Property { }

public abstract class Property<O, T>: Property<T> where O: ObservableObject
{
	public Binding<O, T> this[O @object] => GetBinding(@object);

	public override Binding GetBinding(ObservableObject @object) => GetBinding((O)@object);
	public Binding<O, T> GetBinding(O @object)
	{
		if (!bindings.TryGetValue(@object.id, out var binding))
		{
			binding = CreateBinding(@object);
			bindings[@object.id] = binding;
			@object.boundProperties.Add(this);
		}

		return binding;
	}

	public override void RemoveBinding(ObservableObject @object) => RemoveBinding((O)@object);
	public void RemoveBinding(O @object)
	{
		bindings.Remove(@object.id, out _);
	}

	protected abstract Binding<O, T> CreateBinding(O @object);
	private ConcurrentDictionary<ulong, Binding<O, T>> bindings = new();
}
#endregion

#region Basic Property
public class BasicProperty<O, T> : Property<O, T> where O : ObservableObject
{
	public T Default { get; init; }
	public BasicProperty(T @default) { Default = @default; }
	protected override Binding<O, T> CreateBinding(O @object) => new BasicBinding<O, T>(@object, this);
}
#endregion

#region Computed Property
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
#endregion

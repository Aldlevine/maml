using System;
using System.Collections.Generic;

namespace Maml.Observable;

public abstract class Binding
{
	public abstract void BindTo(Binding from);
}


public abstract class Binding<T> : Binding
{
	public virtual T Value { get; protected set; } = default!;

	public abstract T Get();
	public abstract bool Set(T value);

	public abstract event EventHandler<T>? Changed;
	public abstract void HandleChanged(object? sender, T value);
	public abstract void SetDirty(object? sender, T value);

	public abstract void BindTo(Binding<T> from);

	public override void BindTo(Binding from) => BindTo((Binding<T>)from);
}


public abstract class Binding<O, T> : Binding<T> where O : ObservableObject
{
	public virtual O Object { get; protected set; } = default!;
	public virtual Property<O, T> Property { get; protected set; } = default!;

	// TODO: Do we need to keep a ref around??
	public Binding<O, R> With<R>(Func<T, R> func)
	{
		var property = new ObservableProperty<O, R>(default(R)!);
		var binding = new ObservableBinding<O, R>(Object, property);
		Changed += (s, v) => binding.HandleChanged(s, func(v));
		return binding;
	}
}

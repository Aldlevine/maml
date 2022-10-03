using System;
using System.Collections.Generic;

namespace Maml.Observable;

public interface IBinding
{
	public object? Get();
	public bool Set(object? value);
	public void HandleChanged(object? sender, object? value);
	public void SetDirty(object? sender, object? value);
	public void BindTo(IBinding from);
}

public interface IBinding<T> : IBinding
{
	public T Value { get; }

	public new T Get();
	public bool Set(T value);

	public event EventHandler<T>? Changed;
	public void HandleChanged(object? sender, T value);
	public void SetDirty(object? sender, T value);

	public void BindTo(IBinding<T> from);

	object? IBinding.Get() => ((IBinding<T>)this).Get();
	bool IBinding.Set(object? value) => ((IBinding<T>)this).Set((T)value!);
	void IBinding.HandleChanged(object? sender, object? value) => ((IBinding<T>)this).HandleChanged(sender, (T)value!);
	void IBinding.SetDirty(object? sender, object? value) => ((IBinding<T>)this).SetDirty(sender, (T)value!);
	void IBinding.BindTo(IBinding from) => ((IBinding<T>)this).BindTo((IBinding<T>)from);

}

public interface IBinding<O, T> : IBinding<T> where O : ObservableObject
{
	O Object { get; }
	IProperty<O, T> Property { get; }

	// TODO: Do we need to keep a ref around??
	public IBinding<O, R> With<R>(Func<T, R> func)
	{
		var property = new ObservableProperty<O, R>(default(R));
		var binding = new ObservableBinding<O, R>(Object, property);
		Changed += (s, v) => binding.HandleChanged(s, func(v));
		return binding;
	}
}


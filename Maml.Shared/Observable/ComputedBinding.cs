using System;
using Windows.Media.Devices;

namespace Maml.Observable;

public class ComputedBinding<O, T> : Binding<O, T> where O : ObservableObject
{
	public override event EventHandler<T>? Changed;
	public override void HandleChanged(object? sender, T value) => Set(value);
	public override void SetDirty(object? sender, T value)
	{
		isDirty = true;
		if (ComputedProperty.Get != null)
		{
			Changed?.Invoke(sender, ComputedProperty.Get(Object));
		}
	}

	public override Property<O, T> Property => ComputedProperty;
	public ComputedProperty<O, T> ComputedProperty;
	private bool isDirty = true;

	public ComputedBinding(O @object, ComputedProperty<O, T> property)
	{
		Object = @object;
		ComputedProperty = property;
		Value = default!;
	}

	public override T Get()
	{
		if (ComputedProperty.Get == null)
		{
			throw new InvalidOperationException();
		}

		if (isDirty || !ComputedProperty.Cached)
		{
			Value = ComputedProperty.Get(Object);
			isDirty = false;
		}
		return Value!;
	}

	public override bool Set(T value)
	{
		if (ComputedProperty.Set == null)
		{
			throw new InvalidOperationException();
		}

		ComputedProperty.Set(Object, value);
		if (ComputedProperty.Get != null)
		{
			// TODO: we shouldn't call Get here.
			// It's possible that multiple changes happen before
			// evaluation is needed, so we should have a way of
			// passing the value lazily
			Changed?.Invoke(this, ComputedProperty.Get(Object));
			isDirty = true;
			return true;
		}
		return false;
	}

	public override void BindTo(Binding<T> from)
	{
		from.Changed += HandleChanged;
		if (ComputedProperty.Set != null)
		{
			Set(from.Value);
		}
		SetDirty(this, Value);
	}
}

public record struct LazyGet<O, T>(ComputedBinding<O, T> Binding) where O : ObservableObject
{
	public static implicit operator T(LazyGet<O, T> lazyGet) => lazyGet.Binding.Get();
	public T Value => Binding.Get();
	public override string? ToString() => Value?.ToString();
}

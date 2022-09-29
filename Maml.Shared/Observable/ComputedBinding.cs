using System;

namespace Maml.Observable;

public class ComputedBinding<O, T> : IBinding<O, T> where O : ObservableObject
{
	// public event EventHandler<LazyGet<O, T>>? Changed;
	public event EventHandler<T>? Changed;
	public void HandleChanged(object? sender, T value) => Set(value);
	public void SetDirty(object? sender, T value)
	{
		isDirty = true;
		// Changed?.Invoke(sender, new(this));
		Changed?.Invoke(sender, value);
	}

	public O Object { get; init; }
	public IProperty<O, T> Property => ComputedProperty;
	public ComputedProperty<O, T> ComputedProperty;
	public T Value { get; private set; }
	private bool isDirty = true;

	public ComputedBinding(O @object, ComputedProperty<O, T> property)
	{
		Object = @object;
		ComputedProperty = property;
		Value = default!;
	}

	public T Get()
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

	public bool Set(T value)
	{
		if (ComputedProperty.Set == null)
		{
			throw new InvalidOperationException();
		}

		ComputedProperty.Set(Object, value);
		// Changed?.Invoke(this, new(this));
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

	public void BindTo(IBinding<T> from)
	{
		//from.Changed += HandleChanged;
		//Value = from.Value;
		from.Changed += HandleChanged;
		Value = from.Value;
		SetDirty(this, Value);
	}
}

public record struct LazyGet<O, T>(ComputedBinding<O, T> Binding) where O : ObservableObject
{
	public static implicit operator T(LazyGet<O, T> lazyGet) => lazyGet.Binding.Get();
	public T Value => Binding.Get();
	public override string? ToString() => Value?.ToString();
}

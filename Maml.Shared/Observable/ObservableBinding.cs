using System;

namespace Maml.Observable;

public class ObservableBinding<O, T> : IBinding<O, T> where O : ObservableObject
{
	public event EventHandler<T>? Changed;
	public void HandleChanged(object? sender, T value) => Set(value);

	public O Object { get; init; }
	public IProperty<O, T> Property { get; init; }
	public T Value { get; private set; }

	public ObservableBinding(O @object, ObservableProperty<O, T> property)
	{
		Object = @object;
		Property = property;
		Value = property.Default;
	}

	public T Get() => Value;

	public bool Set(T value)
	{
		if (!object.Equals(this.Value, value))
		{
			Value = value;
			Changed?.Invoke(this, Value);
			return true;
		}
		return false;
	}

	public void BindTo(IBinding<T> from)
	{
		Value = from.Value;
		from.Changed += HandleChanged;
	}

	public void SetDirty(object? sender, T value)
	{
		Changed?.Invoke(sender, value);
	}
}

using System;
using Windows.Gaming.Input;

namespace Maml.Observable;

public class ObservableBinding<O, T> : Binding<O, T> where O : ObservableObject
{
	public override event EventHandler<T>? Changed;
	public override void HandleChanged(object? sender, T value) => Set(value);

	public ObservableBinding(O @object, ObservableProperty<O, T> property)
	{
		Object = @object;
		Property = property;
		Value = property.Default;
	}

	public override T Get() => Value;

	public override bool Set(T value)
	{
		if (!object.Equals(this.Value, value))
		{
			Value = value;
			Changed?.Invoke(this, Value);
			return true;
		}
		return false;
	}

	public override void BindTo(Binding<T> from)
	{
		from.Changed += HandleChanged;
		Value = from.Value;
		SetDirty(this, Value);
	}

	public override void SetDirty(object? sender, T value)
	{
		Changed?.Invoke(sender, value);
	}
}

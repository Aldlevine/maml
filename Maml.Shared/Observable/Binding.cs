using System;
using System.Collections.Generic;

namespace Maml.Observable;

#region Abstract
public abstract class Binding
{
	public abstract void SetDirty();
	public abstract void BindTo(Binding from);

	protected List<WeakReference<Binding>> dependTo { get; init; } = new();
	// protected List<WeakReference<Binding>> dependFrom { get; init; } = new();
	protected List<Binding> dependFrom { get; init; } = new();
	public void DependOn(Binding from)
	{
		// dependFrom.Add(new(from));
		dependFrom.Add(from);
		from.dependTo.Add(new(this));
	}

	public void UndependOn(Binding from)
	{
		//var bf = dependFrom.Find(wr =>
		//{
		//	if (wr.TryGetTarget(out var b))
		//	{
		//		return b == from;
		//	}
		//	return false;
		//});
		//if (bf != null)
		//{
		//	dependFrom.Remove(bf);
		//}
		dependFrom.Remove(from);

		var bt = from.dependTo.Find(wr =>
		{
			if (wr.TryGetTarget(out var b))
			{
				return b == from;
			}
			return false;
		});
		if (bt != null)
		{
			from.dependTo.Remove(bt);
		}
	}

	~Binding()
	{
		// foreach (var wr in dependFrom)
		//foreach (var b in dependFrom)
		//{
		//	//if (wr.TryGetTarget(out var b))
		//	{
		//		UndependOn(b);
		//	}
		//}
		//foreach (var wr in dependTo)
		//{
		//	if (wr.TryGetTarget(out var b))
		//	{
		//		b.UndependOn(this);
		//	}
		//}
	}
}

public abstract class Binding<T> : Binding
{
	public virtual T Value { get; protected set; } = default!;
	protected bool IsDirty = true;

	public abstract T Get(bool keepDirty = false);
	public abstract bool Set(T value);
	public abstract event EventHandler<LazyGet<T>>? Changed;

	public void SetDirty(object? sender, LazyGet<T> value) => SetDirty();

	public override void BindTo(Binding from) => BindTo((Binding<T>)from);
	protected List<WeakReference<Binding<T>>> boundTo { get; init; } = new();
	//protected List<WeakReference<Binding<T>>> boundFrom { get; init; } = new();
	protected List<Binding<T>> boundFrom { get; init; } = new();
	public virtual void BindTo(Binding<T> from)
	{
		boundFrom.Add(from);
		from.boundTo.Add(new(this));
		Set(from.Value);
	}

	public void UnbindFrom(Binding<T> from)
	{
		boundFrom.Remove(from);
		//var bf = boundFrom.Find(wr =>
		//{
		//	if (wr.TryGetTarget(out var b))
		//	{
		//		return b == from;
		//	}
		//	return false;
		//});
		//if (bf != null)
		//{
		//	boundFrom.Remove(bf);
		//}

		var bt = from.boundTo.Find(wr =>
		{
			if (wr.TryGetTarget(out var b))
			{
				return b == from;
			}
			return false;
		});
		if (bt != null)
		{
			from.boundTo.Remove(bt);
		}
	}

	~Binding()
	{
		//foreach (var b in boundFrom)
		//{
		//	//if (wr.TryGetTarget(out var b))
		//	{
		//		UnbindFrom(b);
		//	}
		//}
		//foreach (var wr in boundTo)
		//{
		//	if (wr.TryGetTarget(out var b))
		//	{
		//		b.UnbindFrom(this);
		//	}
		//}
	}
}

public abstract class Binding<O, T> : Binding<T> where O : ObservableObject
{
	public virtual WeakReference<O> Object { get; protected set; } = default!;
	public virtual Property<O, T> Property { get; protected set; } = default!;

	public override event EventHandler<LazyGet<T>>? Changed;



	// TODO: Do we need to keep a ref around??
	public Binding<O, R> With<R>(Func<T, R> func)
	{
		if (!Object.TryGetTarget(out var @object))
		{
			throw new NullReferenceException();
		}
		var property = new BasicProperty<O, R>(default(R)!);
		var binding = new BasicBinding<O, R>(@object, property);
		Changed += (s, v) =>
		{
			binding.Value = func(v.Value);
			binding.SetDirty();
		};
		return binding;
	}

	public override void SetDirty()
	{
		IsDirty = true;

		if (boundTo.Count > 0)
		{
			T value = Get(true);
			foreach (var wr in boundTo)
			{
				if (wr.TryGetTarget(out var b))
				{
					b.Set(value);
				}
				//else
				//{
				//	boundTo.Remove(wr);
				//}
			}
		}

		foreach (var wr in dependTo)
		{
			if (wr.TryGetTarget(out var b))
			{
				b.SetDirty();
			}
			//else
			//{
			//	dependTo.Remove(wr);
			//}
		}

		Changed?.Invoke(this, new LazyGet<O, T>(this));
	}
}
#endregion

#region Concrete
public class BasicBinding<O, T> : Binding<O, T> where O : ObservableObject
{
	public BasicBinding(O @object, BasicProperty<O, T> property)
	{
		Object = new(@object);
		Property = property;
		Value = property.Default;
	}

	public override T Get(bool keepDirty = false) => Value;

	public override bool Set(T value)
	{
		if (!object.Equals(this.Value, value))
		{
			Value = value;
			SetDirty();
			return true;
		}
		return false;
	}
}

public class ComputedBinding<O, T> : Binding<O, T> where O : ObservableObject
{
	public override Property<O, T> Property => ComputedProperty;
	public ComputedProperty<O, T> ComputedProperty;

	public ComputedBinding(O @object, ComputedProperty<O, T> property)
	{
		Object = new(@object);
		ComputedProperty = property;
		Value = default!;

		if (ComputedProperty.Dependencies != null)
		{
			foreach (var b in ComputedProperty.Dependencies(@object))
			{
				DependOn(b);
			}
		}
	}

	public override T Get(bool keepDirty = false)
	{
		if (ComputedProperty.Get == null)
		{
			//return default(T)!;
			throw new InvalidOperationException();
		}

		if (IsDirty || !ComputedProperty.Cached)
		{
			if (!Object.TryGetTarget(out var @object))
			{
				throw new NullReferenceException();
			}
			Value = ComputedProperty.Get(@object);
			IsDirty = keepDirty;
		}
		return Value!;
	}

	public override bool Set(T value)
	{
		if (ComputedProperty.Set == null)
		{
			//return false;
			throw new InvalidOperationException();
		}

		if (!Object.TryGetTarget(out var @object))
		{
			throw new NullReferenceException();
		}
		ComputedProperty.Set(@object, value);
		if (ComputedProperty.Get != null)
		{
			SetDirty();
			return true;
		}
		return false;
	}
}
#endregion

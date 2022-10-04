namespace Maml.Observable;

public record LazyGet<T>(T Value)
{
	public virtual T Value { get; } = Value;
	public static implicit operator LazyGet<T>(T value) => new LazyGet<T>(value);
};

public record LazyGet<O, T>(Binding<O, T> Binding) : LazyGet<T>(default(T)!) where O : ObservableObject
{
	public static implicit operator T(LazyGet<O, T> lazyGet) => lazyGet.Binding.Get();
	public override T Value => Binding.Get();
	public override string ToString() => Value?.ToString() ?? "NULL";
}

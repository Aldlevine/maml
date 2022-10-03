namespace Maml.Observable;

public abstract class Property { }

public abstract class Property<T>: Property { }

public abstract class Property<O, T>: Property<T> where O: ObservableObject
{
	public abstract Binding<O, T> this[O @object] { get; }
	public abstract Binding<O, T> GetBinding(O @object);
	public abstract Binding<O, T> GetBinding(ObservableObject @object);
}

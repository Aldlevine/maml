namespace Maml.Observable;

public interface IProperty { }

public interface IProperty<T> : IProperty { }

public interface IProperty<O, T> : IProperty<T> where O : ObservableObject
{
	IBinding<O, T> this[O @object] { get; }
	IBinding<O, T> GetBinding(O @object);
	IBinding<O, T> GetBinding(ObservableObject @object);
}

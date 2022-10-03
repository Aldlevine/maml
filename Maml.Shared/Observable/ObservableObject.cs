namespace Maml.Observable;

// TODO: Need to clean up subscriptions so that abandoned objects can be GCd
// Possibly implement IDisposable?
// Or possibly implement a custom event that uses weak refs to delegates.
public class ObservableObject
{
	public IBinding this[IProperty property]
	{
		get
		{
			dynamic p = property;
			return p.GetBinding(this);
		}
		init
		{
			dynamic p = property;
			IBinding b = p.GetBinding(this);
			b.BindTo(value);
		}
	}

	public IBinding<O, T> GetBinding<O, T>(IProperty<O, T> property) where O : ObservableObject => property.GetBinding((O)this);
}


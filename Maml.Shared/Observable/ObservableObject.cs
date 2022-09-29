namespace Maml.Observable;

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
			// dynamic v = value;
			b.BindTo(value);
		}
	}

	public IBinding<O, T> GetBinding<O, T>(IProperty<O, T> property) where O : ObservableObject => property.GetBinding((O)this);
}


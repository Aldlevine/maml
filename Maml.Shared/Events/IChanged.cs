using System;
using System.Collections.Generic;
using System.Text;

namespace Maml.Events;

public record ChangedEvent: Event { }

public interface IChanged
{
	event EventHandler<ChangedEvent>? Changed;
	void RaiseChanged(object? sender, ChangedEvent e);
}

using System;

namespace Maml.Events;

public record ChangedEvent : Event { }

public interface IChanged
{
	event EventHandler<ChangedEvent>? Changed;
	void RaiseChanged(object? sender, ChangedEvent e);
}

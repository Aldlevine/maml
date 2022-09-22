using System;

namespace Maml.Events;

[Flags]
public enum PointerButton
{
	None = 0,
	Left = 1,
	Right = 2,
	Middle = 4,
	Back = 8,
	Forward = 16,
};

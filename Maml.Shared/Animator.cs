using Maml.Events;

namespace Maml.Animation;

public partial class Animator
{
	public event EventHandler<FrameEvent>? Frame;

}

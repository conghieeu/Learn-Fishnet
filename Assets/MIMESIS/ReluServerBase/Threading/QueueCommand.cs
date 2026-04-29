namespace ReluServerBase.Threading
{
	public struct QueueCommand
	{
		private Command CommandDelegate;

		public QueueCommand(Command command)
		{
			CommandDelegate = command;
		}

		public void Execute()
		{
			CommandDelegate();
		}
	}
}

using System;
using System.Collections.Concurrent;

namespace ReluServerBase.Threading
{
	public class CommandExecutor : IDisposable
	{
		private static CommandExecutor? dummy;

		private static object syncRoot = new object();

		private readonly string m_name;

		private readonly long m_ownerObjectID;

		private volatile bool m_closed;

		private ConcurrentQueue<QueueCommand> PendingCommnadQueue = new ConcurrentQueue<QueueCommand>();

		private bool disposedValue;

		public static CommandExecutor DummyCommandExecutor
		{
			get
			{
				if (dummy == null)
				{
					lock (syncRoot)
					{
						if (dummy == null)
						{
							dummy = new CommandExecutor();
							dummy.Close();
						}
					}
				}
				return dummy;
			}
		}

		public static CommandExecutor CreateCommandExecutor(string name, long objectID)
		{
			return new CommandExecutor(name, objectID);
		}

		protected CommandExecutor()
		{
			m_name = "Dummy";
			m_ownerObjectID = 0L;
		}

		protected CommandExecutor(string name, long ObjectID)
		{
			m_name = name;
			m_ownerObjectID = ObjectID;
		}

		~CommandExecutor()
		{
			Dispose(disposing: false);
		}

		public override string ToString()
		{
			return $"CommandExecutor: {m_name} :ObjectID({m_ownerObjectID})";
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposedValue)
			{
				return;
			}
			if (disposing)
			{
				QueueCommand result;
				while (PendingCommnadQueue.TryDequeue(out result))
				{
				}
			}
			disposedValue = true;
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public bool IsEmpty()
		{
			return PendingCommnadQueue.IsEmpty;
		}

		public long Length()
		{
			return PendingCommnadQueue.Count;
		}

		public void Close()
		{
			m_closed = true;
		}

		public bool Invoke(Command command)
		{
			if (m_closed)
			{
				return false;
			}
			PendingCommnadQueue.Enqueue(new QueueCommand(command));
			return true;
		}

		public void Execute()
		{
			PumpQueuedCommand();
		}

		private void PumpQueuedCommand()
		{
			QueueCommand result;
			while (PendingCommnadQueue.TryDequeue(out result))
			{
				result.Execute();
			}
		}
	}
}

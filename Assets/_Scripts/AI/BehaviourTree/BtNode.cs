using System;

namespace MellowAbelson.AI.BehaviourTree
{
    public enum BtNodeState
    {
        Running,
        Success,
        Failure
    }

    public abstract class BtNode
    {
        public string Name { get; protected set; }
        public BtNodeState State { get; protected set; }

        protected BtNode(string name = "Node")
        {
            Name = name;
            State = BtNodeState.Failure;
        }

        public abstract BtNodeState Execute();

        public virtual void Reset()
        {
            State = BtNodeState.Failure;
        }
    }
}

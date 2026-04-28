using System;

namespace MellowAbelson.AI.BehaviourTree
{
    public class ActionNode : BtNode
    {
        private readonly Func<BtNodeState> _action;

        public ActionNode(string name, Func<BtNodeState> action) : base(name)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public override BtNodeState Execute()
        {
            State = _action.Invoke();
            return State;
        }
    }
}

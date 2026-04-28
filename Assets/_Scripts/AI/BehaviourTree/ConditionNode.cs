using System;

namespace MellowAbelson.AI.BehaviourTree
{
    public class ConditionNode : BtNode
    {
        private readonly Func<bool> _condition;

        public ConditionNode(string name, Func<bool> condition) : base(name)
        {
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }

        public override BtNodeState Execute()
        {
            State = _condition.Invoke() ? BtNodeState.Success : BtNodeState.Failure;
            return State;
        }
    }
}

using System.Collections.Generic;

namespace MellowAbelson.AI.BehaviourTree
{
    public class SelectorNode : BtNode
    {
        private readonly List<BtNode> _children;

        public SelectorNode(string name = "Selector", List<BtNode> children = null) : base(name)
        {
            _children = children ?? new List<BtNode>();
        }

        public void AddChild(BtNode child) => _children.Add(child);

        public override BtNodeState Execute()
        {
            foreach (var child in _children)
            {
                var result = child.Execute();
                if (result == BtNodeState.Success)
                {
                    State = BtNodeState.Success;
                    return State;
                }
                if (result == BtNodeState.Running)
                {
                    State = BtNodeState.Running;
                    return State;
                }
            }
            State = BtNodeState.Failure;
            return State;
        }

        public override void Reset()
        {
            base.Reset();
            foreach (var child in _children)
                child.Reset();
        }
    }
}

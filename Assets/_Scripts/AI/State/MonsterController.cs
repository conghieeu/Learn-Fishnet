using FishNet.Object;
using FishNet.Object.Synchronizing;
using MellowAbelson.AI.BehaviourTree;
using UnityEngine;

namespace MellowAbelson.AI.State
{
    public enum MonsterState
    {
        Idle,
        Roaming,
        Investigating,
        Chasing,
        Attacking,
        Fleeing,
        Dead
    }

    public class MonsterController : NetworkBehaviour
    {
        [SerializeField] private float _sightRange = 15f;
        [SerializeField] private float _viewAngle = 90f;
        [SerializeField] private float _hearingRange = 20f;
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _chaseSpeed = 6f;

        [SyncVar] private MonsterState _currentState = MonsterState.Idle;
        private BehaviourTreeRunner _btRunner;
        private BtNode _behaviourTree;

        public MonsterState CurrentState => _currentState;
        public float SightRange => _sightRange;
        public float ViewAngle => _viewAngle;
        public float MoveSpeed => _currentState == MonsterState.Chasing ? _chaseSpeed : _moveSpeed;

        private void Awake()
        {
            _btRunner = GetComponent<BehaviourTreeRunner>();
            if (_btRunner == null)
                _btRunner = gameObject.AddComponent<BehaviourTreeRunner>();
        }

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            if (IsServerInitialized)
            {
                _currentState = MonsterState.Idle;
                BuildDefaultBehaviourTree();
            }
        }

        [Server]
        public void SetState(MonsterState newState)
        {
            if (_currentState == newState) return;
            _currentState = newState;
            ObserversOnStateChanged(newState);
        }

        [ObserversRpc]
        private void ObserversOnStateChanged(MonsterState state)
        {
            // Client-side: animation, sound, VFX
        }

        [ServerRpc(RequireOwnership = false)]
        public void ReportSoundHeard(Vector3 position, float intensity)
        {
            if (_currentState == MonsterState.Dead) return;
            SetState(MonsterState.Investigating);
        }

        /// <summary>
        /// Xây dựng behaviour tree mặc định cho monster.
        /// Có thể override để tạo AI khác nhau cho từng loại monster.
        /// </summary>
        protected virtual void BuildDefaultBehaviourTree()
        {
            _behaviourTree = new SelectorNode("Root",
                new System.Collections.Generic.List<BtNode>
                {
                    new SequenceNode("Attack",
                        new System.Collections.Generic.List<BtNode>
                        {
                            new ConditionNode("TargetInSight?", () => CanSeeTarget()),
                            new ActionNode("ChaseTarget", ChaseTarget),
                            new ActionNode("AttackTarget", AttackTarget)
                        }),
                    new SequenceNode("Investigate",
                        new System.Collections.Generic.List<BtNode>
                        {
                            new ConditionNode("IsInvestigating?", () => _currentState == MonsterState.Investigating),
                            new ActionNode("MoveToSound", MoveToLastSound)
                        }),
                    new ActionNode("Patrol", Patrol)
                });

            _btRunner.BuildTree(_behaviourTree);
        }

        private bool CanSeeTarget()
        {
            return false; // Implement: tìm player trong sight range
        }

        private BtNodeState ChaseTarget()
        {
            return BtNodeState.Failure; // Implement
        }

        private BtNodeState AttackTarget()
        {
            return BtNodeState.Failure; // Implement
        }

        private BtNodeState MoveToLastSound()
        {
            SetState(MonsterState.Idle);
            return BtNodeState.Success; // Implement
        }

        private BtNodeState Patrol()
        {
            return BtNodeState.Running; // Implement
        }
    }
}

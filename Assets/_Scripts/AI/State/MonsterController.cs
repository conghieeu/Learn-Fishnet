using FishNet.Object;
using FishNet.Object.Synchronizing;
using NodeCanvas.BehaviourTrees;
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
        [SerializeField] private BehaviourTreeOwner _behaviourTree;
        [SyncVar] private MonsterState _currentState;

        public MonsterState CurrentState => _currentState;

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            if (IsServerInitialized)
            {
                _currentState = MonsterState.Idle;
                if (_behaviourTree != null)
                    _behaviourTree.enabled = true;
            }
        }

        [Server]
        public void SetState(MonsterState newState)
        {
            _currentState = newState;
            ObserversOnStateChanged(newState);
        }

        [ObserversRpc]
        private void ObserversOnStateChanged(MonsterState state)
        {
            // Client-side effects: animation, sound, VFX
        }

        [ServerRpc(RequireOwnership = false)]
        public void ReportSoundHeard(Vector3 position, float intensity)
        {
            if (_currentState == MonsterState.Dead) return;
            SetState(MonsterState.Investigating);
        }
    }
}

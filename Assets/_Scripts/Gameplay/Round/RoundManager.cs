using FishNet.Object;
using FishNet.Object.Synchronizing;
using MellowAbelson.Core.Save;
using UnityEngine;

namespace MellowAbelson.Gameplay.Round
{
    public enum RoundState
    {
        Lobby,
        Landing,
        Exploration,
        ExtractionPhase,
        BetweenRounds,
        GameOver
    }

    public class RoundManager : NetworkBehaviour, ISaveable
    {
        [SyncVar] private RoundState _currentState = RoundState.Lobby;
        [SyncVar] private float _roundTimeRemaining;
        [SyncVar] private int _currentDay = 1;
        [SyncVar] private float _currentQuota;
        [SyncVar] private float _totalScrapCollected;

        public RoundState CurrentState => _currentState;
        public float TimeRemaining => _roundTimeRemaining;
        public int CurrentDay => _currentDay;
        public float Quota => _currentQuota;
        public float ScrapCollected => _totalScrapCollected;
        public string SaveKey => "RoundState";

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
        }

        [Server]
        public void StartRound()
        {
            _currentState = RoundState.Landing;
            _currentDay = 1;
            _currentQuota = CalculateQuota(_currentDay);
            _totalScrapCollected = 0;
            ObserversOnRoundStateChanged(RoundState.Lobby, RoundState.Landing);
        }

        [Server]
        private void EndRound()
        {
            if (_totalScrapCollected >= _currentQuota)
            {
                _currentDay++;
                _currentQuota = CalculateQuota(_currentDay);
                _currentState = RoundState.BetweenRounds;
            }
            else
            {
                _currentState = RoundState.GameOver;
            }
        }

        [Server]
        public void AddScrap(float value)
        {
            _totalScrapCollected += value;
        }

        public void SetQuota(float quota)
        {
            if (IsServer)
                _currentQuota = quota;
        }

        private float CalculateQuota(int day)
        {
            return Mathf.Round(100 * Mathf.Pow(1.15f, day - 1));
        }

        [ObserversRpc]
        private void ObserversOnRoundStateChanged(RoundState oldState, RoundState newState) { }

        public object CaptureState()
        {
            return new RoundSaveData
            {
                CurrentDay = _currentDay,
                CurrentQuota = _currentQuota,
                TotalScrapCollected = _totalScrapCollected,
                CurrentState = _currentState
            };
        }

        public void RestoreState(object state)
        {
            if (state is RoundSaveData data)
            {
                _currentDay = data.CurrentDay;
                _currentQuota = data.CurrentQuota;
                _totalScrapCollected = data.TotalScrapCollected;
                _currentState = data.CurrentState;
            }
        }

        [System.Serializable]
        private struct RoundSaveData
        {
            public int CurrentDay;
            public float CurrentQuota;
            public float TotalScrapCollected;
            public RoundState CurrentState;
        }
    }
}

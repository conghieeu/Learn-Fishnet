using FishNet.Object;
using MellowAbelson.Core.Save;
using System.Collections.Generic;
using UnityEngine;

namespace MellowAbelson.Gameplay.Persistence
{
    [System.Serializable]
    public struct WorldObjectSaveData
    {
        public int NetworkId;
        public string PrefabId;
        public Vector3 Position;
        public Quaternion Rotation;
        public bool IsDestroyed;
    }

    public class WorldPersistenceManager : NetworkBehaviour, ISaveable
    {
        private readonly List<ISaveable> _saveableObjects = new();
        public string SaveKey => "WorldState";

        public void RegisterObject(ISaveable obj)
        {
            if (!_saveableObjects.Contains(obj))
                _saveableObjects.Add(obj);
        }

        public void UnregisterObject(ISaveable obj)
        {
            _saveableObjects.Remove(obj);
        }

        [Server]
        public void SaveAll()
        {
            var worldData = new List<WorldObjectSaveData>();
            foreach (var saveable in _saveableObjects)
            {
                if (saveable is Component comp)
                {
                    worldData.Add(new WorldObjectSaveData
                    {
                        NetworkId = comp.gameObject.GetInstanceID(),
                        PrefabId = comp.name,
                        Position = comp.transform.position,
                        Rotation = comp.transform.rotation,
                        IsDestroyed = comp == null
                    });
                }
            }

            var saveManager = Object.FindFirstObjectByType<SaveManager>();
            saveManager?.Save(SaveKey, worldData);
        }

        [Server]
        public void LoadAll()
        {
            var saveManager = Object.FindFirstObjectByType<SaveManager>();
            if (saveManager == null) return;

            var worldData = saveManager.Load<List<WorldObjectSaveData>>(SaveKey);
            if (worldData == null) return;
        }

        public object CaptureState() => null;
        public void RestoreState(object state) { }
    }
}

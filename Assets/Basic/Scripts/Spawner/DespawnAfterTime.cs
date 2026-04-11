using FishNet.Object;
using System.Collections;
using UnityEngine;

namespace Basic
{
    public class DespawnAfterTime : NetworkBehaviour
    {
        public float SecondsBeforeDespawn = 3f;

        public override void OnStartServer()
        {
            StartCoroutine(DespawnAfterSeconds());
        }

        private IEnumerator DespawnAfterSeconds()
        {
            yield return new WaitForSeconds(SecondsBeforeDespawn);

            Despawn(); // NetworkBehaviour shortcut for ServerManager.Despawn(gameObject);
        }
    }

}
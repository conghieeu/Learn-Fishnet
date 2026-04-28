using FishNet.Managing.Scened;
using UnityEngine;

namespace MellowAbelson.Networking.Scene
{
    public class SceneLoadManager : MonoBehaviour
    {
        public void LoadSceneForAll(string sceneName)
        {
            var sdata = new SceneLoadData(sceneName);
            InstanceFinder.SceneManager.LoadGlobalScenes(sdata);
        }

        public void LoadSceneForConnection(string sceneName, int connectionId)
        {
            var sdata = new SceneLoadData(sceneName);
            InstanceFinder.SceneManager.LoadConnectionScenes(sdata);
        }

        public void UnloadScene(string sceneName)
        {
            var sdata = new SceneUnloadData(sceneName);
            InstanceFinder.SceneManager.UnloadGlobalScenes(sdata);
        }
    }
}

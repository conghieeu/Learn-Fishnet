using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
using QFSW.QC;
using FishNet.Object;
using FishNet.Connection;

public class LoadGameplayScene : NetworkBehaviour
{
    // một số client cụ thể load scene
    [Command("loadscene")]
    [ServerRpc(RequireOwnership = false)]
    public void LoadScene(NetworkConnection connection, string sceneName)
    {
        // Lấy reference đến NetworkManager
        NetworkManager networkManager = InstanceFinder.NetworkManager;

        // Gọi LoadScene của FishNet
        // một số client cụ thể load scene
        SceneLoadData sld = new SceneLoadData(sceneName);
        sld.ReplaceScenes = ReplaceOption.All;
        networkManager.SceneManager.LoadConnectionScenes(connection, sld);
    }

    // hàm tất cả client load scene
    [Command("loadallscene")]
    [Server]
    public void LoadAllScene(string sceneName)
    {
        // Lấy reference đến NetworkManager
        NetworkManager networkManager = InstanceFinder.NetworkManager;

        // Gọi LoadScene của FishNet
        // tất cả client load scene
        SceneLoadData sld = new SceneLoadData(sceneName);
        sld.ReplaceScenes = ReplaceOption.All;
        networkManager.SceneManager.LoadGlobalScenes(sld);
    }
}
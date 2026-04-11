using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Transporting;
using QFSW.QC;

public class FishNetTestCode : NetworkBehaviour
{
    // loại trừ owner, giữ tin gửi cho người cho cả người về sau
    [Command]
    [ObserversRpc(ExcludeOwner = true, BufferLast = true)]
    private void RpcSetNumber(int next)
    {
        // chỗ này sẽ không chạy ở owner nhưng sẽ chạy ở các client khác và new client
        Debug.Log($"Received number {next} from the server.");
    }

    [ServerRpc(RequireOwnership = false)]
    private void RpcSendChat(string msg, NetworkConnection conn = null)
    {
        Debug.Log($"Received {msg} on the server from connection {conn.ClientId}.");
    }

    [ObserversRpc]
    [TargetRpc]
    private void DisplayChat(NetworkConnection target, string sender, string message)
    {
        Debug.Log($"{sender}: {message}."); // Display a message from sender.
    }

    [Server]
    private void SendChatMessage()
    {
        // Send to only the owner.
        DisplayChat(base.Owner, "Bob", "Hello world");
        // Send to everyone.
        DisplayChat(null, "Bob", "Hello world");
    }

    private bool _reliable;

    /* This example uses ServerRpc, but any RPC will work.
    * Although this example shows a default value for the channel,
    * you do not need to include it. */
    [ServerRpc]
    private void RpcTest(string txt, Channel channel = Channel.Reliable)
    {
        if (channel == Channel.Reliable)
            Debug.Log("Message received! I never doubted you.");
        else if (channel == Channel.Unreliable)
            Debug.Log($"Glad you got here, I wasn't sure you'd make it.");
    }

    /* Example of using Channel with a ServerRpc that
* also provides the calling connection. */
    [ServerRpc(RequireOwnership = false)]
    private void RpcTest(string txt, Channel channel = Channel.Reliable, NetworkConnection sender = null)
    {
        Debug.Log($"Received on channel {channel} from {sender.ClientId}.");
    }

    // Keeping in mind all RPC types can use RunLocally.
    [ServerRpc(RunLocally = true)]
    private void RpcTest()
    {
        // This debug will print on the server and the client calling the RPC.
        Debug.Log("Rpc Test!");
    }

    // This implies we know the data will never be larger than 3500 bytes.
    // If the data is larger then a resize will occur resulting in garbage collection.
    [ServerRpc(DataLength = 3500)]
    private void ServerSendBytes(byte[] data) { }
}

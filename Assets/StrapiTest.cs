using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Whoo.Data;

public class StrapiTest : MonoBehaviour
{
    private const string testRoomId = "5f9f2aa027150201c869f0c4";
    private const string testZoneId = "5f9eeef7aa6668005297347c";

    private void Start()
    {
        _StartAsync().Forget();
    }

    private async UniTaskVoid _StartAsync()
    {
        var room = new StrapiRoom();
        await room.LoadRoom(testRoomId);
        Debug.Break();
    }
}
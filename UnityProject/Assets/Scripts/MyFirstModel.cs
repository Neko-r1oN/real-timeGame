using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.Interfaces.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MyFirstModel : MonoBehaviour
{
    const string ServerURL = "http://localhost:7000";
    async void Start()
    {
        int result = await Sum(100, 234);
      
         Debug.Log(result);

        int result2 = await Sub(100, 234);

        Debug.Log(result2);

    }

    public async UniTask<int> Sum(int x,int y)
    {
        var handler = new YetAnotherHttpHandler() { Http2Only = true};
        var channel = GrpcChannel.ForAddress( ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IMyFirstService>(channel);
        var result = await client.SumAsync(x,y);

        return result;
    }

    public async UniTask<int> Sub(int x, int y)
    {
        var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IMyFirstService>(channel);
        var result = await client.SubAsync(x, y);

        return result;
    }

}

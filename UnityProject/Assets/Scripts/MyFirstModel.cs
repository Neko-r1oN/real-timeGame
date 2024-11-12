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

        result = await Sub(100, 234);

        Debug.Log(result);


       //result = await SumAll(1,0);

        //Debug.Log(result);

        //result = await CalcOperation(1, 7);

        //Debug.Log(result);

        //result = await SumAllNumber(1.1f,12.2f);

        //Debug.Log(result);

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

    public async UniTask<int> SumAll(int[] numList)
    {
        var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IMyFirstService>(channel);
        var result = await client.SumAllAsync(numList);

        return result;
    }

    public async UniTask<int> CalcOperation(int x, int y)
    {
        var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IMyFirstService>(channel);
        int[] result = await client.ColcForOperotionAsync(x, y);

        return result[1];
    }

    public async UniTask<float> SumAllNumber(Number NumArray)
    {
        Number nums;
        nums = new Number {x=NumArray.x,y=NumArray.y};
        

        var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IMyFirstService>(channel);
        var result = await client.SumAllNumberAsync(nums);

        return result;
    }

}

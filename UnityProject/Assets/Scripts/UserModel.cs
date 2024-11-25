//==============================================================
//
//               ユーザーAPI(クライアントサイド)
//
//
//==============================================================


using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.Interfaces.Services;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UserModel : BaseModel
{
    private int userId;       //登録ユーザーID
    private string userName;  //登録ユーザー名


    //ユーザー移動通知
    public Action<MoveData> MovedUser { get; set; }

    /// <summary>
    /// ユーザー登録処理
    /// </summary>
    /// <param name="name">ユーザー名</param>
    /// <returns>登録成功 or 失敗</returns>
    public async UniTask<bool> RegistUserAsync(string name)
    {
        var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IUserService>(channel);

        try
        {//登録成功
            Debug.Log("サーバー接続成功");
            userId = await client.RegistUserAsync(name);
            return true;
        }
        catch (Exception e)
        {//登録失敗
            Debug.Log("サーバー接続エラー");
            Debug.Log(e);
            return false;
        }
    }
}

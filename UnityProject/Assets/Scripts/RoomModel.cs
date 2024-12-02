using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RoomModel : BaseModel, IRoomHubReceiver
{
    private GrpcChannel channel;
    private IRoomHub roomHub;

    //接続ID
    public Guid ConnectionId { get; set; }

    //ユーザー接続通知
    public Action<JoinedUser> OnJoinedUser {  get; set; }

    //ユーザー切断通知
    public Action<Guid> LeavedUser { get; set; }

    //ユーザー移動通知
    public Action<MoveData> MovedUser { get; set; }

    //ユーザー準備完了通知
    public Action<JoinedUser> ReadiedUser { get; set; }




    //MoajicOnion接続処理
    public async UniTask ConnectAsync()
    {
        var handler = new YetAnotherHttpHandler() {Http2Only = true};
        channel = GrpcChannel.ForAddress(ServerURL,new GrpcChannelOptions() { HttpHandler = handler });
        roomHub = await StreamingHubClient.ConnectAsync<IRoomHub, IRoomHubReceiver>(channel,this);
    }

    //MagicOnion切断処理
    public async UniTask DisConnectAsync()
    {
        if(roomHub != null)
        {
            await roomHub.DisposeAsync();
        }
        if(channel != null)
        {
            await channel.ShutdownAsync();
        }

        roomHub = null;channel = null;
    }

    //破棄処理
    async void OnDestroy()
    {
        await DisConnectAsync();
    }

    //入室処理
    public async UniTask JoinAsync(string roomName, int userId)
    {
        //配列に引数で受け取った情報を追加
        JoinedUser[] users = await roomHub.JoinAsync(roomName,userId);

        //配列の要素分ループ
        foreach (var user in users)
        {
            if (user.UserData.Id == userId)
            {
                this.ConnectionId = user.ConnectionId;
            }
                OnJoinedUser(user);
            
        }
    }

    //入室通知
    public void OnJoin(JoinedUser user)
    {
        OnJoinedUser(user);
    }


    //退出処理
    public async UniTask LeaveAsync()
    {
       　await roomHub.LeaveAsync();

    }

    //退出通知
    public void Leave(Guid LeaveId)
    {
        LeavedUser(LeaveId);
    }

    //プレイヤー移動処理
    public async Task MoveAsync(MoveData moveData)
    {
        await roomHub.MoveAsync(moveData);
    }

    //移動通知
    public void OnMove(MoveData moveData)
    {
        MovedUser(moveData);
    }

    //ゲーム開始処理
    public async Task ReadyAsync()
    {
        await roomHub.ReadyAsync();
    }
    //準備完了通知
    public void Ready(JoinedUser user)
    {
        ReadiedUser(user);
    }
}

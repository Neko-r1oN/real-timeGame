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
using UnityEngine.Playables;

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

    //ユーザーマッチング通知
    public Action<string> MatchedUser { get; set; }

    //ユーザー移動通知
    public Action<MoveData> MovedUser { get; set; }

    //ユーザー移動通知
    public Action<MoveData> MovedBall { get; set; }

    //ボール発射通知
    public Action<ThrowData> ThrowedBall { get; set; }

    //ボール発射通知
    public Action getBall { get; set; }

    //ユーザー準備状態確認通知
    public Action<bool> ReadyUser { get; set; }

    //ゲームカウント開始通知
    public Action StartGameCount { get; set; }

    //カウントダウン通知
    public Action<int> GameCountUser { get; set; }

    //ゲームカウント終了通知
    public Action FinishGameCount { get; set; }

    //参加者全員のゲーム内カウントダウン終了通知
    public Action FinishGameCountAllUser { get; set; }

    //ユーザー状態更新通知
    public Action<Guid,UserState> UpdateStateUser { get; set; }

    //ゲーム終了通知
    public Action<Guid, string, bool> FinishGameUser { get; set; }

    //ゲーム開始通知
    public Action StartGameUser { get; set; }

    //ユーザー状態
    public enum USER_STATE
    {
        NONE = 0,             //停止中
        CONNECT = 1,          //接続中
        JOIN = 2,             //入室中
        LEAVE = 3,            //退出中
    }

    USER_STATE userState = USER_STATE.NONE;

    //MoajicOnion接続処理
    public async UniTask ConnectAsync()
    {
        var handler = new YetAnotherHttpHandler() {Http2Only = true};
        channel = GrpcChannel.ForAddress(ServerURL,new GrpcChannelOptions() { HttpHandler = handler });
        roomHub = await StreamingHubClient.ConnectAsync<IRoomHub, IRoomHubReceiver>(channel,this);
    
        //ユーザー状態を接続中に変更
        userState = USER_STATE.CONNECT;
    
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

        //ユーザー状態を停止中に変更
        userState = USER_STATE.NONE;
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
        //ユーザー状態を入室中に変更
        userState = USER_STATE.JOIN;
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

    //マッチング処理
    public async UniTask MatchingAsync(string roomName)
    {
        //受け取った
        await roomHub.MatchAsync(roomName);
    }
    //マッチング通知
    public void OnMatch(string roomName)
    {
        MatchedUser(roomName);
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

    //ボール座標同期処理
    public async Task MoveBallAsync(MoveData moveBallData)
    {
        await roomHub.MoveBallAsync(moveBallData);
    }
    //ボール座標同期通知
    public void OnMoveBall(MoveData moveBallData)
    {
        MovedBall(moveBallData);
    }

    //ボール発射処理
    public async Task ThrowBallAsync(ThrowData ThrowData)
    {
        await roomHub.ThrowBallAsync(ThrowData);
    }

    //ボール発射通知
    public void OnThrowBall(ThrowData ThrowData)
    {
        ThrowedBall(ThrowData);
    }

    //ボール取得処理
    public async Task GetBallAsync()
    {
        await roomHub.GetBallAsync();
    }

    //ボール発射通知
    public void OnGetBall()
    {
        getBall();
    }

    //準備完了処理
    public async Task ReadyAsync(bool isReady)
    {
        await roomHub.ReadyAsync(isReady);
    }

    public void Ready(bool isStart)
    {
        ReadyUser(isStart);
    }
    //準備完了通知
   
    //ユーザー情報更新処理
    public async UniTask UpdateUserStateAsync(UserState state)
    {
        await roomHub.UpdateUserStateAsync(state);
    }
    //ユーザー情報更新通知
    public void UpdateUserState(Guid connectionId,UserState state)
    {
        UpdateStateUser(connectionId, state);
    }


    //自身のゲーム内カウントダウン終了処理
    public async UniTask FinishGameCountAsync()
    {
        await roomHub.GameCountFinishAsync();
    }
    //全員のゲーム内カウントダウン終了通知
    public void GameCountFinish()
    {
        FinishGameCountAllUser();
    }

    //カウントダウン同期処理
    public async UniTask GameCountAsync(int currentTime)
    {
        await roomHub.GameCountAsync(currentTime);
    }
    //カウントダウン同期通知
    public void GameCount(int currentTime)
    {
        GameCountUser(currentTime);
    }

    // ゲーム開始通知
    public void StartGame()
    {
        StartGameUser();
    }
    //ゲーム終了処理
    public async UniTask FinishGameAsync()
    {
        await roomHub.GameFinishAsync();
        Debug.Log("ゲーム終了");
    }
    //ゲーム終了通知
    public void FinishGame(Guid connectionId,string userName,bool isFinishAllUser)
    {
        FinishGameUser(connectionId, userName, isFinishAllUser);
    }
}


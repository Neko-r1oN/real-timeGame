////////////////////////////////////////////////////////////////////////////
///
///  ルームモデルスクリプト
///  Author : 川口京佑  2025.01/28
///
////////////////////////////////////////////////////////////////////////////

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
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using static UnityEngine.UIElements.UxmlAttributeDescription;

/// <summary>
/// ルーム処理クラス
/// </summary>
public class RoomModel : BaseModel, IRoomHubReceiver
{
    private GrpcChannel channel;
    private IRoomHub roomHub;

    private int userId;

    public bool isMaster;

    //接続ID
    public Guid ConnectionId { get; set; }
    //ユーザー接続通知
    public Action<JoinedUser> OnJoinedUser {  get; set; }
    //ユーザー切断通知
    public Action<Guid> LeavedUser { get; set; }
    //ユーザーマッチング通知
    public Action<string> MatchedUser { get; set; }
    //マスターチェック
    public Action MasterCheckedUser { get; set; }


    //ユーザー移動通知
    public Action<MoveData> MovedUser { get; set; }
    //ボール移動通知
    public Action<MoveData> MovedBall { get; set; }
    //ボール発射通知
    public Action<ThrowData> ThrowedBall { get; set; }
    //ボール獲得通知
    public Action<Guid> GetBall { get; set; }

    //ボールヒット通知
    public Action<HitData> HitBall { get; set; }
    //カーソル移動通知
    public Action<Vector3> MoveCursor { get; set; }
    //ユーザーダウン通知
    public Action<Guid> DownUser { get; set; }
    //ユーザーダウン復帰通知
    public Action<Guid> DownBackUser { get; set; }

    //準備遷移通知
    public Action StandUser { get; set; }
    //ユーザー準備状態確認通知
    public Action<Guid,bool> ReadyUser { get; set; }
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
    //ユーザー死亡通知
    public Action<DeadData, int> UserDead { get; set; }
    //ゲーム終了通知
    public Action FinishGameUser { get; set; }
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

        //マスタークライアント判定
        isMaster = false;
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

                Debug.Log(this.ConnectionId);
            }
            OnMasterCheck(user);

            //
            OnJoinedUser(user);
        }

        //ユーザー状態を入室中に変更
        userState = USER_STATE.JOIN;

        Debug.Log("入室中");
    }

    //入室通知
    public void OnJoin(JoinedUser user)
    {
        //nullじゃなかったら実行(null条件演算子)
        OnJoinedUser?.Invoke(user);
    }

   
    //ロビー入室
    public async UniTask JoinLobbyAsync(int userId)
    {
        //配列に引数で受け取った情報を追加
        await roomHub.JoinLobbyAsync(userId);

        Debug.Log("ロビー入室");
    }
    //入室通知
    public void OnJoinLobby(JoinedUser user)
    {
        OnJoinedUser(user);
    }

    //マッチング通知
    public void OnMatch(string roomName)
    {
        MatchedUser(roomName);
        Debug.Log("マッチング成立:"+  roomName);
    }


    //退出処理
    public async UniTask LeaveAsync()
    {
        //マスター判定削除
        isMaster = false;
        await roomHub.LeaveAsync();
    }

    //退出通知
    public void Leave(Guid LeaveId)
    {
        LeavedUser(LeaveId);
    }


    //マスタークライアント判定処理
    public void OnMasterCheck(JoinedUser user)
    {
        //マスタークライアント判定
        if (user.ConnectionId == this.ConnectionId && user.JoinOrder == 1)
        {
            Debug.Log(user.UserData.Name + "マスター");
            isMaster = true;
        }
        else Debug.Log(user.UserData.Name + "マスターじゃない");
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
    public async Task GetBallAsync(Guid getUserId)
    {
        //自身の接続IDを通知
        await roomHub.GetBallAsync(getUserId); 
    }

    //ボール発射通知
    public void OnGetBall(Guid getUserId)
    {
        GetBall(getUserId);
    }

    //ヒット処理
    public async Task HitBallAsync(HitData hitData)
    {
        await roomHub.HitBallAsync(hitData);
    }
    //ヒット通知
    public void OnHitBall(HitData hitData)
    {
        HitBall(hitData);
    }

    //標準カーソル座標同期処理
    public async Task MoveCursorAsync(Vector3 cursorPos)
    {
        await roomHub.MoveCursorAsync(cursorPos);
    }
    //標準カーソル座標同期通知
    public void OnMoveCursor(Vector3 cursorPos)
    {
        MoveCursor(cursorPos);
    }

    //ダウン処理
    public async Task DownUserAsync(Guid downUserId)
    {
        await roomHub.DownUserAsync(downUserId);
    }
    //ダウン通知
    public void OnDownUser(Guid downUserId)
    {
        DownUser(downUserId);
    }
    //ダウン復帰処理
    public async Task DownBackUserAsync(Guid downUserId)
    {
        await roomHub.DownBackUserAsync(downUserId);
    }
    //ダウン復帰通知
    public void OnDownBackUser(Guid downUserId)
    {
        DownBackUser(downUserId);
    }

    //準備状態遷移
    public void OnStand()
    {
        StandUser();
    }

    //準備完了処理
    public async Task ReadyAsync(Guid id ,bool isReady)
    {
        await roomHub.ReadyAsync(id,isReady);
    }
    //通知
    public void Ready(Guid id, bool isStart)
    {
        ReadyUser(id, isStart);
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

    //ユーザー死亡処理
    public async UniTask DeadUserAsync(DeadData deadData,int deadNum)
    {
        await roomHub.DeadUserAsync(deadData,deadNum);
    }
    //ユーザー死亡通知
    public void DeadUser(DeadData deadData, int deadNum)
    {
        UserDead(deadData, deadNum);
    }

    //ゲーム終了通知
    public void FinishGame()
    {
        Debug.Log("終了通知受け取り");
        FinishGameUser();
    }
}


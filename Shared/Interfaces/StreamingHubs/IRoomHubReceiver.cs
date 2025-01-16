using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using MagicOnion;
using Shared.Model.Entity;

namespace Shared.Interfaces.StreamingHubs
{
    public interface IRoomHubReceiver
    {
        //ここからサーバー側からクライアント側を呼び出す
        //関数を定義する

        //サーバーの入室通知
        void OnJoin(JoinedUser user);

        //ロビー入室通知
        void OnJoinLobby(JoinedUser user);

        //マスターチェック通知
        void OnMasterCheck(JoinedUser user);

        //マッチング成立通知
        void OnMatch(string roomName);

        //サーバーの退出通知
        void Leave(Guid connectionId);

        //ユーザーの移動通知
        void OnMove(MoveData moveData);

        //ボールの移動通知
        void OnMoveBall(MoveData moveData);

        //ユーザーの移動通知
        void OnThrowBall(ThrowData throwData);

        //ボール取得通知
        void OnGetBall(Guid getUserId);
        
        //ヒット通知
        void OnHitBall(HitData hitData);


        //ユーザー状態更新通知
        void UpdateUserState(Guid connectionId, UserState state);

        //全ユーザー準備完了通知
        void Ready(bool isAllUserReady);

        //ゲーム内カウント開始通知
        //void StartGame();

        //ゲーム内カウント通知
        void GameCount(int currentTime);

        //ゲーム内カウント通知
        void StartGame();

        //ユーザー死亡通知
        void DeadUser(DeadData deadData, int deadNum);

        //ゲーム終了通知
        void FinishGame();


    }
}

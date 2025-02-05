////////////////////////////////////////////////////////////////////////////
///
///  通知関数設定スクリプト
///  Author : 川口京佑  2025.01/28
///
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using UnityEngine;
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

        //指定人数集合通知
        void OnStand();

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

        //標準カーソルの移動通知
        void OnMoveCursor(Vector3 cursorPos);


        //ダウン通知
        void OnDownUser(Guid downUserId);
        //ダウン復帰通知
        void OnDownBackUser(Guid downBackUserId);

        //ユーザー状態更新通知
        void UpdateUserState(Guid connectionId, UserState state);

        //全ユーザー準備完了通知
        void Ready(Guid id ,bool isAllUserReady);

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

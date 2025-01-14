﻿using MagicOnion;
using Shared.Model.Entity;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces.StreamingHubs
{
    public interface IRoomHub : IStreamingHub<IRoomHub, IRoomHubReceiver>
    {
        //ここにクライアント側からサーバー側を呼び出す関数定義を作成

        /// <summary>
        /// ユーザー入室関数
        /// </summary>
        /// <param name="roomName">ルーム名</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns>入室処理</returns>
        Task<JoinedUser[]> JoinAsync(string roomName, int userId);

        Task MastaerCheckAsync(JoinedUser user);

        /// <summary>
        /// ロビー入室処理
        /// </summary>
        /// <param name="userId">入室ユーザーID</param>
        /// <returns></returns>
        Task/*<JoinedUser[]>*/ JoinLobbyAsync(int userId);

        
        /// <summary>
        /// ユーザー退出関数
        /// </summary>
        /// <returns></returns>
        Task LeaveAsync();

        /// <summary>
        /// マッチング成立関数
        /// </summary>
        /// <param name="roomName">ルーム名(4番目に入室したユーザーのID)</param>
        
        /// <returns></returns>
        Task MatchAsync(string roomName);

        /// <summary>
        /// ユーザー移動関数
        /// </summary>
        /// <returns></returns>
        Task MoveAsync(MoveData moveData);


        /// <summary>
        /// ボール移動関数
        /// </summary>
        /// <returns></returns>
        Task MoveBallAsync(MoveData moveData);

        /// <summary>
        /// ホール発射関数
        /// </summary>
        /// <returns></returns>
        Task ThrowBallAsync(ThrowData throwData);

        /// <summary>
        /// ホール取得関数
        /// </summary>
        /// <returns></returns>
        Task GetBallAsync(Guid getUserId);
        
        /// <summary>
        /// ヒット関数
        /// </summary>
        /// <returns></returns>
        Task HitBallAsync(HitData hitData);

        /// <summary>
        /// 全ユーザー準備状態確認関数
        /// </summary>
        /// <param name="isAllUserReady"></param>
        /// <returns></returns>
        Task ReadyAsync(bool isAllUserReady);

        /// <summary>
        /// ユーザー状態更新関数
        /// </summary>
        /// <param name="state">ユーザー状態</param>
        /// <returns></returns>
        Task UpdateUserStateAsync(UserState state);

        /// <summary>
        /// ゲームカウント関数
        /// </summary>
        /// <param name="currentTime">呼び出した時点での時間</param>
        /// <returns></returns>
        Task GameCountAsync(int currentTime);

        /// <summary>
        /// ゲームにカウント終了関数
        /// </summary>
        /// <returns></returns>
        Task GameCountFinishAsync();

 
        /// <summary>
        /// ユーザーゲームオーバー関数
        /// </summary>
        /// <returns></returns>
        Task DeadUserAsync();


        
    }
}

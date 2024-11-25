using MagicOnion;
using Shared.Model.Entity;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces.StreamingHubs
{
    public interface IRoomHub : IStreamingHub<IRoomHub,IRoomHubReceiver>
    {
        //ここにクライアント側からサーバー側を呼び出す関数定義を作成

        /// <summary>
        /// ユーザー入室関数
        /// </summary>
        /// <param name="roomName">ルーム名</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns>入室処理</returns>
        Task<JoinedUser[]> JoinAsync(string roomName,int userId);

        /// <summary>
        /// ユーザー退出関数
        /// </summary>
        /// <returns></returns>
        Task LeaveAsync();

        /// <summary>
        /// ユーザー移動関数
        /// </summary>
        /// <returns></returns>
        Task MoveAsync(MoveData moveData);
    }
}

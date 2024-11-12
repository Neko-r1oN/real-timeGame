using MagicOnion;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces.StreamingHubs
{
    public interface IRoomHub : IStreamingHub<IRoomHub,IRoomHubReceiver>
    {
        //ここにクライアント側からサーバー側を呼び出す関数を定義する
        //ここにどのようなAPIを作るか、関数形式で定義を作成

        /// <summary>
        /// ユーザー入室関数
        /// </summary>
        /// <param name="roomName">ルーム名</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns>入室済みユーザー一覧</returns>
        Task<JoinedUser> JoinAsync(string roomName,int userId);
    }
}

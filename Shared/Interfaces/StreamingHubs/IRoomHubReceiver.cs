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

        //サーバーの退出通知
        void Leave(Guid connectionId);

        //ユーザーの移動通知
        void OnMove(MoveData moveData);


    }
}

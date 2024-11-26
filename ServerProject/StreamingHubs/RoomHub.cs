using MagicOnion.Server.Hubs;
using MagicOnionServer.Model.Context;
using ServerProject.StreamingHubs;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System.Diagnostics;
using System.Numerics;

namespace StreamingHubs
{
    public class RoomHub : StreamingHubBase<IRoomHub,IRoomHubReceiver>,IRoomHub
    {
        private IGroup room;

        //private MoveData moveData;

        /// <summary>
        /// ユーザー入室処理
        /// </summary>
        /// <param name="roomName">参加ルーム名</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns>参加者リスト</returns>
        public async Task<JoinedUser[]>JoinAsync(string roomName,int userId)
        {
            //ルームに参加＆ルームを保持
            this.room = await this.Group.AddAsync(roomName);

            //DBからユーザー情報を取得
            GameDbContext context = new GameDbContext();
            var user = context.Users.Where(user => user.Id == userId).First();

            //グループストレージにユーザー情報を格納
            var roomStorage = this.room.GetInMemoryStorage<RoomData>();
            var joinedUser = new JoinedUser() { ConnectionId = this.ConnectionId,  UserData = user, JoinOrder = 100};
            var roomData = new RoomData() {JoinedUser = joinedUser};
            roomData = new RoomData() { IsStartGame = false };
            roomStorage.Set(this.ConnectionId, roomData);

            //ルーム参加者全員にユーザーの入室通知を送信
            this.BroadcastExceptSelf(room).OnJoin(joinedUser);

            RoomData[] roomDataList = roomStorage.AllValues.ToArray<RoomData>();

            //参加中のユーザー情報を流す
            JoinedUser[] joinedUserList = new JoinedUser[roomDataList.Length];

            Debug.WriteLine(this.ConnectionId);
            //RoomDataList内のJoinedUserを格納
            for (int i = 0; i < joinedUserList.Length; i++)
            {
                joinedUserList[i] = roomDataList[i].JoinedUser;
            } 

            return joinedUserList;
        }

        /// <summary>
        /// 強制退出(切断)処理
        /// </summary>
        /// <returns></returns>
        protected override ValueTask OnDisconnected()
        {
            //ルームデータ削除
            this.room.GetInMemoryStorage<RoomData>().Remove(this.ConnectionId);
            //退出したことを全メンバーに通知
            this.Broadcast(room).Leave(this.ConnectionId);
            //ルーム内のメンバーから削除
            room.RemoveAsync(this.Context);

            return CompletedTask;
        }

        /// <summary>
        /// 退出処理
        /// </summary>
        /// <returns></returns>
        public async Task LeaveAsync()
        {
            //グループデータから削除
            this.room.GetInMemoryStorage<RoomData>().Remove(this.ConnectionId);

            //ルーム内のメンバーから自分を削除
            await room.RemoveAsync(this.Context);

            //ルーム参加者全員にユーザーの退出通知を送信
            this.BroadcastExceptSelf(room).Leave(this.ConnectionId);

        }

        /// <summary>
        /// 移動処理
        /// </summary>
        /// <returns></returns>
        public async Task MoveAsync(MoveData moveData)
        {

            //ルーム参加者全員にユーザーの移動通知を送信
            this.BroadcastExceptSelf(room).OnMove(moveData);

        }


        /// <summary>
        /// ゲーム開始処理
        /// </summary>
        /// <param name="isStart"></param>
        /// <returns></returns>
        public async Task StartGameAsync(bool isStart)
        {
            //準備できたことを自分のRoomDataに保存
            var roomDataStrage = this.room.GetInMemoryStorage<RoomData>();
            var roomData = roomDataStrage.Get(this.ConnectionId);
            roomData = new RoomData() { IsStartGame = true };

            //全員準備できたか判定
            bool isReady = false;

            var roomDataList = roomDataStrage.AllValues.ToArray<RoomData>();

            foreach (var roomDatas in roomDataList)
            {
                //準備が完了していないプレイヤーがいた場合
                if (!roomDatas.IsStartGame) {
                    //準備未完了としてbreak
                    isReady = false;
                    break;
                };

                //true
                isReady = true;
                

            }

            //ルーム参加者全員に通知を送信
            this.BroadcastExceptSelf(room).IsStartGame(isReady);

        }
    }
}
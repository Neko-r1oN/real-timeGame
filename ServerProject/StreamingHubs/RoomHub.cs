using MagicOnion.Server.Hubs;
using MagicOnionServer.Model.Context;
using ServerProject.StreamingHubs;
using Shared.Interfaces.StreamingHubs;

namespace StreamingHubs
{
    public class RoomHub : StreamingHubBase<IRoomHub,IRoomHubReceiver>,IRoomHub
    {
        private IGroup room;

        /// <summary>
        /// ユーザー入室処理
        /// </summary>
        /// <param name="roomName">参加ルーム名</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns></returns>
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
            roomStorage.Set(this.ConnectionId, roomData);

            //ルーム参加者全員にユーザーの入室通知を送信
            this.BroadcastExceptSelf(room).OnJoin(joinedUser);

            RoomData[] roomDataList = roomStorage.AllValues.ToArray<RoomData>();

            //参加中のユーザー情報を流す
            JoinedUser[] joinedUserList = new JoinedUser[roomDataList.Length];

            //RoomDataList内のJoinedUserを格納
            for (int i = 0; i < joinedUserList.Length; i++)
            {
                joinedUserList[i] = roomDataList[i].JoinedUser;
            } 

            return joinedUserList;
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
    }
}
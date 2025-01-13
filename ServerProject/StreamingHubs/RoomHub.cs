﻿using MagicOnion.Server.Hubs;
using MagicOnionServer.Model.Context;
using Newtonsoft.Json.Linq;
using ServerProject.StreamingHubs;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System.Diagnostics;


namespace StreamingHubs
{
    public class RoomHub : StreamingHubBase<IRoomHub,IRoomHubReceiver>,IRoomHub
    {
        private IGroup room;

        //プレイヤー番号リスト
        private int[] numberList = {1,2,3,4};
        //ルーム最大収容人数
        private int MAX_PLAYER = 4;

     
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

            //自身の入室番号を取得
            int joinOrderNum = GetJoinOrder(roomStorage.AllValues.ToArray<RoomData>());

            //参加ユーザーの情報を挿入
            var joinedUser = new JoinedUser() { ConnectionId = this.ConnectionId,
                UserData = user,IsSelf = true, JoinOrder = joinOrderNum};


            //ルームデータにユーザーとゲーム状態を挿入
            var roomData = new RoomData() {JoinedUser = joinedUser , UserState = new UserState(), MoveData = new MoveData()};



            roomStorage.Set(this.ConnectionId, roomData);

            //プライベートマッチで入室した際
            if (roomName != "Lobby")
            {
                //ルーム参加者全員にユーザーの入室通知を送信
                
                //通常通り通知
                this.BroadcastExceptSelf(room).OnJoin(joinedUser);
            }

            Console.WriteLine("参加者名:" + joinedUser.UserData.Name);
            //Console.WriteLine("スコア:" + joinedUser.UserState.Score);

            //await Task.Delay(1000);
            //同時実行されないように１回だけ実行するよう lock で設定(排他的処理)
            lock (roomStorage)
            {
                RoomData[] roomDataList = roomStorage.AllValues.ToArray<RoomData>();

                //参加中のユーザー情報を流す
                JoinedUser[] joinedUserList = new JoinedUser[roomDataList.Length];

                Debug.WriteLine("接続ID"+this.ConnectionId);

                //RoomDataList内のJoinedUserを格納
                for (int i = 0; i < joinedUserList.Length; i++)
                {
                    joinedUserList[i] = roomDataList[i].JoinedUser;
                }

                //参加者が上限に
                if (roomDataList.Length == MAX_PLAYER && roomName != "Lobby")
                {


                   
                    Console.WriteLine("プラべ:"+roomName);
                        Console.WriteLine("ゲーム開始");
                        //ゲーム開始通知
                        this.Broadcast(room).StartGame();


                   
                }

                return joinedUserList;
            }
        }

        public async Task MastaerCheckAsync(JoinedUser user)
        {
            this.Broadcast(room).OnMasterCheck(user);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">参加ユーザーID</param>
        /// <returns></returns>
        public async Task/*<JoinedUser[]>*/ JoinLobbyAsync(int userId)
        {
            JoinedUser[] joinedUserList = await JoinAsync("Lobby", userId);

            Console.WriteLine("参加");

            Console.WriteLine(joinedUserList.Length);

            //参加人数が４人になったら
            if (joinedUserList.Length == MAX_PLAYER)
            {
               
                //書式を桁無し指定にして再入室
                this.Broadcast(room).OnMatch(Guid.NewGuid().ToString("N"));

              

            }
            //return joinedUserList;
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
        /// 入室番号取得処理
        /// </summary>
        /// <param name="roomData">ルーム情報</param>
        /// <returns>対象者の入室番号</returns>
        int GetJoinOrder(RoomData[] roomData)
        {
            int joinOrder = 1;     //参加順変数

            int loop = 0;          //ループ用変数
            while (loop < roomData.Length)
            {
                loop = 0;
                for (int i = roomData.Length - 1; i >= 0; i--, loop++)
                {
                    if (roomData[i].JoinedUser.JoinOrder == joinOrder)
                    {
                        joinOrder++;
                        break;
                    }
                }
            }
            return joinOrder;
        }

        /// <summary>
        /// マッチング処理
        /// </summary>
        /// <param name="roomName">4番目に入室したユーザーのID</param>
        /// <returns></returns>
        public async Task MatchAsync(string roomName)
        {
            //ルーム参加者全員にマッチング通知を送信
           this.BroadcastExceptSelf(room).OnMatch(roomName);

        }


        /// <summary>
        /// 移動処理
        /// </summary>
        /// <returns></returns>
        public async Task MoveAsync(MoveData moveData)
        {

            //移動情報を自分のRoomDataに保存
            var roomStrage = this.room.GetInMemoryStorage<RoomData>();
            var roomData = roomStrage.Get(this.ConnectionId);
            roomData = new RoomData() { MoveData = moveData };

            //ルーム参加者全員にユーザーの移動通知を送信
            this.BroadcastExceptSelf(room).OnMove(moveData);

        }

        /// <summary>
        /// ボール移動処理(接続IDが1番の人のみ実行)
        /// </summary>
        /// <returns></returns>
        public async Task MoveBallAsync(MoveData moveData)
        {

            //ボール移動情報を自分のRoomDataに保存
            var roomStrage = this.room.GetInMemoryStorage<RoomData>();
            var roomData = roomStrage.Get(this.ConnectionId);
            roomData = new RoomData() { BallMoveData = moveData };

            //自分以外のルーム参加者全員にユーザーの移動通知を送信
            this.BroadcastExceptSelf(room).OnMoveBall(moveData);

        }

        /// <summary>
        /// ボール発射処理
        /// </summary>
        /// <returns></returns>
        public async Task ThrowBallAsync(ThrowData throwData)
        {

            //ボール座標情報を自分のRoomDataに保存
            var roomStrage = this.room.GetInMemoryStorage<RoomData>();
            var roomData = roomStrage.Get(this.ConnectionId);
            roomData = new RoomData() { ThrowData = throwData };

            //自分以外のルーム参加者全員にボールの座標通知を送信
            this.BroadcastExceptSelf(room).OnThrowBall(throwData);

        }

        /// <summary>
        /// ボール発射処理
        /// </summary>
        /// <param name="getUserId">取得したユーザーID</param>
        /// <returns></returns>
        public async Task GetBallAsync(Guid getUserId)
        {

            //自分以外のルーム参加者全員にボール取得通知を送信
            this.BroadcastExceptSelf(room).OnGetBall(getUserId);
            //this.Broadcast(room).OnGetBall();

        }

        public async Task HitBallAsync(HitData hitData)
        {
            //自分含め全員に通知
            this.Broadcast(room).OnHitBall(hitData);
        }
        /// <summary>
        /// 準備確認処理
        /// </summary>
        /// <param name="isReady">準備状態</param>
        /// <returns></returns>
        public async Task ReadyAsync(bool isReady)
        {
            //準備できたことを自分のRoomDataに保存
            var roomDataStrage = this.room.GetInMemoryStorage<RoomData>();
            var roomData = roomDataStrage.Get(this.ConnectionId);
            roomData.UserState.isReady = isReady;

            Console.WriteLine("準備完了");

            bool isAllReady = true;

            RoomData[] roomDataList = roomDataStrage.AllValues.ToArray<RoomData>();
            //参加ユーザー分準備チェック
            foreach (var data in roomDataList)
            {
                //準備完了していないユーザーがいた場合
                if (!data.UserState.isReady) isAllReady = false;
            }

            if (isAllReady)//ルーム参加者全員に準備状態通知を送信
            this.BroadcastExceptSelf(room).Ready(isAllReady);
            
        }

        /// <summary>
        /// ユーザー情報更新処理
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task UpdateUserStateAsync(UserState state)
        {
            var roomStorage = room.GetInMemoryStorage<RoomData>();

            // ストレージ内のプレイヤー情報を更新する
            var data = roomStorage.Get(this.ConnectionId);
            data.UserState = state;

            //ルーム参加者にプレイヤー情報更新通知を送信
            this.BroadcastExceptSelf(room).UpdateUserState(this.ConnectionId, state);
        }

        /// <summary>
        /// ゲーム開始カウント終了処理
        /// </summary>
        /// <returns></returns>
        public async Task GameCountFinishAsync()
        {
            //カウントダウンが終わったことを自分のRoomDataに保存
            var roomStorage = room.GetInMemoryStorage<RoomData>();
            var roomData = roomStorage.Get(this.ConnectionId);
            roomData.UserState.isGameCountFinish = true;

            //排他制御
            lock (roomStorage)
            {
                //全員がカウントダウン終了したかどうかチェック
                bool isAllCountFinish = true;

                RoomData[] roomDataList = roomStorage.AllValues.ToArray<RoomData>();
                foreach (var data in roomDataList)
                {
                    if (!data.UserState.isGameCountFinish) isAllCountFinish = false;
                }

                //ルーム参加者にゲーム開始通知を送信
                if (isAllCountFinish)
                {
                    this.Broadcast(room).StartGame();
                }
            }
        }

        //ゲーム内時間同期処理
        public async Task GameCountAsync(int currentTime)
        {
            //接続IDが1番のクライアントのみ実行
            this.Broadcast(room).GameCount(currentTime);
        }

        

        /// ゲーム終了処理
        public async Task GameFinishAsync()
        {
            var roomStorage = room.GetInMemoryStorage<RoomData>();
            RoomData[] roomDataList = roomStorage.AllValues.ToArray<RoomData>();

            //送信したユーザーのデータを更新
            var data = roomStorage.Get(this.ConnectionId);
            data.UserState.isGameFinish = true;
            data.UserState.Ranking = GetRanking(roomDataList);

            //排他制御
            //lock (roomStorage)
            //{

                //全員がゲーム終了したかチェック
                bool isAllGameFinish = true;
                foreach (var roomData in roomDataList)
                {
                    if (!roomData.UserState.isGameFinish) isAllGameFinish = false;
                }

                //ルーム参加者全員にゲーム終了通知を送信
                this.Broadcast(room).FinishGame(this.ConnectionId, data.JoinedUser.UserData.Name, isAllGameFinish);
            //}
        }

        //ランキング取得処理
        int GetRanking(RoomData[] roomData)
        {
            int rankNum = 1;        //ランキング変数

            int loop = 0;           //ループ変数
            while (loop < roomData.Length)
            {
                loop = 0;
                for (int i = roomData.Length - 1; i >= 0; i--, loop++)
                {
                    if (roomData[i].UserState.Ranking == rankNum)
                    {
                        rankNum++;
                        break;
                    }
                }
            }
            return rankNum;
        }
    }
}
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;

namespace ServerProject.StreamingHubs
{
    //ルーム内に依存するクラス
    public class RoomData
    {
        public JoinedUser JoinedUser { get; set; }      //参加ユーザー情報

        public UserState UserState { get; set; }        //参加ユーザー状態

        public MoveData MoveData { get; set; }          //プレイヤー移動情報
    }  
}

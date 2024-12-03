using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;

namespace ServerProject.StreamingHubs
{
    //ルーム内に依存するクラス
    public class RoomData
    {
        public JoinedUser JoinedUser { get; set; }

        public int GameState { get; set; }

        public MoveData MoveData { get; set; }
    }
}

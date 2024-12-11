using MessagePack;
using System;
using UnityEngine;

namespace Shared.Model.Entity
{
    /// <summary>
    /// 移動データクラス
    /// </summary>
    [MessagePackObject]
    public class ThrowData
    {
        [Key(0)]
        public Guid ConnectionId { get; set; }                //投げたユーザーのID
        [Key(1)]
        public Vector3 ThorwPos { get; set; }                 //投げた座標
        [Key(2)]
        public Vector3 GoalPos { get; set; }                  //目標座標

    }
}

using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Model.Entity
{
    /// <summary>
    /// デッドデータクラス
    /// </summary>
    [MessagePackObject]
    public class DeadData
    {
        [Key(0)]
        public Guid ConnectionId { get; set; }                //当てられたユーザーのID
        [Key(1)]
        public Guid EnemyId { get; set; }                     //当てたユーザーのID
        [Key(2)]
        public int DamagedHP { get; set; }                       //ダメージを受けた後のHP
        [Key(3)]
        public int Point { get; set; }                        //獲得ポイント

    }
}

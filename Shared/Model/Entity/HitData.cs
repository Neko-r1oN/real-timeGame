////////////////////////////////////////////////////////////////////////////
///
///  ヒットデータスクリプト
///  Author : 川口京佑  2025.01/28
///
////////////////////////////////////////////////////////////////////////////

using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Model.Entity
{
    /// <summary>
    /// ヒットデータクラス
    /// </summary>
    [MessagePackObject]
    public class HitData
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

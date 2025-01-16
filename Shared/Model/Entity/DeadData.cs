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
        public Guid ConnectionId { get; set; }                //やられたユーザーのID
        [Key(1)]
        public string Name { get; set; }                      //ユーザー名
        [Key(2)]
        public int Point { get; set; }                        //最終ポイント
        [Key(3)]
        public int Time { get; set; }                         //生存時間
        [Key(4)] 
        public int ThrowNum { get; set; }                     //投げた回数
        [Key(5)]
        public int HitNum { get; set; }                       //当てた回数
        [Key(6)]
        public int CatchNum { get; set; }                     //獲得ポイント
        [Key(7)]
        public int JoinOrder { get; set; }                   //プレイヤー番号
        [Key(8)]
        public bool IsLast { get; set; }                      //最後のプレイヤーか

    }
}

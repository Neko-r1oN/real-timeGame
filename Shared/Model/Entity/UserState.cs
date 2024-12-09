using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Model.Entity
{
    /// <summary>
    /// 移動データクラス
    /// </summary>
    [MessagePackObject]
    public class UserState
    {
        [Key(0)]
        public bool isReady { get; set; }                    //準備状態
        [Key(1)]
        public bool isGameCountFinish { get; set; }          //カウントダウンが終了しているか
        [Key(2)]
        public bool isGameFinish { get; set; }               //ゲームが終了しているか
        [Key(3)]
        public int Ranking { get; set; }                     //順位
        [Key(4)]
        public int Score { get; set; }                       //獲得スコア
        [Key(5)]
        public int UseCharaId { get; set; }                  //使用キャラクターID
        [Key(6)]
        public int AnimeId { get; set; }                     //アニメーションID
    }
}
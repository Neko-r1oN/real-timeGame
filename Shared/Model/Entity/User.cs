﻿////////////////////////////////////////////////////////////////////////////
///
///  ユーザーデータスクリプト
///  Author : 川口京佑  2025.01/28
///
////////////////////////////////////////////////////////////////////////////

using MessagePack;
using System;

namespace Shared.Model.Entity
{
    /// <summary>
    /// ユーザークラス
    /// </summary>
    [MessagePackObject]
    public class User
    {
        [Key(0)]
        public int Id { get; set; }                //ユーザーID
        [Key(1)]
        public string Name { get; set; }           //ユーザー名
        [Key(2)]
        public string Token { get; set; }          //トークン
        [Key(3)]
        public DateTime Created_at { get; set; }   //生成日時
        [Key(4)]
        public DateTime Updated_at { get; set; }   //更新日時
    }
}

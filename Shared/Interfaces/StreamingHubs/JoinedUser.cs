﻿using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
using Shared.Model.Entity;
using UnityEngine;


namespace Shared.Interfaces.StreamingHubs
{
    /// <summary>
    /// 参加ユーザークラス
    /// </summary>
    [MessagePackObject]
    public class JoinedUser
    {
        [Key(0)]
        public Guid ConnectionId { get; set; }  //接続ID
        [Key(1)]
        public User UserData { get; set; }      //ユーザー情報
        [Key(2)]
        public int JoinOrder;                   //参加順番
    }
}
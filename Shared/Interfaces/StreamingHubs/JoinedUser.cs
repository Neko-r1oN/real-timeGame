using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
using Shared.Model.Entity;
using UnityEngine;


namespace Shared.Interfaces.StreamingHubs
{
    [MessagePackObject]
    public class JoinedUser
    {
        [Key(0)]
        public Guid ConnectionId { get; set; }
        [Key(1)]
        public User UserData { get; set; }
        [Key(2)]
        public int JoinOrder; //参加順番
    }
}

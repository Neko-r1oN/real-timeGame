using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Model.Entity
{
    [MessagePackObject]
    public class ResultData
    {
        [Key(0)]
        public Guid ConnectionId { get; set; }
        [Key(1)]
        public int JoinOrder { get; set; }
        [Key(2)]
        public int Ranking { get; set; }
        [Key(3)]
        public int Point { get; set; }
    }
}

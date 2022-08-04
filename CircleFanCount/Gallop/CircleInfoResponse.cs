using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleFanCount.Gallop
{
    [MessagePackObject]
    public class CircleInfoResponse
    {
        [Key("data")]
        public CommonResponse data; // 0x18

        [MessagePackObject]
        public class CommonResponse // TypeDefIndex: 6740
        {
            [Key("circle_info")]
            public CircleInfo circle_info;
            [Key("circle_ranking_this_month")]
            public CircleRanking circle_ranking_this_month;
            [Key("circle_ranking_last_month")]
            public CircleRanking circle_ranking_last_month;
            [Key("summary_user_info_array")]
            public SummaryUserInfo[] summary_user_info_array;
        }
    }
    [MessagePackObject]
    public class CircleInfo
    {
        [Key("circle_id")]
        public long circle_id;
        [Key("leader_viewer_id")]
        public long leader_viewer_id;
        [Key("name")]
        public string name;
        [Key("comment")]
        public string comment;
        [Key("member_num")]
        public int member_num;
        [Key("join_style")]
        public int join_style;
        [Key("policy")]
        public int policy;
        [Key("make_time")]
        public string make_time;
    }
    [MessagePackObject]
    public class CircleRanking
    {
        [Key("circle_id")]
        public long circle_id;
        [Key("monthly")]
        public int monthly;
        [Key("rank")]
        public int rank;
        [Key("point")]
        public long point;
    }
    [MessagePackObject]
    public class SummaryUserInfo
    {
        [Key("viewer_id")]
        public long viewer_id;
        [Key("name")]
        public string name;
        [Key("fan")]
        public long fan;
    }
}

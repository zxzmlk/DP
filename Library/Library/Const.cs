using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class Const
    {
        public static readonly string TextsSetKey = "TEXTS-SET";
        public static readonly string TextKey = "TEXT-";
        public static readonly string SimilarityKey = "SIMILARITY-";
        public static readonly string RankKey = "RANK-";
        public static readonly string SegmentKey = "SEGMENT-";

        public static readonly string[] Countries = new string[] {
            "Russia", 
            "France",
            "Germany",
            "USA",
            "India"
        };

        public static readonly string SEGMENT_RUS = "RUS";
        public static readonly string SEGMENT_EU = "EU";
        public static readonly string SEGMENT_OTHER = "OTHER";
    }
}

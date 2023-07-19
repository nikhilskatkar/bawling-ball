﻿using bawling_ball.Common;
using System.Collections.Generic;
using System.Linq;

namespace BowlingBall
{
    public class Frame
    {
        public int Index { get; private set; }
        public FrameScoreType ScoreType { get; set; }

        public int TotalScore { get { return Roll.Sum(roll => roll.Score) + BonusScore; } }
        public List<Roll> Roll { get; private set; }
        internal int BonusScore { get; set; }
        public int BonusScoreTacker { get; set; }

        public Frame(int index)
        {
            Index = index;
            Roll = new List<Roll>();
        }
    }
}

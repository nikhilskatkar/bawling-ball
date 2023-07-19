using bawling_ball.Common;

namespace BowlingBall
{
    public class Roll
    {
        public int Score { get; private set; }
        public FrameScoreType ScoreType { get; private set; }

        public Roll(FrameScoreType scoreType, int score)
        {
            Score = score;
            ScoreType = scoreType;
        }
    }
}

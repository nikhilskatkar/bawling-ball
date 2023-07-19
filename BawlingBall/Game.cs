using bawling_ball.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace BowlingBall
{
    public class Game
    {
        public string Name { get; private set; }
        public Frame[] Frames { get; private set; }
        public int TotalScore { get { return Frames.Sum(frame => frame.TotalScore); } }

        private int currentFrame;
        private int remainingRolls;
        private bool bonusRollAvailable;
        
        public Game(string name)
        {
            Name = name;

            Frames = new Frame[Constants.framesPerMatch];
            
            for (var i = 0; i < Frames.Length; i++)
            {
                Frames[i] = new Frame(i);
            }

            remainingRolls = 2;
        }

        public void Roll(int[] pins)
        {
            for (int i = 0; i < pins.Length; i++)
            {
                Roll(pins[i]);
            }
        }

        public void Roll(int score)
        {
            if (score > Constants.maxRollScore)
            {
                throw new ArgumentOutOfRangeException(string.Format("Score cannot be more than {0}.", Constants.maxRollScore));
            }

            if (currentFrame >= Constants.framesPerMatch)
            {
                throw new Exception("No more frames to roll for.");
            }

            var frame = Frames[currentFrame];

            var endOfFrame = false;

            //this variable is to add bonus if strike - trackRolls = 2 and Spare = trackRolls = 1 
            var trackBonusRolls = 0;

            // Add the score
            var frameType = GetScoreType(score, frame.Roll.FirstOrDefault());
            frame.Roll.Add(new Roll(frameType, score));

            // Set the frame type based on the score
            frame.ScoreType = score > 0 || frame.TotalScore > 0 ? FrameScoreType.Standard : FrameScoreType.Miss;

            // Check if it was a strike
            if (IsStrike(score))
            {
                frame.ScoreType = FrameScoreType.Strike;

                // Grant a bonus roll if applicable
                if (IsLastFrame() && bonusRollAvailable)
                {
                    remainingRolls += 1;
                    bonusRollAvailable = false;
                }
                // else register the strike and move on to the next frame
                else if (!IsLastFrame())
                {
                    endOfFrame = true;
                    trackBonusRolls = 2;
                }
            }

            // Reduce the remaining rolls
            remainingRolls -= 1;

            // Check if there are no more rolls remaining, if true then move to the next frame
            if (remainingRolls <= 0)
            {
                if (!IsLastFrame() && frame.TotalScore >= Constants.maxRollScore)
                {
                    frame.ScoreType = FrameScoreType.Spare;
                    trackBonusRolls = 1;
                }

                endOfFrame = true;
            }

            // Update the score tracker with this rolls score.
            HandleBonusScore(score);

            // If this roll is a strike, register the strike. Adding this at end because it should add the scroe of next iterations as bonus score.
            if (trackBonusRolls > 0)
            {
                frame.BonusScoreTacker = trackBonusRolls;
            }

            // If this roll causes the frame to complete, set next frame.
            if (endOfFrame)
            {
                NextFrame(frame);
            }

        }

        private void HandleBonusScore(int score)
        {
            var frames = Frames.Where(x => x.BonusScoreTacker > 0);

            foreach (var frame in frames)
            {
                frame.BonusScore += score;
                frame.BonusScoreTacker = frame.BonusScoreTacker - 1;
            }
        }

        private static FrameScoreType GetScoreType(int score, Roll previousRoll = null)
        {
            switch (score)
            {
                case 0: return FrameScoreType.Miss;
                case Constants.maxRollScore: return FrameScoreType.Strike;
            }

            return previousRoll != null && score + previousRoll.Score == Constants.maxRollScore
                ? FrameScoreType.Spare
                : FrameScoreType.Standard;
        }

        private void NextFrame(Frame frame)
        {
            // Grant a bonus roll if applicable and Do not Increment frame after at the End of the frame.
            if (IsLastFrame() && bonusRollAvailable && frame.TotalScore >= Constants.maxRollScore)
            {
                remainingRolls += 1;
                bonusRollAvailable = false;
            }
            else
            {
                currentFrame++;
                remainingRolls = 2;
                bonusRollAvailable = IsLastFrame();
            }
        }

        private static bool IsStrike(int score)
        {
            return score == Constants.maxRollScore;
        }

        private bool IsLastFrame()
        {
            return currentFrame == Constants.framesPerMatch - 1;
        }
    }
}

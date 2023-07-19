using BowlingBall;
using Xunit;

namespace BowlingBallTests
{
    public class GameTests
    {
        private Game game;

        public GameTests()
        {
            game = new Game("New");
        }

        private void rollMany(int rolls, int pins)
        {
            for (int i = 0; i < rolls; i++)
            {
                game.Roll(pins);
            }
        }

        private void rollSpare()
        {
            game.Roll(6);
            game.Roll(4);
        }

        [Fact]
        public void TestBowlingGameClass()
        {
            Assert.IsType<Game>(game);
        }

        [Fact]
        public void TestGutterGame()
        {
            rollMany(20, 0);
            Assert.Equal(0, game.TotalScore);
        }

        [Fact]
        public void TestAllOnes()
        {
            rollMany(20, 1);
            Assert.Equal(20, game.TotalScore);
        }

        [Fact]
        public void TestOneSpare()
        {
            rollSpare();
            game.Roll(4);
            rollMany(17, 0);
            Assert.Equal(18, game.TotalScore);
        }

        [Fact]
        public void TestOneStrike()
        {
            game.Roll(10);
            game.Roll(4);
            game.Roll(5);
            rollMany(16, 0);
            Assert.Equal(28, game.TotalScore);
        }

        [Fact]
        public void TestPerfectGame()
        {
            rollMany(12, 10);
            Assert.Equal(300, game.TotalScore);
        }

        [Fact]
        public void TestRandomGameNoExtraRoll()
        {
            game.Roll(new int[] { 1, 3, 7, 3, 10, 1, 7, 5, 2, 5, 3, 8, 2, 8, 2, 10, 9, 0 });
            Assert.Equal(131, game.TotalScore);
        }

        [Fact]
        public void TestRandomGameWithSpareThenStrikeAtEnd()
        {
            game.Roll(new int[] { 1, 3, 7, 3, 10, 1, 7, 5, 2, 5, 3, 8, 2, 8, 2, 10, 9, 1, 10 });
            Assert.Equal(143, game.TotalScore);
        }

        [Fact]
        public void TestRandomGameWithThreeStrikesAtEnd()
        {
            game.Roll(new int[] { 1, 3, 7, 3, 10, 1, 7, 5, 2, 5, 3, 8, 2, 8, 2, 10, 10, 10, 10 });
            Assert.Equal(163, game.TotalScore);
        }

        [Fact]
        public void TestShouldFailIfRollExtraBallAfterSpareAtEnd()
        {
            Action action = () => game.Roll(new int[] { 1, 3, 7, 3, 10, 1, 7, 5, 2, 5, 3, 8, 2, 8, 2, 10, 9, 1, 10, 5 });

            Exception exception = Assert.Throws<Exception>(action);
            
            Assert.Equal("No more frames to roll for.", exception.Message);
        }


        [Fact]
        public void TestShouldFailIfRollExtraBallAfterThreeStrikesAtEnd()
        {
            Action action = () => game.Roll(new int[] { 1, 3, 7, 3, 10, 1, 7, 5, 2, 5, 3, 8, 2, 8, 2, 10, 10, 10, 10, 6, 7, 9 }); 

            Exception exception = Assert.Throws<Exception>(action);

            Assert.Equal("No more frames to roll for.", exception.Message);
        }
    }
}
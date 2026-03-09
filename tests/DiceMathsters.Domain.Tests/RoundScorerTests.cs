using System;
using DiceMathsters.Domain.Game;
using Xunit;

namespace DiceMathsters.Domain.Tests.Game
{
    public class RoundScorerTests
    {
        // ================================================================
        //  Perfect match
        // ================================================================

        [Fact]
        public void PerfectMatch_ScoresMaximum()
        {
            Assert.Equal(RoundScorer.MaxScore, RoundScorer.ScoreSubmission(20, 20.0));
        }

        [Fact]
        public void PerfectMatch_NonTrivialTarget_ScoresMaximum()
        {
            Assert.Equal(RoundScorer.MaxScore, RoundScorer.ScoreSubmission(137, 137.0));
        }

        // ================================================================
        //  Near misses — generous scoring close to target
        // ================================================================

        [Fact]
        public void NearMiss_OffByOne_ScoresHighButNotMax()
        {
            int score = RoundScorer.ScoreSubmission(20, 21.0);
            Assert.InRange(score, 85, 99);
        }

        [Fact]
        public void NearMiss_BelowTarget_SameAsSameDistanceAbove()
        {
            int above = RoundScorer.ScoreSubmission(20, 21.0);
            int below = RoundScorer.ScoreSubmission(20, 19.0);
            Assert.Equal(above, below);
        }

        // ================================================================
        //  Midrange misses
        // ================================================================

        [Fact]
        public void MidMiss_HalfTargetDistance_ScoresInMidRange()
        {
            // 50% miss on target 20 → off by 10
            int score = RoundScorer.ScoreSubmission(20, 30.0);
            Assert.InRange(score, 20, 50);
        }

        // ================================================================
        //  Score floor — valid submissions always score at least 1
        // ================================================================

        [Fact]
        public void MaxMiss_AtTargetDistance_ScoresFloor()
        {
            // Off by exactly the target value (100% miss) → floor
            int score = RoundScorer.ScoreSubmission(20, 40.0);
            Assert.Equal(RoundScorer.MinScore, score);
        }

        [Fact]
        public void BeyondMaxMiss_ClampsToFloor()
        {
            // Miss far beyond the target — should still be MinScore, not below
            int score = RoundScorer.ScoreSubmission(20, 1000.0);
            Assert.Equal(RoundScorer.MinScore, score);
        }

        [Fact]
        public void ResultZero_ScoresFloor()
        {
            int score = RoundScorer.ScoreSubmission(20, 0.0);
            Assert.Equal(RoundScorer.MinScore, score);
        }

        [Fact]
        public void NegativeResult_ScoresFloor()
        {
            int score = RoundScorer.ScoreSubmission(20, -50.0);
            Assert.Equal(RoundScorer.MinScore, score);
        }

        // ================================================================
        //  Score is monotonically decreasing as distance increases
        // ================================================================

        [Fact]
        public void Score_DecreasesMonotonically_AsDistanceIncreases()
        {
            int target = 50;
            int previous = RoundScorer.MaxScore;

            for (int miss = 1; miss <= target; miss++)
            {
                int score = RoundScorer.ScoreSubmission(target, target + miss);
                Assert.True(score <= previous,
                    $"Score increased from {previous} to {score} at miss={miss}");
                previous = score;
            }
        }

        // ================================================================
        //  Score is always in valid range for valid submissions
        // ================================================================

        [Theory]
        [InlineData(1,   1)]
        [InlineData(10,  5)]
        [InlineData(10,  10)]
        [InlineData(10,  15)]
        [InlineData(20,  20)]
        [InlineData(20,  25)]
        [InlineData(100, 0)]
        [InlineData(100, 100)]
        [InlineData(100, 199)]
        [InlineData(100, 500)]
        public void Score_AlwaysInValidRange(int target, double result)
        {
            int score = RoundScorer.ScoreSubmission(target, result);
            Assert.InRange(score, RoundScorer.MinScore, RoundScorer.MaxScore);
        }

        // ================================================================
        //  Constants
        // ================================================================

        [Fact]
        public void InvalidScore_IsZero()
        {
            Assert.Equal(0, RoundScorer.InvalidScore);
        }

        [Fact]
        public void InvalidScore_IsLessThanMinScore()
        {
            Assert.True(RoundScorer.InvalidScore < RoundScorer.MinScore);
        }

        // ================================================================
        //  Guard
        // ================================================================

        [Fact]
        public void TargetBelowOne_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => RoundScorer.ScoreSubmission(0, 5.0));
        }
    }
}

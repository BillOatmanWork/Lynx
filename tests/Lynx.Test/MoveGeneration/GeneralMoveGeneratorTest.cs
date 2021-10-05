﻿using Lynx.Model;
using System.Linq;
using Xunit;

namespace Lynx.Test.MoveGeneration
{
    public class GeneralMoveGeneratorTest
    {
        /// <summary>
        /// http://www.talkchess.com/forum3/viewtopic.php?f=7&t=78241&sid=c0f623952408bbd4a891bd36adcc132d&start=10#p907063
        /// </summary>
        [Fact]
        public void DiscoveredCheckAfterEnPassantCapture()
        {
            var originalPosition = new Position("8/8/8/k1pP3R/8/8/8/n4K2 w - c6 0 1");
            var enPassantMove = originalPosition.AllPossibleMoves().Single(m => m.IsEnPassant());
            var positionAferEnPassant = new Position(originalPosition, enPassantMove);

            foreach (var move in MoveGenerator.GenerateAllMoves(positionAferEnPassant))
            {
                if (new Position(positionAferEnPassant, move).IsValid())
                {
                    Assert.NotEqual(Piece.n, (Piece)move.Piece());
                    Assert.Equal(Piece.k, (Piece)move.Piece());
                }
            }
        }
    }
}

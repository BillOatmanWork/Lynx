﻿using SharpFish.Model;
using Xunit;
using CR = SharpFish.Model.CastlingRights;

namespace SharpFish.Test
{
#pragma warning disable S101 // Types should be named in PascalCase
    public class FENParserTest
#pragma warning restore S101 // Types should be named in PascalCase
    {
        private readonly Game _game;

        public FENParserTest()
        {
            _game = new Game();
        }

        [Fact]
        public void PieceBitBoards_InitialPosition()
        {
            // Arrange

            // Make sure a previous Fen doesn't change anything
            const string previuosFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            bool success;
            (success, _, _, _, _, _) = FENParser.ParseFEN(previuosFen, _game.PieceBitBoards, _game.OccupancyBitBoards);
            Assert.True(success);

            const string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

            // Act
            (success, _, _, _, _, _) = FENParser.ParseFEN(fen, _game.PieceBitBoards, _game.OccupancyBitBoards);

            // Assert
            Assert.True(success);

            var whitePawns = _game.PieceBitBoards[(int)Piece.P];
            Assert.Equal(0b1111_1111UL << (6 * 8), whitePawns.Board);
            var blackPawns = _game.PieceBitBoards[(int)Piece.p];
            Assert.Equal(0b1111_1111UL << (1 * 8), blackPawns.Board);

            var whiteRooks = _game.PieceBitBoards[(int)Piece.R];
            Assert.Equal(0b1000_0001UL << (7 * 8), whiteRooks.Board);
            var blackRooks = _game.PieceBitBoards[(int)Piece.r];
            Assert.Equal(0b1000_0001UL << (0 * 8), blackRooks.Board);

            var whiteKnights = _game.PieceBitBoards[(int)Piece.N];
            Assert.Equal(0b0100_0010UL << (7 * 8), whiteKnights.Board);
            var blackKnights = _game.PieceBitBoards[(int)Piece.n];
            Assert.Equal(0b0100_0010UL << (0 * 8), blackKnights.Board);

            var whiteBishops = _game.PieceBitBoards[(int)Piece.B];
            Assert.Equal(0b0010_0100UL << (7 * 8), whiteBishops.Board);
            var blackBishops = _game.PieceBitBoards[(int)Piece.b];
            Assert.Equal(0b0010_0100UL << (0 * 8), blackBishops.Board);

            var whiteQueen = _game.PieceBitBoards[(int)Piece.Q];
            Assert.Equal(0b0000_1000UL << (int)BoardSquares.a1, whiteQueen.Board);
            var blackQueen = _game.PieceBitBoards[(int)Piece.q];
            Assert.Equal(0b0000_1000UL << (0 * 8), blackQueen.Board);

            var whiteKing = _game.PieceBitBoards[(int)Piece.K];
            Assert.Equal(0b0001_0000UL << (7 * 8), whiteKing.Board);
            var blackKing = _game.PieceBitBoards[(int)Piece.k];
            Assert.Equal(0b0001_0000UL << (0 * 8), blackKing.Board);
        }

        [Fact]
        public void PieceBitBoards()
        {
            // Arrange
            const string fen = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1 ";

            // Act
            bool success;
            (success, _, _, _, _, _) = FENParser.ParseFEN(fen, _game.PieceBitBoards, _game.OccupancyBitBoards);

            // Assert
            Assert.True(success);

            Assert.True(_game.PieceBitBoards[(int)Piece.p].GetBit(BoardSquares.a7));
            Assert.True(_game.PieceBitBoards[(int)Piece.p].GetBit(BoardSquares.b4));
            Assert.True(_game.PieceBitBoards[(int)Piece.p].GetBit(BoardSquares.c7));
            Assert.True(_game.PieceBitBoards[(int)Piece.p].GetBit(BoardSquares.d7));
            Assert.True(_game.PieceBitBoards[(int)Piece.p].GetBit(BoardSquares.e6));
            Assert.True(_game.PieceBitBoards[(int)Piece.p].GetBit(BoardSquares.f7));
            Assert.True(_game.PieceBitBoards[(int)Piece.p].GetBit(BoardSquares.g6));
            Assert.True(_game.PieceBitBoards[(int)Piece.p].GetBit(BoardSquares.h3));

            Assert.True(_game.PieceBitBoards[(int)Piece.r].GetBit(BoardSquares.a8));
            Assert.True(_game.PieceBitBoards[(int)Piece.r].GetBit(BoardSquares.h8));

            Assert.True(_game.PieceBitBoards[(int)Piece.n].GetBit(BoardSquares.b6));
            Assert.True(_game.PieceBitBoards[(int)Piece.n].GetBit(BoardSquares.f6));

            Assert.True(_game.PieceBitBoards[(int)Piece.b].GetBit(BoardSquares.a6));
            Assert.True(_game.PieceBitBoards[(int)Piece.b].GetBit(BoardSquares.g7));

            Assert.True(_game.PieceBitBoards[(int)Piece.q].GetBit(BoardSquares.e7));
            Assert.True(_game.PieceBitBoards[(int)Piece.k].GetBit(BoardSquares.e8));

            Assert.True(_game.PieceBitBoards[(int)Piece.P].GetBit(BoardSquares.a2));
            Assert.True(_game.PieceBitBoards[(int)Piece.P].GetBit(BoardSquares.b2));
            Assert.True(_game.PieceBitBoards[(int)Piece.P].GetBit(BoardSquares.c2));
            Assert.True(_game.PieceBitBoards[(int)Piece.P].GetBit(BoardSquares.d5));
            Assert.True(_game.PieceBitBoards[(int)Piece.P].GetBit(BoardSquares.e4));
            Assert.True(_game.PieceBitBoards[(int)Piece.P].GetBit(BoardSquares.f2));
            Assert.True(_game.PieceBitBoards[(int)Piece.P].GetBit(BoardSquares.g2));
            Assert.True(_game.PieceBitBoards[(int)Piece.P].GetBit(BoardSquares.h2));

            Assert.True(_game.PieceBitBoards[(int)Piece.R].GetBit(BoardSquares.a1));
            Assert.True(_game.PieceBitBoards[(int)Piece.R].GetBit(BoardSquares.h1));

            Assert.True(_game.PieceBitBoards[(int)Piece.N].GetBit(BoardSquares.c3));
            Assert.True(_game.PieceBitBoards[(int)Piece.N].GetBit(BoardSquares.e5));

            Assert.True(_game.PieceBitBoards[(int)Piece.B].GetBit(BoardSquares.d2));
            Assert.True(_game.PieceBitBoards[(int)Piece.B].GetBit(BoardSquares.e2));

            Assert.True(_game.PieceBitBoards[(int)Piece.Q].GetBit(BoardSquares.f3));
            Assert.True(_game.PieceBitBoards[(int)Piece.K].GetBit(BoardSquares.e1));
        }

        [Theory]
        [InlineData("8/8/8/8/8/8/8/8 w - - 0 1")]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1 ")]
        [InlineData("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1 ")]
        [InlineData("rnbqkb1r/pp1p1pPp/8/2p1pP2/1P1P4/3P3P/P1P1P3/RNBQKBNR w KQkq e6 0 1")]
        [InlineData("r2q1rk1/ppp2ppp/2n1bn2/2b1p3/3pP3/3P1NPP/PPP1NPB1/R1BQ1RK1 b - - 0 9 ")]
        public void OccupancyBitBoards(string fen)
        {
            // Arrange
            // Make sure a previous Fen doesn't change anything
            const string previuosFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            bool success;
            (success, _, _, _, _, _) = FENParser.ParseFEN(previuosFen, _game.PieceBitBoards, _game.OccupancyBitBoards);
            Assert.True(success);

            // Act
            (success, _, _, _, _, _) = FENParser.ParseFEN(fen, _game.PieceBitBoards, _game.OccupancyBitBoards);

            // Assert
            Assert.True(success);

            var expectedWhiteOccupancy = 0UL;
            expectedWhiteOccupancy |= _game.PieceBitBoards[(int)Piece.P].Board;
            expectedWhiteOccupancy |= _game.PieceBitBoards[(int)Piece.N].Board;
            expectedWhiteOccupancy |= _game.PieceBitBoards[(int)Piece.B].Board;
            expectedWhiteOccupancy |= _game.PieceBitBoards[(int)Piece.R].Board;
            expectedWhiteOccupancy |= _game.PieceBitBoards[(int)Piece.Q].Board;
            expectedWhiteOccupancy |= _game.PieceBitBoards[(int)Piece.K].Board;

            Assert.Equal(expectedWhiteOccupancy, _game.OccupancyBitBoards[(int)Side.White].Board);

            var expectedBlackOccupancy = 0UL;
            expectedBlackOccupancy |= _game.PieceBitBoards[(int)Piece.p].Board;
            expectedBlackOccupancy |= _game.PieceBitBoards[(int)Piece.n].Board;
            expectedBlackOccupancy |= _game.PieceBitBoards[(int)Piece.b].Board;
            expectedBlackOccupancy |= _game.PieceBitBoards[(int)Piece.r].Board;
            expectedBlackOccupancy |= _game.PieceBitBoards[(int)Piece.q].Board;
            expectedBlackOccupancy |= _game.PieceBitBoards[(int)Piece.k].Board;

            Assert.Equal(expectedBlackOccupancy, _game.OccupancyBitBoards[(int)Side.Black].Board);

            var expectedCombinedOccupancy = expectedWhiteOccupancy | expectedBlackOccupancy;

            Assert.Equal(expectedCombinedOccupancy, _game.OccupancyBitBoards[(int)Side.Both].Board);
        }

        [Theory]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", Side.White)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR W KQkq - 0 1", Side.White)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", Side.Black)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR B KQkq - 0 1", Side.Black)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR A KQkq - 0 1", Side.Both)]
        public void SideToMove(string fen, Side expectedSide)
        {
            bool success;
            Side side;
            (success, side, _, _, _, _) = FENParser.ParseFEN(fen, _game.PieceBitBoards, _game.OccupancyBitBoards);

            if (expectedSide != Side.Both)
            {
                Assert.True(success);
                Assert.Equal(expectedSide, side);
            }
            else
            {
                Assert.False(success);
                Assert.Equal(Side.Both, side);
            }
        }

        [Theory]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", (int)CR.WK | (int)CR.WQ | (int)CR.BK | (int)CR.BQ)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w Qkq - 0 1", (int)CR.WQ | (int)CR.BK | (int)CR.BQ)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w Kkq - 0 1", (int)CR.WK | (int)CR.BK | (int)CR.BQ)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQq - 0 1", (int)CR.WK | (int)CR.WQ | (int)CR.BQ)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQk - 0 1", (int)CR.WK | (int)CR.WQ | (int)CR.BK)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQ - 0 1", (int)CR.WK | (int)CR.WQ)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w kq - 0 1", (int)CR.BK | (int)CR.BQ)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w Qq - 0 1", (int)CR.WQ | (int)CR.BQ)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w Kk - 0 1", (int)CR.WK | (int)CR.BK)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w - - 0 1", 0)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w A - 0 1", -1)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KA - 0 1", -1)]
        public void CastlingRights(string fen, int expectedCastleResult)
        {
            // Arrange
            // Make sure a previous Fen doesn't change anything
            const string previuosFen = "8/8/8/8/8/8/8/8 w KQkq 1234 0 1";
            bool success;
            (success, _, _, _, _, _) = FENParser.ParseFEN(previuosFen, _game.PieceBitBoards, _game.OccupancyBitBoards);
            Assert.False(success);

            // Act
            int castleResult;
            (success, _, castleResult, _, _, _) = FENParser.ParseFEN(fen, _game.PieceBitBoards, _game.OccupancyBitBoards);

            // Assert
            if (expectedCastleResult >= 0)
            {
                Assert.True(success);
                Assert.Equal(expectedCastleResult, castleResult);
            }
            else
            {
                Assert.False(success);
            }
        }

        [Theory]
        [InlineData("rnbqkbnr/pppp1ppp/8/8/3pP3/8/PPPP1PPP/RNBQKBNR b Qq e3 0 1", BoardSquares.e3)]
        [InlineData("rnbqkbnr/ppppp1pp/8/8/4pP2/8/PPPPP1PP/RNBQKBNR b Qq f3 0 1", BoardSquares.f3)]
        [InlineData("rnbqkbnr/pp1ppppp/8/1Pp5/8/8/P1PPPPPP/RNBQKBNR w KQkq c6 0 1", BoardSquares.c6)]
        [InlineData("rnbqkbnr/ppp1pppp/8/2Pp4/8/8/PP1PPPPP/RNBQKBNR w KQkq d6 0 1", BoardSquares.d6)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardSquares.noSquare)]
        public void EnPassant(string fen, BoardSquares expectedEnPassantSquare)
        {
            bool success;
            BoardSquares enPassant;
            (success, _, _, enPassant, _, _) = FENParser.ParseFEN(fen, _game.PieceBitBoards, _game.OccupancyBitBoards);

            Assert.True(success);
            Assert.Equal(expectedEnPassantSquare, enPassant);
        }

        [Theory]
        [InlineData("8/8/8/8/8/8/8/8 w KQkq e6 0 1")]
        [InlineData("8/8/8/8/8/8/8/8 w KQkq e7 0 1")]
        [InlineData("rnbqkbnr/pppp1ppp/8/8/3pP3/8/PPPP1PPP/RNBQKBNR b Qq a3 0 1")]  // e3 could be
        [InlineData("rnbqkbnr/ppppp1pp/8/8/4pP2/8/PPPPP1PP/RNBQKBNR b Qq b3 0 1")]  // f3 could be
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq i1 0 1")]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq i3 0 1")]
        public void EnPassant_Error(string fen)
        {
            bool success;
            (success, _, _, _, _, _) = FENParser.ParseFEN(fen, _game.PieceBitBoards, _game.OccupancyBitBoards);

            Assert.False(success);
        }

        [Theory]
        [InlineData("8/8/8/8/8/8/8/8 w KQkq - 0 1", 0)]
        [InlineData("8/8/8/8/8/8/8/8 w KQkq - 1 1", 1)]
        [InlineData("8/8/8/8/8/8/8/8 w KQkq - 51 1", 51)]
        public void HalfMoveClock(string fen, int expectedHalfMoves)
        {
            bool success;
            int halfMoves;
            (success, _, _, _, halfMoves, _) = FENParser.ParseFEN(fen, _game.PieceBitBoards, _game.OccupancyBitBoards);

            Assert.True(success);
            Assert.Equal(expectedHalfMoves, halfMoves);
        }

        [Theory]
        [InlineData("8/8/8/8/8/8/8/8 w KQkq - 0 1", 1)]
        [InlineData("8/8/8/8/8/8/8/8 w KQkq - 0 50", 50)]
        [InlineData("8/8/8/8/8/8/8/8 w KQkq - 0 100", 100)]
        public void FullMoveCounter(string fen, int expectedFullMoveCounter)
        {
            bool success;
            int fullMoveCounter;
            (success, _, _, _, _, fullMoveCounter) = FENParser.ParseFEN(fen, _game.PieceBitBoards, _game.OccupancyBitBoards);

            Assert.True(success);
            Assert.Equal(expectedFullMoveCounter, fullMoveCounter);
        }
    }
}
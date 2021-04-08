﻿using SharpFish.Model;
using Xunit;
using BS = SharpFish.Model.BoardSquares;

namespace SharpFish.Test.PregeneratedAttacks
{
    public class RookAttacksTest
    {
        [Theory]
        [InlineData(BS.a8, new BS[] { }, new[] { BS.b8, BS.c8, BS.d8, BS.e8, BS.f8, BS.g8, BS.h8, BS.a7, BS.a6, BS.a5, BS.a4, BS.a3, BS.a2, BS.a1 })]
        [InlineData(BS.a8, new[] { BS.g8, BS.a2 }, new[] { BS.b8, BS.c8, BS.d8, BS.e8, BS.f8, BS.g8, BS.a7, BS.a6, BS.a5, BS.a4, BS.a3, BS.a2 })]
        [InlineData(BS.a8, new[] { BS.b8, BS.a7 }, new[] { BS.b8, BS.a7 })]

        [InlineData(BS.a1, new BS[] { }, new[] { BS.b1, BS.c1, BS.d1, BS.e1, BS.f1, BS.g1, BS.h1, BS.a2, BS.a3, BS.a4, BS.a5, BS.a6, BS.a7, BS.a8 })]
        [InlineData(BS.a1, new[] { BS.g1, BS.a7 }, new[] { BS.b1, BS.c1, BS.d1, BS.e1, BS.f1, BS.g1, BS.a2, BS.a3, BS.a4, BS.a5, BS.a6, BS.a7 })]
        [InlineData(BS.a1, new[] { BS.b1, BS.a2 }, new[] { BS.b1, BS.a2 })]

        [InlineData(BS.h8, new BS[] { }, new[] { BS.g8, BS.f8, BS.e8, BS.d8, BS.c8, BS.b8, BS.a8, BS.h7, BS.h6, BS.h5, BS.h4, BS.h3, BS.h2, BS.h1 })]
        [InlineData(BS.h8, new[] { BS.b8, BS.h2 }, new[] { BS.g8, BS.f8, BS.e8, BS.d8, BS.c8, BS.b8, BS.h7, BS.h6, BS.h5, BS.h4, BS.h3, BS.h2 })]
        [InlineData(BS.h8, new[] { BS.g8, BS.h7 }, new[] { BS.g8, BS.h7 })]

        [InlineData(BS.h1, new BS[] { }, new[] { BS.g1, BS.f1, BS.e1, BS.d1, BS.c1, BS.b1, BS.a1, BS.h2, BS.h3, BS.h4, BS.h5, BS.h6, BS.h7, BS.h8 })]
        [InlineData(BS.h1, new[] { BS.b1, BS.h7 }, new[] { BS.g1, BS.f1, BS.e1, BS.d1, BS.c1, BS.b1, BS.h2, BS.h3, BS.h4, BS.h5, BS.h6, BS.h7 })]
        [InlineData(BS.h1, new[] { BS.g1, BS.h2 }, new[] { BS.g1, BS.h2 })]

        [InlineData(BS.d4, new[] { BS.d3, BS.d2, BS.b4, BS.d7, BS.h4 }, new[] { BS.b4, BS.c4, BS.e4, BS.f4, BS.g4, BS.h4, BS.d7, BS.d6, BS.d5, BS.d3 })]
        public void GenerateRookAttacksOnTheFly(BS rookSquare, BS[] occupiedSquares, BS[] attackedSquares)
        {
            // Arrange
            var occupiedBoard = new BitBoard(occupiedSquares);

            // Act
            var attacks = AttacksGenerator.GenerateRookAttacksOnTheFly((int)rookSquare, occupiedBoard);

            // Assert
            foreach (var attackedSquare in attackedSquares)
            {
                Assert.True(attacks.GetBit(attackedSquare));
                attacks.PopBit(attackedSquare);
            }

            Assert.Equal(default, attacks);
        }

        /// <summary>
        /// Implicitly tests <see cref="AttacksGenerator.InitializeRookAttacks"/> and <see cref="Constants.RookMagicNumbers"/>
        /// </summary>
        /// <param name="rookSquare"></param>
        /// <param name="occupiedSquares"></param>
        /// <param name="attackedSquares"></param>
        [Theory]
        [InlineData(BS.a8, new BS[] { }, new[] { BS.b8, BS.c8, BS.d8, BS.e8, BS.f8, BS.g8, BS.h8, BS.a7, BS.a6, BS.a5, BS.a4, BS.a3, BS.a2, BS.a1 })]
        [InlineData(BS.a8, new[] { BS.g8, BS.a2 }, new[] { BS.b8, BS.c8, BS.d8, BS.e8, BS.f8, BS.g8, BS.a7, BS.a6, BS.a5, BS.a4, BS.a3, BS.a2 })]
        [InlineData(BS.a8, new[] { BS.b8, BS.a7 }, new[] { BS.b8, BS.a7 })]

        [InlineData(BS.a1, new BS[] { }, new[] { BS.b1, BS.c1, BS.d1, BS.e1, BS.f1, BS.g1, BS.h1, BS.a2, BS.a3, BS.a4, BS.a5, BS.a6, BS.a7, BS.a8 })]
        [InlineData(BS.a1, new[] { BS.g1, BS.a7 }, new[] { BS.b1, BS.c1, BS.d1, BS.e1, BS.f1, BS.g1, BS.a2, BS.a3, BS.a4, BS.a5, BS.a6, BS.a7 })]
        [InlineData(BS.a1, new[] { BS.b1, BS.a2 }, new[] { BS.b1, BS.a2 })]

        [InlineData(BS.h8, new BS[] { }, new[] { BS.g8, BS.f8, BS.e8, BS.d8, BS.c8, BS.b8, BS.a8, BS.h7, BS.h6, BS.h5, BS.h4, BS.h3, BS.h2, BS.h1 })]
        [InlineData(BS.h8, new[] { BS.b8, BS.h2 }, new[] { BS.g8, BS.f8, BS.e8, BS.d8, BS.c8, BS.b8, BS.h7, BS.h6, BS.h5, BS.h4, BS.h3, BS.h2 })]
        [InlineData(BS.h8, new[] { BS.g8, BS.h7 }, new[] { BS.g8, BS.h7 })]

        [InlineData(BS.h1, new BS[] { }, new[] { BS.g1, BS.f1, BS.e1, BS.d1, BS.c1, BS.b1, BS.a1, BS.h2, BS.h3, BS.h4, BS.h5, BS.h6, BS.h7, BS.h8 })]
        [InlineData(BS.h1, new[] { BS.b1, BS.h7 }, new[] { BS.g1, BS.f1, BS.e1, BS.d1, BS.c1, BS.b1, BS.h2, BS.h3, BS.h4, BS.h5, BS.h6, BS.h7 })]
        [InlineData(BS.h1, new[] { BS.g1, BS.h2 }, new[] { BS.g1, BS.h2 })]

        [InlineData(BS.d4, new[] { BS.d3, BS.d2, BS.b4, BS.d7, BS.h4 }, new[] { BS.b4, BS.c4, BS.e4, BS.f4, BS.g4, BS.h4, BS.d7, BS.d6, BS.d5, BS.d3 })]
        public void GetRookAttacks(BS rookSquare, BS[] occupiedSquares, BS[] attackedSquares)
        {
            // Arrange
            var occupancy = new BitBoard(occupiedSquares);

            // Act
            var attacks = Attacks.RookAttacks((int)rookSquare, occupancy);

            // Assert
            ValidateAttacks(attackedSquares, attacks);

            static void ValidateAttacks(BS[] attackedSquares, BitBoard attacks)
            {
                // Assert
                foreach (var attackedSquare in attackedSquares)
                {
                    Assert.True(attacks.GetBit(attackedSquare));
                    attacks.PopBit(attackedSquare);
                }

                Assert.Equal(default, attacks);
            }
        }
    }
}
﻿using SharpFish.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFish
{
    public static class Attacks
    {
        private static readonly BitBoard[] _bishopOccupancyMasks;
        private static readonly BitBoard[] _rookOccupancyMasks;

        /// <summary>
        /// [64 (Squares), 512 (Occupancies)]
        /// Use <see cref="BishopAttacks(int, BitBoard)"/>
        /// </summary>
        private static readonly BitBoard[,] _bishopAttacks;

        /// <summary>
        /// [64 (Squares), 4096 (Occupancies)]
        /// Use <see cref="RookAttacks(int, BitBoard)"/>
        /// </summary>
        private static readonly BitBoard[,] _rookAttacks;

        /// <summary>
        /// [2 (B|W), 64 (Squares)]
        /// </summary>
        public static BitBoard[,] PawnAttacks { get; }
        public static BitBoard[] KnightAttacks { get; }
        public static BitBoard[] KingAttacks { get; }

        static Attacks()
        {
            KingAttacks = AttacksGenerator.InitializeKingAttacks();
            PawnAttacks = AttacksGenerator.InitializePawnAttacks();
            KnightAttacks = AttacksGenerator.InitializeKnightAttacks();

            (_bishopOccupancyMasks, _bishopAttacks) = AttacksGenerator.InitializeBishopAttacks();
            (_rookOccupancyMasks, _rookAttacks) = AttacksGenerator.InitializeRookAttacks();
        }

        /// <summary>
        /// Get Bishop attacks assuming current board occupancy
        /// </summary>
        /// <param name="squareIndex"></param>
        /// <param name="occupancy"></param>
        /// <returns></returns>
        public static BitBoard BishopAttacks(int squareIndex, BitBoard occupancy)
        {
            var occ = occupancy.Board & _bishopOccupancyMasks[squareIndex].Board;
            occ *= Constants.BishopMagicNumbers[squareIndex];
            occ >>= (64 - Constants.BishopRelevantOccupancyBits[squareIndex]);

            return _bishopAttacks[squareIndex, occ];
        }

        /// <summary>
        /// Get Rook attacks assuming current board occupancy
        /// </summary>
        /// <param name="squareIndex"></param>
        /// <param name="occupancy"></param>
        /// <returns></returns>
        public static BitBoard RookAttacks(int squareIndex, BitBoard occupancy)
        {
            var occ = occupancy.Board & _rookOccupancyMasks[squareIndex].Board;
            occ *= Constants.RookMagicNumbers[squareIndex];
            occ >>= (64 - Constants.RookRelevantOccupancyBits[squareIndex]);

            return _rookAttacks[squareIndex, occ];
        }

        /// <summary>
        /// Get Queen attacks assuming current board occupancy
        /// Use <see cref="QueenAttacks(BitBoard, BitBoard)"/> if rook and bishop attacks are already calculated
        /// </summary>
        /// <param name="squareIndex"></param>
        /// <param name="occupancy"></param>
        /// <returns></returns>
        public static BitBoard QueenAttacks(int squareIndex, BitBoard occupancy)
        {
            return QueenAttacks(
                RookAttacks(squareIndex, occupancy),
                BishopAttacks(squareIndex, occupancy));
        }

        /// <summary>
        /// Get Queen attacks having rook and bishop attacks pre-calculated
        /// </summary>
        /// <param name="rookAttacks"></param>
        /// <param name="bishopAttacks"></param>
        /// <returns></returns>
        public static BitBoard QueenAttacks(BitBoard rookAttacks, BitBoard bishopAttacks)
        {
            return new BitBoard(
                rookAttacks.Board | bishopAttacks.Board);
        }

        public static bool IsSquaredAttacked(int squareIndex, Side sideToMove, BitBoard[] piecePosition, BitBoard[] occupancy)
        {
            if (sideToMove == Side.Both)
            {
                Console.WriteLine($"[Warn] {nameof(IsSquaredAttacked)} doesn't support Side.Both");
                return false;
            }

            var offset = (int)Piece.p;
            offset -= (offset * (int)sideToMove);    // 0 if Side.White, 6 (Piece.p) if Side.Black

            // I tried to order them from most to least likely

            return
                IsSquareAttackedByPawns(squareIndex, sideToMove, offset, piecePosition)
                || IsSquareAttackedByKnights(squareIndex, offset, piecePosition)
                || IsSquareAttackedByBishops(squareIndex, offset, piecePosition, occupancy, out var bishopAttacks)
                || IsSquareAttackedByRooks(squareIndex, offset, piecePosition, occupancy, out var rookAttacks)
                || IsSquareAttackedByQueens(offset, bishopAttacks, rookAttacks, piecePosition)
                || IsSquareAttackedByKing(squareIndex, offset, piecePosition);
        }

        public static bool IsSquaredAttacked(int squaredIndex, Game game) =>
            IsSquaredAttacked(squaredIndex, game.Side, game.PieceBitBoards, game.OccupancyBitBoards);

        private static bool IsSquareAttackedByPawns(int squareIndex, Side sideToMove, int offset, BitBoard[] pieces)
        {
            var oppositeColorIndex = ((int)sideToMove + 1) % 2;

            return (PawnAttacks[oppositeColorIndex, squareIndex].Board & pieces[offset].Board) != default;
        }

        private static bool IsSquareAttackedByKnights(int squareIndex, int offset, BitBoard[] piecePosition)
        {
            return (KnightAttacks[squareIndex].Board & piecePosition[(int)Piece.N + offset].Board) != default;
        }

        private static bool IsSquareAttackedByKing(int squareIndex, int offset, BitBoard[] piecePosition)
        {
            return (KingAttacks[squareIndex].Board & piecePosition[(int)Piece.K + offset].Board) != default;
        }

        private static bool IsSquareAttackedByBishops(int squareIndex, int offset, BitBoard[] piecePosition, BitBoard[] occupancy, out BitBoard bishopAttacks)
        {
            bishopAttacks = BishopAttacks(squareIndex, occupancy[(int)Side.Both]);
            return (bishopAttacks.Board & piecePosition[(int)Piece.B + offset].Board) != default;
        }

        private static bool IsSquareAttackedByRooks(int squareIndex, int offset, BitBoard[] piecePosition, BitBoard[] occupancy, out BitBoard rookAttacks)
        {
            rookAttacks = RookAttacks(squareIndex, occupancy[(int)Side.Both]);
            return (rookAttacks.Board & piecePosition[(int)Piece.R + offset].Board) != default;
        }

        private static bool IsSquareAttackedByQueens(int offset, BitBoard bishopAttacks, BitBoard rookAttacks, BitBoard[] piecePosition)
        {
            var queenAttacks = QueenAttacks(rookAttacks, bishopAttacks);
            return (queenAttacks.Board & piecePosition[(int)Piece.Q + offset].Board) != default;
        }
    }
}
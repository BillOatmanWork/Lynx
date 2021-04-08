﻿using SharpFish.Model;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SharpFish
{
#pragma warning disable S101 // Types should be named in PascalCase

    public static class FENParser
#pragma warning restore S101 // Types should be named in PascalCase
    {
        private static readonly Regex RanksRegex = new Regex(@"(?<=^|\/)[P|N|B|R|Q|K|p|n|b|r|q|k|\d]{1,8}", RegexOptions.Compiled);

        public static (bool Success, Side Side, int Castle, BoardSquares EnPassant, int HalfMoveClock, int FullMoveCounter)
            ParseFEN(string fen, BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
        {
            for (int i = 0; i < pieceBitBoards.Length; ++i)
            {
                pieceBitBoards[i].Clear();
            }

            for (int i = 0; i < occupancyBitBoards.Length; ++i)
            {
                occupancyBitBoards[i].Clear();
            }

            bool success = true;
            Side side = Side.Both;
            int castle = 0;
            int halfMoveClock = 0, fullMoveCounter = 1;
            BoardSquares enPassant = BoardSquares.noSquare;

            try
            {
                MatchCollection matches;
                (matches, success) = ParseBoard(fen, pieceBitBoards, occupancyBitBoards);

                var unparsedString = fen[(matches.Last().Index + matches.Last().Length)..];
                var parts = unparsedString.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 3)
                {
                    throw new($"Error parsing second half (after board) of fen {fen}");
                }

                side = ParseSide(parts[0]);

                castle = ParseCastlingRights(parts[1]);

                (enPassant, success) = ParseEnPassant(parts[2], pieceBitBoards, side);

                if (parts.Length < 4 || !int.TryParse(parts[3], out halfMoveClock))
                {
                    Logger.Warn("No half move clock detected");
                }

                if (parts.Length < 5 || !int.TryParse(parts[4], out fullMoveCounter))
                {
                    Logger.Warn("No full move counter detected");
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error parsing FEN string {fen}");
                Logger.Error(e.Message);
                success = false;
            }

            return (success, side, castle, enPassant, halfMoveClock, fullMoveCounter);
        }

        private static (MatchCollection Matches, bool Success) ParseBoard(string fen, BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
        {
            bool success = true;

            var rankIndex = 0;
            var matches = RanksRegex.Matches(fen);
            foreach (var match in matches)
            {
                var fileIndex = 0;
                foreach (var ch in ((Group)match).Value)
                {
                    if (Constants.PiecesByChar.TryGetValue(ch, out Piece piece))
                    {
                        pieceBitBoards[(int)piece].SetBit(BitBoard.SquareIndex(rankIndex, fileIndex));
                        ++fileIndex;
                    }
                    else if (int.TryParse($"{ch}", out int emptySquares))
                    {
                        fileIndex += emptySquares;
                    }
                    else
                    {
                        Logger.Error($"Unrecognized character in FEN: {ch} (within {((Group)match).Value})");
                        success = false;
                        break;
                    }
                }

                PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

                ++rankIndex;
            }

            return (matches, success);

            static void PopulateOccupancies(BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
            {
                var limit = (int)Piece.K;
                for (int piece = (int)Piece.P; piece <= limit; ++piece)
                {
                    occupancyBitBoards[(int)Side.White].Board |= pieceBitBoards[piece].Board;
                }

                limit = (int)Piece.k;
                for (int piece = (int)Piece.p; piece <= limit; ++piece)
                {
                    occupancyBitBoards[(int)Side.Black].Board |= pieceBitBoards[piece].Board;
                }

                occupancyBitBoards[(int)Side.Both].Board = occupancyBitBoards[(int)Side.White].Board | occupancyBitBoards[(int)Side.Black].Board;
            }
        }

        private static Side ParseSide(string sideString)
        {
            bool isWhite = sideString.Equals("w", StringComparison.OrdinalIgnoreCase);
            return isWhite || sideString.Equals("b", StringComparison.OrdinalIgnoreCase)
                ? (isWhite ? Side.White : Side.Black)
                : throw new($"Unrecognized side: {sideString}");
        }

        private static int ParseCastlingRights(string castleString)
        {
            int castle = 0;

            foreach (var ch in castleString)
            {
                castle |= ch switch
                {
                    'K' => (int)CastlingRights.WK,
                    'Q' => (int)CastlingRights.WQ,
                    'k' => (int)CastlingRights.BK,
                    'q' => (int)CastlingRights.BQ,
                    '-' => castle,
                    _ => throw new($"Unrecognized castling char: {ch}")
                };
            }

            return castle;
        }

        private static (BoardSquares EnPassant, bool Success) ParseEnPassant(string enPassantString, BitBoard[] PieceBitBoards, Side side)
        {
            bool success = true;
            BoardSquares enPassant = BoardSquares.noSquare;

            if (Enum.TryParse(enPassantString, ignoreCase: true, out BoardSquares result))
            {
                enPassant = result;

                var rank = 1 + ((int)enPassant / 8);
                if (rank != 3 && rank != 6)
                {
                    success = false;
                    Logger.Error($"Invalid en passant square: {enPassantString}");
                }

                // Check that there's an actual pawn to be captured
                var pawnOffset = side == Side.White
                    ? +8
                    : -8;

                var pawnSquare = (int)enPassant + pawnOffset;

                var pawnBitBoard = side == Side.White
                    ? PieceBitBoards[(int)Piece.p]
                    : PieceBitBoards[(int)Piece.P];

                if (!pawnBitBoard.GetBit(pawnSquare))
                {
                    success = false;
                    Logger.Error($"Invalid board: en passant square {enPassantString}, but no {side} pawn located in {pawnBitBoard}");
                }
            }
            else if (enPassantString != "-")
            {
                success = false;
                Logger.Error($"Invalid en passant square: {enPassantString}");
            }

            return (enPassant, success);
        }
    }
}
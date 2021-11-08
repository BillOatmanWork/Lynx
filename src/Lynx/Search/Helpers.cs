﻿using Lynx.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Lynx.Search
{
    public static partial class SearchAlgorithms
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private const int MinValue = -2 * Position.CheckMateEvaluation;
        private const int MaxValue = +2 * Position.CheckMateEvaluation;

        public record SearchResult(Move BestMove, double Evaluation, int TargetDepth, int DepthReached, int Nodes, long Time, long NodesPerSecond, List<Move> Moves)
        {
            public bool IsCancelled { get; set; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CopyMoves(Move[] pvTable, int target, int source, int moveCountToCopy)
        {
            if (pvTable[source].EncodedMove is default(int))
            {
                Array.Clear(pvTable, target, pvTable.Length - target);  // TODO remove?
                return;
            }

            //PrintPvTable(pvTable, target, source);
            Array.Copy(pvTable, source, pvTable, target, moveCountToCopy);
            //PrintPvTable(pvTable);
        }

        [Conditional("DEBUG")]
        private static void PrintPreMove(Position position, int plies, Move move, bool isQuiescence = false)
        {
            var sb = new StringBuilder();
            for (int i = 0; i <= plies; ++i)
            {
                sb.Append("\t\t");
            }
            string depthStr = sb.ToString();

            //if (plies < Configuration.Parameters.Depth - 1)
            {
                //Console.WriteLine($"{Environment.NewLine}{depthStr}{move} ({position.Side}, {depth})");
                _logger.Trace($"{Environment.NewLine}{depthStr}{(isQuiescence ? "[Qui] " : "")}{move} ({position.Side}, {plies})");
            }
        }

        [Conditional("DEBUG")]
        private static void PrintMove(int plies, Move move, int evaluation, bool isQuiescence = false, bool prune = false)
        {
            var sb = new StringBuilder();
            for (int i = 0; i <= plies; ++i)
            {
                sb.Append("\t\t");
            }
            string depthStr = sb.ToString();

            //Console.ForegroundColor = depth switch
            //{
            //    0 => ConsoleColor.Red,
            //    1 => ConsoleColor.Blue,
            //    2 => ConsoleColor.Green,
            //    3 => ConsoleColor.White,
            //    _ => ConsoleColor.White
            //};
            //Console.WriteLine($"{depthStr}{move} ({position.Side}, {depthLeft}) | {evaluation}");
            //Console.WriteLine($"{depthStr}{move} | {evaluation}");

            _logger.Trace($"{depthStr}{(isQuiescence ? "[Qui] " : "")}{move,-6} | {evaluation}{(prune ? " | prunning" : "")}");

            //Console.ResetColor();
        }

        [Conditional("DEBUG")]
        private static void PrintMessage(int plies, string message)
        {
            var sb = new StringBuilder();
            for (int i = 0; i <= plies; ++i)
            {
                sb.Append("\t\t");
            }
            string depthStr = sb.ToString();

            _logger.Trace(depthStr + message);
        }

        [Conditional("DEBUG")]
        private static void PrintPvTable(Move[] pvTable, int target = -1, int source = -1)
        {
            Console.WriteLine(
(target != -1 ? $"src: {source}, tgt: {target}" + Environment.NewLine : "") +
$" {0,-3} {pvTable[0],-6} {pvTable[1],-6}" + $" {pvTable[2],-6} {pvTable[3],-6}" + $" {pvTable[4],-6} {pvTable[5],-6}" + $" {pvTable[6],-6} {pvTable[7],-6}" + Environment.NewLine +
$" {32,-3}        {pvTable[32],-6} {pvTable[33],-6}" + $" {pvTable[34],-6} {pvTable[35],-6}" + $" {pvTable[36],-6} {pvTable[37],-6}" + $" {pvTable[38],-6}" + Environment.NewLine +
$" {63,-3}               {pvTable[63],-6} {pvTable[64],-6}" + $" {pvTable[65],-6} {pvTable[66],-6}" + $" {pvTable[67],-6} {pvTable[68],-6}" + Environment.NewLine +
$" {93,-3}                      {pvTable[93],-6} {pvTable[94],-6}" + $" {pvTable[95],-6} {pvTable[96],-6}" + $" {pvTable[97],-6}" + Environment.NewLine +
$" {122,-3}                             {pvTable[122],-6} {pvTable[123],-6}" + $" {pvTable[124],-6} {pvTable[125],-6}" + Environment.NewLine +
$" {150,-3}                                    {pvTable[150],-6} {pvTable[151],-6}" + $" {pvTable[152],-6}" + Environment.NewLine +
(target == -1 ? "------------------------------------------------------------------------------------" + Environment.NewLine : ""));
        }
    }
}

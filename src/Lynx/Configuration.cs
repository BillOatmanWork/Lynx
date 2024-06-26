﻿using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Lynx;

public static class Configuration
{
    public static EngineSettings EngineSettings { get; set; } = new EngineSettings();
    public static GeneralSettings GeneralSettings { get; set; } = new GeneralSettings();

    private static int _isDebug = 0;
#pragma warning disable IDE1006 // Naming Styles
    private static int _UCI_AnalyseMode = 0;
#pragma warning restore IDE1006 // Naming Styles
    private static int _ponder = 0;

    public static bool IsDebug
    {
        get => Interlocked.CompareExchange(ref _isDebug, 1, 1) == 1;
        set
        {
            if (value)
            {
                Interlocked.CompareExchange(ref _isDebug, 1, 0);
            }
            else
            {
                Interlocked.CompareExchange(ref _isDebug, 0, 1);
            }
        }
    }

    public static bool UCI_AnalyseMode
    {
        get => Interlocked.CompareExchange(ref _UCI_AnalyseMode, 1, 1) == 1;
        set
        {
            if (value)
            {
                Interlocked.CompareExchange(ref _UCI_AnalyseMode, 1, 0);
            }
            else
            {
                Interlocked.CompareExchange(ref _UCI_AnalyseMode, 0, 1);
            }
        }
    }

    public static bool IsPonder
    {
        get => Interlocked.CompareExchange(ref _ponder, 1, 1) == 1;
        set
        {
            if (value)
            {
                Interlocked.CompareExchange(ref _ponder, 1, 0);
            }
            else
            {
                Interlocked.CompareExchange(ref _ponder, 0, 1);
            }
        }
    }

    public static int Hash
    {
        get => EngineSettings.TranspositionTableSize;
        set => EngineSettings.TranspositionTableSize = value;
    }
}

public sealed class GeneralSettings
{
    public bool EnableLogging { get; set; } = false;
}

public sealed class EngineSettings
{
    private int _maxDepth = 128;
    public int MaxDepth { get => _maxDepth; set => _maxDepth = Math.Clamp(value, 1, Constants.AbsoluteMaxDepth); }

    /// <summary>
    /// Depth for bench command
    /// </summary>
    public int BenchDepth { get; set; } = 8;

    /// <summary>
    /// MB
    /// </summary>
    public int TranspositionTableSize { get; set; } = 256;

    public bool UseOnlineTablebaseInRootPositions { get; set; } = false;

    /// <summary>
    /// Experimental, might misbehave due to tablebase API limits
    /// </summary>
    public bool UseOnlineTablebaseInSearch { get; set; } = false;

    /// <summary>
    /// This can also de used to reduce online probing
    /// </summary>
    public int OnlineTablebaseMaxSupportedPieces { get; set; } = 7;

    public bool ShowWDL { get; set; } = false;

    #region Time management

    public double HardTimeBoundMultiplier { get; set; } = 0.52;

    public double SoftTimeBoundMultiplier { get; set; } = 1;

    public int DefaultMovesToGo { get; set; } = 45;

    public double SoftTimeBaseIncrementMultiplier { get; set; } = 0.8;

    #endregion

    public int LMR_MinDepth { get; set; } = 3;

    public int LMR_MinFullDepthSearchedMoves { get; set; } = 4;

    /// <summary>
    /// Value originally from Stormphrax, who apparently took it from Viridithas
    /// </summary>
    public double LMR_Base { get; set; } = 0.85;

    /// <summary>
    /// Value originally from Akimbo
    /// </summary>
    public double LMR_Divisor { get; set; } = 2.84;

    public int NMP_MinDepth { get; set; } = 3;

    public int NMP_BaseDepthReduction { get; set; } = 1;

    public int AspirationWindow_Delta { get; set; } = 20;

    public int AspirationWindow_MinDepth { get; set; } = 7;

    public int RFP_MaxDepth { get; set; } = 4;

    public int RFP_DepthScalingFactor { get; set; } = 87;

    public int Razoring_MaxDepth { get; set; } = 3;

    public int Razoring_Depth1Bonus { get; set; } = 105;

    public int Razoring_NotDepth1Bonus { get; set; } = 161;

    public int IIR_MinDepth { get; set; } = 2;

    public int LMP_MaxDepth { get; set; } = 2;

    public int LMP_BaseMovesToTry { get; set; } = 0;

    public int LMP_MovesDepthMultiplier { get; set; } = 10;

    public int History_MaxMoveValue { get; set; } = 8_192;

    /// <summary>
    /// 1896: constant from depth 12
    /// </summary>
    public int History_MaxMoveRawBonus { get; set; } = 1_896;

    public int SEE_BadCaptureReduction { get; set; } = 1;

    #region Evaluation

    public TaperedEvaluationTerm DoubledPawnPenalty { get; set; } = new(-6, -12);

    public TaperedEvaluationTerm IsolatedPawnPenalty { get; set; } = new(-17, -13);

    public TaperedEvaluationTerm OpenFileRookBonus { get; set; } = new(47, 10);

    public TaperedEvaluationTerm SemiOpenFileRookBonus { get; set; } = new(18, 17);

    public TaperedEvaluationTerm BishopMobilityBonus { get; set; } = new(10, 9);

    public TaperedEvaluationTerm RookMobilityBonus { get; set; } = new(5, 5);

    public TaperedEvaluationTerm QueenMobilityBonus { get; set; } = new(4, 7);

    public TaperedEvaluationTerm SemiOpenFileKingPenalty { get; set; } = new(-36, 24);

    public TaperedEvaluationTerm OpenFileKingPenalty { get; set; } = new(-105, 8);

    public TaperedEvaluationTerm KingShieldBonus { get; set; } = new(16, -6);

    public TaperedEvaluationTerm BishopPairBonus { get; set; } = new(31, 80);

    public TaperedEvaluationTermByRank PassedPawnBonus { get; set; } = new(
        new(0, 0),
        new(-2, 7),
        new(-15, 13),
        new(-14, 41),
        new(20, 74),
        new(60, 150),
        new(98, 217),
        new(0, 0));

    #endregion
}

public sealed class TaperedEvaluationTerm
{
    [JsonIgnore]
    public int PackedEvaluation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;

        [Obsolete("Test only")]
        private set;
    }

    public int MG
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return Utils.UnpackMG(PackedEvaluation);
        }
        [Obsolete("Test only, will reset internal value")]
        set
        {
            PackedEvaluation = value;
        }
    }

    public int EG
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return Utils.UnpackEG(PackedEvaluation);
        }
        [Obsolete("Test only")]
        set
        {
            PackedEvaluation += (value << 16);
        }
    }

    public TaperedEvaluationTerm(int mg, int eg)
    {
#pragma warning disable CS0618 // Type or member is obsolete - correct usage here, setter wouldn't even be needed
        PackedEvaluation = Utils.Pack((short)mg, (short)eg);
#pragma warning restore CS0618 // Type or member is obsolete
    }

    public override string ToString()
    {
        return $"{{\"MG\":{MG},\"EG\":{EG}}}";
    }
}

public sealed class TaperedEvaluationTermByRank
{
    private readonly List<TaperedEvaluationTerm> _evaluationTermsIndexedByPiece;

    public TaperedEvaluationTerm Rank0 { get; set; }
    public TaperedEvaluationTerm Rank1 { get; set; }
    public TaperedEvaluationTerm Rank2 { get; set; }
    public TaperedEvaluationTerm Rank3 { get; set; }
    public TaperedEvaluationTerm Rank4 { get; set; }
    public TaperedEvaluationTerm Rank5 { get; set; }
    public TaperedEvaluationTerm Rank6 { get; set; }
    public TaperedEvaluationTerm Rank7 { get; set; }

    public TaperedEvaluationTermByRank(
        TaperedEvaluationTerm rank0, TaperedEvaluationTerm rank1, TaperedEvaluationTerm rank2,
        TaperedEvaluationTerm rank3, TaperedEvaluationTerm rank4, TaperedEvaluationTerm rank5,
        TaperedEvaluationTerm rank6, TaperedEvaluationTerm rank7)
    {
        Rank0 = rank0;
        Rank1 = rank1;
        Rank2 = rank2;
        Rank3 = rank3;
        Rank4 = rank4;
        Rank5 = rank5;
        Rank6 = rank6;
        Rank7 = rank7;

        _evaluationTermsIndexedByPiece = [rank0, rank1, rank2, rank3, rank4, rank5, rank6, rank7];
    }

    public TaperedEvaluationTerm this[int i]
    {
        get { return _evaluationTermsIndexedByPiece[i]; }
    }

    public override string ToString()
    {
        return "{" +
            $"\"{nameof(Rank0)}\":{Rank0}," +
            $"\"{nameof(Rank1)}\":{Rank1}," +
            $"\"{nameof(Rank2)}\":{Rank2}," +
            $"\"{nameof(Rank3)}\":{Rank3}," +
            $"\"{nameof(Rank4)}\":{Rank4}," +
            $"\"{nameof(Rank5)}\":{Rank5}," +
            $"\"{nameof(Rank6)}\":{Rank6}," +
            $"\"{nameof(Rank7)}\":{Rank7}" +
            "}";
    }
}

[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Default, WriteIndented = true)] // https://github.com/dotnet/runtime/issues/78602#issuecomment-1322004254
[JsonSerializable(typeof(EngineSettings))]
[JsonSerializable(typeof(TaperedEvaluationTerm))]
[JsonSerializable(typeof(TaperedEvaluationTermByRank))]
internal partial class EngineSettingsJsonSerializerContext : JsonSerializerContext
{
}

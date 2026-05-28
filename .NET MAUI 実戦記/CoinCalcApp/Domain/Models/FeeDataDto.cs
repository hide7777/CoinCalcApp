namespace CoinCalcApp.Domain.Models;

/// <summary>
/// fees.json のルートオブジェクトに対応する DTO。
/// JSON デシリアライゼーション専用。ドメイン層では FeeDataParser 経由で
/// BankFeeRule のリストに変換してから利用する。
/// </summary>
public sealed class FeeDataDto
{
    public int SchemaVersion { get; init; }
    public string Version { get; init; } = "";
    public string LastUpdated { get; init; } = "";
    public string? Source { get; init; }
    public List<FeeRuleDto> Rules { get; init; } = new();
}

/// <summary>
/// 1 つの手数料ルールに対応する平坦 DTO。
/// Type フィールドの値で具象クラスへ分岐する。
/// 使われないフィールドは null のまま無視される。
/// </summary>
public sealed class FeeRuleDto
{
    /// <summary>"Free" | "TieredCounter" | "YuchoAtm"</summary>
    public string Type { get; init; } = "";

    public string BankId { get; init; } = "";
    public string DisplayName { get; init; } = "";
    public string Channel { get; init; } = "";

    public int? MaxCoinsPerTransaction { get; init; }
    public int? MaxFiveHundredYenPerTransaction { get; init; }

    // TieredCounter / YuchoAtm 用
    public List<FeeTierDto>? Tiers { get; init; }
    public List<FeeTierDto>? RemainderTiers { get; init; }
    public int? BatchSize { get; init; }
    public int? BatchFee { get; init; }
}

public sealed class FeeTierDto
{
    public int MaxCoins { get; init; }
    public int Fee { get; init; }
}

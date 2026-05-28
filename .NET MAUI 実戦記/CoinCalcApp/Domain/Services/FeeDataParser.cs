using System.Text.Json;
using CoinCalcApp.Domain.Models;
using CoinCalcApp.Domain.Rules;

namespace CoinCalcApp.Domain.Services;

/// <summary>
/// JSON 文字列 → BankFeeRule のリストへの変換を担当。
/// DTO のディスクリミネータ(Type フィールド)で具象クラスへ分岐する。
/// </summary>
public static class FeeDataParser
{
    /// <summary>
    /// JSON 文字列をパースして BankFeeRule のリストを返す。
    /// パース失敗・スキーマ不一致時は FormatException を投げる。
    /// JSON Source Generator(FeeDataJsonContext)を使うことで AOT/トリミング対応。
    /// </summary>
    public static IReadOnlyList<BankFeeRule> Parse(string json)
    {
        FeeDataDto? data;
        try
        {
            data = JsonSerializer.Deserialize(json, FeeDataJsonContext.Default.FeeDataDto);
        }
        catch (JsonException ex)
        {
            throw new FormatException("fees.json の JSON 形式が不正です。", ex);
        }

        if (data is null)
            throw new FormatException("fees.json が空または null として読み込まれました。");

        if (data.SchemaVersion != 1)
            throw new FormatException($"未対応の schemaVersion: {data.SchemaVersion}");

        var rules = new List<BankFeeRule>(data.Rules.Count);
        foreach (var dto in data.Rules)
        {
            rules.Add(ConvertRule(dto));
        }
        return rules;
    }

    private static BankFeeRule ConvertRule(FeeRuleDto dto)
    {
        switch (dto.Type)
        {
            case "Free":
                return new FreeFeeRule(dto.BankId, dto.DisplayName, dto.Channel)
                {
                    MaxCoinsPerTransaction = dto.MaxCoinsPerTransaction,
                    MaxFiveHundredYenPerTransaction = dto.MaxFiveHundredYenPerTransaction,
                };

            case "TieredCounter":
            {
                var tiers = (dto.Tiers ?? new List<FeeTierDto>())
                    .Select(t => new FeeTier(t.MaxCoins, t.Fee))
                    .ToList();
                return new TieredCounterFeeRule(
                    dto.BankId, dto.DisplayName, dto.Channel,
                    Tiers: tiers,
                    BatchSize: dto.BatchSize ?? throw new FormatException(
                        $"TieredCounter ルールに BatchSize が必要です ({dto.BankId}/{dto.Channel})"),
                    BatchFee: dto.BatchFee ?? throw new FormatException(
                        $"TieredCounter ルールに BatchFee が必要です ({dto.BankId}/{dto.Channel})"))
                {
                    MaxCoinsPerTransaction = dto.MaxCoinsPerTransaction,
                    MaxFiveHundredYenPerTransaction = dto.MaxFiveHundredYenPerTransaction,
                };
            }

            case "YuchoAtm":
            {
                var rem = (dto.RemainderTiers ?? new List<FeeTierDto>())
                    .Select(t => new FeeTier(t.MaxCoins, t.Fee))
                    .ToList();
                return new YuchoAtmFeeRule(
                    dto.BankId, dto.DisplayName, dto.Channel,
                    BatchSize: dto.BatchSize ?? throw new FormatException(
                        $"YuchoAtm ルールに BatchSize が必要です ({dto.BankId}/{dto.Channel})"),
                    BatchFee: dto.BatchFee ?? throw new FormatException(
                        $"YuchoAtm ルールに BatchFee が必要です ({dto.BankId}/{dto.Channel})"),
                    RemainderTiers: rem)
                {
                    MaxCoinsPerTransaction = dto.MaxCoinsPerTransaction,
                    MaxFiveHundredYenPerTransaction = dto.MaxFiveHundredYenPerTransaction,
                };
            }

            default:
                throw new FormatException($"未対応の手数料ルール Type: {dto.Type}");
        }
    }
}

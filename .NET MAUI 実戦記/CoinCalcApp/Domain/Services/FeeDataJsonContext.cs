using System.Text.Json;
using System.Text.Json.Serialization;
using CoinCalcApp.Domain.Models;

namespace CoinCalcApp.Domain.Services;

/// <summary>
/// fees.json のデシリアライズ用 JsonSerializerContext。
/// コンパイル時に Source Generator が必要な型のシリアライズコードを生成するため、
/// リフレクションに依存せず AOT/トリミング環境でも安全に動作する。
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    ReadCommentHandling = JsonCommentHandling.Skip,
    AllowTrailingCommas = true)]
[JsonSerializable(typeof(FeeDataDto))]
[JsonSerializable(typeof(FeeRuleDto))]
[JsonSerializable(typeof(FeeTierDto))]
internal partial class FeeDataJsonContext : JsonSerializerContext
{
}

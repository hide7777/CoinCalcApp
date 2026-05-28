namespace CoinCalcApp.Domain.Services;

/// <summary>
/// アプリ同梱の Resources/Raw/fees.json を読み込むデータソース。
/// MAUI の FileSystem.OpenAppPackageFileAsync を利用。
///
/// ConfigureAwait(false) を付け、ライブラリ層として UI スレッドに継続を戻さない。
/// (呼び出し側は更に Task.Run でラップしているのでデッドロック対策は二重。)
/// </summary>
public sealed class BundledFeeDataSource : IFeeDataSource
{
    private const string AssetFileName = "fees.json";

    public async Task<string> LoadJsonAsync(CancellationToken cancellationToken = default)
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync(AssetFileName)
            .ConfigureAwait(false);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
    }
}

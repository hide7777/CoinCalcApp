using CoinCalcApp.Domain.Rules;

namespace CoinCalcApp.Domain.Services;

/// <summary>
/// IFeeDataSource から取得した JSON を Parser でドメインオブジェクトに変換し
/// 結果をキャッシュして提供するリポジトリ。
///
/// 重要:UI スレッドからのデッドロックを防ぐため、ロードは Task.Run でスレッドプール上に
/// オフロードしている。コンストラクタで eager 開始するため、典型的なフローでは
/// GetAll() が呼ばれる時点で既に完了している。
/// </summary>
public sealed class JsonBankFeeRepository : IBankFeeRepository
{
    private readonly Task<IReadOnlyList<BankFeeRule>> _loadingTask;

    public JsonBankFeeRepository(IFeeDataSource dataSource)
    {
        // バックグラウンドスレッド上で読み込みを開始(eager)。
        // Task.Run を使うことで内部の await が UI スレッドに戻ろうとせず、
        // GetAll() で .GetResult() を呼んでもデッドロックしない。
        _loadingTask = Task.Run(async () =>
        {
            try
            {
                var json = await dataSource.LoadJsonAsync().ConfigureAwait(false);
                return FeeDataParser.Parse(json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "手数料データ(fees.json)の読み込みに失敗しました。", ex);
            }
        });
    }

    public IReadOnlyList<BankFeeRule> GetAll()
    {
        // 通常は構築時のバックグラウンド読み込みが既に完了しているため即返る。
        // 万一未完了でも、タスクはスレッドプール上で UI スレッドと独立に進行するため
        // ここでの待機はデッドロックにならない。
        return _loadingTask.GetAwaiter().GetResult();
    }
}

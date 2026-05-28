namespace CoinCalcApp.Services.Ads;

/// <summary>
/// インタースティシャル広告の頻度キャップを管理する。
/// - 初回起動からの抑止期間 = なし(初回起動直後から表示)
/// - 1 日あたり最大 5 回まで(Google 推奨レンジ内)
///
/// 永続化は Preferences(端末固有のキーバリュー)を使う。
/// アプリ削除で初期化される。
/// </summary>
public sealed class InterstitialFrequencyGate
{
    private const string KeyFirstLaunch    = "interstitial.firstLaunchUtc";
    private const string KeyTodayDate      = "interstitial.todayDate";        // "yyyy-MM-dd"
    private const string KeyTodayCount     = "interstitial.todayCount";

    // 初回起動から表示を抑止する期間。0 にすると初回起動直後から表示される。
    private static readonly TimeSpan FirstLaunchQuietPeriod = TimeSpan.Zero;
    // 1 日あたりの上限回数。Google 推奨は 5 回まで。
    private const int MaxPerDay = 5;

    private readonly Func<DateTimeOffset> _now;
    private readonly IPreferences _prefs;

    public InterstitialFrequencyGate(IPreferences prefs, Func<DateTimeOffset>? nowProvider = null)
    {
        _prefs = prefs;
        _now = nowProvider ?? (() => DateTimeOffset.Now);

        // 初回起動日時を未記録なら記録
        if (string.IsNullOrEmpty(_prefs.Get(KeyFirstLaunch, "")))
        {
            _prefs.Set(KeyFirstLaunch, _now().ToString("O")); // ISO 8601 round-trip
        }
    }

    /// <summary>
    /// この呼び出し時点でインタースティシャルを「出して良い」か。
    /// true を返した時点では実際にはまだ表示されていない。
    /// 実際に表示が成立した時に MarkShown() を呼ぶこと。
    /// </summary>
    public bool CanShow()
    {
        // 初回起動から 24 時間は出さない
        var firstLaunchRaw = _prefs.Get(KeyFirstLaunch, "");
        if (DateTimeOffset.TryParse(firstLaunchRaw, out var firstLaunch))
        {
            if (_now() - firstLaunch < FirstLaunchQuietPeriod)
            {
                return false;
            }
        }

        // 1 日 N 回まで
        return TodayCount() < MaxPerDay;
    }

    /// <summary>
    /// 広告表示が成功した時に呼び、カウンタを進める。
    /// </summary>
    public void MarkShown()
    {
        var today = _now().Date.ToString("yyyy-MM-dd");
        var lastDate = _prefs.Get(KeyTodayDate, "");
        var count = lastDate == today ? _prefs.Get(KeyTodayCount, 0) : 0;
        count++;
        _prefs.Set(KeyTodayDate, today);
        _prefs.Set(KeyTodayCount, count);
    }

    private int TodayCount()
    {
        var today = _now().Date.ToString("yyyy-MM-dd");
        var lastDate = _prefs.Get(KeyTodayDate, "");
        return lastDate == today ? _prefs.Get(KeyTodayCount, 0) : 0;
    }
}

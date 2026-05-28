using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CoinCalcApp.Domain.Models;
using CoinCalcApp.Domain.Services;
using CoinCalcApp.Services.Ads;
using CoinCalcApp.Services.Dialog;
using CoinCalcApp.Services.Purchase;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace CoinCalcApp.ViewModels
{
    /// <summary>
    /// 計算結果ページの ViewModel。MAUI Shell ナビゲーション環境では
    /// Shell.Current.GoToAsync(route, parameters) で渡された辞書が
    /// IQueryAttributable.ApplyQueryAttributes に届く。
    /// </summary>
    public class CalcPageViewModel : ViewModelBase, IQueryAttributable, IDisposable
    {
        private readonly IDialogService _dialog;
        private readonly IFeeCalculator _feeCalculator;
        private readonly IPurchaseService _purchaseService;
        private readonly IAdService _adService;
        private readonly CompositeDisposable _disposables = new();

        /// <summary>
        /// コンストラクタ。サービスは DI 経由で受け取る。
        /// </summary>
        public CalcPageViewModel(
            IDialogService dialog,
            IFeeCalculator feeCalculator,
            IPurchaseService purchaseService,
            IAdService adService)
        {
            _dialog = dialog;
            _feeCalculator = feeCalculator;
            _purchaseService = purchaseService;
            _adService = adService;
            Title = "計算結果";

            // バナー表示可否
            ShouldShowBanner = _adService.ShouldShowBanner
                .ToReadOnlyReactivePropertySlim(initialValue: _adService.ShouldShowBanner.Value)
                .AddTo(_disposables);

            // 「広告を消す ¥160」ボタンの表示制御(課金未済時のみ表示)
            ShouldShowPurchaseButton = _purchaseService.IsAdFree
                .Select(isAdFree => !isAdFree)
                .ToReadOnlyReactivePropertySlim(initialValue: !_purchaseService.IsAdFree.Value)
                .AddTo(_disposables);

            PurchaseAdFreeCommand = new AsyncReactiveCommand()
                .WithSubscribe(PurchaseAdFreeAsync)
                .AddTo(_disposables);

            RestorePurchasesCommand = new AsyncReactiveCommand()
                .WithSubscribe(RestorePurchasesAsync)
                .AddTo(_disposables);
        }

        /// <summary>
        /// Shell ナビゲーション時にパラメータを受け取るためのコールバック。
        /// MAUI Shell が BindingContext から IQueryAttributable を検出して自動呼び出しする。
        /// (Prism の INavigatedAware.OnNavigatedTo の代替。)
        /// </summary>
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            try
            {
                var coins = new CoinSet(
                    One:         GetIntParam(query, "one"),
                    Five:        GetIntParam(query, "five"),
                    Ten:         GetIntParam(query, "ten"),
                    Fifty:       GetIntParam(query, "fifty"),
                    OneHundred:  GetIntParam(query, "oneHundred"),
                    FiveHundred: GetIntParam(query, "fiveHundred")
                );

                var result = _feeCalculator.Calculate(coins);

                Goukei = result.TotalAmount.ToString("N0");
                Maisu  = result.TotalCount.ToString("N0");

                BankRows.Clear();
                foreach (var fee in result.BankFees)
                {
                    BankRows.Add(new BankFeeRowVm
                    {
                        Label = $"{fee.DisplayName}{fee.Channel}",
                        ValueText = FormatFeeValue(fee),
                        IsImpractical = fee.RequiredTransactions > FeeCalculator.PracticalMaxTransactions,
                    });
                }

                CheapestRecommendation = BuildCheapestText(result);
                HasRecommendation = result.CheapestOption is not null;
            }
            catch (Exception e)
            {
                // 非同期だが、エラー時は fire-and-forget で十分(ApplyQueryAttributes は同期メソッド)
                _ = _dialog.DisplayAlertAsync("ERROR", e.ToString(), "OK");
                System.Diagnostics.Debug.WriteLine("ERROR:" + e.ToString());
            }
        }

        private async Task PurchaseAdFreeAsync()
        {
            var result = await _purchaseService.PurchaseAdFreeAsync();
            switch (result)
            {
                case PurchaseResult.Success:
                    await _dialog.DisplayAlertAsync(
                        "ありがとうございます", "広告を非表示にしました。", "OK");
                    break;
                case PurchaseResult.Canceled:
                    // 何も通知しない(ユーザー意思によるキャンセル)
                    break;
                case PurchaseResult.Failed:
                    await _dialog.DisplayAlertAsync(
                        "購入できませんでした",
                        "通信状況をご確認の上、しばらくしてからお試しください。", "OK");
                    break;
                case PurchaseResult.NothingToRestore:
                    // 購入処理側では起こらない想定
                    break;
            }
        }

        private async Task RestorePurchasesAsync()
        {
            var result = await _purchaseService.RestorePurchasesAsync();
            switch (result)
            {
                case PurchaseResult.Success:
                    await _dialog.DisplayAlertAsync(
                        "復元しました", "広告非表示版が復元されました。", "OK");
                    break;
                case PurchaseResult.NothingToRestore:
                    await _dialog.DisplayAlertAsync(
                        "復元する購入はありません",
                        "このアカウントでの過去の購入が見つかりませんでした。", "OK");
                    break;
                case PurchaseResult.Failed:
                    await _dialog.DisplayAlertAsync(
                        "復元できませんでした",
                        "通信状況をご確認の上、しばらくしてからお試しください。", "OK");
                    break;
            }
        }

        private static int GetIntParam(IDictionary<string, object> query, string key)
        {
            if (!query.TryGetValue(key, out var raw)) return 0;
            // Shell パラメータは object 型で渡される。string や int を含む可能性がある。
            return raw switch
            {
                int i => i,
                string s => int.TryParse(s, out var v) ? v : 0,
                _ => int.TryParse(raw?.ToString() ?? "0", out var v) ? v : 0,
            };
        }

        private static string FormatFeeValue(BankFee fee)
        {
            var amount = fee.Fee == 0 ? "無料" : $"¥{fee.Fee:N0}";
            if (fee.RequiredTransactions > 1)
            {
                return $"{amount}({fee.RequiredTransactions}回投入)";
            }
            return amount;
        }

        private static string BuildCheapestText(CalculationResult result)
        {
            if (result.CheapestOption is null) return string.Empty;
            var c = result.CheapestOption;
            var trips = c.RequiredTransactions > 1 ? $"({c.RequiredTransactions}回投入)" : "";
            return c.Fee == 0
                ? $"{c.DisplayName}{c.Channel}{trips} なら無料で入金できます"
                : $"{c.DisplayName}{c.Channel}(¥{c.Fee:N0}){trips}が最安です";
        }

        public void Dispose() => _disposables.Dispose();

        // ─────────── 計算結果バインドプロパティ ───────────

        private string _goukei = "";
        public string Goukei
        {
            get => _goukei;
            set => SetProperty(ref _goukei, value);
        }

        private string _maisu = "";
        public string Maisu
        {
            get => _maisu;
            set => SetProperty(ref _maisu, value);
        }

        public ObservableCollection<BankFeeRowVm> BankRows { get; } = new();

        private string _cheapestRecommendation = "";
        public string CheapestRecommendation
        {
            get => _cheapestRecommendation;
            set => SetProperty(ref _cheapestRecommendation, value);
        }

        private bool _hasRecommendation;
        public bool HasRecommendation
        {
            get => _hasRecommendation;
            set => SetProperty(ref _hasRecommendation, value);
        }

        // ─────────── 広告・課金関連バインドプロパティ ───────────

        public IReadOnlyReactiveProperty<bool> ShouldShowBanner { get; }
        public string BannerAdUnitId => AdConfig.BannerAdUnitId;
        public IReadOnlyReactiveProperty<bool> ShouldShowPurchaseButton { get; }
        public AsyncReactiveCommand PurchaseAdFreeCommand { get; }
        public AsyncReactiveCommand RestorePurchasesCommand { get; }
    }
}

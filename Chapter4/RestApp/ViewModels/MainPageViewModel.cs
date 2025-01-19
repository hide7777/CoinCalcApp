using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using RsstApp.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Reactive.Linq;
using System.Windows.Input;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace RestApp.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    ///<summary>
    ///ID(検索・削除用)
    ///</summary>
    [IntValidation(ErrorMessage = "半角数字")]
    [Range(1000, 9999, ErrorMessage = "1000以上9999以下")]
    public ReactiveProperty<string> Input1 { get; }
    public ReadOnlyReactiveProperty<string?> Input1Error { get; }

    ///<summary>
    ///氏名(あいまい検索用)
    ///</summary>
    public ReactiveProperty<string> Input2 { get; }
    public ReadOnlyReactiveProperty<string?> Input2Error { get; }

    ///<summary>
    ///Eメール(あいまい検索用)
    ///</summary>
    public ReactiveProperty<string> Input3 { get; }
    public ReadOnlyReactiveProperty<string?> Input3Error { get; }

    ///<summary>
    ///ID(登録・更新用)
    ///</summary>
    [Required(ErrorMessage = "必須項目")]
    [IntValidation(ErrorMessage = "半角数字")]
    [Range(1000, 9999, ErrorMessage = "1000以上9999以下")]
    public ReactiveProperty<string> Text1 { get; }
    public ReadOnlyReactiveProperty<string?> Text1Error { get; }

    ///<summary>
    ///氏名(登録・更新用)
    ///</summary>
    [Required(ErrorMessage = "必須項目")]
    public ReactiveProperty<string> Text2 { get; }
    public ReadOnlyReactiveProperty<string?> Text2Error { get; }

    ///<summary>
    ///Eメール(登録・更新用)
    ///</summary>
    [Required(ErrorMessage = "必須項目")]
    public ReactiveProperty<string> Text3 { get; }
    public ReadOnlyReactiveProperty<string?> Text3Error { get; }

    ///<summary>
    ///開始日(年)
    ///</summary>
    [Required(ErrorMessage = "必須項目")]
    [IntValidation(ErrorMessage = "半角数字")]
    [Range(2000, 10000, ErrorMessage = "2000以上")]
    public ReactiveProperty<string> Text4 { get; }
    public ReadOnlyReactiveProperty<string?> Text4Error { get; }

    ///<summary>
    ///開始日(月)
    ///</summary>
    [Required(ErrorMessage = "必須項目")]
    [IntValidation(ErrorMessage = "半角数字")]
    [Range(1, 12, ErrorMessage = "1以上12以下")]
    public ReactiveProperty<string> Text5 { get; }
    public ReadOnlyReactiveProperty<string?> Text5Error { get; }

    ///<summary>
    ///開始日(日)
    ///</summary>
    [Required(ErrorMessage = "必須項目")]
    [IntValidation(ErrorMessage = "半角数字")]
    [Range(1, 31, ErrorMessage = "1以上31以下")]
    public ReactiveProperty<string> Text6 { get; }
    public ReadOnlyReactiveProperty<string?> Text6Error { get; }

    ///<summary>
    ///職能コード
    ///</summary>
    [Required(ErrorMessage = "必須項目")]
    [IntValidation(ErrorMessage = "半角数字")]
    [Range(0, 10, ErrorMessage = "0以上9以下")]
    public ReactiveProperty<string> Text7 { get; }
    public ReadOnlyReactiveProperty<string?> Text7Error { get; }

    ///<summary>
    ///Listの高さ
    ///</summary>
    public string ListHeight { get; set; }

    ///<summary>
    ///Listの幅
    ///</summary>
    public string ListWidth { get; set; }

    ///<summary>
    ///リスト」ビューの値
    ///</summary>    
    public ObservableCollection<RestData> Items { get; set; } = [];

    public MainPageViewModel(IPageDialogService pageDialogService, INavigationService navigationService)
            : base(pageDialogService, navigationService)
    {
        //バリデーション属性を設定
        Input1 = new ReactiveProperty<string>().SetValidateAttribute(() => this.Input1);
        Input2 = new ReactiveProperty<string>().SetValidateAttribute(() => this.Input2);
        Input3 = new ReactiveProperty<string>().SetValidateAttribute(() => this.Input3);

        Text1 = new ReactiveProperty<string>().SetValidateAttribute(() => this.Text1);
        Text2 = new ReactiveProperty<string>().SetValidateAttribute(() => this.Text2);
        Text3 = new ReactiveProperty<string>().SetValidateAttribute(() => this.Text3);
        Text4 = new ReactiveProperty<string>().SetValidateAttribute(() => this.Text4);
        Text5 = new ReactiveProperty<string>().SetValidateAttribute(() => this.Text5);
        Text6 = new ReactiveProperty<string>().SetValidateAttribute(() => this.Text6);
        Text7 = new ReactiveProperty<string>().SetValidateAttribute(() => this.Text7);

        //初期値をセット
        Input1.Value = "";
        Input2.Value = "";
        Input3.Value = "";

        Text1.Value = "";
        Text2.Value = "";
        Text3.Value = "";
        Text4.Value = "";
        Text5.Value = "";
        Text6.Value = "";
        Text7.Value = "";

        //エラーテキストを定義
        Input1Error = this.Input1.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();
        Input2Error = this.Input2.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();
        Input3Error = this.Input3.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();

        Text1Error = this.Text1.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();
        Text2Error = this.Text2.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();
        Text3Error = this.Text3.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();
        Text4Error = this.Text4.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();
        Text5Error = this.Text5.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();
        Text6Error = this.Text6.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();
        Text7Error = this.Text7.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();

        //検索ボタンの定義
        ButtonCommand1 = new[]{
                    Input1.ObserveHasErrors,                 //バリデーションチェック
            }.CombineLatestValuesAreAllFalse()              // 全てエラーなしの場合に押せるようにする
         .ToAsyncReactiveCommand()
         .WithSubscribe(ButtonProcessing1); //ボタンが押された際の処理メソッドを登録

        //曖昧検索ボタンの定義
        ButtonCommand2 = new[]{
                    Input2.ObserveHasErrors,                 //バリデーションチェック
                    Input3.ObserveHasErrors,                 //バリデーションチェック
            }.CombineLatestValuesAreAllFalse()              // 全てエラーなしの場合に押せるようにする
          .ToAsyncReactiveCommand()
          .WithSubscribe(ButtonProcessing2); //ボタンが押された際の処理メソッドを登録

        //追加・更新ボタンの定義
        ButtonCommand3 = new[]{
                    Text1.ObserveHasErrors,                 //バリデーションチェック
                    Text2.ObserveHasErrors,                 //バリデーションチェック
                    Text3.ObserveHasErrors,                 //バリデーションチェック
                    Text4.ObserveHasErrors,                 //バリデーションチェック
                    Text5.ObserveHasErrors,                 //バリデーションチェック
                    Text6.ObserveHasErrors,                 //バリデーションチェック
                    Text7.ObserveHasErrors,                 //バリデーションチェック
            }.CombineLatestValuesAreAllFalse()              // 全てエラーなしの場合に押せるようにする
          .ToAsyncReactiveCommand()
          .WithSubscribe(ButtonProcessing3); //ボタンが押された際の処理メソッドを登録

        //削除ボタンの定義
        ButtonCommand4 = new[]{
                    Input1.ObserveHasErrors,                 //バリデーションチェック
            }.CombineLatestValuesAreAllFalse()              // 全てエラーなしの場合に押せるようにする
          .ToAsyncReactiveCommand()
          .WithSubscribe(ButtonProcessing4); //ボタンが押された際の処理メソッドを登録

        //Listを初期化
        Items.Clear();

        //OS毎にListの高さを設定
#if ANDROID
        int height = DisplayHeight();
        int ListHeightInt = (height - 660);
        ListHeight = ListHeightInt.ToString();
#else
        int height = DisplayHeight();
        int ListHeightInt = (height - 600);
        ListHeight = ListHeightInt.ToString();
#endif
        //Listの横幅を設定
        int width = DisplayWidth();
        ListWidth = width.ToString();

        //選択行を初期化
        SelectedItem = new RestData();
    }

    //検索
    ///<summary> 
    /// Binding変数を定義します。
    ///</summary>
    public AsyncReactiveCommand ButtonCommand1 { get; set; }
    ///<summary> 
    /// 検索ボタンの処理メソッドを定義します。
    ///</summary>
    public async Task ButtonProcessing1()
    {
        var result = await PageDialogService.DisplayAlertAsync("質問", "ID：" + Text1.Value + "を検索しますか？", "はい", "いいえ");
        if (result == true)
        {
            //検索実行
            System.Diagnostics.Debug.WriteLine("■検索-開始");

            try
            {
                //中身を消す
                Items.Clear();

                String resultString = DoRest.DoGet(Input1.Value);
                if (resultString != null)
                {
                    System.Diagnostics.Debug.WriteLine("JSON:" + resultString);

                    #pragma warning disable CS8600
                    RestData data = JsonSerializer.Deserialize<RestData>(resultString);

                    if (data != null)
                    {
                        //リストに追加する
                        Items.Add(data);
                    }
                }
                System.Diagnostics.Debug.WriteLine("■検索-終了");
            }
            catch (Exception e)
            {
                Console.WriteLine("■検索-例外：" + e.ToString());
                await PageDialogService.DisplayAlertAsync("エラー発生", "一致するデータがありません", "はい");
            }
        }
        else
        {
            //何もしない
        }
    }

    //曖昧検索
    ///<summary> 
    /// Binding変数を定義します。
    ///</summary>
    public AsyncReactiveCommand ButtonCommand2 { get; set; }
    ///<summary> 
    /// 曖昧検索ボタンの処理メソッドを定義します。
    ///</summary>
    public async Task ButtonProcessing2()
    {
        String msg = "";
        if (Input2.Value == "" & Input3.Value == "") {
            msg = "全件検索を実行します。検索に時間が掛かっても、よろしいですか？";
        }else if (Input2.Value != "" & Input3.Value != "") {
            msg = "氏名：" + Input2.Value + "と、Eメール：" + Input3.Value + "で曖昧検索しますが、よろしいですか？";
        } else if (Input2.Value != "") {
            msg = "氏名：" + Input2.Value + "を曖昧検索します。よろしいですか？";
        }else if (Text3.Value != "")  {
            msg = "Eメール：" + Input3.Value + "を曖昧検索します。よろしいですか？";
        }
        var result = await PageDialogService.DisplayAlertAsync("質問", msg, "はい", "いいえ");
        if (result == true)
        {
            //曖昧検索実行
            System.Diagnostics.Debug.WriteLine("■曖昧検索-開始");

            try
            {
                //ResultText.Value = "";
                string resultString = DoRest.DoGet(Input2.Value, Input3.Value);
                if (!string.IsNullOrWhiteSpace(resultString)) {
                    System.Diagnostics.Debug.WriteLine("JSON:" + resultString);

                    //中身を消す
                    Items.Clear();

                    #pragma warning disable CS8600
                    List<RestData> data = JsonSerializer.Deserialize<List<RestData>>(resultString);

                    data?.ForEach(RestData =>
                    {
                        Console.Write(RestData.Id + " ");
                        Console.Write(RestData.Name + " ");
                        Console.Write(RestData.Email + " ");
                        Console.Write(RestData.Startdate + " ");
                        Console.WriteLine(RestData.Lank);

                        //リストに追加する
                        Items.Add(RestData);
                    });
                }
                System.Diagnostics.Debug.WriteLine("■曖昧検索-終了");
            }
            catch (Exception e)
            {
                Console.WriteLine("■曖昧検索-例外：" + e.ToString());
                await PageDialogService.DisplayAlertAsync("エラー発生", e.ToString(), "はい");
            }
        }
        else
        {
            //何もしない
        }
    }

    //追加・更新
    ///<summary> 
    /// Binding変数を定義します。
    ///</summary>
    public AsyncReactiveCommand ButtonCommand3 { get; set; }
    ///<summary> 
    /// 追加・変更ボタンの処理メソッドを定義します。
    ///</summary>
    public async Task ButtonProcessing3()
    {
        String msg = "";
        if (Text2.Value != "" & Text3.Value != "")
        {
            msg = "";
        }
        else if (Text2.Value == "" & Text3.Value == "")
        {
            msg = "氏名とEメールが空です。入力して下さい。";
        }
        else if (Text2.Value != "")
        {
            msg = "Eメールが空です。入力して下さい。";
        }
        else if (Text3.Value != "")
        {
            msg = "氏名が空です。入力して下さい。";
        }

        if (msg == "")
        {
            var result = await PageDialogService.DisplayAlertAsync("質問", "[追加・更新]を実行しますか？", "はい", "いいえ");
            if (result == true)
            {
                //追加・更新実行
                System.Diagnostics.Debug.WriteLine("■追加・更新-開始");

                try
                {
                    RestData data = new()
                    {
                        Id = Int32.Parse(Text1.Value),
                        Name = Text2.Value,
                        Email = Text3.Value,
                        Startdate = Text4.Value + "-" + Text5.Value.PadLeft(2, '0') + "-" + Text6.Value.PadLeft(2, '0'),
                        Lank = Int32.Parse(Text7.Value)
                    };
                    String json = JsonSerializer.Serialize(data);
                    Console.WriteLine("登録データ：" + json);

                    //中身を消す
                    //Items.Clear();

                    DoRest.DoPost(json);
                    System.Diagnostics.Debug.WriteLine("■追加・更新-終了");
                }
                catch (Exception e)
                {
                    Console.WriteLine("■追加・更新-例外：" + e.ToString());
                    await PageDialogService.DisplayAlertAsync("エラー発生", e.ToString(), "はい");
                }
            }
            else
            {
                //何もしない
            }
        }
        else
        {
            await PageDialogService.DisplayAlertAsync("エラー発生", msg, "はい");
        }

    }

    //削除
    ///<summary> 
    /// Binding変数を定義します。
    ///</summary>
    public AsyncReactiveCommand ButtonCommand4 { get; set; }
    ///<summary> 
    /// 削除ボタンの処理メソッドを定義します。
    ///</summary>
    public async Task ButtonProcessing4()
    {
        var result = await PageDialogService.DisplayAlertAsync("質問", "ID：" + Text1.Value + "を削除しますか？", "はい", "いいえ");
        if (result == true)
        {
            //追加・更新実行
            System.Diagnostics.Debug.WriteLine("■削除-開始");

            try
            {
                //中身を消す
                //Items.Clear();

                DoRest.DoDelete(Input1.Value);
                System.Diagnostics.Debug.WriteLine("■削除-終了");
            }
            catch (Exception e)
            {
                Console.WriteLine("■削除-例外：" + e.ToString());
                await PageDialogService.DisplayAlertAsync("エラー発生", e.ToString(), "はい");
            }
        }
        else
        {
            //何もしない
        }
    }

    ///<summary> 
    /// ディスプレイの高さを取得します。
    ///</summary>
    private static int DisplayHeight()
    {
        string heightString = $"{DeviceDisplay.Current.MainDisplayInfo.Height}";
        string densityString = $"{DeviceDisplay.Current.MainDisplayInfo.Density}";

        int height   = Int32.Parse(heightString);
        float density = float.Parse(densityString);
        int maxHeight = (int)(height / density);
        return maxHeight;
    }

    ///<summary> 
    /// ディスプレイの幅を取得します。
    ///</summary>
    private static int DisplayWidth()
    {
        string widhString = $"{DeviceDisplay.Current.MainDisplayInfo.Width}";
        string densityString = $"{DeviceDisplay.Current.MainDisplayInfo.Density}";

        int width = Int32.Parse(widhString);
        float density = float.Parse(densityString);
        int maxWidth = (int)(width / density);
        return maxWidth;
    }

    ///<summary> 
    /// 選択した行の内容を格納します。
    ///</summary>
    public RestData SelectedItem { get; set; }

    ///<summary> 
    /// 行を選択押下した場合に、内容をText1～Text7とInput1に格納します。
    ///</summary>
    public ICommand CollectionViewSelectedCommand => new Command<Object>((Object e) =>
    {
        Console.WriteLine("■CollectionViewSelected");
        Console.WriteLine("■selected ID:" + SelectedItem.Id);
        Console.WriteLine("■selected Name:" + SelectedItem.Name);
        Console.WriteLine("■selected Email:" + SelectedItem.Email);
        Console.WriteLine("■selected StartDate:" + SelectedItem.Startdate);
        Console.WriteLine("■selected Lank:" + SelectedItem.Lank);

        Text1.Value = SelectedItem.Id.ToString();
        if (SelectedItem.Name is null) { Text2.Value = ""; } else { Text2.Value = SelectedItem.Name; }
        if (SelectedItem.Email is null) { Text3.Value = ""; } else { Text3.Value = SelectedItem.Email; }
        if (SelectedItem.Startdate is null) 
        {
            Text4.Value = "";
            Text5.Value = "";
            Text6.Value = "";
        }
        else {
            DateTime parsedDate = DateTime.Parse(SelectedItem.Startdate);
            Text4.Value = parsedDate.ToString("yyyy");
            Text5.Value = parsedDate.ToString("MM");
            Text6.Value = parsedDate.ToString("dd");
        }
        Text7.Value = SelectedItem.Lank.ToString();

        Input1.Value = SelectedItem.Id.ToString();

    });
}

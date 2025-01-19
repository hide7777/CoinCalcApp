using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RestApp.ViewModels
{
    class DoRest
    {
        //条件付きコンパイルで切り替え
#if ANDROID
        //URLを定義
        //WindowsのAndroidエミュレータはlocalhost(127.0.0.1)だとエラーになるので、代わりに10.0.2.2を定義した。
        private static readonly string BaseUrl = "http://10.0.2.2:8081/api/v1/users";
#else
        //URLを定義
        //Mac上のシミュレータはlocalhost(127.0.0.1)もlocalhost(10.0.2.2)もエラーになるので、Mac上のhostsにVisual Studoを動かしているWindows PC(test)のIPを定義した。
        private static readonly string BaseUrl = "http://test:8081/api/v1/users";
#endif

        /// <summary>
        /// 検索
        /// </summary>
        /// <param name="id"></param>
        /// <returns>json</returns>
        public static string DoGet(String id)
        {
            //引数チェック
            string url;
            if (!string.IsNullOrWhiteSpace(id))
            {
                //1件検索
                url = BaseUrl + "/" + id;
            }
            else
            {
                //全件検索
                url = BaseUrl;
            }

            //----------------------------------------------------------------
            // 登録結果の確認/GETを実行
            //----------------------------------------------------------------
            Console.WriteLine("【GET】:" + url);

            // Requestを作成
            RestRequest request = new(url, Method.Get);

            //通信実行
            var response = DoRequest(request, url,"GET");

            // レスポンスボディ
            var content = response.Content;

            if (content == null) return "";
            return content;
        }

        /// <summary>
        /// 曖昧検索
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <returns>json</returns>
        public static string DoGet(String name,String email)
        {
            string url = BaseUrl + "/search";
            string _name = name;
            string _email = email;

            //引数チェック
            if (string.IsNullOrWhiteSpace(name))
            {
                _name = "";
            }

            //引数チェック
            if (string.IsNullOrWhiteSpace(email))
            {
                _email = "";
            }

            //----------------------------------------------------------------
            // 登録結果の確認/GETを実行
            //----------------------------------------------------------------
            Console.WriteLine("【GET】:" + url);

            // RestRequestを作成
            RestRequest request = new(url, Method.Get);

            // クエリ追加
            request.AddParameter("Name", _name, ParameterType.QueryString);
            request.AddParameter("Email", _email, ParameterType.QueryString);

            //通信実行
            var response = DoRequest(request, url,"GET");

            // レスポンスボディ
            var content = response.Content;

            if (content == null) return "";
            return content;
        }

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="Exception"></exception>
        public static void DoDelete(String id)
        {
            //引数チェック
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new Exception("IDが指定されていません");
            }

            //1件削除
            string url;
            url = BaseUrl + "/" + id;

            //----------------------------------------------------------------
            // 登録結果の確認/GETを実行
            //----------------------------------------------------------------
            Console.WriteLine("【DELETE】:" + url);

            // RestRequestを作成
            RestRequest request = new(url, Method.Delete);

            //通信実行
            DoRequest(request, url,"DELETE");
        }

        /// <summary>
        /// 登録・更新
        /// </summary>
        /// <param name="json"></param>
        /// <exception cref="Exception"></exception>
        public static void DoPost(String json)
        {
            //引数チェック
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new Exception("JSONが指定されていません");
            }

            //1件削除
            string url;
            url = BaseUrl;

            //----------------------------------------------------------------
            // 登録結果の確認/GETを実行
            //----------------------------------------------------------------
            Console.WriteLine("【POST】:" + url);
            Console.WriteLine("【POST】:json:" + json);

            // RestRequestを作成
            RestRequest request = new(url, Method.Post);
            request.AddBody(json);

            //通信実行
            DoRequest(request, url,"POST");
        }

        /// <summary>
        /// リクエスト送信
        /// </summary>
        /// <param name="request"></param>
        /// <returns>json</returns>
        /// <exception cref="Exception"></exception>
        static RestResponse DoRequest(RestRequest request, String url,String flag)
        {
            RestClientOptions options = new RestClientOptions(url);
            var _client = new RestClient(options);

            RestResponse? response = null;

            // 送信
            switch (flag)
            {
                case "GET":
                    response = _client.Get(request);
                    break;
                case "POST":
                    response = _client.Post(request);
                    break;
                case "DELETE":
                    response = _client.Delete(request);
                    break;
                default:
                    break;
            }

            if  (response==null)
            {
                throw new Exception("RestResponse：" +"NULL value");
            }

            if (string.IsNullOrWhiteSpace(response.Content) || response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var errorMessage = response.ErrorMessage;
                string errmsg;
                if (errorMessage != null)
                {
                    errmsg = errorMessage.ToString();
                    Console.WriteLine("エラー:" + errmsg);
                    throw new Exception("DoRequest：" + errmsg);
                }
            }

            // ステータスコードを取得
            var statusCode = response.StatusCode;

            Console.WriteLine("StatusCode:" + statusCode);

            // レスポンスボディを取得
            var content = response.Content;

            // ステータスコード
            Console.WriteLine("ステータスコード:" + statusCode.ToString());

            // レスポンスボディ
            if (content != null)
            {
                Console.WriteLine("レスポンスボディ:" + content.ToString());
            }
            return response;
        }
    }
}

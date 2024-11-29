using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PPH.Library.Helpers;

namespace PPH.Library.Services
{
    // 处理百度API的AppId和API Key
    public static class BaiduArgs
    {
        // Base64编码后的APPID，直接写在代码里
        public static string GetId() => "MjAyNDExMjgwMDIyMTQyNDg=";  // 替换为你的编码后的AppId

        // Base64编码后的API Key，直接写在代码里
        public static string GetKey() => "ZV9sbnlLUFdCcHRweTdaUTFQUlk=";  // 替换为你的编码后的API Key

        // 解码方法：解码 Base64 编码后的值
        public static string Decode(string encodedValue)
        {
            byte[] decodedBytes = Convert.FromBase64String(encodedValue);
            return Encoding.UTF8.GetString(decodedBytes);
        }
    }

    public class TranslateService : ITranslateService
    {
        private readonly IAlertService _alertService;
        private readonly Random _random = new();

        public TranslateService(IAlertService alertService)
        {
            _alertService = alertService;
        }

        // MD5加密为32位字符串
        private static string Md5Crypto(string plainText)
        {
            using var md5 = MD5.Create();
            return string.Concat(md5.ComputeHash(Encoding.UTF8.GetBytes(plainText)).Select(b => b.ToString("x2")));
        }

        // 翻译
        public async Task<string> Translate(string sourceText, string from = "auto", string to = "zh")
        {
            var salt = _random.Next(1000, 10000).ToString();  // 生成随机数
            var sign = GetSign(sourceText, salt);  // 获取签名

            const string server = "百度翻译服务器";  // 定义服务名
            using var httpClient = new HttpClient();

            // 发送请求并处理错误
            HttpResponseMessage response;
            try
            {
                // 拼接 API 请求
                var url = $"http://api.fanyi.baidu.com/api/trans/vip/translate?q={sourceText}&from={from}&to={to}&appid={BaiduArgs.Decode(BaiduArgs.GetId())}&salt={salt}&sign={sign}";
                response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();  // 确保请求成功
            }
            catch (Exception e)
            {
                // 异常处理
                await _alertService.AlertAsync(ErrorMessageHelper.HttpClientErrorTitle, ErrorMessageHelper.GetHttpClientError(server, e.Message));
                return string.Empty;
            }

            // 处理响应
            var json = await response.Content.ReadAsStringAsync();
            try
            {
                // 反序列化 JSON 响应
                var baiduTranslateResponse = JsonSerializer.Deserialize<BaiduTranslateResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (baiduTranslateResponse?.trans_result != null && baiduTranslateResponse.trans_result.Length > 0)
                {
                    return baiduTranslateResponse.trans_result[0].Dst;
                }
                else
                {
                    return string.Empty;  // 如果没有翻译结果，返回空字符串
                }
            }
            catch (Exception e)
            {
                // 反序列化失败时的异常处理
                await _alertService.AlertAsync(ErrorMessageHelper.JsonDeserializationErrorTitle, ErrorMessageHelper.GetJsonDeserializationError(server, e.Message));
                return string.Empty;
            }
        }

        // 获取签名：拼接 APPID + q + salt + API Key 后进行 MD5 加密
        private static string GetSign(string q, string salt)
        {
            return Md5Crypto(BaiduArgs.Decode(BaiduArgs.GetId()) + q + salt + BaiduArgs.Decode(BaiduArgs.GetKey()));
        }
    }

    // 百度翻译 API 响应格式
    public class BaiduTranslateResponse
    {
        public string From { get; set; }
        public string To { get; set; }
        public BaiduTransResult[] trans_result { get; set; }
    }

    // 翻译结果
    public class BaiduTransResult
    {
        public string Src { get; set; }
        public string Dst { get; set; }
    }
}

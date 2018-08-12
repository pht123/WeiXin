using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WeiXin.Common
{
    public class HttpHelper
    {
        private static int timeout = 800;
        //private static ILoggerRepository _loggerRepository;
        private static readonly HttpClient client;
        static HttpHelper()
        {
            //_loggerRepository = Startup.loggerRepository;
            client = new HttpClient();
        }

        /// <summary>
        /// 异步GET
        /// </summary>
        /// <param name="url"></param>
        /// <param name="datas"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<string> GetAsync(string url, Dictionary<string, string> datas, Dictionary<string, string> headers = null)
        {
            try
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                foreach (var data in datas ?? new Dictionary<string, string>())
                {
                    url += $"{(url.Contains("?") ? "&" : "?")}{data.Key}={data.Value}";
                }
                HttpResponseMessage resp = await client.GetAsync(url);
                if (resp.IsSuccessStatusCode)
                {
                    var jsonString = await resp.Content.ReadAsStringAsync();
                    return jsonString;
                }
                else
                {
                    //var log = log4net.LogManager.GetLogger(_loggerRepository.Name, "HTTP-GET");
                    //log.Error($"URL: {url}; StatusCode:{resp.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                //var log = log4net.LogManager.GetLogger(_loggerRepository.Name, "HTTP-GET");
                //log.Error($"URL: { url } ; Exception:{ex.Message}");
                return null;
            }

        }

        /// <summary>
        /// 异步GET
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="datas"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(string url, Dictionary<string, string> datas, Dictionary<string, string> headers = null)
        {
            try
            {
                string json = await GetAsync(url, datas, headers);
                if (!string.IsNullOrEmpty(json))
                {
                    var jsonResult = JsonConvert.DeserializeObject<T>(json);
                    return jsonResult;
                }
                else
                {
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                //var log = log4net.LogManager.GetLogger(_loggerRepository.Name, "HTTP-GET");
                //log.Error($"URL: { url } ; Exception:{ex.Message}");
                return default(T);
            }
        }

        /// <summary>
        /// 异步post
        /// application/json
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postjson">body请求参数</param>
        /// <param name="pathdatas">query请求参数</param>
        /// <param name="headers">header请求参数</param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static async Task<string> PostAsync(string url, string postjson, Dictionary<string, string> pathdatas = null, Dictionary<string, string> headers = null, string charset = "UTF-8")
        {
            HttpContent content = new StringContent(postjson);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.ContentType.CharSet = charset;
            string json = await PostAsync(content, url, pathdatas, headers);
            return json;
        }

        /// <summary>
        /// 异步POST
        /// application/x-www-form-urlencoded
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="keyValues">body请求参数</param>
        /// <param name="pathdatas">query请求参数</param>
        /// <param name="headers">header请求参数</param>
        /// <param name="charset"></param>
        /// <returns></returns>
        /// 
        public static async Task<string> PostAsync(string url, List<KeyValuePair<string, string>> keyValues, Dictionary<string, string> pathdatas = null, Dictionary<string, string> headers = null, string charset = "UTF-8")
        {
            HttpContent content = new FormUrlEncodedContent(keyValues);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            content.Headers.ContentType.CharSet = charset;
            string json = await PostAsync(content, url, pathdatas, headers);
            return json;
        }

        /// <summary>
        /// 异步POST
        /// application/json
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postdatas">body请求参数</param>
        /// <param name="pathdatas">query请求参数</param>
        /// <param name="headers">header请求参数</param>
        /// <param name="mediaType"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        private static async Task<string> PostAsync(HttpContent content, string url, Dictionary<string, string> pathdatas = null, Dictionary<string, string> headers = null)
        {
            try
            {
                if (url.StartsWith("https"))
                {
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;
                }

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                if (pathdatas != null)
                {
                    foreach (var data in pathdatas)
                    {
                        url += $"{(url.Contains("?") ? "&" : "?")}{data.Key}={data.Value}";
                    }
                }

                HttpResponseMessage resp = await client.PostAsync(url, content);
                if (resp.IsSuccessStatusCode)
                {
                    string token = await resp.Content.ReadAsStringAsync();
                    return token;
                }
                else
                {
                    //var log = log4net.LogManager.GetLogger(_loggerRepository.Name, "HTTP-POST");
                    //log.Error("URL: " + url + "; StatusCode:" + resp.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                //var log = log4net.LogManager.GetLogger(_loggerRepository.Name, "HTTP-POST");
                //log.Error($"URL: { url } ; Exception:{ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 异步POST
        /// application/JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="postdatas">body请求参数</param>
        /// <param name="pathdatas">query请求参数</param>
        /// <param name="headers">header请求参数</param>
        /// <param name="mediaType"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static async Task<T> PostAsync<T>(string url, string postjson, Dictionary<string, string> pathdatas = null, Dictionary<string, string> headers = null, string charset = "UTF-8")
        {
            try
            {
                string json = await PostAsync(url, postjson, pathdatas, headers, charset);
                if (!string.IsNullOrEmpty(json))
                {
                    var jsonResult = JsonConvert.DeserializeObject<T>(json);
                    return jsonResult;
                }
                return default(T);
            }
            catch (Exception ex)
            {
                //var log = log4net.LogManager.GetLogger(_loggerRepository.Name, "HTTP-POST");
                //log.Error($"URL: { url } ; Exception:{ex.Message}");
                return default(T);
            }
        }

        /// <summary>
        /// 异步POST
        /// application/x-www-form-urlencoded
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="keyValues">body请求参数</param>
        /// <param name="pathdatas">query请求参数</param>
        /// <param name="headers">header请求参数</param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static async Task<T> PostAsync<T>(string url, List<KeyValuePair<string, string>> keyValues, Dictionary<string, string> pathdatas = null, Dictionary<string, string> headers = null, string charset = "UTF-8")
        {
            try
            {
                string json = await PostAsync(url, keyValues, pathdatas, headers, charset);
                if (!string.IsNullOrEmpty(json))
                {
                    var jsonResult = JsonConvert.DeserializeObject<T>(json);
                    return jsonResult;
                }
                return default(T);
            }
            catch (Exception ex)
            {
                //var log = log4net.LogManager.GetLogger(_loggerRepository.Name, "HTTP-POST");
                //log.Error($"URL: { url } ; Exception:{ex.Message}");
                return default(T);
            }
        }

        /// <summary>
        /// 异步PUT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="headers"></param>
        /// <param name="charset"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public static async Task<T> PutAsync<T>(string url, string data, Dictionary<string, string> headers = null, string charset = "UTF-8", string mediaType = "application/json")
        {
            try
            {
                HttpContent content = new StringContent(data);
                content.Headers.ContentType = new MediaTypeHeaderValue(mediaType)
                {
                    CharSet = charset
                };
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                HttpResponseMessage resp = await client.PutAsync(url, content);
                if (resp.IsSuccessStatusCode)
                {
                    var jsonString = await resp.Content.ReadAsStringAsync();
                    var jsonResult = JsonConvert.DeserializeObject<T>(jsonString);
                    return jsonResult;
                }
                else
                {
                    //var log = log4net.LogManager.GetLogger(_loggerRepository.Name, "HTTP-PUT");
                    //log.Error($"URL: {url}; StatusCode: {resp.StatusCode}");
                    return default(T);
                }           
            }
            catch (Exception ex)
            {
                //var log = log4net.LogManager.GetLogger(_loggerRepository.Name, "HTTP-PUT");
                //log.Error($"URL: {url}; Exception: {ex.Message}");
                return default(T);
            }
        }
    }

    public static class HttpClientExtensions
    {
        public async static Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content)
        {
            var method = new HttpMethod("PATCH");

            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = content
            };

            return await client.SendAsync(request);
        }

        public async static Task<HttpResponseMessage> PatchAsync(this HttpClient client, Uri requestUri, HttpContent content)
        {
            var method = new HttpMethod("PATCH");

            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = content
            };

            return await client.SendAsync(request);
        }

        public async static Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            var method = new HttpMethod("PATCH");

            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = content
            };

            return await client.SendAsync(request, cancellationToken);
        }

        public async static Task<HttpResponseMessage> PatchAsync(this HttpClient client, Uri requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            var method = new HttpMethod("PATCH");

            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = content
            };

            return await client.SendAsync(request, cancellationToken);
        }
    }
}

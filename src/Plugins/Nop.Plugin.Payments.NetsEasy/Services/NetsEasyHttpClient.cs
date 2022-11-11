using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Plugin.Payments.NetsEasy;
using Vendr.Contrib.PaymentProviders.Api.Models;

namespace Nop.Plugins.Payments.NetsEasy.Services
{
    public class NetsEasyHttpClient
    {
        private readonly HttpClient _httpClient;
        private string _url;

        public NetsEasyHttpClient(HttpClient client, NetsEasySettings netsEasySettings)
        {
            _httpClient = client;
            _httpClient.Timeout = TimeSpan.FromMilliseconds(5000);
            //_httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, $"nopCommerce-{NopVersion.CurrentVersion}");
            _httpClient.DefaultRequestHeaders.Add(Microsoft.Net.Http.Headers.HeaderNames.Accept, "application/json;charset=UTF-8");

            if(!string.IsNullOrEmpty(netsEasySettings.SecretKey))
            {
                _httpClient.DefaultRequestHeaders.Add(Microsoft.Net.Http.Headers.HeaderNames.Authorization, netsEasySettings.SecretKey);
            }
            _url = netsEasySettings.Url;
            if(!_url.EndsWith("/"))
            {
                _url = _url + "/";
            }

            //var sandbox = true;
            //var secretKey = "test-secret-key-012c020434294aa28639923e5ce936e5";
            //_httpClient.DefaultRequestHeaders.Add(Microsoft.Net.Http.Headers.HeaderNames.Authorization, secretKey);
            //if (sandbox)
            //{
            //    _url = "https://test.api.dibspayment.eu/";
            //}
            //else
            //{
            //    _url = "https://api.dibspayment.eu/";
            //}
        }

        public async Task<NetsPaymentResult> CreatePaymentAsync(NetsPaymentRequest data)
        {
            var responseObject = await PostAsJson<NetsPaymentResult>(data, "v1/payments/", data.Order.Reference);
            return responseObject;
        }

        public async Task<NetsPaymentResponse> GetPaymentAsync(string paymentId)
        {
            var httpResponse = await _httpClient.GetAsync(_url + $"v1/payments/{paymentId}");
            return await ReadResponseObject<NetsPaymentResponse>(httpResponse);
        }

        private async Task<Tresult> PostAsJson<Tresult>(NetsPaymentRequest data, string path, string idempotencyKey=null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _url + path);
            request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            if (idempotencyKey != null)
            {
                request.Headers.Add("Idempotency-Key", idempotencyKey);
            }

            var httpResponse = await _httpClient.SendAsync(request);
            return await ReadResponseObject<Tresult>(httpResponse);
        }

        private static async Task<Tresult> ReadResponseObject<Tresult>(HttpResponseMessage response)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(responseString);
            }
            var responseObject = JsonConvert.DeserializeObject<Tresult>(responseString);
            return responseObject;
        }

        //public async Task<string> Capture(ChargeOrder payment, string paymentId, bool sandbox, string secretKey)
        //{
        //    var obj = JsonConvert.SerializeObject(payment);
        //    var content = new StringContent(obj, Encoding.UTF8, "application/json");
        //    var resp = await _httpClient.PostAsync(_url + $"v1/payments/{paymentId}/charges", content);
        //    if (resp.StatusCode == System.Net.HttpStatusCode.Created || resp.StatusCode == System.Net.HttpStatusCode.OK)
        //    {
        //        return await resp.Content.ReadAsStringAsync();
        //    }
        //    var result = await resp.Content.ReadAsStringAsync();
        //    //_logger.InsertLog(LogLevel.Error, "Bad Request", result);
        //    return "BADREQUEST";
        //}




    }
}

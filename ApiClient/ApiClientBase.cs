using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

public abstract class ApiClientBase {
    private static readonly HttpClient httpClient;

    static ApiClientBase() {
        httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    protected Task<Stream> GetAsync(string path, string content = null) => SendRequestAsync(path, content, HttpMethod.Get);
    protected Task<Stream> PostAsync(string path, string content) => SendRequestAsync(path, content, HttpMethod.Post);
    protected Task<Stream> PutAsync(string path, string content) => SendRequestAsync(path, content, HttpMethod.Put);

    private async Task<Stream> SendRequestAsync(string path, string content, HttpMethod httpMethod) {
        try {
            HttpRequestMessage request = new HttpRequestMessage {
                Method = httpMethod,
                RequestUri = new Uri(path),
                Content = content != null ? new StringContent(content, Encoding.UTF8, "application/json") : null,
            };

            HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            if (response.IsSuccessStatusCode) {
                return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest) {
                var stringContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                OnBadRequest(response);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized
              || response.StatusCode == HttpStatusCode.Forbidden) {
                OnAuthError(response);
            }

            return null;
        }
        catch (Exception ex) {
            OnHttpError(ex);
            return null;
        }
    }

    public static event EventHandler<HttpResponseMessage> AuthError;
    protected virtual void OnAuthError(HttpResponseMessage response) {
        var handler = AuthError;
        if (handler != null) handler(this, response);
    }

    public static event EventHandler<HttpResponseMessage> BadRequest;
    protected virtual void OnBadRequest(HttpResponseMessage response) {
        var handler = BadRequest;
        if (handler != null) handler(this, response);
    }

    public static event EventHandler<Exception> HttpError;
    protected virtual void OnHttpError(Exception ex) {
        var handler = HttpError;
        if (handler != null) handler(this, ex);
    }
}
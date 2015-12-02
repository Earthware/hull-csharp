namespace Hull.io
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;

    using Newtonsoft.Json;

    using RestSharp;
    using RestSharp.Extensions.MonoHttp;

    public class HullClient
    {
        private static readonly List<HttpStatusCode> SuccessfulStatusCodes = new List<HttpStatusCode>
        {
            HttpStatusCode.Accepted,
            HttpStatusCode.Continue,
            HttpStatusCode.Created,
            HttpStatusCode.NoContent,
            HttpStatusCode.OK
        };

        private readonly HullConfiguration config;

        private readonly RestClient httpClient;

        public HullClient()
            : this(new HullConfiguration())
        {
        }

        public HullClient(HullConfiguration config)
        {
            this.config = config;
            this.httpClient = new RestClient(this.config.OrgUrl);
        }

        public HullClient(string appId, string appSecret, string orgUrl)
            : this(new HullConfiguration(appId, appSecret, orgUrl))
        {
        }

        public HullConfiguration Configuration
        {
            get
            {
                return this.config;
            }
        }

        public string ApiPath(string path, string querystring)
        {
            if (!string.IsNullOrEmpty(path) && !path.StartsWith("/"))
            {
                path = "/" + path;
            }
            var rval = new StringBuilder();
            rval.Append("/api/v1").Append(path);
            if (null != querystring && !string.IsNullOrEmpty(querystring))
            {
                rval.Append("?").Append(querystring);
            }
            return rval.ToString();
        }

        public string Delete(string path, Dictionary<string, string> parameters = null, string userId = null)
        {
            var querystring = ConvertToQuerystring(parameters);

            var request = new RestRequest(this.ApiPath(path, querystring), Method.DELETE);

            this.SetHeaders(request, userId);

            return this.ExecuteRequest(request);
        }

        public string Get(string path, Dictionary<string, string> parameters = null, string userId = null)
        {
            var querystring = ConvertToQuerystring(parameters);

            var request = new RestRequest(this.ApiPath(path, querystring), Method.GET);

            this.SetHeaders(request, userId);

            return this.ExecuteRequest(request);
        }

        public T Get<T>(string path, Dictionary<string, string> parameters = null, string userId = null)
        {
            var querystring = ConvertToQuerystring(parameters);

            var request = new RestRequest(this.ApiPath(path, querystring), Method.GET);

            this.SetHeaders(request, userId);

            return this.ExecuteRequest<T>(request);
        }

        public string Post(string path)
        {
            return this.Post(path, null);
        }

        public string Post(string path, Dictionary<string, object> parameters)
        {
            var request = this.BuildPostRequest(path, parameters);

            return this.ExecuteRequest(request);
        }

        public T Post<T>(string path, Dictionary<string, object> parameters)
        {
            var request = this.BuildPostRequest(path, parameters);

            return this.ExecuteRequest<T>(request);
        }

        public string Put(string path, Dictionary<string, object> parameters = null, string userId = null)
        {
            var request = this.BuildPutRequest(path, parameters, userId);

            return this.ExecuteRequest(request);
        }

        public T Put<T>(string path, Dictionary<string, object> parameters = null, string userId = null)
        {
            var request = this.BuildPutRequest(path, parameters, userId);

            return this.ExecuteRequest<T>(request);
        }

        private static string ConvertToQuerystring(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var querystring = string.Empty;

            if (parameters != null)
            {
                querystring = string.Join(
                    "&",
                    parameters.Select(x => string.Format("{0}={1}", x.Key, HttpUtility.UrlEncode(x.Value))));
            }

            return querystring;
        }

        private RestRequest BuildPostRequest(string path, Dictionary<string, object> parameters)
        {
            var request = new RestRequest(this.ApiPath(path, null), Method.POST);
            
            this.SetHeaders(request);

            AddParametersToRequestBody(parameters, request);
            
            return request;
        }

        private static void AddParametersToRequestBody(Dictionary<string, object> parameters, RestRequest request)
        {
            try
            {
                var jsonData = new JsonObject();
                foreach (var current in parameters)
                {
                    jsonData.Add(current);
                }

                request.RequestFormat = DataFormat.Json;
                request.AddBody(jsonData);
            }
            catch (Exception e)
            {
                throw new HullException("Could not convert params to json string", e);
            }
        }

        private RestRequest BuildPutRequest(string path, Dictionary<string, object> parameters, string userId)
        {
            var request = new RestRequest(this.ApiPath(path, null), Method.PUT);
            this.SetHeaders(request, userId);

            AddParametersToRequestBody(parameters, request);

            return request;
        }

        private string ExecuteRequest(IRestRequest request)
        {
            IRestResponse response;

            try
            {
                response = this.httpClient.Execute(request);
            }
            catch (Exception e)
            {
                throw new HullException("Could not execute Hull request", e);
            }

            if (!SuccessfulStatusCodes.Contains(response.StatusCode))
            {
                throw new HullException(response, "Failed to execute Hull request");
            }

            return response.Content;
        }

        private T ExecuteRequest<T>(RestRequest request)
        {
            var data = this.ExecuteRequest(request);
            return JsonConvert.DeserializeObject<T>(data);
        }

        private void SetHeaders(IRestRequest req, string userId = null)
        {
            req.AddHeader("Content-Type", "application/json");
            req.AddHeader("Hull-App-Id", this.config.AppId);
            req.AddHeader("Hull-Access-Token", this.config.AppSecret);

            if (!string.IsNullOrEmpty(userId))
            {
                req.AddHeader("Hull-User-Id", userId);
            }
        }
    }
}
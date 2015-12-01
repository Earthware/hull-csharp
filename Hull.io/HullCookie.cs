namespace Hull.io
{
    using System;
    using System.Text;

    using Newtonsoft.Json;

    public class HullCookie
    {
        public static string Generate(string userId, HullConfiguration configuration)
        {
            var signature = Utilities.BuildSignedUserId(userId, configuration.AppSecret);
            var raw = new RawCookieData
                          {
                              AuthScope = "User:" + userId,
                              UserId = userId,
                              UserSig = signature
                          };

            var rawJson = JsonConvert.SerializeObject(raw);
            var jsonBytes = Encoding.UTF8.GetBytes(rawJson);
            var encodedData = Convert.ToBase64String(jsonBytes);

            return encodedData;
        }

        public HullCookie(string rawCookie, HullConfiguration configuration)
        {
            // Invalid by default
            this.IsValid = false;

            if (!string.IsNullOrEmpty(rawCookie))
            {
                var decodedData = Convert.FromBase64String(rawCookie);
                var userJson = Encoding.UTF8.GetString(decodedData);

                var userData = JsonConvert.DeserializeObject<RawCookieData>(userJson);

                if (userData != null && !string.IsNullOrEmpty(userData.UserId))
                {
                    this.UserId = userData.UserId;

                    this.IsValid = Utilities.CheckSignedUserId(
                        userData.UserId,
                        userData.UserSig,
                        configuration.AppSecret);
                }
            }
        }

        public string UserId { get; private set; }

        public bool IsValid { get; private set; }

        private class RawCookieData
        {
            [JsonProperty("Hull-Auth-Scope")]
            public string AuthScope { get; set; }

            [JsonProperty("Hull-User-Id")]
            public string UserId { get; set; }

            [JsonProperty("Hull-User-Sig")]
            public string UserSig { get; set; }
        }
    }
}
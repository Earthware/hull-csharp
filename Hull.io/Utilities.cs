namespace Hull.io
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    using RestSharp.Serializers;

    public static class Utilities
    {
        private static readonly DateTime JavascriptBaseTime = new DateTime(1970, 1, 1);

        public static string BuildSignedUserId(string userId, string secret)
        {
            var timestamp = ((long)Math.Round(DateTime.UtcNow.Subtract(JavascriptBaseTime).TotalSeconds)).ToString(CultureInfo.InvariantCulture);
            var digest = BuildHullDigest(userId, timestamp, secret);
            return string.Concat(timestamp, ".", digest);
        }

        public static bool CheckSignedUserId(string userId, string userSignature, string secret)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("userId must be provided", "userId");
            }

            if (string.IsNullOrEmpty(userSignature))
            {
                throw new ArgumentException("userSignature must be provided", "userSignature");
            }

            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentException("secret must be provided", "secret");
            }

            var signatureComponents = userSignature.Split('.');

            if (signatureComponents.Length != 2)
            {
                return false;
            }

            var time = signatureComponents[0];
            var signature = signatureComponents[1];

            var digest = BuildHullDigest(userId, time, secret);

            return digest == signature;
        }

        private static string BuildHullDigest(string userId, string time, string secret)
        {
            var hmacsha1 = new HMACSHA1(Encoding.Default.GetBytes(secret));

            var hmacInput = string.Concat(time, "-", userId);
            var digestBytes = hmacsha1.ComputeHash(Encoding.Default.GetBytes(hmacInput));

            var digest = string.Join(string.Empty, digestBytes.Select(x => x.ToString("X2").ToLowerInvariant()));
            return digest;
        }

        public static string SignUserData(object userData, string secret)
        {
            var timestamp = (long)Math.Round(DateTime.UtcNow.Subtract(JavascriptBaseTime).TotalSeconds);
            var serializer = new JsonSerializer();
            var messageJson = serializer.Serialize(userData);
            var messageEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(messageJson));

            var hmacsha1 = new HMACSHA1(Encoding.Default.GetBytes(secret));
            var hmacInput = string.Concat(messageEncoded, " ", timestamp);
            var hmacInputBytes = Encoding.Default.GetBytes(hmacInput);
            var signatureBytes = hmacsha1.ComputeHash(hmacInputBytes);
            var signature = string.Join(string.Empty, signatureBytes.Select(x => x.ToString("X2").ToLowerInvariant()));

            return string.Concat(messageEncoded, " ", signature, " ", timestamp);
        }
    }
}
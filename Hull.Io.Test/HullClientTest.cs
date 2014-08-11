namespace Hull.io.Test
{
    using NUnit.Framework;

    public class HullClientTest
    {
        private const string AppId = "12345";

        private const string AppSecret = "abcde";

        private const string OrgUrl = "http://test.hullapp.io";

        private static HullClient client;

        [SetUp]
        public static void SetUp()
        {
            client = new HullClient(AppId, AppSecret, OrgUrl);
        }

        [Test]
        public void TestApiPath()
        {
            const string Path = "12345/app";
            const string QueryString = "key1=val1&key2=val2";
            var url = client.ApiPath(Path, QueryString);

            Assert.AreEqual(url, OrgUrl + "/api/v1/" + Path + "?" + QueryString);
        }

        [Test]
        public void TestApiPathWithEmptyQueryString()
        {
            const string Path = "12345/app";
            var url = client.ApiPath(Path, "");

            Assert.AreEqual(url, OrgUrl + "/api/v1/" + Path);
        }

        [Test]
        public void TestApiPathWithLeadingSlash()
        {
            const string Path = "/12345/app";
            const string QueryString = "key1=val1&key2=val2";
            var url = client.ApiPath(Path, QueryString);

            Assert.AreEqual(url, OrgUrl + "/api/v1" + Path + "?" + QueryString);
        }

        [Test]
        public void TestApiPathWithoutQueryString()
        {
            const string Path = "12345/app";
            var url = client.ApiPath(Path, null);

            Assert.AreEqual(url, OrgUrl + "/api/v1/" + Path);
        }
    }
}
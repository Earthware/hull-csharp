namespace Hull.io.Test
{
    using NUnit.Framework;

    public class HullCookieTests
    {
        private const string AppId = "12345";

        private const string AppSecret = "abcde";

        private const string OrgUrl = "http://test.hullapp.io";

        private static HullConfiguration configuration;

        [SetUp]
        public static void Setup()
        {
            configuration = new HullConfiguration(AppId, AppSecret, OrgUrl);
        }

        [Test]
        public static void CookieGeneratedWithTheHullCookieClassCanBeDecodedWithTheHullCookieClass()
        {
            var cookie = HullCookie.Generate("user", configuration);

            var output = new HullCookie(cookie, configuration);

            Assert.AreEqual("user", output.UserId);
            Assert.IsTrue(output.IsValid);
        }
    }
}
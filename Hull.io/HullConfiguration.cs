namespace Hull.io
{
    using System;
    using System.Configuration;

    public class HullConfiguration
    {
        public HullConfiguration(string appId, string appSecret, string orgUrl)
        {
            this.AppId = appId;
            this.AppSecret = appSecret;
            this.OrgUrl = orgUrl;
            this.Validate();
        }

        public HullConfiguration()
            : this(
                ConfigurationManager.AppSettings["Hull.AppId"],
                ConfigurationManager.AppSettings["Hull.AppSecret"],
                ConfigurationManager.AppSettings["Hull.OrgUrl"])
        {
        }

        public string AppId { get; set; }

        public string AppSecret { get; set; }

        public string OrgUrl { get; set; }

        public string OrgSecret { get; set; }

        private void Validate()
        {
            if (string.IsNullOrEmpty(this.AppId))
            {
                throw new ArgumentException("An appId must be configured");
            }

            if (string.IsNullOrEmpty(this.AppSecret))
            {
                throw new ArgumentException("An appSecret must be configured");
            }

            if (string.IsNullOrEmpty(this.OrgUrl))
            {
                throw new ArgumentException("An orgUrl must be configured");
            }
        }
    }
}
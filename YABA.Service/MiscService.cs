using HtmlAgilityPack;
using System;
using System.Net;
using System.Text.RegularExpressions;
using YABA.Common.DTOs;
using YABA.Service.Interfaces;

namespace YABA.Service
{
    public class MiscService : IMiscService
    {
        public MiscService() { }

        public WebsiteMetaDataDTO GetWebsiteMetaData(string url)
        {
            try
            {
                var webClient = new WebClient();
                webClient.Headers.Add("user-agent", "API-Application");
                var sourceData = webClient.DownloadString(url);
                var title = Regex.Match(sourceData, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
                var description = string.Empty;

                var getHtmlDoc = new HtmlWeb();
                var document = getHtmlDoc.Load(url);
                var metaTags = document.DocumentNode.SelectNodes("//meta");
                if (metaTags != null)
                {
                    foreach (var sitetag in metaTags)
                    {
                        if (sitetag.Attributes["name"] != null && sitetag.Attributes["content"] != null && sitetag.Attributes["name"].Value == "description")
                        {
                            description = sitetag.Attributes["content"].Value;
                        }
                    }
                }

                return new WebsiteMetaDataDTO
                {
                    Title = title,
                    Description = description
                };
            } catch(Exception)
            {
                return new WebsiteMetaDataDTO
                {
                    Title = url,
                    Description = string.Empty
                };
            }
        }
    }
}

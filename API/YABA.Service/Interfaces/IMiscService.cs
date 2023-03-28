using YABA.Common.DTOs;

namespace YABA.Service.Interfaces
{
    public interface IMiscService
    {
        public WebsiteMetaDataDTO GetWebsiteMetaData(string url);
    }
}

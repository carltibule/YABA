using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using YABA.Common.DTOs.Tags;
using YABA.Common.Extensions;
using YABA.Data.Context;
using YABA.Service.Interfaces;

namespace YABA.Service
{
    public class TagsService : ITagsService
    {
        private readonly YABAReadOnlyContext _roContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public TagsService(YABAReadOnlyContext roContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _roContext = roContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public IEnumerable<TagSummaryDTO> GetAll()
        {
            var currentUserId = GetCurrentUserId();

            var activeUserTags = _roContext.BookmarkTags
                .Include(x => x.Tag)
                .Where(x => x.Tag.UserId == currentUserId)
                .ToList()
                .GroupBy(x => x.Tag.Id)
                .Select(g => g.First()?.Tag);

            return _mapper.Map<IEnumerable<TagSummaryDTO>>(activeUserTags);

        }

        private int GetCurrentUserId()
        {
            int.TryParse(_httpContextAccessor.HttpContext.User.Identity.GetUserId(), out int userId);
            return userId;
        }
    }
}

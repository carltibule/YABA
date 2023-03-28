using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YABA.Common.DTOs.Tags;
using YABA.Common.Extensions;
using YABA.Data.Context;
using YABA.Data.Extensions;
using YABA.Models;
using YABA.Service.Interfaces;

namespace YABA.Service
{
    public class TagsService : ITagsService
    {
        private readonly YABAReadOnlyContext _roContext;
        private readonly YABAReadWriteContext _rwContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public TagsService(
            YABAReadOnlyContext roContext, 
            YABAReadWriteContext rwContext,
            IHttpContextAccessor httpContextAccessor, 
            IMapper mapper)
        {
            _roContext = roContext;
            _rwContext = rwContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TagDTO>?> GetAll()
        {
            var currentUserId = GetCurrentUserId();
            if (!await _roContext.Users.UserExistsAsync(currentUserId)) return null;

            var activeUserTags = _roContext.Tags
                .Where(x => x.UserId == currentUserId)
                .ToList();

            return _mapper.Map<IEnumerable<TagDTO>>(activeUserTags);
        }

        public async Task<TagDTO?> Get(int id)
        {
            var currentUserId = GetCurrentUserId();
            if (!await _roContext.Users.UserExistsAsync(currentUserId)) return null;

            var bookmark = await _roContext.Tags.FirstOrDefaultAsync(x => x.Id == id && x.UserId == currentUserId);
            return _mapper.Map<TagDTO>(bookmark);
        }

        public async Task<IEnumerable<TagDTO>?> UpsertTags(IEnumerable<TagDTO> tags)
        {
            var currentUserId = GetCurrentUserId();
            if (!await _roContext.Users.UserExistsAsync(currentUserId)) return null;

            var tagsToUpsert = tags.Select(x => new Tag { Id = x.Id, Name = x.Name.ToLower(), IsHidden = x.IsHidden, UserId = currentUserId});
            _rwContext.Tags.UpsertRange(tagsToUpsert);
            await _rwContext.SaveChangesAsync();

            var newlyUpsertedTags = _roContext.Tags.Where(x => tagsToUpsert.Select(x => x.Name).Contains(x.Name)).ToList();
            return _mapper.Map<IEnumerable<TagDTO>>(newlyUpsertedTags);
        }

        public async Task<IEnumerable<int>?> DeleteTags(IEnumerable<int> ids)
        {
            var currentUserId = GetCurrentUserId();
            if (!await _roContext.Users.UserExistsAsync(currentUserId)) return null;

            var entriesToDelete = _rwContext.Tags.Where(x => x.UserId == currentUserId && ids.Contains(x.Id)).ToList();
            _rwContext.Tags.RemoveRange(entriesToDelete);

            if (await _rwContext.SaveChangesAsync() <= 0) return null;

            return entriesToDelete.Select(x => x.Id);
        }

        public async Task<IEnumerable<int>?> HideTags(IEnumerable<int> ids)
        {
            var currentUserId = GetCurrentUserId();
            if (!await _roContext.Users.UserExistsAsync(currentUserId)) return null;

            var entriesToHide = _rwContext.Tags.Where(x => x.UserId == currentUserId && ids.Contains(x.Id)).ToList();
            entriesToHide.ForEach((x) => { x.IsHidden = !x.IsHidden; });

            if (await _rwContext.SaveChangesAsync() <= 0) return null;

            return entriesToHide.Select(x => x.Id);
        }

        public async Task<TagDTO?> UpsertTag(TagDTO tag) => (await UpsertTags(new List<TagDTO>() { tag }))?.FirstOrDefault();

        public async Task<TagDTO?> CreateTag(CreateTagDTO request)
        {
            var currentUserId = GetCurrentUserId();

            if(!(await _roContext.Users.UserExistsAsync(currentUserId))
                || await _roContext.Tags.AnyAsync(x => x.UserId == currentUserId && x.Name.ToLower() == request.Name.ToLower()))
                return null;

            var tag = _mapper.Map<Tag>(request);
            tag.UserId = currentUserId;

            var newEntity = await _rwContext.Tags.AddAsync(tag);

            return await _rwContext.SaveChangesAsync() > 0 ? _mapper.Map<TagDTO>(newEntity.Entity) : null;
        }

        public async Task<TagDTO?> UpdateTag(int id, UpdateTagDTO request)
        {
            var currentUserId = GetCurrentUserId();
            var tag = await _rwContext.Tags.FirstOrDefaultAsync(x => x.UserId == currentUserId && x.Id == id);

            if (tag == null) return null;

            tag.Name = !string.IsNullOrEmpty(request.Name) ? request.Name.ToLower() : tag.Name;
            tag.IsHidden = request.IsHidden.HasValue ? request.IsHidden.Value : tag.IsHidden;

            return await _rwContext.SaveChangesAsync() > 0 ? _mapper.Map<TagDTO>(tag) : null;
        }

        private int GetCurrentUserId()
        {
            int.TryParse(_httpContextAccessor.HttpContext.User.Identity.GetUserId(), out int userId);
            return userId;
        }
    }
}

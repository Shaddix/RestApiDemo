using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NeinLinq;
using RestApiDemo.App.Features.Topics.Dto;
using RestApiDemo.App.Features.Users;
using RestApiDemo.Domain;
using RestApiDemo.Persistence;
using RestApiDemo.WebApi.Pagination;
using RestApiDemo.WebApi.PatchRequests;
using RestApiDemo.WebApi.Specifications;

namespace RestApiDemo.App.Features.Topics
{
    public class TopicService
    {
        private readonly MainDbContext _mainDbContext;

        public TopicService(MainDbContext mainDbContext)
        {
            _mainDbContext = mainDbContext;
        }

        public async Task<PagingList<TopicListDto>> GetAll(TopicSearchDto searchDto)
        {
            IQueryable<Topic> query = _mainDbContext.Topics
                    .ToEntityInjectable()
                ;
            if (searchDto.Id.HasValue)
            {
                query = query.Where(x => x.Id == searchDto.Id.Value);
            }

            if (!string.IsNullOrEmpty(searchDto.AuthorSearch))
            {
                query = query.Where(UserExtensions.SearchByText(searchDto.AuthorSearch).Translate()
                    .To<Topic>(topic => topic.Author));
            }

            // query = query.Where(topic =>
            //     string.IsNullOrEmpty(searchDto.Category) || topic.Category == searchDto.Category);
            if (!string.IsNullOrEmpty(searchDto.Category))
            {
                query = query.Where(topic => topic.Category == searchDto.Category);
            }

            return await query
                .Select(x => new TopicListDto()
                {
                    Id = x.Id,
                    Date = x.Date,
                    ShortText = x.ShortText,
                    Author = x.Author.ToUserListDto(),
                })
                .ToPagingListAsync(searchDto.Offset ?? 0, searchDto.Limit ?? 20, searchDto.Sort,
                    nameof(TopicListDto.Id));
        }

        public async Task<TopicDto> Get(int id)
        {
            return await _mainDbContext.Topics
                .ToEntityInjectable()
                .GetOne(Topic.HasId(id),
                    x => new TopicDto()
                    {
                        Id = x.Id,
                        Category = x.Category,
                        Date = x.Date,
                        Text = x.Text,
                        IsDraft = x.IsDraft,
                        ShortText = x.ShortText,
                        Author = x.Author.ToUserListDto(),
                    });
        }

        public async Task<TopicDto> Create(CreateTopicDto dto)
        {
            Topic topic = new()
            {
                Category = dto.Category,
                Text = dto.Text,
                IsDraft = dto.IsDraft,
                Date = dto.Date ?? DateTime.UtcNow
            };
            _mainDbContext.Add(topic);
            await _mainDbContext.SaveChangesAsync();

            return await Get(topic.Id);
        }

        public async Task Delete(int id)
        {
            Topic topic = await _mainDbContext.Topics.GetOne(Topic.HasId(id));
            _mainDbContext.Remove(topic);
            await _mainDbContext.SaveChangesAsync();
        }

        public async Task<TopicDto> Patch(int id, PatchTopicDto dto)
        {
            Topic topic = await _mainDbContext.Topics.GetOne(Topic.HasId(id));
            topic.Update(dto);
            await _mainDbContext.SaveChangesAsync();

            return await Get(id);
        }
    }
}
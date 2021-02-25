using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NeinLinq;
using RestApiDemo.App.Features.Topics.Dto;
using RestApiDemo.App.Features.Users;
using RestApiDemo.App.Features.Users.Dto;
using RestApiDemo.Domain;
using RestApiDemo.Persistence;
using RestApiDemo.WebApi.Pagination;
using RestApiDemo.WebApi.PatchRequests;
using RestApiDemo.WebApi.Specifications;

namespace RestApiDemo.App.Features.Comments
{
    public class CommentService
    {
        private readonly MainDbContext _mainDbContext;

        public CommentService(MainDbContext mainDbContext)
        {
            _mainDbContext = mainDbContext;
        }


        public async Task<PagingList<CommentListDto>> GetAll(CommentSearchDto searchDto)
        {
            IQueryable<Comment> query = _mainDbContext.Comments
                    .ToEntityInjectable()
                ;
            if (searchDto.Id.HasValue)
            {
                query = query.Where(x => x.Id == searchDto.Id.Value);
            }

            if (searchDto.TopicId.HasValue)
            {
                query = query.Where(x => x.TopicId == searchDto.TopicId.Value);
            }

            if (!string.IsNullOrEmpty(searchDto.AuthorSearch))
            {
                query = query.Where(UserExtensions.SearchByText(searchDto.AuthorSearch).Translate()
                    .To<Comment>(comment => comment.Author));
            }

            return await _mainDbContext.Comments
                .Where(x => true)
                .Select(x => new CommentListDto()
                {
                    Id = x.Id,
                    Author = x.Author.ToUserListDto(),
                    Text = x.Text,
                    Date = x.Date
                })
                .ToPagingListAsync(searchDto.Offset ?? 0, searchDto.Limit ?? 20, searchDto.Sort,
                    nameof(CommentListDto.Id));
        }

        

        public async Task<List<CommentListDto>> GetAll1(CommentSearchDto searchDto)
        {
            return await _mainDbContext.Comments
                .Where(x => true)
                .Select(x => new CommentListDto()
                {
                    Id = x.Id,
                    Author = new UserListDto()
                    {
                        Id = x.Author.Id,
                        FullName = x.Author.FullName,
                        ProfileImage = x.Author.ProfileImage,
                    },
                    Text = x.Text,
                    Date = x.Date
                }).ToListAsync();
        }


        public async Task<CommentListDto> Get(int id)
        {
            return await _mainDbContext.Comments
                .ToEntityInjectable()
                .GetOne(Comment.HasId(id),
                    x => new CommentListDto()
                    {
                        Id = x.Id,
                        Author = x.Author.ToUserListDto(),
                        Text = x.Text,
                        Date = x.Date
                    });
        }

        public async Task<CommentListDto> Create(CreateCommentDto dto)
        {
            Comment comment = new()
            {
                Text = dto.Text,
                Date = dto.Date ?? DateTime.UtcNow,
                TopicId = dto.TopicId,
            };
            _mainDbContext.Add(comment);
            await _mainDbContext.SaveChangesAsync();

            return await Get(comment.Id);
        }

        public async Task Delete(int id)
        {
            Comment comment = await _mainDbContext.Comments.GetOne(Comment.HasId(id));
            _mainDbContext.Remove(comment);
            await _mainDbContext.SaveChangesAsync();
        }

        public async Task<CommentListDto> Patch(int id, PatchCommentDto dto)
        {
            Comment comment = await _mainDbContext.Comments.GetOne(Comment.HasId(id));
            comment.Update(dto);
            await _mainDbContext.SaveChangesAsync();

            return await Get(id);
        }
    }
}
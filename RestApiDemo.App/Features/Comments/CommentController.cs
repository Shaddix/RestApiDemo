using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeinLinq;
using RestApiDemo.App.Features.Topics.Dto;
using RestApiDemo.App.Features.Users;
using RestApiDemo.App.Features.Users.Dto;
using RestApiDemo.Domain;
using RestApiDemo.Persistence;
using RestApiDemo.WebApi.Pagination;
using RestApiDemo.WebApi.Specifications;

namespace RestApiDemo.App.Features.Comments
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController
    {
        private readonly CommentService _commentService;
        private readonly MainDbContext _mainDbContext;

        public CommentController(CommentService commentService,
            MainDbContext mainDbContext)
        {
            _commentService = commentService;
            _mainDbContext = mainDbContext;
        }


        [HttpGet()]
        public async Task<PagingList<CommentListDto>> GetAll([FromQuery] CommentSearchDto searchDto)
        {
            return await _commentService.GetAll(searchDto);
        }


        [HttpPost()]
        public async Task<CommentListDto> Create(CreateCommentDto dto)
        {
            return await _commentService.Create(dto);
        }

        [HttpPatch("{id:int}")]
        public async Task<CommentListDto> Patch(int id, PatchCommentDto dto)
        {
            return await _commentService.Patch(id, dto);
        }

        [HttpDelete("{id:int}")]
        public async Task Delete(int id)
        {
            await _commentService.Delete(id);
        }

        [HttpGet("comment0")]
        public async Task<List<CommentListDto>> GetAll0([FromQuery] CommentSearchDto searchDto)
        {
            return await _mainDbContext.Comments
                .Where(comment => true)
                .Select(comment => new CommentListDto()
                {
                    Id = comment.Id,
                    Author = new UserListDto()
                    {
                        Id = comment.Author.Id,
                        ProfileImage = comment.Author.ProfileImage,
                    },
                    Text = comment.Text,
                    Date = comment.Date
                }).Take(20).ToListAsync();
        }


        [HttpGet("comment1")]
        public async Task<List<CommentListDto>> GetAll1([FromQuery] CommentSearchDto searchDto)
        {
            return await _mainDbContext.Comments
                .Where(comment => true)
                .Select(comment => new CommentListDto()
                {
                    Id = comment.Id,
                    Author = comment.Author.ToUserListDto(),
                    Text = comment.Text,
                    Date = comment.Date
                }).Take(20).ToListAsync();
        }

        [HttpGet("comment2")]
        public async Task<List<CommentListDto>> GetAll2([FromQuery] CommentSearchDto searchDto)
        {
            return await _mainDbContext.Comments
                .ToEntityInjectable()
                .Where(comment => true)
                .Select(comment => new CommentListDto()
                {
                    Id = comment.Id,
                    Author = comment.Author.ToUserListDto(),
                    Text = comment.Text,
                    Date = comment.Date
                }).Take(20).ToListAsync();
        }

        [HttpGet("comment3")]
        public async Task<PagingList<CommentListDto>> GetAll3(
            [FromQuery] CommentSearchDto searchDto)
        {
            return await _mainDbContext.Comments
                .Select(comment => new CommentListDto()
                {
                    Id = comment.Id,
                    Author = comment.Author.ToUserListDto(),
                    Text = comment.Text,
                }).ToPagingListAsync(searchDto.Offset ?? 0, searchDto.Limit ?? 20, searchDto.Sort,
                    nameof(CommentListDto.Id));
        }

        [HttpGet("comment4")]
        public async Task<PagingList<CommentListDto>> GetAll4(
            [FromQuery] CommentSearchDto searchDto)
        {
            return await _mainDbContext.Comments
                .ToEntityInjectable()
                .Select(comment => new CommentListDto()
                {
                    Id = comment.Id,
                    Author = comment.Author.ToUserListDto(),
                    Text = comment.Text,
                }).ToPagingListAsync(searchDto.Offset ?? 0, searchDto.Limit ?? 20, searchDto.Sort,
                    nameof(CommentListDto.Id));
        }

        [HttpGet("comment5")]
        public async Task<List<CommentListDto>> GetAll5(
            [FromQuery] CommentSearchDto searchDto)
        {
            return await _mainDbContext.Comments
                .ToEntityInjectable()
                .Where(UserExtensions.SearchByText(searchDto.AuthorSearch)
                    .Translate()
                    .To<Comment>(x => x.Author))
                .Select(comment => new CommentListDto()
                {
                    Id = comment.Id,
                    Author = comment.Author.ToUserListDto(),
                    Text = comment.Text,
                }).Take(20).ToListAsync();
        }

        [HttpGet("comment6")]
        public async Task<List<CommentListDto>> GetAll6(
            [FromQuery] CommentSearchDto searchDto)
        {
            return await _mainDbContext.Comments
                .ToEntityInjectable()
                .Where(comment =>
                    searchDto.TopicId == null || comment.TopicId == searchDto.TopicId)
                .Select(comment => new CommentListDto()
                {
                    Id = comment.Id,
                    Author = comment.Author.ToUserListDto(),
                    Text = comment.Text,
                }).Take(20).ToListAsync();
        }
    }
}
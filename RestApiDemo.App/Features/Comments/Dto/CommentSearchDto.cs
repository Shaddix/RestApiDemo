using System;
using RestApiDemo.WebApi.Pagination;

namespace RestApiDemo.App.Features.Topics.Dto
{
    public class CommentSearchDto : PagingBaseDto
    {
        public int? Id { get; set; }
        public int? TopicId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string AuthorSearch { get; set; }
    }
}
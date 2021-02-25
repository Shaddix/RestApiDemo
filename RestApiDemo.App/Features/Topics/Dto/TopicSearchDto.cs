using System;
using RestApiDemo.WebApi.Pagination;

namespace RestApiDemo.App.Features.Topics.Dto
{
    public class TopicSearchDto : PagingBaseDto
    {
        public int? Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string AuthorSearch { get; set; }
        public string Category { get; set; }
    }
}
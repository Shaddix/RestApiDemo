using System;
using RestApiDemo.WebApi.Pagination;

namespace RestApiDemo.App.Features.Topics.Dto
{
    public class UserSearchDto : PagingBaseDto
    {
        public int? Id { get; set; }
        public string Search { get; set; }
    }
}
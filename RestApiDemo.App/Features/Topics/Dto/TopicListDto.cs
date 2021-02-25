using System;
using RestApiDemo.App.Features.Users.Dto;

namespace RestApiDemo.App.Features.Topics.Dto
{
    public class TopicListDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string ShortText { get; set; }
        public UserListDto Author { get; set; }
    }
}
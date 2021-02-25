using System;
using RestApiDemo.App.Features.Users.Dto;

namespace RestApiDemo.App.Features.Topics.Dto
{
    public class TopicDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string ShortText { get; set; }

        public UserListDto Author { get; set; }

        public DateTime Date { get; set; }

        public string Category { get; set; }
        public bool IsDraft { get; set; }
    }
}
using System;
using Bogus.DataSets;
using RestApiDemo.App.Features.Users.Dto;

namespace RestApiDemo.App.Features.Topics.Dto
{
    public class CommentListDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public UserListDto Author { get; set; }
    }
}
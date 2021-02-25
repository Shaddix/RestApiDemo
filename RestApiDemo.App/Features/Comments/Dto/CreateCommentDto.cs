using System;

namespace RestApiDemo.App.Features.Topics.Dto
{
    public class CreateCommentDto
    {
        public int TopicId { get; set; }
        public string Text { get; set; }
        public DateTime? Date { get; set; }
    }
}
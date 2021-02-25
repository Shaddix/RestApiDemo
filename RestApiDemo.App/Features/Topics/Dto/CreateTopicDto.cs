using System;

namespace RestApiDemo.App.Features.Topics.Dto
{
    public class CreateTopicDto
    {
        public string Text { get; set; }

        public string Category { get; set; }
        public bool IsDraft { get; set; }

        public DateTime? Date { get; set; }
    }
}
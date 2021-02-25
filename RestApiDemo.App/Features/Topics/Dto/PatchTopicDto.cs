using System;
using RestApiDemo.Domain;
using RestApiDemo.WebApi.PatchRequests;

namespace RestApiDemo.App.Features.Topics.Dto
{
    public class PatchTopicDto : PatchRequest<Topic>
    {
        public string Text { get; set; }
        public string ShortText { get; set; }

        public DateTime Date { get; set; }

        public string Category { get; set; }
        public bool IsDraft { get; set; }
    }
}
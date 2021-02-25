using System;
using RestApiDemo.Domain;
using RestApiDemo.WebApi.PatchRequests;

namespace RestApiDemo.App.Features.Topics.Dto
{
    public class PatchCommentDto : PatchRequest<Comment>
    {
        public string Text { get; set; }
    }
}
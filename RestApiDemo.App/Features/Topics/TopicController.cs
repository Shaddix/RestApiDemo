using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestApiDemo.App.Features.Topics.Dto;
using RestApiDemo.WebApi.Pagination;

namespace RestApiDemo.App.Features.Topics
{
    [ApiController]
    [Route("[controller]")]
    public class TopicController
    {
        private readonly TopicService _topicService;

        public TopicController(TopicService topicService)
        {
            _topicService = topicService;
        }

        [HttpGet()]
        public async Task<PagingList<TopicListDto>> GetAll([FromQuery] TopicSearchDto searchDto)
        {
            return await _topicService.GetAll(searchDto);
        }

        [HttpGet("{id:int}")]
        public async Task<TopicDto> Get(int id)
        {
            return await _topicService.Get(id);
        }

        [HttpPost()]
        public async Task<TopicDto> Create(CreateTopicDto dto)
        {
            return await _topicService.Create(dto);
        }

        [HttpPatch("{id:int}")]
        public async Task<TopicDto> Patch(int id, PatchTopicDto dto)
        {
            return await _topicService.Patch(id, dto);
        }

        [HttpPut("{id:int}")]
        public async Task<TopicDto> Put(int id, PatchTopicDto dto)
        {
            return await _topicService.Patch(id, dto);
        }

        [HttpDelete("{id:int}")]
        public async Task Delete(int id)
        {
            await _topicService.Delete(id);
        }
    }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestApiDemo.App.Features.Topics.Dto;
using RestApiDemo.App.Features.Users.Dto;
using RestApiDemo.WebApi.Pagination;

namespace RestApiDemo.App.Features.Users
{
    [ApiController]
    [Route("[controller]")]
    public class UserController
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }


        [HttpGet()]
        public async Task<PagingList<UserListDto>> GetAll([FromQuery] UserSearchDto searchDto)
        {
            return await _userService.GetAll(searchDto);
        }

        [HttpGet("{id}")]
        public async Task<UserDto> Get(string id)
        {
            return await _userService.Get(id);
        }

        [HttpPost()]
        public async Task<UserDto> Create(CreateUserDto dto)
        {
            return await _userService.Create(dto);
        }

        [HttpPatch("{id:int}")]
        public async Task<UserDto> Patch(string id, PatchUserDto dto)
        {
            return await _userService.Patch(id, dto);
        }

        [HttpDelete("{id:int}")]
        public async Task Delete(string id)
        {
            await _userService.Delete(id);
        }
    }
}
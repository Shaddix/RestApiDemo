using System;
using System.Linq;
using System.Threading.Tasks;
using NeinLinq;
using RestApiDemo.App.Features.Topics.Dto;
using RestApiDemo.App.Features.Users.Dto;
using RestApiDemo.Domain;
using RestApiDemo.Persistence;
using RestApiDemo.WebApi.Pagination;
using RestApiDemo.WebApi.PatchRequests;
using RestApiDemo.WebApi.Specifications;

namespace RestApiDemo.App.Features.Users
{
    public class UserService
    {
        private readonly MainDbContext _mainDbContext;

        public UserService(MainDbContext mainDbContext)
        {
            _mainDbContext = mainDbContext;
        }


        public async Task<PagingList<UserListDto>> GetAll(UserSearchDto searchDto)
        {
            IQueryable<User> query = _mainDbContext.Users
                    .ToEntityInjectable()
                ;

            return await query
                .Select(x => x.ToUserListDto())
                .ToPagingListAsync(searchDto.Offset ?? 0, searchDto.Limit ?? 20, searchDto.Sort,
                    nameof(UserListDto.Id));
        }

        public async Task<UserDto> Get(string id)
        {
            return await _mainDbContext.Users
                .ToEntityInjectable()
                .GetOne(User.HasId(id),
                    x => new UserDto()
                    {
                        Id = x.Id,
                        BirthDate = x.BirthDate,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        FullName = x.FullName,
                        ProfileImage = x.ProfileImage,
                    });
        }

        public async Task<UserDto> Create(CreateUserDto dto)
        {
            User user = new()
            {
                BirthDate = dto.BirthDate,
                ProfileImage = dto.ProfileImage,
                FirstName = dto.FirstName ?? "",
                LastName = dto.LastName ?? "",
            };
            _mainDbContext.Add(user);
            await _mainDbContext.SaveChangesAsync();

            return await Get(user.Id);
        }

        public async Task Delete(string id)
        {
            User user = await _mainDbContext.Users.GetOne(User.HasId(id));
            _mainDbContext.Remove(user);
            await _mainDbContext.SaveChangesAsync();
        }

        public async Task<UserDto> Patch(string id, PatchUserDto dto)
        {
            User user = await _mainDbContext.Users.GetOne(User.HasId(id));
            user.Update(dto);
            await _mainDbContext.SaveChangesAsync();

            return await Get(id);
        }
    }
}
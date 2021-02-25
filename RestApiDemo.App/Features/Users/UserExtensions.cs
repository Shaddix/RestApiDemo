using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NeinLinq;
using RestApiDemo.App.Features.Users.Dto;
using RestApiDemo.Domain;

namespace RestApiDemo.App.Features.Users
{
    public static class UserExtensions
    {
        [InjectLambda]
        public static UserListDto ToUserListDto(this User user)
        {
            return ToUserListDto().Compile().Invoke(user);
        }

        public static Expression<Func<User, UserListDto>> ToUserListDto() =>
            user => new UserListDto
            {
                Id = user.Id,
                ProfileImage = user.ProfileImage,
                FullName = user.FullName,
            };

        public static Expression<Func<User, bool>> SearchByText(string search)
        {
            Expression<Func<User, bool>> predicate = u => true;
            if (string.IsNullOrEmpty(search))
            {
                return predicate;
            }

            string[] splitSearch = search.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in splitSearch)
            {
                predicate = predicate.And(x => EF.Functions.Like(x.FullName, $"%{word}%"));
            }

            return predicate;
        }
    }
}
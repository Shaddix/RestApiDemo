using System;
using System.ComponentModel.DataAnnotations;
using RestApiDemo.Domain;
using RestApiDemo.WebApi.PatchRequests;

namespace RestApiDemo.App.Features.Users.Dto
{
    public class PatchUserDto : PatchRequest<User>
    {
        [MinLength(1)]
        public string FirstName { get; set; }

        [MinLength(1)]
        public string LastName { get; set; }

        public DateTime? BirthDate { get; set; }
    }
}
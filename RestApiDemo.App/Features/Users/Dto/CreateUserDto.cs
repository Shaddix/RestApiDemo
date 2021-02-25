using System;

namespace RestApiDemo.App.Features.Users.Dto
{
    public class CreateUserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string ProfileImage { get; set; }
    }
}
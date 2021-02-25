using System;
using Newtonsoft.Json;
using RestApiDemo.WebApi.Serialization;

namespace RestApiDemo.App.Features.Users.Dto
{
    public class UserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }

        [JsonConverter(typeof(TimezoneIndependentDateTimeConverter))]
        public DateTime? BirthDate { get; set; }

        public DateTime Now { get; set; } = DateTime.UtcNow;
        public string ProfileImage { get; set; }
    }
}
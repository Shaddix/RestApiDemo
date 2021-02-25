using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using NeinLinq;
using RestApiDemo.WebApi.Specifications;

namespace RestApiDemo.Domain
{
    public class User
    {
        public User()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }

        [Required]
        public string FirstName { get; set; } = "";

        [Required]
        public string LastName { get; set; } = "";

        public DateTime? BirthDate { get; set; }
        
        public string ProfileImage { get; set; }

        [InjectLambda]
        public string FullName => FirstName + " " + LastName;

        public static Expression<Func<User, string>> FullNameExpr =>
            u => u.FirstName + " " + u.LastName;
        // public TimeSpan Age => DateTime.Now.Subtract(BirthDate);

        public IList<Comment> Comments { get; set; }

        public IList<Topic> Topics { get; set; }


        public static Specification<User> HasId(string id) =>
            new(
                nameof(HasId),
                p => p.Id == id,
                id);
    }
}
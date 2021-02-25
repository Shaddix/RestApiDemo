using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RestApiDemo.WebApi.Specifications;

namespace RestApiDemo.Domain
{
    public class Topic
    {
        public Topic()
        {
            Date = DateTime.UtcNow;
            Comments = new List<Comment>();
        }

        public int Id { get; private set; }

        public string Text { get; set; }
        public string ShortText { get; set; }

        public User Author { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public DateTime Date { get; set; }

        public string Category { get; set; }
        public bool IsDraft { get; set; }

        public List<Comment> Comments { get; set; }


        public static Specification<Topic> HasId(int id) =>
            new(
                nameof(HasId),
                p => p.Id == id,
                id);
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using RestApiDemo.WebApi.Specifications;

namespace RestApiDemo.Domain
{
    public class Comment
    {
        public int Id { get; set; }
        public Topic Topic { get; set; }
        public int TopicId { get; set; }

        public User Author { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public string Text { get; set; }
        public DateTime Date { get; set; }

        public static Specification<Comment> HasId(int id) =>
            new(
                nameof(HasId),
                p => p.Id == id,
                id);

        public static Specification<Comment> HasTopicId(int topicId) =>
            new(
                nameof(HasTopicId),
                p => p.TopicId == topicId,
                topicId);
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace QandA.Data.Models
{
    public class QuestionPostRequest
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please include some content for the question")]
        public string Content { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public DateTime Created { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace TestQuestion.Models
{
    public class RequestModel
    {
        [Required]
        public string topic { get; set; }  
        public string question { get; set; }
    }
}

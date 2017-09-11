using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace yamvc.Models
{
    public class UserModel
    {
        [Required]
        public string Login { get; set; }
        [Required]
        [JsonIgnore]
        public string Password { get; set; }
    }
}

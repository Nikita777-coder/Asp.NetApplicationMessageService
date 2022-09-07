using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ServiceOfMessages.Controllers.Models
{
    /// <summary>
    /// Creating user
    /// </summary>
    public class User
    {
        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// User email
        /// </summary>
        [Required]
        public string Email { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ServiceOfMessages.Controllers.Models
{
    /// <summary>
    /// Класс для создания пользователя.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Св-во для имени пользователя.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Св-во для почты пользователя.
        /// </summary>
        [Required]
        public string Email { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ServiceOfMessages.Controllers.Models
{
    /// <summary>
    /// Класс для создания сообщения между пользователями.
    /// </summary>
    public class MessageOne
    {
        /// <summary>
        /// Св-во для темы сообщения.
        /// </summary>
        [Required]
        public string Subject { get; set; }

        /// <summary>
        /// Св-во для сообщения.
        /// </summary>
        [Required]
        public string Message { get; set; }

        /// <summary>
        /// Св-во для пользователя-отправителя.
        /// </summary>
        [Required]
        public string SenderId { get; set; }

        /// <summary>
        /// Св-во для пользователя-получателя.
        /// </summary>
        [Required]
        public string ReceiverId { get; set; }
    }
}
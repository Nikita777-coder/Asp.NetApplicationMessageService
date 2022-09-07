using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ServiceOfMessages.Controllers.Models
{
    /// <summary>
    /// Creating message between users.
    /// </summary>
    public class MessageOne
    {
        /// <summary>
        /// Message Subject
        /// </summary>
        [Required]
        public string Subject { get; set; }

        /// <summary>
        /// User message
        /// </summary>
        [Required]
        public string Message { get; set; }

        /// <summary>
        /// User sender id 
        /// </summary>
        [Required]
        public string SenderId { get; set; }

        /// <summary>
        /// User receiver id
        /// </summary>
        [Required]
        public string ReceiverId { get; set; }
    }
}
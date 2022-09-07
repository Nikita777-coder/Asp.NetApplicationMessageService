using Microsoft.AspNetCore.Mvc;
using ServiceOfMessages.Controllers.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.Json;
using System.IO;

namespace ServiceOfMessages.Controllers
{
    /// <summary>
    /// Определение нужных атрибутов для контроллера и самого контроллера.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ServiceOfMessages : Controller
    {
        private static List<User> users=new();
        private static List<MessageOne> messages=new();
        private Random random = new();

        /// <summary>
        /// Database cleaning
        /// </summary>
        /// <returns>info about successful cleaning or error 404</returns>
        [HttpPost("POST/ClearDatabase")]
        public IActionResult ClearJsonFiles()
        {
            try
            {
                using StreamWriter sw = new("users.json"), sw1 = new("messages.json");
                sw.Flush();
                sw1.Flush();
                return Ok("Database was cleaned");
            }

            catch(Exception)
            {
                return BadRequest("Database can't be cleaned");
            }
        }
        /// <summary>
        /// Инициализация списка пользователей и сообщений с помощью Random.
        /// </summary>
        /// <returns>сообщение об успешной инициализации списков.</returns>
        [HttpPost("POST/Initialization")]
        public IActionResult Post()
        {
            // Десереализаиция пользоватлей и сообщений.
            try
            {
                using StreamReader sw = new("users.json"), sw1 = new("messages.json");
                string jsonTextUsers = sw.ReadToEnd(), jsonTextMessages = sw1.ReadToEnd();
                users = JsonSerializer.Deserialize<List<User>>(jsonTextUsers);
                messages = JsonSerializer.Deserialize<List<MessageOne>>(jsonTextMessages);
            }

            catch(Exception)
            {

            }
            var counUsers = random.Next(1, 100);
            // Экземпляр класса для генерации случайной почты пользователя.
            GenerationOfEmail goe = new();
            // Инициализация пользователей.
            for (int i =0; i < counUsers; ++i)
            {
                string[] email = new string[3];
                
                email[0] = goe.GenerateLoginAndDomainName();
                email[1] = goe.GenerateLoginAndDomainName(true);
                email[2] = goe.GenerateRegion();
                
                users.Add(new User{Email = String.Join("",email), UserName = email[0][0..^1]});
            }

            // Сортировка пользователей по почте.
            users = users.OrderBy(el => el.Email).ToList();

            // Сериализация пользователей.
            try
            {
                var text = JsonSerializer.Serialize(users);
                using StreamWriter sw = new("users.json");
                sw.Write(text);
            }

            catch(Exception)
            {

            }

            // Экземпляр класса для генерации случайной темы и сообщения между пользователями.
            GenerationOfSubjectAndMessage gosam = new();
            // Инициализация сообщений.
            foreach (var sender in users)
            {
                foreach (var receiver in users)
                {
                    messages.Add(new MessageOne
                    {
                        Subject = gosam.GenerateSubject(), Message = gosam.GenerateMessage(),
                        ReceiverId = receiver.Email, SenderId = sender.Email
                    });
                }
            }

            // Сериализация сообщений.
            try
            {
                var text = JsonSerializer.Serialize(messages);
                using StreamWriter sw = new("messages.json");
                sw.Write(text);
            }

            catch (Exception)
            {

            }

            return Ok("List of users and message list were successful initialized");
        }

        /// <summary>
        /// User adding.
        /// </summary>
        /// <param name="email">new user email</param>
        /// <param name="name">new user name</param>
        /// <returns>info about successful user adding or 400 error with error description.</returns>>
        [HttpPost("POST/AddUser/{email}")]
        public IActionResult Post(string email, string name = "")
        {
            if (users.Select(el => el.Email).Contains(email))
                return BadRequest("User with the same mail already exist");

            // По умолчанию имя пользователя такое же, как и логин почты.
            var nameOfUser = name == "" ? email.Split("@")[0] : name;

            users.Add(new User { Email = email, UserName = nameOfUser });
            users = users.OrderBy(el => el.Email).ToList();
            return Ok($"User with mail - {email} - successfully added.");
        }

        /// <summary>
        /// New message adding to service.
        /// </summary>
        /// <param name="senderEmail">user-sender mail</param>
        /// <param name="receiverEmail">user-receiver</param>
        /// <param name="message">new message</param>
        /// <param name="subject">new message theme</param>
        /// <returns>info about successful message adding or 400 error with error description.</returns>
        [HttpPost("POST/AddMessage/{senderEmail}/{receiverEmail}/{message}")]
        public IActionResult Post(string senderEmail, string receiverEmail, string message, string subject="")
        {
            if (!users.Select(el => el.Email).Contains(senderEmail))
                return NotFound("Entered user-sender is not in list");
            if (!users.Select(el => el.Email).Contains(receiverEmail))
                return NotFound("Entered user-receiver is not in list");

            // Генерация темы сообщения в случае, если она оказалась не указана.
            var subj = subject == "" ? new GenerationOfSubjectAndMessage().GenerateSubject() : subject;

            messages.Add(new MessageOne { Message = message, Subject = subj, ReceiverId = receiverEmail, SenderId
                = senderEmail });

            return Ok($"Message from {senderEmail} to {receiverEmail} was added successfully");
        }

        /// <summary>
        /// Getting user list who register in service.
        /// </summary>
        /// <returns>user list who register in service</returns>
        [HttpGet("GET/ListOfUsers")]
        public IEnumerable<User> Get() => users;

        /// <summary>
        /// Getting all messages from service.
        /// </summary>
        /// <returns>all messages from service</returns>
        [HttpGet("GET/ListOfMessagesBetweenUsers")]
        public IEnumerable<MessageOne> ListOfMessagesBetweenUsers() => messages;

        /// <summary>
        /// Getting all users from file.
        /// </summary>
        /// <returns>list of users from file</returns>
        [HttpGet("GET/UsersFromDatabase")]
        public List<User> UsersFromJson()
        {
            try
            {
                using StreamReader sw = new("users.json"); // file located where .exe;
                string jsonTextUsers = sw.ReadToEnd();
                return JsonSerializer.Deserialize<List<User>>(jsonTextUsers);
            }

            catch(Exception)
            {
                return new List<User>();
            }
        }

        /// <summary>
        /// Getting all messages from file.
        /// </summary>
        /// <returns>message list from file</returns>
        [HttpGet("GET/MessagesFromDatabase")]
        public List<MessageOne> MessagesFromJson()
        {
            try
            {
                using StreamReader sw = new("messages.json"); // file located where .exe;
                string jsonTextMessages = sw.ReadToEnd();
                return JsonSerializer.Deserialize<List<MessageOne>>(jsonTextMessages);
            }

            catch (Exception)
            {
                return new List<MessageOne>();
            }
        }

        /// <summary>
        /// Searching user by his email.
        /// </summary>
        /// <param name="email">user-mail</param>
        /// <returns>info about successful user searching or 400 error with error description.</returns>
        [HttpGet("GET/FindUser/{email}")]
        public IActionResult Get(string email)
        {
            var user = users.Find(el => el.Email == email);
            if (user == null)
                return NotFound("User wasn't found in server");
            return Ok(user);
        }

        /// <summary>
        /// Getting all messages between users.
        /// </summary>
        /// <param name="senderId">user-sender email</param>
        /// <param name="receiverId">user-receiver email</param>
        /// <returns>list of messages between users or 404 error with description</returns>
        [HttpGet("GET/GetMessageBySenderAndReceiverEmails/{senderId}/{receiverId}")]
        public IActionResult Get(string senderId, string receiverId)
        {
            var result = messages.FindAll(el => el.ReceiverId == receiverId && el.SenderId == senderId);
            if (result != null)
            {
                return Ok(result.Select(el => el.Message));
            }

            List<string> receivers = messages.Select(el => el.ReceiverId).ToList(), senders = messages.Select
                (el => el.SenderId).ToList();
            if (!receivers.Contains(receiverId))
            {
                return NotFound("В спсике отсутствует данный получатель");
            }

            return NotFound("В спсике отсутствует данный отправитель");
        }

        /// <summary>
        /// Getting all messages which were sent by user.
        /// </summary>
        /// <param name="senderId">user-sender</param>
        /// <returns>all messages which were sent by user</returns>
        [HttpGet("GET/GetBySenderEmail/{senderId}")]
        public IActionResult GEt(string senderId)
        {
            var messagesOfSenderId = messages.Where(el => el.SenderId == senderId).Select(el => el.Message).ToList();

            if (messagesOfSenderId.Count == 0)
                return Ok("This user didn't send any messsage");
            return Ok(messagesOfSenderId);
        }

        /// <summary>
        /// Getting all messages which were sent to user.
        /// </summary>
        /// <param name="receiverId">user-receiver mail</param>
        /// <returns>all messages which were sent to user.</returns>
        [HttpGet("GET/GetByReceiverEmail/{receiverId}")]
        public IActionResult GET(string receiverId)
        {
            var messagesOfReceiverId = messages.Where(el => el.ReceiverId == receiverId).Select(el => el.Message).ToList();

            if (messagesOfReceiverId.Count == 0)
                return Ok("This user didn't received any message");
            return Ok(messagesOfReceiverId);
        }

        /// <summary>
        /// Getting range of users.
        /// </summary>
        /// <param name="limit">user list length limit</param>
        /// <param name="offset">begin user id</param>
        /// <returns>list of users or bad request with description.</returns>
        [HttpGet("GET/GetLimitUsersFromOffset/{limit}/{offset}")]
        public IActionResult Get(int limit, int offset)
        {
            if (limit <= 0)
                return BadRequest($"Limit must be > 0");
            if (offset < 0)
                return BadRequest("Offset must be >= 0");
            if (offset > users.Count - 1)
                return BadRequest("Outrange offset");

            var selectedUsers = new List<User>();
            int countOfUsers = 0;
            for (int i = offset; i < users.Count; ++i)
            {
                if (countOfUsers == limit)
                    break;
                selectedUsers.Add(users[i]);
                countOfUsers++;
            }
            return Ok(selectedUsers);
        }
    }
}
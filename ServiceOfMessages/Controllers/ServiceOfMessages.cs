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
        /// Очищение Json файлов.
        /// </summary>
        /// <returns>информацию об умпешном очищении файлов или ошибку 404</returns>
        [HttpPost("POST/ClearJsonFiles")]
        public IActionResult ClearJsonFiles()
        {
            try
            {
                using StreamWriter sw = new("users.json"), sw1 = new("messages.json");
                sw.Flush();
                sw1.Flush();
                return Ok("Файлы очищены");
            }

            catch(Exception)
            {
                return BadRequest("Файлы не могут быть почищены.");
            }
        }
        /// <summary>
        /// Инициализация списка пользователей и сообщений с помощью Random.
        /// </summary>
        /// <returns>сообщение об успешной инициализации списков.</returns>
        [HttpPost("POST/initialization")]
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

            return Ok("Список пользователей и сообщений успешно инициализированы.");
        }

        /// <summary>
        /// Добавление пользователя.
        /// </summary>
        /// <param name="email">почта новго пользователя</param>
        /// <param name="name">имя новго пользователя</param>
        /// <returns>информацию об успешном добавлении пользователя или ошибку 400 с описанием неккоректности 
        /// для умпешного добавления пользователя</returns>
        [HttpPost("POST/AddUser/{email}")]
        public IActionResult Post(string email, string name = "")
        {
            if (users.Select(el => el.Email).Contains(email))
                return BadRequest("Пользователь с такой почтой уже существует.");

            // По умолчанию имя пользователя такое же, как и логин почты.
            var nameOfUser = name == "" ? email.Split("@")[0] : name;

            users.Add(new User { Email = email, UserName = nameOfUser });
            users = users.OrderBy(el => el.Email).ToList();
            return Ok($"Пользователь с почтой - {email} - успешно добавлен.");
        }

        /// <summary>
        /// Добавление новго сообщения в сервис.
        /// </summary>
        /// <param name="senderEmail">почта пользователя-отправителя</param>
        /// <param name="receiverEmail">почта пользователя-получателя</param>
        /// <param name="message">новое сообщение</param>
        /// <param name="subject">тема новго сообщения</param>
        /// <returns>информацию об успешном добавлении сообщения в сервис или ошибку 404, если не была найдена почта пользователя
        /// -отправителя или пользователя-получателя</returns>
        [HttpPost("POST/AddMessage/{senderEmail}/{receiverEmail}/{message}")]
        public IActionResult Post(string senderEmail, string receiverEmail, string message, string subject="")
        {
            if (!users.Select(el => el.Email).Contains(senderEmail))
                return NotFound("В списке пользователей отсутствует введённый отправитель.");
            if (!users.Select(el => el.Email).Contains(receiverEmail))
                return NotFound("В списке пользователей отсутствует введённый получатель.");

            // Генерация темы сообщения в случае, если она оказалась не указана.
            var subj = subject == "" ? new GenerationOfSubjectAndMessage().GenerateSubject() : subject;

            messages.Add(new MessageOne { Message = message, Subject = subj, ReceiverId = receiverEmail, SenderId
                = senderEmail });

            return Ok($"Сообщение от {senderEmail} до {receiverEmail} успешно добавлено.");
        }

        /// <summary>
        /// Получение списка пользователей, зарегистрированных на сервере.
        /// </summary>
        /// <returns>список пользователей, зарегистрированных на сервере</returns>
        [HttpGet("GET/ListOfUsers")]
        public IEnumerable<User> Get() => users;

        /// <summary>
        /// Получение списка сообщений, отправленных на сервер.
        /// </summary>
        /// <returns>список сообщений, отправленных на сервер</returns>
        [HttpGet("GET/ListOfMessagesBetweenUsers")]
        public IEnumerable<MessageOne> ListOfMessagesBetweenUsers() => messages;

        /// <summary>
        /// Получение пользователей, хранящихся в файле users.json (файл располагается там же, где exe).
        /// </summary>
        /// <returns>список пользователей, хранящихся в файле users.json</returns>
        [HttpGet("GET/UsersFromJson")]
        public List<User> UsersFromJson()
        {
            try
            {
                using StreamReader sw = new("users.json");
                string jsonTextUsers = sw.ReadToEnd();
                return JsonSerializer.Deserialize<List<User>>(jsonTextUsers);
            }

            catch(Exception)
            {
                return new List<User>();
            }
        }

        /// <summary>
        /// Получение сообщений, хранящихся в файле messages.json (файл располагается там же, где exe).
        /// </summary>
        /// <returns>список сообщений, хранящихся в файле messages.json</returns>
        [HttpGet("GET/MessagesFromJson")]
        public List<MessageOne> MessagesFromJson()
        {
            try
            {
                using StreamReader sw = new("messages.json");
                string jsonTextMessages = sw.ReadToEnd();
                return JsonSerializer.Deserialize<List<MessageOne>>(jsonTextMessages);
            }

            catch (Exception)
            {
                return new List<MessageOne>();
            }
        }

        /// <summary>
        /// Посик пользователя на сервере по его почте.
        /// </summary>
        /// <param name="email">почта пользователя</param>
        /// <returns>информацию о пользователе, если был такой найден или сообщение о том, что такого пользователя не 
        /// существует</returns>
        [HttpGet("GET/FindUser/{email}")]
        public IActionResult Get(string email)
        {
            var user = users.Find(el => el.Email == email);
            if (user == null)
                return NotFound("Такого пользователя нет на сервере.");
            return Ok(user);
        }

        /// <summary>
        /// Получение всех сообщений между заданными пользователями.
        /// </summary>
        /// <param name="senderId">почта пользователя-отправителя</param>
        /// <param name="receiverId">почта пользователя-получателя</param>
        /// <returns>список сообщений между заданными пользователями или ошибку 404 о том, что не был найден 
        /// пользователь-отправитель или пользователь-получатель</returns>
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
        /// Получение всех сообщений, отправленных пользователем.
        /// </summary>
        /// <param name="senderId">почта пользователя-отпарвителя</param>
        /// <returns>все сообщения, отправленные пользователем, или информацию о том, что у заданного пользователя-отпарвителя 
        /// отсутствуют сообщения</returns>
        [HttpGet("GET/GetBySenderEmail/{senderId}")]
        public IActionResult GEt(string senderId)
        {
            var messagesOfSenderId = messages.Where(el => el.SenderId == senderId).Select(el => el.Message).ToList();

            if (messagesOfSenderId.Count == 0)
                return Ok("У заданного отправителя отсутсвуют сообщения");
            return Ok(messagesOfSenderId);
        }

        /// <summary>
        /// Получение всех сообщений, отправленных пользователю.
        /// </summary>
        /// <param name="receiverId">почта пользователя-получателя</param>
        /// <returns>все сообщения, отправленные пользователю, или информацию о том, что у заданного пользователя-получателя 
        /// отсутствуют сообщения</returns>
        [HttpGet("GET/GetByReceiverEmail/{receiverId}")]
        public IActionResult GET(string receiverId)
        {
            var messagesOfReceiverId = messages.Where(el => el.ReceiverId == receiverId).Select(el => el.Message).ToList();

            if (messagesOfReceiverId.Count == 0)
                return Ok("У заданного отправителя отсутсвуют сообщения");
            return Ok(messagesOfReceiverId);
        }

        /// <summary>
        /// Получение ограниченного кол-ва пользователей, начиная с некоторого пользователя.
        /// </summary>
        /// <param name="limit">ограничение по кол-ву пользователй</param>
        /// <param name="offset">индекс пользователя, с которого надо вывести ограниченное кол-во пользоватлей</param>
        /// <returns>возможных пользователей с заданнного или ошибку 400 с указанием информации об ошибки</returns>
        [HttpGet("GET/GetLimitUsersFromOffset/{limit}/{offset}")]
        public IActionResult Get(int limit, int offset)
        {
            if (limit <= 0)
                return BadRequest($"Limit должно быть > 0");
            if (offset < 0)
                return BadRequest("Offset должно быть >= 0");
            if (offset > users.Count - 1)
                return BadRequest("Offset превышает максимальный индекс списка пользователей");

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
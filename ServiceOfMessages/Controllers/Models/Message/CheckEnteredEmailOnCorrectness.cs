using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceOfMessages.Controllers.Models
{
    /// <summary>
    /// Класс для проверки введённой почты.
    /// </summary>
    public class CheckEnteredEmailOnCorrectness
    {
        [EmailAddress]
        private string email = null;

        /// <summary>
        /// Конструктор для получения заданной почты.
        /// </summary>
        /// <param name="email">введённая почта</param>
        public CheckEnteredEmailOnCorrectness(string email) => this.email = email;

        /// <summary>
        /// Проверка заданной почты.
        /// </summary>
        /// <returns>true, если почта корректна, false иначе</returns>
        public bool IsEnteredEmailCorrect()
        {
            try
            {
                if (email == null)
                    return false;
                if (email.Split("@")[0] == "")
                    return false;
                if (email.Split(".")[0].Split("@")[1] == "")
                    return false;
                if (email.Split(".")[1] == "")
                    return false;
                return true;
            }

            catch(Exception)
            {
                return false;
            }
        }
    }
}
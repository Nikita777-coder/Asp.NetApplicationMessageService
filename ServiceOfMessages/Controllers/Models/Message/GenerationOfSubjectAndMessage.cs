using System;
using System.Text;

namespace ServiceOfMessages.Controllers.Models
{
    /// <summary>
    /// Класс для генерации сообщения и его темы.
    /// </summary>
    public class GenerationOfSubjectAndMessage
    {
        private Random random = new();
        private StringBuilder outputStr;
        private string validSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        
        /// <summary>
        /// Генерация темы.
        /// </summary>
        /// <returns>сгенерированную тему</returns>
        public string GenerateSubject()
        {
            outputStr = new StringBuilder(random.Next(1, 50));
            
            outputStr = new StringBuilder(random.Next(1, 50));
            for (int j = 0; j < outputStr.Capacity; ++j)
                outputStr.Append(validSymbols[random.Next(1, validSymbols.Length)]);

            return outputStr.ToString();
        }

        /// <summary>
        /// Генерация сообщения.
        /// </summary>
        /// <returns>сгенерированное сообщение</returns>
        public string GenerateMessage()
        {
            outputStr = new StringBuilder(random.Next(1, 500));
            for (int j = 0; j < outputStr.Capacity; ++j)
                outputStr.Append(validSymbols[random.Next(1, validSymbols.Length)]);

            return outputStr.ToString();
        }
    }
}
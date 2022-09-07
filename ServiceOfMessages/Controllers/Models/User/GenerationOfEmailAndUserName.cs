using System;
using System.Text;

namespace ServiceOfMessages.Controllers.Models
{
    /// <summary>
    /// Класс для генерации почты пользователя.
    /// </summary>
    class GenerationOfEmail
    {
        private Random random = new();
        private string validSymbols;
        private StringBuilder outputString;
        
        /// <summary>
        /// Генерация логина и домена.
        /// </summary>
        /// <param name="domainName">метот нужен для генерации домена?</param>
        /// <returns>сгенерированный логин или домен.</returns>
        public string GenerateLoginAndDomainName(bool domainName=false)
        {
            validSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            outputString = new StringBuilder(random.Next(1, 64));
    
            for (int i = 0; i < outputString.Capacity; ++i)
            {
                // Отбор одного из валидных символов для логина или домена.
                outputString.Append(validSymbols[random.Next(1, validSymbols.Length)]);
            }
            
            if (domainName)
                outputString.Append(".");
            else
                outputString.Append("@");
    
            return outputString.ToString();
        }
        
        /// <summary>
        /// Генерация региона почты.
        /// </summary>
        /// <returns>сгенерированный регион.</returns>
        public string GenerateRegion()
        {
            validSymbols = "abcdefghijklmnopqrstuvwxyz";
            outputString = new StringBuilder(random.Next(2, 4));
            
            for (int i = 0; i < outputString.Capacity; ++i)
            {
                outputString.Append(validSymbols[random.Next(1, validSymbols.Length)]);
            }
    
            return outputString.ToString();
        }
    }
}
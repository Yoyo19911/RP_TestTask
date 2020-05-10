using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestTaskServer.Utils;

namespace TestTaskServer.Pages
{
    /// <summary>
    /// Предоставляет интерфейс взаимодействия с сервером
    /// </summary>
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class IndexModel : PageModel
    {
        /// <summary>Коллекция сообщений, присланных от клиента(ов)</summary>
        public List<Message> Messages;

        /// <summary>
        /// Отображает страницу с сообщениями, загруженными из БД
        /// </summary>
        public void OnGet()
        {
            using (var db = new ApplicationContext())
            {
                Messages = db.Messages.ToList();
            }
        }


        /// <summary>
        /// Принимает текстовое сообщение от клиентов POST-запросом
        /// </summary>
        /// <param name="message">Содержание сообщения</param>
        /// <returns>Возвращает статус "ОК", если в процессе выполнения не возникло ошибок</returns>
        public IActionResult OnPost(string message)
        {
            using (var db = new ApplicationContext())
            {
                db.Messages.Add(new Message()
                {
                    Text = message,
                    ReceiveDate = DateTime.Now,
                    SenderIp = GetIp()
                });
                db.SaveChanges();
            }

            return StatusCode(200);
        }

        /// <summary>
        /// Получает IP отправителя http-запроса
        /// </summary>
        /// <remarks>
        /// Этот метод, как и известные его автору аналоги, не дает надежного результата.
        /// </remarks>
        string GetIp() =>
            HttpContext.Connection.RemoteIpAddress.ToString();
    }
}

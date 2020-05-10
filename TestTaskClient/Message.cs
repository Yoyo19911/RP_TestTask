using System;
using System.Collections.Generic;
using System.Text;

namespace TestTaskClient
{
    /// <summary>
    /// Сообщение, посылаемое пользователем на сервер
    /// </summary>
    public class Message
    {
        /// <summary>Уникальный идентификатор</summary>
        public int Id { get; set; }
        
        /// <summary>Текст посылаемого сообщения</summary>
        public string Text { get; set; }

        /// <summary>Определяет было ли успешно отправлено сообщение на сервер</summary>
        public bool IsSended { get; set; }
    }
}

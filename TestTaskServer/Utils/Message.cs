using System;

namespace TestTaskServer.Utils
{
    /// <summary>
    /// Текстовое сообщение, присылаемое на сервер
    /// </summary>
    public class Message
    {
        /// <summary>Уникальный идентификатор</summary>
        public int Id { get; set; }

        /// <summary>Содержание сообщения</summary>
        public string Text { get; set; }

        /// <summary>Дата получения сервером сообщения</summary>
        public DateTime ReceiveDate { get; set; }

        /// <summary>IP адрес отправителя</summary>
        public string SenderIp { get; set; }
    }
}
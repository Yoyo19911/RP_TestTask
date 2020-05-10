using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using System.Net.Sockets;

namespace TestTaskClient
{
    class Program
    {
        static HttpClient client;
        static string serverUri = "https://localhost:44351/";
        static int sendInterval = 5000; //в миллисекундах
        static bool closeProgram = false; //флаг, обозначающий, что пользователь дал команду закрыть приложение

        /// <summary>
        /// Очередь сообщений на отправку
        /// </summary>
        static AsyncQueue<Message> messagesToSend = new AsyncQueue<Message>();

        static async Task Main(string[] args)
        {
            InitialSetup();

            var askMessage = Task.Run(AskingUserForMessages);
            var sendMessage = Task.Run(TryingToSendMessagesAsync);

            await Task.WhenAll(askMessage, sendMessage);
        }

        /// <summary>
        /// Осуществляет первоначальную настройку, загружает необходимые данные
        /// </summary>
        private static void InitialSetup()
        {
            Console.WriteLine("Загрузка..");

            client = new HttpClient() { BaseAddress = new Uri(serverUri) };
            LoadMessages();

            Console.WriteLine("Загрузка завершена");
            Console.WriteLine("*** Обратите внимание, что пустое сообщение будет интерпретироваться как команда для выхода из приложения");
        }

        /// <summary>
        /// Загружает данные о сообщениях из базы
        /// </summary>
        private static void LoadMessages()
        {
            using (var db = new ApplicationContext())
            {
                foreach (var message in db.Messages.Where(x => !x.IsSended))
                    messagesToSend.Enqueue(message);
            }
        }

        /// <summary>
        /// Бесконечно запрашивает у пользователя сообщения для отправки
        /// </summary>
        static void AskingUserForMessages()
        {
            while (true)
            {
                Console.WriteLine("Введите сообщение для отправки на сервер: ");
                var messageText = Console.ReadLine();
                if (messageText == string.Empty)
                {
                    closeProgram = true;
                    break;
                }
                var newMessage = new Message() { Text = messageText };

                SaveMessage(newMessage);
                messagesToSend.TryEnqueue(newMessage);
            }
        }

        /// <summary>
        /// Бесконечно пытается отправлять сообщения в порядке их поступления
        /// </summary>
        /// <remarks>
        /// Предполагается, что содержание сообщения не может вызвать ошибку на сервере, в противном случае реализацию надо изменить
        /// </remarks>
        static async Task TryingToSendMessagesAsync()
        {
            while (!closeProgram)
            {
                while (messagesToSend.TryPeek(out var userMessage)
                    && await SendMessageAsync(userMessage))
                {
                    MarkMessageSended(userMessage);
                    await messagesToSend.DequeueAsync();
                }

                Thread.Sleep(sendInterval);
            }
        }

        /// <summary>
        /// Отправляет сообщение на сервер
        /// </summary>
        static async Task<bool> SendMessageAsync(Message message)
        {
            try
            {
                var values = new Dictionary<string, string> { { "message", message.Text } };

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync("index", content);
                var success = response.IsSuccessStatusCode;

                if (!success)
                    Console.WriteLine($"Произошла ошибка {response.StatusCode} при отправке сообщения на сервер");

                return success;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Произошла ошибка при отправке сообщения на сервер: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Сохраняет новый экземпляр неотправленного сообщения в БД
        /// </summary>
        static void SaveMessage(Message message)
        {
            using (var db = new ApplicationContext())
            {
                db.Messages.Add(message);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Помечает в БД сообщение как отправленное
        /// </summary>
        static void MarkMessageSended(Message message)
        {
            using (var db = new ApplicationContext())
            {
                var dbMessage = db.Messages.FirstOrDefault(x => x.Id == message.Id);
                if (dbMessage != null)
                    dbMessage.IsSended = true;
                db.SaveChanges();
            }
        }
    }
}


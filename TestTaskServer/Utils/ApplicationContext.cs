using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestTaskServer.Utils
{
    /// <summary>
    /// Класс контекста для работы с EntityFramework
    /// </summary>
    public class ApplicationContext : DbContext
    {
        /// <summary>Коллекция сообщений, присланных от клиента(ов)</summary>
        public DbSet<Message> Messages { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=rpserverdb;Trusted_Connection=True;");
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Shared.Model.Entity;

namespace MagicOnionServer.Model.Context
{
    public class GameDbContext : DbContext
    {
        //------------------------------------------------------
        //テーブルを追加したら書き足す

        public DbSet<User> Users { get; set; }



        //------------------------------------------------------
       
        //接続先を規定
        readonly string connectionString = "server=localhost;database=realtime_game;user=jobi;password=jobi;";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0)));
        }
    }
}

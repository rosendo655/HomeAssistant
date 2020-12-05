using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTService.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int ChatId { get; set; }
        public string Token { get; set; }

        public static User FromDataReader(MySqlDataReader reader)
        {
            User user = new User()
            {
                UserId = reader.GetInt32("userId"),
                Username = reader.GetString("username"),
                ChatId = reader.GetInt32("chatId"),
                Token = reader.GetString("Token")
            };
            return user;
        }

        public static string InsertQuery(User usr)
        {
            return $"INSERT INTO `user` (`username`, `chatId`, `token`) VALUES ('{usr.Username}', '{usr.ChatId}', '{usr.Token}');";
        }
    }
}

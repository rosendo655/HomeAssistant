using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTService.Models
{
    public class Device
    {
        public int DeviceId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }

        public string KeyItem => $"{DeviceId} - {Name}";


        public static Device FromReader(MySqlDataReader reader)
        {
            return new Device()
            {
                DeviceId = reader.GetInt32("deviceId"),
                UserId = reader.GetInt32("userId"),
                Name = reader.GetString("name"),

            };
        }

        public static string DeleteQuery(Device dev)
            => $"DELETE FROM `device` WHERE  `deviceId`={dev.DeviceId};";

        public static string UpdateQuery(Device dev)
            => $"UPDATE `device` SET `name`='{dev.Name}' WHERE  `deviceId`={dev.DeviceId};";
        

        public static string InsertQuery(Device dev)
            => $"INSERT INTO `device` (`userId`, `name`) VALUES ('{dev.UserId}', '{dev.Name}');";
        
    }
}

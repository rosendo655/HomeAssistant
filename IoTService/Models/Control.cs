using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTService.Models
{
    public class Control
    {
        public int ControlId { get; set; }
        public int DeviceId { get; set; }
        public string Name { get; set; }

        public string KeyItem => $"{ControlId} - {Name}";

        public static Control FromReader(MySqlDataReader reader) =>
            new Control()
            {
                ControlId = reader.GetInt32("controlId"),
                DeviceId = reader.GetInt32("deviceId"),
                Name = reader.GetString("name"),
            };

        public static string DeleteQuery(Control control)
            => $"DELETE FROM `control` WHERE  `controlId`={control.ControlId};";

        public static string InsertQuery(Control control)
            => $"INSERT INTO `control` (`deviceId`, `name`) VALUES ('{control.DeviceId}', '{control.Name}');";

        public static string UpdateQuery(Control control)
            => $"UPDATE `control` SET `name`='{control.Name}' WHERE  `controlId`={control.ControlId};";
    }
}

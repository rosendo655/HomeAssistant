using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTService.Models
{
    public class ControlQueue
    {
        public int ControlQueueId { get; set;}
        public int ControlId { get; set; }
        public int Value { get; set; }
        public DateTime InsertedAt { get; set; }


        public static ControlQueue FromReader(MySqlDataReader reader)
            => new ControlQueue()
            {
                ControlQueueId = reader.GetInt32("controlQueueId"),
                ControlId = reader.GetInt32("controlId"),
                Value = reader.GetInt32("value"),
                InsertedAt = reader.GetDateTime("insertedAt")
            };

        public static string InsertQuery(ControlQueue control)
            => $"INSERT INTO `controlqueue` (`controlId`, `value`, `insertedAt`) VALUES ('{control.ControlId}', '{control.Value}', '{control.InsertedAt.ToString("u")}');";

        public static string DeleteQuery(ControlQueue control)
            => $"DELETE FROM `controlqueue` WHERE  `controlQueueId`={control.ControlQueueId};";

        public static string UpdateQuery(ControlQueue control)
            => $"UPDATE `controlqueue` SET `value`='{control.Value}', `insertedAt`='{control.InsertedAt.ToString("u")}' WHERE  `controlQueueId`={control.ControlQueueId};";
    }
}

using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTService.Models
{
    public class Watch
    {
        public int WatchId { get; set; }
        public int MetricId { get; set; }
        public string Name { get; set; }
        public string Expression { get; set; }
        public string Message { get; set; }
        public byte Sent { get; set; }

        public string KeyItem => $"{WatchId} - {Name}";


        public static Watch FromReader(MySqlDataReader reader) =>
            new Watch()
            {
                WatchId = reader.GetInt32("watchId"),
                MetricId = reader.GetInt32("metricId"),
                Name = reader.GetString("name"),
                Expression = reader.GetString("expression"),
                Message = reader.GetString("message"),
                Sent = reader.GetByte("sent")
            };

        public static string InsertQuery(Watch watch)
            => $"INSERT INTO `watch` (`metricId`, `name`, `expression`, `message`, `sent`) VALUES ('{watch.MetricId}', '{watch.Name}', '{watch.Expression}', '{watch.Message}', b'{watch.Sent}');";

        public static string UpdateQuery(Watch watch)
            => $"UPDATE `watch` SET `name`='{watch.Name}', `expression`='{watch.Expression}', `message`='{watch.Message}', `sent`=b'{watch.Sent}' WHERE  `watchId`={watch.WatchId};";

        public static string DeleteQuery(Watch watch)
        => $"DELETE FROM `watch` WHERE  `watchId`={watch.WatchId};";
    }
}

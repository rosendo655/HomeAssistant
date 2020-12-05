using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTService.Models
{
    public class Metric
    {
        public int MetricId { get; set; }
        public int DeviceId { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }

        public string KeyItem => $"{MetricId} - {Name}";


        public static Metric FromReader(MySqlDataReader reader)
        {
            return new Metric()
            {
                MetricId = reader.GetInt32("metricId"),
                DeviceId = reader.GetInt32("deviceId"),
                Name = reader.GetString("name"),
                Unit = reader.GetString("unit")

            };
        }

        public static string InsertQuery(Metric metric)
        {
            return $"INSERT INTO `metric` (`deviceId`, `name`, `unit`) VALUES ('{metric.DeviceId}', '{metric.Name}', '{metric.Unit}');";
        }

        public static string UpdateQuery(Metric metric)
        {
            return $"UPDATE `metric` SET `name`='{metric.Name}', `unit`='{metric.Unit}' WHERE  `metricId`={metric.MetricId};";
        }

        public static string Delete(Metric metric)
        {

            return $"DELETE FROM `metric` WHERE  `metricId`={metric.MetricId};";
        }
    }
}

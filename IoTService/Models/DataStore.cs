using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTService.Models
{
    public class DataStore
    {
        public int DataStoreId { get; set; }
        public int MetricId { get; set; }
        public float Value { get; set; }
        public DateTime Date { get; set; }

        public static DataStore FromReader(MySqlDataReader reader)
        {
            return new DataStore
            {
                DataStoreId = reader.GetInt32("datastoreid"),
                MetricId = reader.GetInt32("metricId"),
                Value = reader.GetFloat("value"),
                Date = reader.GetDateTime("date")
            };
        }

        public static string InsertQuery(DataStore register)
            => $"INSERT INTO `datastore` (`metricId`, `value`, `date`) VALUES ('{register.MetricId}', '{register.Value}', '{register.Date.ToString("u")}');";
    }
}

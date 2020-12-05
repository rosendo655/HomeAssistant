using IoTService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Services
{
    public class DataService
    {
        public class Users
        {
            public static async Task<User> FindUserByUsername(string username)
            {
                return (await MySQLService.Query($"SELECT * FROM user WHERE username = '{username}';", User.FromDataReader)).FirstOrDefault() ;
            }

            public static async Task InsertUser(User user)
            {
                await MySQLService.NonQuery(new[] { user }, User.InsertQuery);
            }

            public static async Task<int> UserIdByMetric(int metricId)
            {
                string query = $@"SELECT 
u.chatId AS CHATID
 FROM
user u JOIN device d ON d.userId = u.userId
JOIN metric m ON m.deviceId = d.deviceId
WHERE m.metricId = {metricId}";
                return  (await MySQLService.Query(query, reader => reader.GetInt32("CHATID"))).FirstOrDefault();
            }
        }

        public class Devices
        {
            public static Task<IEnumerable<Device>> DevicesByUser(int userId)
            {
                return MySQLService.Query($"SELECT * FROM device WHERE userId = {userId}", Device.FromReader);
            }

            public static Task UpdateDevice(Device dev)
            {
                return MySQLService.NonQuery(new[] { dev }, Device.UpdateQuery);
            }

            public static async Task InsertDevice(Device dev)
            {
                dev.DeviceId = await MySQLService.Insert( dev , Device.InsertQuery);
            }

            public static Task DeleteDevice(Device dev)
            => MySQLService.NonQuery(new[] { dev }, Device.DeleteQuery);
        }

        public class Metrics
        {
            public static  Task<IEnumerable<Metric>> MetricsByDevice(int deviceId)
            {
                return MySQLService.Query($"SELECT * FROM metric WHERE deviceId = {deviceId}", Metric.FromReader);
            }

            public static Task UpdateMetric(Metric metric)
            {
                return MySQLService.NonQuery(new[] { metric }, Metric.UpdateQuery);
            }

            public static async Task InsertMetric(Metric metric)
            {
                metric.MetricId = await MySQLService.Insert(metric, Metric.InsertQuery);
            }

            public static Task DeleteMetric (Metric metric)
            {
                return MySQLService.NonQuery(new[] { metric }, Metric.Delete);
            }

            
        }

        public class Controls
        {
            public static Task<IEnumerable<Control>> ControlsByDevice(int deviceId)
            {
                return MySQLService.Query($"SELECT * FROM Control WHERE deviceId = {deviceId}", Control.FromReader);
            }

            public static Task UpdateControl(Control Control)
            {
                return MySQLService.NonQuery(new[] { Control }, Control.UpdateQuery);
            }

            public static async Task InsertControl(Control Control)
            {
                Control.ControlId = await MySQLService.Insert(Control, Control.InsertQuery);
            }

            public static Task DeleteControl(Control control)
            {
                return MySQLService.NonQuery(new[] { control }, Control.DeleteQuery);
            }
        }

        public class Watches
        {
            public static Task<IEnumerable<Watch>> WatchesByMetric(int metricId)
            {
                return MySQLService.Query($"SELECT * FROM Watch WHERE metricId = {metricId}", Watch.FromReader);
            }

            public static Task UpdateWatch(Watch Watch)
            {
                return MySQLService.NonQuery(new[] { Watch }, Watch.UpdateQuery);
            }

            public static async Task InsertWatch(Watch Watch)
            {
                Watch.WatchId = await MySQLService.Insert(Watch, Watch.InsertQuery);
            }

            public static Task DeleteWatch(Watch watch)
            {
                return MySQLService.NonQuery(new[] { watch }, Watch.DeleteQuery);
            }
        }

        public class DataRegisters
        {
            public static async Task<KeyValuePair<int,string>> MetricToken(int metricId)
            {
                string query = $@"SELECT 
m.metricId AS METRIC,
u.token AS TOKEN
 FROM
user u JOIN device d ON d.userId = u.userId
JOIN metric m ON m.deviceId = d.deviceId
WHERE m.metricId = {metricId}";

                return (await MySQLService.Query(query, reader => new KeyValuePair<int, string>(reader.GetInt32("METRIC"), reader.GetString("TOKEN")))).FirstOrDefault();
            }

            public static async Task InsertDataRegister(DataStore dataStore)
            {
                dataStore.DataStoreId = await MySQLService.Insert(dataStore, DataStore.InsertQuery);
            }
        }

        public class ControlQueues
        {
            public static  Task DeleteControlQueue(ControlQueue control)
            {
                return MySQLService.NonQuery(new[] { control }, ControlQueue.DeleteQuery);
            }

            public static async Task InsertControlQueue(ControlQueue control)
            {
                control.ControlQueueId = await MySQLService.Insert(control, ControlQueue.InsertQuery);
            }

            public static async Task<ControlQueue> FirstControlQueue(int controlId)
            {
                string query = $@"SELECT * FROM controlqueue
WHERE controlId = {controlId}
ORDER BY insertedAt ASC
LIMIT 1";

                return (await MySQLService.Query(query, ControlQueue.FromReader)).FirstOrDefault();
            }

            public static async Task<KeyValuePair<int, string>> ControlToken(int controlId)
            {
                string query = $@"SELECT 
c.controlId AS CONTROL,
u.token AS TOKEN
 FROM
user u JOIN device d ON d.userId = u.userId
JOIN control c ON c.deviceId = d.deviceId
WHERE c.controlId = {controlId}";

                return (await MySQLService.Query(query, reader => new KeyValuePair<int, string>(reader.GetInt32("CONTROL"), reader.GetString("TOKEN")))).FirstOrDefault();
            }
        }
    }
}

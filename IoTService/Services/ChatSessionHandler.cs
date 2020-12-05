using IoTService.Models;
using Newtonsoft.Json;
using Swan.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Services
{
    public class ChatSessionHandler
    {
        private static Dictionary<ChatStatus, Func<ChatSession, Task>> AsyncActions = new Dictionary<ChatStatus, Func<ChatSession, Task>>()
        {
            {ChatStatus.NEW , New },
            {ChatStatus.ASK_REGISTER , AskRegister },
            {ChatStatus.USER_EXISTS , UserExists },
            {ChatStatus.MAIN_MENU , MainMenu },
            {ChatStatus.DEVICES , Devices },
            {ChatStatus.ADD_DEVICE , AddDevice },
            {ChatStatus.DEVICE_SELECTED , DeviceSelected },
            {ChatStatus.DEVICE_RENAMING , DeviceRenaming },
            {ChatStatus.DEVICE_ASK_ELIMINATION , DeviceAskElimination },
            {ChatStatus.DEVICE_METRICS , DeviceMetrics },
            {ChatStatus.DEVICE_CONTROLS , DeviceControls },
            {ChatStatus.ADD_METRIC ,AddMetric},
            {ChatStatus.METRIC_UNIT_DESC , MetricUnitDesc },
            {ChatStatus.METRIC_SELECTED , MetricSelected },
            {ChatStatus.METRIC_ASK_ELIMINATION , MetricAskElimination },
            {ChatStatus.CONTROL_SELECTED , ControlSelected },
            {ChatStatus.CONTROL_RENAMING , ControlRenaming },
            {ChatStatus.CONTROL_ASK_ELIMINATION , ControlAskElimination },
            {ChatStatus.ADD_CONTROL , AddControl },
            {ChatStatus.CONTROL_DEVICE , ControlDevice },
            {ChatStatus.METRIC_NAMING , MetricNaming },
            {ChatStatus.WATCHES , Watches },
            {ChatStatus.WATCH_SELECTED, WatchSelected },
            {ChatStatus.WATCH_NAMING , WatchNaming },
            {ChatStatus.WATCH_EXPRESSION , WatchExpression },
            {ChatStatus.WATCH_MESSAGE , WatchMessage },
            {ChatStatus.WATCH_ASK_ELIMINATION , WatchAskElimination }



        };

        

        private const string CONSULTAR_TOKEN = "Consultar Token";
        private const string VER_DISPOSITIVOS = "Ver Dispositivos 💻";

        private const string VER_CONTROLES = "Ver Controles 🎮";
        private const string VER_METRICAS = "Ver Metricas 📈";
        private const string ACTUALIZAR = "Actualizar 🔁";
        private const string ELIMINAR = "Eliminar ❌";

        private const string CONTROLAR = "Controlar 🕹";
        private const string WATCHES = "Watches 👁";
        private const string ANADIR_DISPOSITIVO = "Anadir dispositivo ➕";
        private const string ANADIR_CONTROL = "Anadir control ➕";
        private const string ANADIR_METRICA = "Anadir metrica ➕";
        private const string ANADIR_WATCH = "Anadir watch ➕";
        private const string REINICIAR_WATCHES = "Reiniciar alertas 🔁";


        private const string ENCENDER = "Encender ⭕";
        private const string APAGAR = "Apagar ❌";

        private const string SI = "SI ⭕";
        private const string NO = "NO ❌";

        private const string REGRESAR = "Regresar ↩";


        public static IKeyboard SiNoKeyboard => ReplyKeyboard.FromStringKeys(new[] { new[] { SI, NO } });

        private static IKeyboard MainMenuKeyboard =>
            ReplyKeyboard.FromStringKeysVertical(
                new[]
                {
                    CONSULTAR_TOKEN,
                    VER_DISPOSITIVOS,
                });

        private static IKeyboard DeviceMenuKeyboard =>
            ReplyKeyboard.FromStringKeysVertical(
                new[] {
                    VER_CONTROLES,
                    VER_METRICAS,
                    REGRESAR,
                        ACTUALIZAR,
                    ELIMINAR,
                });

        private static IKeyboard MetricsMenuKeyboard =>
            ReplyKeyboard.FromStringKeysVertical(
                    new[] {
                    WATCHES,
                    REINICIAR_WATCHES,
                    ACTUALIZAR,
                    REGRESAR,
                    ELIMINAR,
                });

        private static IKeyboard WatchesMenuKeyboard =>
            ReplyKeyboard.FromStringKeysVertical(
                    new[] {
                    ACTUALIZAR,
                    ELIMINAR,
                    REGRESAR,
                });

        private static IKeyboard ControlMenuKeyboard =>
            ReplyKeyboard.FromStringKeysVertical(
                new[] {
                    CONTROLAR,
                    ACTUALIZAR,
                    ELIMINAR,
                    REGRESAR,
                });

        private static IKeyboard ControlDeviceMenu =>
            ReplyKeyboard.FromStringKeysVertical(
                new[] {
                    ENCENDER,
                    APAGAR,
                    REGRESAR,
                });

        private static async Task<IKeyboard> GenerateKeyboard<T>(int input, Func<int, Task<IEnumerable<T>>> func, Func<T, string> stringFromT, string append)
        {
            var devices = await func(input);
            var keys = devices.Select(s => stringFromT(s)).Append(append);
            return ReplyKeyboard.FromStringKeysVertical(keys);
        }

        private static async Task<IKeyboard> DevicesKeyboard(int userId)
        //=> await GenerateKeyboard(userId, DataService.Devices.DevicesByUser, s => $"{s.DeviceId} {s.Name}", ANADIR_DISPOSITIVO);
        {
            var devices = await DataService.Devices.DevicesByUser(userId);
            IEnumerable<string> keys = devices.Select(s => s.KeyItem).Append(ANADIR_DISPOSITIVO).Append(REGRESAR);
            return ReplyKeyboard.FromStringKeysVertical(keys);
        }

        private static async Task<IKeyboard> ControlsKeyboard(int deviceId)
        {
            var controls = await DataService.Controls.ControlsByDevice(deviceId);
            IEnumerable<string> keys = controls.Select(s => s.KeyItem).Append(ANADIR_CONTROL).Append(REGRESAR);
            return ReplyKeyboard.FromStringKeysVertical(keys);
        }

        private static async Task<IKeyboard> MetricsKeyboard(int deviceId)
        {
            var metrics = await DataService.Metrics.MetricsByDevice(deviceId);
            IEnumerable<string> keys = metrics.Select(s => s.KeyItem).Append(ANADIR_METRICA).Append(REGRESAR);
            return ReplyKeyboard.FromStringKeysVertical(keys);
        }

        private static async Task<IKeyboard> WatchesKeyboard(int metricId)
        {
            var watches = await DataService.Watches.WatchesByMetric(metricId);
            IEnumerable<string> keys = watches.Select(s => s.KeyItem).Append(ANADIR_WATCH).Append(REGRESAR);
            return ReplyKeyboard.FromStringKeysVertical(keys);
        }

        private static int IntOption(string str)
        {
            string number = str.Split(" ").FirstOrDefault();
            int option = -1;
            if (number == null || !int.TryParse(number, out option))
            {
                return -1;
            }
            return option;
        }


        private const string chars = "1234567890qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";
        private static string CreateToken(int seed)
        {
            Random random = new Random(seed);
            string output = "";
            for(int i = 0; i < 50; i ++)
            {
                output += chars[random.Next() % chars.Length];
            }
            return output;
        }


        public static async Task<bool> HandleSession(ChatSession chatSession)
        {
            try
            {
                await AsyncActions[chatSession.Status].Invoke(chatSession);
            }
            catch (Exception ex)
            {
                ex.Dump("EXCEPTION");
            }
            return true;
        }

        private static async Task New(ChatSession session)
        {
            User usr = await DataService.Users.FindUserByUsername(session.Username);

            if (usr == null)
            {
                await TelegramBotManager.SendMessage(session.ChatId, "No existes en el registro, deseas registrarte?", SiNoKeyboard);
                session.Status = ChatStatus.ASK_REGISTER;
            }
            else
            {
                session.Status = ChatStatus.USER_EXISTS;
                await UserExists(session);
            }
        }


        private static async Task AskRegister(ChatSession session)
        {
            if (session.CurrentMessage == SI)
            {
                User new_usr = new User() { ChatId = session.ChatId, Username = session.Username, Token = CreateToken(session.ChatId) };
                await DataService.Users.InsertUser(new_usr);
                await TelegramBotManager.SendMessage(session.ChatId, $"Has sido registrado, tu token es : {new_usr.Token}");
                session.Status = ChatStatus.USER_EXISTS;
                await UserExists(session);
            }
            else if (session.CurrentMessage == NO)
            {
                await TelegramBotManager.SendMessage(session.ChatId, "De acuerdo, si deseas registrarte en otro momento, mandame un mensaje");
                session.Status = ChatStatus.NEW;
            }
            else
            {
                await TelegramBotManager.SendMessage(session.ChatId, "Entrada no valida");
                session.Status = session.PreviousStatus;
            }
        }

        private static async Task UserExists(ChatSession session)
        {

            await TelegramBotManager.SendMessage(session.ChatId, "Que accion deseas realizar", MainMenuKeyboard);
            session.Status = ChatStatus.MAIN_MENU;
        }

        private static async Task MainMenu(ChatSession session)
        {
            User usr;
            switch (session.CurrentMessage)
            {
                case CONSULTAR_TOKEN:
                    usr = await DataService.Users.FindUserByUsername(session.Username);
                    await TelegramBotManager.SendMessage(session.ChatId, $"Tu token es : {usr.Token}");
                    await UserExists(session);
                    break;

                case VER_DISPOSITIVOS:
                    usr = await DataService.Users.FindUserByUsername(session.Username);
                    var keyboard = await DevicesKeyboard(usr.UserId);
                    await TelegramBotManager.SendMessage(session.ChatId, "Estos son tus dispositivos:", keyboard);

                    session.Status = ChatStatus.DEVICES;
                    break;

                default:
                    await TelegramBotManager.SendMessage(session.ChatId, "Entrada no valida");
                    session.Status = session.PreviousStatus;
                    await UserExists(session);
                    break;
            }
        }

        private static async Task Devices(ChatSession session)
        {
            User usr;
            if (session.CurrentMessage == ANADIR_DISPOSITIVO)
            {
                await TelegramBotManager.SendMessage(session.ChatId, "Cual es el nombre de tu dispositivo?");
                session.Status = ChatStatus.ADD_DEVICE;
            }
            else if (session.CurrentMessage == REGRESAR)
            {
                await UserExists(session);
            }
            else
            {
                usr = await DataService.Users.FindUserByUsername(session.Username);
                var devices = await DataService.Devices.DevicesByUser(usr.UserId);

                int option = IntOption(session.CurrentMessage);

                if (option < 0)
                {
                    await TelegramBotManager.SendMessageSameKeyboard(session.ChatId, "Entrada no valida");
                    return;
                }

                Device dev = devices.FirstOrDefault(f => f.DeviceId == option);

                if (dev == null)
                {
                    await TelegramBotManager.SendMessageSameKeyboard(session.ChatId, "No existe ese dispositivo");
                    return;
                }

                session.SelectedDevice = dev;
                session.Status = ChatStatus.DEVICE_SELECTED;

                await TelegramBotManager.SendMessage(session.ChatId, $"Dispositivo seleccionado ({session.SelectedDevice.KeyItem}), que deseas realizar", DeviceMenuKeyboard);
            }
        }

        private static async Task AddDevice(ChatSession session)
        {
            var usr = await DataService.Users.FindUserByUsername(session.Username);
            var new_dev = new Device()
            {
                UserId = usr.UserId,
                Name = session.CurrentMessage.Trim()
            };
            await DataService.Devices.InsertDevice(new_dev);

            session.SelectedDevice = new_dev;

            session.Status = ChatStatus.DEVICE_SELECTED;
            await TelegramBotManager.SendMessage(session.ChatId, "Dispositivo nuevo anadido, Que deseas realizar", DeviceMenuKeyboard);
        }

        private static async Task DeviceSelected(ChatSession session)
        {
            switch (session.CurrentMessage)
            {
                case ACTUALIZAR:
                    await TelegramBotManager.SendMessage(session.ChatId, "Cual sera el nuevo nombre del dispositivo?");
                    session.Status = ChatStatus.DEVICE_RENAMING;
                    break;
                case ELIMINAR:
                    await TelegramBotManager.SendMessage(session.ChatId, "Estas seguro que deseas eliminar este dispositivo?", SiNoKeyboard);
                    session.Status = ChatStatus.DEVICE_ASK_ELIMINATION;
                    break;
                case VER_METRICAS:
                    var keys = await MetricsKeyboard(session.SelectedDevice.DeviceId);
                    await TelegramBotManager.SendMessage(session.ChatId, "Estas son las metricas del dispositivo", keys);
                    session.Status = ChatStatus.DEVICE_METRICS;
                    break;
                case VER_CONTROLES:
                    var keyboard = await ControlsKeyboard(session.SelectedDevice.DeviceId);
                    await TelegramBotManager.SendMessage(session.ChatId, "Estas son los controles del dispositivo", keyboard);
                    session.Status = ChatStatus.DEVICE_CONTROLS;
                    break;
                case REGRESAR:
                    await UserExists(session);
                    break;
                default:
                    await TelegramBotManager.SendMessage(session.ChatId, "Entrada no valida" , DeviceMenuKeyboard);
                    break;
            }
        }

        private static async Task DeviceRenaming(ChatSession session)
        {
            session.SelectedDevice.Name = session.CurrentMessage.Trim();
            await DataService.Devices.UpdateDevice(session.SelectedDevice);
            session.Status = ChatStatus.DEVICE_SELECTED;
            await TelegramBotManager.SendMessage(session.ChatId, "Renombrado, Que accion deseas realizar", DeviceMenuKeyboard);
        }

        private static async Task DeviceAskElimination(ChatSession session)
        {
            switch (session.CurrentMessage)
            {
                case SI:
                    await DataService.Devices.DeleteDevice(session.SelectedDevice);
                    session.SelectedDevice = null;
                    await UserExists(session);
                    break;
                case NO:
                    await TelegramBotManager.SendMessage(session.ChatId, "Operacion cancelada, Que deseas realizar?", DeviceMenuKeyboard);
                    session.Status = ChatStatus.DEVICE_SELECTED;
                    break;
                default:
                    await TelegramBotManager.SendMessage(session.ChatId, "Entrada no valida",SiNoKeyboard);
                    break;
            }
        }

        private static async Task DeviceControls(ChatSession session)
        {

            if (session.CurrentMessage == ANADIR_CONTROL)
            {
                await TelegramBotManager.SendMessage(session.ChatId, "Cual es el nombre del nuevo control?");
                session.Status = ChatStatus.ADD_CONTROL;
            }
            else if(session.CurrentMessage == REGRESAR)
            {
                await TelegramBotManager.SendMessage(session.ChatId, $"Dispositivo seleccionado ({session.SelectedDevice.KeyItem}) que accion deseas realizar", DeviceMenuKeyboard);
                session.Status = ChatStatus.DEVICE_SELECTED;
            }
            else
            {
                var controls = await DataService.Controls.ControlsByDevice(session.SelectedDevice.DeviceId);
                int option = IntOption(session.CurrentMessage);
                if(option < 0)
                {
                    await TelegramBotManager.SendMessageSameKeyboard(session.ChatId, "Entrada no valida");
                    return;
                }
                Control control = controls.FirstOrDefault(f => f.ControlId == option);
                if (control == null)
                {
                    await TelegramBotManager.SendMessageSameKeyboard(session.ChatId, "No existe ese dispositivo");
                    return;
                }
                session.SelectedControl = control;
                session.Status = ChatStatus.CONTROL_SELECTED; 

                await TelegramBotManager.SendMessage(session.ChatId, $"Control seleccionado ({session.SelectedControl.KeyItem}), que deseas realizar", ControlMenuKeyboard);

            }
        }

        private static async Task AddControl(ChatSession session)
        {
            Control new_control = new Control
            {
                DeviceId = session.SelectedDevice.DeviceId,
                Name = session.CurrentMessage,
            };
            await DataService.Controls.InsertControl(new_control);
            session.SelectedControl = new_control;
            session.Status = ChatStatus.CONTROL_SELECTED;
            await TelegramBotManager.SendMessage(session.ChatId, $"Si quieres consultar las acciones de tu control GET a la siguiente direccion {Program.MainUrl}/api/control/{session.SelectedControl.ControlId}/TOKEN");

            await TelegramBotManager.SendMessage(session.ChatId, $"Control seleccionado ({session.SelectedControl.KeyItem}), que deseas realizar", ControlMenuKeyboard);

        }

        private static async Task ControlSelected(ChatSession session)
        {
            switch (session.CurrentMessage)
            {
                case CONTROLAR:
                    await TelegramBotManager.SendMessage(session.ChatId, "Que accion deseas realizar?" , ControlDeviceMenu);
                    session.Status = ChatStatus.CONTROL_DEVICE;
                    break;
                case ACTUALIZAR:
                    await TelegramBotManager.SendMessage(session.ChatId, "Cual sera el nuevo nombre del Control?");
                    session.Status = ChatStatus.CONTROL_RENAMING;
                    break;
                case REGRESAR:
                    session.SelectedControl = null;
                    await TelegramBotManager.SendMessage(session.ChatId, $"Dispositivo seleccionado ({session.SelectedDevice.KeyItem})", DeviceMenuKeyboard);
                    session.Status = ChatStatus.DEVICE_SELECTED;
                    break;
                case ELIMINAR:
                    await TelegramBotManager.SendMessage(session.ChatId, "Estas seguro que deseas eliminar este control?", SiNoKeyboard);
                    session.Status = ChatStatus.CONTROL_ASK_ELIMINATION;
                    break;

                default:
                    await TelegramBotManager.SendMessageSameKeyboard(session.ChatId, "Entrada no valida");
                    break;
            }
        }

        private static async Task ControlRenaming(ChatSession session)
        {
            session.SelectedControl.Name = session.CurrentMessage.Trim();
            await DataService.Controls.UpdateControl(session.SelectedControl);
            session.Status = ChatStatus.CONTROL_SELECTED;
            
            await TelegramBotManager.SendMessage(session.ChatId, $"Control actualizado ({session.SelectedControl.KeyItem}), que deseas realizar", ControlMenuKeyboard);

        }

        private static async Task ControlDevice(ChatSession session)
        {
            switch (session.CurrentMessage)
            {
                case ENCENDER:
                case APAGAR:
                    ControlQueue controlQueueDesactivar = new ControlQueue()
                    {
                        ControlId = session.SelectedControl.ControlId,
                        InsertedAt = DateTime.UtcNow,
                        Value = session.CurrentMessage == ENCENDER ? 1 : 0
                    };
                    await DataService.ControlQueues.InsertControlQueue(controlQueueDesactivar);
                    await TelegramBotManager.SendMessage(session.ChatId, $"El control ({session.SelectedControl.KeyItem}) ha sido {(session.CurrentMessage == ENCENDER ? "encendido" : "apagado")}, Que deseas realizar?", ControlMenuKeyboard);
                    session.Status = ChatStatus.CONTROL_SELECTED;
                    break;
                case REGRESAR:
                    await TelegramBotManager.SendMessage(session.ChatId, $"Control seleccionado ({session.SelectedControl.KeyItem})", ControlMenuKeyboard);
                    session.Status = ChatStatus.CONTROL_SELECTED;
                    break;
                

                default:
                    await TelegramBotManager.SendMessageSameKeyboard(session.ChatId, "Entrada no valida");
                    break;
            }
        }

        private static async Task ControlAskElimination(ChatSession session)
        {
            switch (session.CurrentMessage)
            {
                case SI:
                    await DataService.Controls.DeleteControl (session.SelectedControl);
                    session.SelectedControl = null;
                    await UserExists(session);
                    break;
                case NO:
                    await TelegramBotManager.SendMessage(session.ChatId, "Operacion cancelada, Que deseas realizar?", ControlMenuKeyboard);
                    session.Status = ChatStatus.METRIC_SELECTED;
                    break;
                default:
                    await TelegramBotManager.SendMessage(session.ChatId, "Entrada no valida", SiNoKeyboard);
                    break;
            }
        }

        private static async Task DeviceMetrics(ChatSession session)
        {
            if (session.CurrentMessage == ANADIR_METRICA)
            {
                await TelegramBotManager.SendMessage(session.ChatId, "Cual es el nombre de tu nueva metrica?");
                session.IsUpdatingMetric = false;
                session.Status = ChatStatus.METRIC_NAMING;
            }
            else if( session.CurrentMessage == REGRESAR)
            {
                await TelegramBotManager.SendMessage(session.ChatId, $"Dispositivo seleccionado ({session.SelectedDevice.Name}), que accion deseas realizar", DeviceMenuKeyboard);
                session.Status = ChatStatus.DEVICE_SELECTED;
            }
            else
            {
                var metrics = await DataService.Metrics.MetricsByDevice(session.SelectedDevice.DeviceId);
                int option = IntOption(session.CurrentMessage);
                if(option < 0)
                {
                    await TelegramBotManager.SendMessageSameKeyboard(session.ChatId, "Entrada no valida");
                    return;
                }

                Metric metric = metrics.FirstOrDefault(f => f.MetricId == option);
                if (metric == null)
                {
                    await TelegramBotManager.SendMessageSameKeyboard(session.ChatId, "No existe ese dispositivo");
                    return;
                }
                session.SelectedMetric = metric;
                session.Status = ChatStatus.METRIC_SELECTED;
                await TelegramBotManager.SendMessage(session.ChatId, "Dispositivo seleccionado, que deseas realizar", MetricsMenuKeyboard);
            }
        }

        private static async Task AddMetric(ChatSession session)
        {
            session.SelectedMetric = session.IsUpdatingMetric ? session.SelectedMetric : new Metric();
            session.SelectedMetric.Name = session.CurrentMessage.Trim();
            await TelegramBotManager.SendMessage(session.ChatId, "En que unidad esta descrita la metrica?");
            session.Status = ChatStatus.METRIC_UNIT_DESC;
        }

        private static async Task MetricNaming(ChatSession session)
        {
            session.SelectedMetric = session.IsUpdatingMetric ? session.SelectedMetric : new Metric();
            session.SelectedMetric.Name = session.CurrentMessage.Trim();
            await TelegramBotManager.SendMessage(session.ChatId, "En que unidad esta descrita la metrica?");
            session.Status = ChatStatus.METRIC_UNIT_DESC;
        }

        private static async Task MetricUnitDesc(ChatSession session)
        {
            session.SelectedMetric.Unit = session.CurrentMessage.Trim();
            session.SelectedMetric.DeviceId = session.SelectedDevice.DeviceId;
            if(session.IsUpdatingMetric)
            {
                await DataService.Metrics.UpdateMetric(session.SelectedMetric);
            }
            else
            {
                await DataService.Metrics.InsertMetric(session.SelectedMetric);
            }
            
            session.Status = ChatStatus.METRIC_SELECTED;
            await TelegramBotManager.SendMessage(session.ChatId, $"Si quieres registrar datos de esta metrica manda una peticion GET a la siguiente direccion {Program.MainUrl}/api/sendData/{session.SelectedMetric.MetricId}/TOKEN/VALOR");
            await TelegramBotManager.SendMessage(session.ChatId, $"Metrica {(session.IsUpdatingMetric? "actualizada" : "creada")} ({session.SelectedMetric.KeyItem}) que deseas realizar",MetricsMenuKeyboard);
        }

        private static async Task MetricSelected(ChatSession session)
        {
            switch(session.CurrentMessage)
            {
                case WATCHES:
                    var keyboard = await WatchesKeyboard(session.SelectedMetric.MetricId);
                    session.Status = ChatStatus.WATCHES;
                    await TelegramBotManager.SendMessage(session.ChatId, "Estos son tus watches", keyboard);
                    break;
                case REINICIAR_WATCHES:
                    var watches = await DataService.Watches.WatchesByMetric(session.SelectedMetric.MetricId);
                    session.Status = ChatStatus.METRIC_SELECTED;
                    foreach( var watch in watches)
                    {
                        watch.Sent = 0;
                        await DataService.Watches.UpdateWatch(watch);
                    }
                    await TelegramBotManager.SendMessage(session.ChatId, "Watches reiniciados", MetricsMenuKeyboard);
                    break;
                case ACTUALIZAR:
                    await TelegramBotManager.SendMessage(session.ChatId, "Cual sera el nuevo nombre de la metrica?");
                    session.IsUpdatingMetric = true;
                    session.Status = ChatStatus.ADD_METRIC;
                    break;
                case REGRESAR:
                    session.SelectedMetric = null;
                    await TelegramBotManager.SendMessage(session.ChatId, $"Dispositivo seleccionado ({session.SelectedDevice.KeyItem})", DeviceMenuKeyboard);
                    session.Status = ChatStatus.DEVICE_SELECTED;
                    break;
                case ELIMINAR:
                    await TelegramBotManager.SendMessage(session.ChatId, "Estas seguro que deseas eliminar este dispositivo?", SiNoKeyboard);
                    session.Status = ChatStatus.METRIC_ASK_ELIMINATION;
                    break;

                default:
                    break;
            }
        }

        private static async Task MetricAskElimination(ChatSession session)
        {
            switch (session.CurrentMessage)
            {
                case SI:
                    await DataService.Metrics.DeleteMetric(session.SelectedMetric);
                    session.SelectedMetric = null;
                    await UserExists(session);
                    break;
                case NO:
                    await TelegramBotManager.SendMessage(session.ChatId, "Operacion cancelada, Que deseas realizar?", MetricsMenuKeyboard);
                    session.Status = ChatStatus.METRIC_SELECTED;
                    break;
                default:
                    await TelegramBotManager.SendMessage(session.ChatId, "Entrada no valida", SiNoKeyboard);
                    break;
            }
        }


        private static async Task Watches(ChatSession session)
        {
            if (session.CurrentMessage == ANADIR_WATCH)
            {
                await TelegramBotManager.SendMessage(session.ChatId, "Cual es el nombre de tu watch?");
                session.IsUpdatingWatch = false; 
                session.Status = ChatStatus.WATCH_NAMING;
            }
            else if (session.CurrentMessage == REGRESAR)
            {
                await TelegramBotManager.SendMessage(session.ChatId, $"Metrica ({session.SelectedMetric.KeyItem}), que deseas realizar?", MetricsMenuKeyboard);
                session.Status = ChatStatus.METRIC_SELECTED;
            }
            else
            {

                var watches = await DataService.Watches.WatchesByMetric(session.SelectedMetric.MetricId);

                int option = IntOption(session.CurrentMessage);

                if (option < 0)
                {
                    await TelegramBotManager.SendMessageSameKeyboard(session.ChatId, "Entrada no valida");
                    return;
                }

                Watch watch = watches.FirstOrDefault(f => f.WatchId == option);

                if (watch == null)
                {
                    await TelegramBotManager.SendMessageSameKeyboard(session.ChatId, "No existe ese watch");
                    return;
                }

                session.SelectedWatch = watch;
                session.Status = ChatStatus.WATCH_SELECTED;

                await TelegramBotManager.SendMessage(session.ChatId, $"Watch seleccionado ({session.SelectedWatch.KeyItem}), que deseas realizar", WatchesMenuKeyboard);
            }
        }

        private static async Task WatchNaming(ChatSession session)
        {
            session.SelectedWatch = session.IsUpdatingWatch ? session.SelectedWatch : new Watch();
            session.SelectedWatch.Name = session.CurrentMessage.Trim();
            session.Status = ChatStatus.WATCH_EXPRESSION;
            await TelegramBotManager.SendMessage(session.ChatId, "Que expresion se va a evaluar para mandar el mensaje? ( usar 'val' como variable de la metrica ejemplo val > 30)");
        }

        private static async Task WatchExpression(ChatSession session)
        {
            var result = JintService.TestExpression(session.CurrentMessage.Trim());

            switch (result)
            {
                case EvaluationResult.INVALID:
                    await TelegramBotManager.SendMessage(session.ChatId, "Expresion invalida, verificar identificadores");
                    break;
                case EvaluationResult.NOT_BOOLEAN:
                    await TelegramBotManager.SendMessage(session.ChatId, "Esta expresion no es booleana");
                    break;
                case EvaluationResult.NO_CONTAINS_VAL:
                    await TelegramBotManager.SendMessage(session.ChatId, "Esta expresion no contiene el identificador");
                    break;
                case EvaluationResult.SUCCESS:
                    session.SelectedWatch.Expression = session.CurrentMessage.Trim();
                    await TelegramBotManager.SendMessage(session.ChatId, "Expresion valida, Que mensaje quieres que se envie al cumplirse esta condicion");
                    session.Status = ChatStatus.WATCH_MESSAGE;
                    break;
                default:
                    break;
            }
        }

        private static async Task WatchMessage(ChatSession session)
        {
            session.SelectedWatch.Message = session.CurrentMessage.Trim();
            session.SelectedWatch.MetricId = session.SelectedMetric.MetricId;
            if (session.IsUpdatingWatch)
            {
                await DataService.Watches.UpdateWatch(session.SelectedWatch);
            }
            else
            {
                await DataService.Watches.InsertWatch(session.SelectedWatch);
            }

            session.Status = ChatStatus.WATCH_SELECTED;
            await TelegramBotManager.SendMessage(session.ChatId, $"Watch {(session.IsUpdatingWatch ? "actualizado" : "creado")} ({session.SelectedWatch.KeyItem}) que deseas realizar", WatchesMenuKeyboard);
        }

        private static async Task WatchSelected(ChatSession session)
        {
            switch (session.CurrentMessage)
            {
                case ACTUALIZAR:
                    await TelegramBotManager.SendMessage(session.ChatId, "Cual sera el nuevo nombre del watch?");
                    session.IsUpdatingWatch = true;
                    session.Status = ChatStatus.WATCH_NAMING;
                    break;
                case ELIMINAR:
                    await TelegramBotManager.SendMessage(session.ChatId, "Estas seguro que deseas eliminar este dispositivo?", SiNoKeyboard);
                    session.Status = ChatStatus.WATCH_ASK_ELIMINATION;
                    break;
                case REGRESAR:
                    await TelegramBotManager.SendMessage(session.ChatId, $"Metrica ({session.SelectedMetric.KeyItem}), Que accion deseas realizar?", MetricsMenuKeyboard);
                    session.Status = ChatStatus.METRIC_SELECTED;
                    break;
                default:
                    await TelegramBotManager.SendMessage(session.ChatId, "Entrada no valida", DeviceMenuKeyboard);
                    break;
            }
        }

        public static async Task WatchAskElimination(ChatSession session)
        {
            switch (session.CurrentMessage)
            {
                case SI:
                    await DataService.Watches.DeleteWatch(session.SelectedWatch);
                    session.SelectedWatch = null;
                    await UserExists(session);
                    break;
                case NO:
                    await TelegramBotManager.SendMessage(session.ChatId, $"Operacion cancelada, watch seleccionado ({session.SelectedWatch.KeyItem}) Que deseas realizar?", WatchesMenuKeyboard);
                    session.Status = ChatStatus.WATCH_SELECTED;
                    break;
                default:
                    await TelegramBotManager.SendMessage(session.ChatId, "Entrada no valida", SiNoKeyboard);
                    break;
            }
        }





    }
}

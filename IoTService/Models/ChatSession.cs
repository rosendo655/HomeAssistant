using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IoTService.Models
{
    public enum ChatStatus
    {
        NEW,
        ASK_REGISTER,
        USER_EXISTS,
        MAIN_MENU,
        DEVICES,
        DEVICE_SELECTED,
        DEVICE_RENAMING,
        DEVICE_ASK_ELIMINATION,
        DEVICE_METRICS,
        DEVICE_CONTROLS,
        ADD_DEVICE,

        ADD_METRIC,
        ADD_CONTROL,
        CONTROL_RENAMING,
        ADD_WATCH,
        CONTROL_SELECTED,
        METRIC_SELECTED,
        METRIC_NAMING,
        METRIC_UNIT_DESC,
        WATCHES,
        METRIC_ASK_ELIMINATION,
        CONTROL_ASK_ELIMINATION,
        CONTROL_DEVICE,
        WATCH_NAMING,
        WATCH_SELECTED,
        WATCH_EXPRESSION,
        WATCH_MESSAGE,
        WATCH_ASK_ELIMINATION,
    }
    public class ChatSession
    {
        private static int MAX_MESSAGE_COUNT = 5;
        public ChatSession(int ChatId,string username)
        {
            this.ChatId = ChatId;
            Username = username;
            Status = ChatStatus.NEW;
            PreviousStatus = ChatStatus.NEW;
            PreviousMessages = new Queue<string>();
        }


        public int ChatId { get; set; }
        public string Username { get; set; }
        public ChatStatus PreviousStatus { get; set; }
        public Device SelectedDevice { get; set; }
        public Control SelectedControl { get; set;}
        public Metric SelectedMetric { get; set; }
        public bool IsUpdatingMetric { get; set; }

        public Watch SelectedWatch { get; set; }
        public bool IsUpdatingWatch { get; set; }

        private ChatStatus _status;
        public ChatStatus Status
        {
            get => _status;
            set
            {
                PreviousStatus = _status;
                _status = value;
            }
        }
        public Queue<string> PreviousMessages { get; }
        public string CurrentMessage => PreviousMessages?.LastOrDefault();
        
        public void AddMessage(string message)
        {
            if(PreviousMessages.Count >= MAX_MESSAGE_COUNT)
            {
                PreviousMessages.Dequeue();
            }
            PreviousMessages.Enqueue(message);
        }
    }
}

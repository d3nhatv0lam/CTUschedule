using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTUschedule.Resources.Dialogs
{

    public interface INotificationPopup
    {

        public string symbolKind {  get; set; }
        public string symbolColorHex { get; set; }
        
        public string LineColorHex { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

        public void ShowNotification();

    }


    public class NotificationPopupController : INotificationPopup
    {
        public NotificationPopup window;

        public string symbolKind { get; set; }
        public string symbolColorHex { get; set; }
        public string LineColorHex { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

        public enum Type
        {
            Error,
            Warning,
            Information,
        }

        public NotificationPopupController(Type type,string title, string message)
        {
           this. Title = title;
           this. Message = message;
            GetType(type);
          
           window = new NotificationPopup(symbolKind, symbolColorHex, LineColorHex, Title, Message);
        }

        private void GetType(Type type)
        {
            switch (type)
            {
                case Type.Error:
                    {
                        symbolKind = "AlertOctagonOutline";
                        symbolColorHex = "#971c38";
                        LineColorHex = "#FF0000";
                    }
                    break;
                case Type.Warning:

                    break;
                case Type.Information:
                    break;
                default:
                    break;
            };
        }

        public void ShowNotification()
        {
            window.Show();
        }

    }
   
}

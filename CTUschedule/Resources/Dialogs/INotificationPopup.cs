using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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

        private static List<bool> PopupStorage = new List<bool>(new bool[5]);

        private int PosIndex;

        public enum Type
        {
            Error,
            Warning,
            Information,
            Succes,
        }

        public NotificationPopupController(Type type,string title, string message)
        {
           this. Title = title;
           this. Message = message;
            GetType(type);
            GetPosIndex();
            window = new NotificationPopup(PosIndex,symbolKind, symbolColorHex, LineColorHex, Title, Message);
        }

        private void GetPosIndex()
        {
            PosIndex = -1;
            for (int index = 0; index < PopupStorage.Count; index++)
            {
                if (PopupStorage[index]) continue;

                PosIndex = index;
                break;
            }
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
                    {
                        symbolKind = "AlertOutline";
                        symbolColorHex = "#ffbc11";
                        LineColorHex = "#ff9966";
                    }
                    break;
                case Type.Information:
                    break;

                case Type.Succes:
                    {
                        symbolKind = "CheckCircleOutline";
                        symbolColorHex = "#99cc33";
                        LineColorHex = "#339900";
                    }
                    break;
                default:
                    break;
            };
        }

        public async void ShowNotification()
        {
            if (PosIndex == -1) return;
            PopupStorage[PosIndex] = true;
            window.Show();
            PopupStorage[PosIndex] = await Task.Run(() =>
            {
                Thread.Sleep(5100);
                return false;
                
            });
            window = null;
        }

    }
   
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace AppConfig.Net
{
    public class UdpNotification
    {
        #region Receive
        private static UdpClient udpListener;
        private static IPEndPoint udpEndPoint;
        public static bool IsListening { get { return udpListener != null; } }

        public static void StartListening()
        {
            StartListening(IPAddress.Loopback, 4315);
        }
        public static void StartListening(IPAddress IPAddress, int Port)
        {
            if (udpListener != null)
                throw new Exception("UdpListener is already listening.  Call StopListening before calling StartListening a second time.");

            udpEndPoint = new IPEndPoint(IPAddress, Port);
            udpListener = new UdpClient(udpEndPoint);
            startListening();
        }
        private static void startListening()
        {
            udpListener.BeginReceive(ReceiveMessage, null);
        }
        private static void ReceiveMessage(IAsyncResult ar)
        {
            byte[] bytes = udpListener.EndReceive(ar, ref udpEndPoint);
            startListening();
            RaiseNotificationReceived(Encoding.ASCII.GetString(bytes));
        }

        public static event EventHandler<UdpNotificationRecivedEventArgs> NotificationReceived;
        private static void RaiseNotificationReceived(string Message)
        {
            if (NotificationReceived == null)
                return;
            NotificationReceived.Invoke(null, new UdpNotificationRecivedEventArgs(Message));
        }
        #endregion

        #region Send
        public static void SendNotification(string Message)
        {
            SendNotification(IPAddress.Loopback, 4315, Message);
        }
        public static void SendNotification(IPAddress IPAddress, int Port, string Message)
        {
            UdpClient client = new UdpClient();
            IPEndPoint ip = new IPEndPoint(IPAddress, Port);
            byte[] bytes = Encoding.ASCII.GetBytes(Message);
            client.Send(bytes, bytes.Length, ip);
            client.Close();
        }
        #endregion
    }

    public class UdpNotificationRecivedEventArgs : EventArgs
    {
        internal UdpNotificationRecivedEventArgs(string Message)
        {
            this.Message = Message;
        }
        public string Message { get; private set; }
    }
}

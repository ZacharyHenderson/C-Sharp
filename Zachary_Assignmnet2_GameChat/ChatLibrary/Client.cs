using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary
{
    /// <summary>
    /// Class for Client code
    /// </summary>
    public class Client
    {
        private static Socket clientSocket;
        private static IAsyncResult currentAsyncResult;
        private static byte[] byteData = new byte[2048];
        private static string currMessage;

        /// <summary>
        /// Tries to connect to server on IP and Port
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static Boolean Connect(IPAddress server, Int32 port)//String server, String message)
        {
            try
            {
                 clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //Server is listening on port 1000
                IPEndPoint ipEndPoint = new IPEndPoint(server, port);
              
                //Connect to the server
                clientSocket.BeginConnect(ipEndPoint, new AsyncCallback(OnConnect), null);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        /// <summary>
        /// When AsyncCallBack gets confirmation of connection it stops trying to connect,
        /// Sends a confirmation message,
        /// And starts recieving messages
        /// </summary>
        /// <param name="ar"></param>
        public static void OnConnect(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndConnect(ar);
                byteData = System.Text.Encoding.ASCII.GetBytes("Zach Connected^");

                //Send the message to the server
                clientSocket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnSend), null);
                currentAsyncResult = clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(Receive), clientSocket);
            }
            catch (Exception ex)
            {
               
            }
        }

        /// <summary>
        /// Method is called from outside the class to send messages
        /// </summary>
        /// <param name="message"></param>
        public static void Send(string message)
        {
            try
            {
                byte[] byteData = System.Text.Encoding.ASCII.GetBytes(message + '^');
                clientSocket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnSend), null);

            }
            catch (ObjectDisposedException)
            { }
            catch (Exception ex)
            {
               
            }
        }

        /// <summary>
        /// Run when AsyncCallBack gets confirmation of sent message
        /// </summary>
        /// <param name="ar"></param>
        public static void OnSend(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndSend(ar);
            }
            catch (ObjectDisposedException)
            { }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Run when AsyncCallBack gets a message
        /// </summary>
        /// <param name="ar"></param>
        public static void Receive(IAsyncResult ar)
        {
            try
            {
                if (ar == currentAsyncResult)
                {
                    Socket clientSocket = ar.AsyncState as Socket;
                    clientSocket.EndReceive(ar);

                    string data = UTF8Encoding.UTF8.GetString(byteData);
                    string[] dataAry;
                    //I am looping here to continune recieving data until the delimiter is recieved
                    while (!data.Contains('^')) { clientSocket.Receive(byteData, 0, byteData.Length, SocketFlags.None); data += UTF8Encoding.UTF8.GetString(byteData); }

                    dataAry = data.Split('^');//Split data at delimiter "^"
                    currMessage += dataAry[0];//Set class variable to be the current message

                    currentAsyncResult = clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(Receive), clientSocket);
                }
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// Called from outside the class to send the last recieved message
        /// </summary>
        /// <returns></returns>
        public static String SendBackRecieved()
        {
            string msg = currMessage;
            currMessage = null;
            return msg;
        }

        /// <summary>
        /// Called from outside class to close clientSocket
        /// </summary>
        public static void CloseClient()//Close the stream and connection
        {
            try
            {
                clientSocket.Close();

            }
            catch (Exception e) { }
        }

    }
}

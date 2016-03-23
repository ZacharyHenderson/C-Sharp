using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatLibrary
{
    /// <summary>
    /// Class for the server code
    /// </summary>
    public class Server
    {
        static Socket serverSocket;
        static IAsyncResult currentAsyncResult;
        static String currMessage;

        static byte[] byteData = new byte[2048];
        

        /// <summary>
        /// Tries to let clients connect on a given IP and Port
        /// </summary>
        /// <param name="localAddr"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static Boolean Connect(IPAddress localAddr, Int32 port)
        {
            try {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipEndPoint = new IPEndPoint(localAddr, port);

                serverSocket.Bind(ipEndPoint);
                serverSocket.Listen(4);

                serverSocket.BeginAccept(new AsyncCallback(AcceptClient), null);
                return true;
            }
            catch(Exception e) { return false; }
            
        }
        /// <summary>
        /// When the AsyncCallBack gets a response from a client it connects the two
        /// and starts listening for messages
        /// </summary>
        /// <param name="ar"></param>
        public static void AcceptClient(IAsyncResult ar)
        {
            try
            {
                serverSocket = serverSocket.EndAccept(ar);

                //Once the client connects then start receiving the messages 
                currentAsyncResult = serverSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(Receive), serverSocket);
            }
            catch (Exception ex)
            {
                
            }
        }

        /// <summary>
        /// Method used to send messages
        /// Called from outside the Server class
        /// </summary>
        /// <param name="message"></param>
        public static void Send(string message)
        {
            try
            {
                byte[] byteData = System.Text.Encoding.ASCII.GetBytes(message + '^');
                serverSocket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnSend), null);

            }
            catch (ObjectDisposedException)
            { }
            catch (Exception ex)
            {
               
            }
        }

        /// <summary>
        /// Runs when the AsyncCallBack gets confirmation that a message was sent
        /// Just Ends the sending of the old socket to signify that there will
        /// be no further messages
        /// </summary>
        /// <param name="ar"></param>
        private static void OnSend(IAsyncResult ar)
        {
            try
            {
                serverSocket.EndSend(ar);
            }
            catch (ObjectDisposedException)
            {  }
            catch (Exception ex)
            {
                Console.WriteLine("OnSend2");
            }
        }

        /// <summary>
        /// Run when AsyncCallBack gets a message
        /// Parses the message and stores it in a class level var 
        /// for further use
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
                    while (!data.Contains("^")) { clientSocket.Receive(byteData, 0, byteData.Length, SocketFlags.None); data += UTF8Encoding.UTF8.GetString(byteData); }

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
        /// Called from outside server class and returns the last recieved message
        /// </summary>
        /// <returns></returns>
        public static String SendBackRecieved()
        {
            string msg = currMessage;
            currMessage = null;
            return msg;
        }

        /// <summary>
        /// Closes the server socket
        /// </summary>
        public static void CloseServer()//Close the stream and connection
        {
            //serverSocket.Close(); 
            try
            {
                serverSocket.Close();

            }
            catch (Exception e) { }
        }
    }
}

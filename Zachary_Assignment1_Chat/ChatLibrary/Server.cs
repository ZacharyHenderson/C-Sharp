using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatLibrary
{
    public class Server
    {
        private static TcpListener serverConnection;
        private static TcpClient clientConnection;
        private static NetworkStream stream;

        public static Boolean Connect(String server, Int32 port)
        {
            try
            {
                //Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse(server);
                serverConnection = new TcpListener(localAddr, port);

                serverConnection.Start();
                clientConnection = serverConnection.AcceptTcpClient();
                stream = clientConnection.GetStream();

                return true;
            }
            catch (SocketException e) { return false; }
        }

        public static Boolean Send(String message)
        {
            try
            {
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message); //Convert the string into a Byte array so it can be sent
                stream.Write(data, 0, data.Length);

                return true;
            }
            catch (ArgumentNullException e) { return false; }//Catch any errors and send false back

        }

        public static String Receive()
        {
            try
            {
                String message = null;
                Byte[] data = new Byte[256];

                if (stream.DataAvailable)// If there was data sent grab it. Else do nothing
                {
                    Int32 bytes = stream.Read(data, 0, data.Length);
                    message = System.Text.Encoding.ASCII.GetString(data, 0, bytes); //Convert the Byte array received into a String
                }

                return message;
            }
            catch (Exception e) { return null; }//Catch any errors and send null back
        }

        public static void CloseServer()//Close the stream and connection
        {
            stream.Close();
            clientConnection.Close();
            //serverConnection.Close();
        }
    }
}

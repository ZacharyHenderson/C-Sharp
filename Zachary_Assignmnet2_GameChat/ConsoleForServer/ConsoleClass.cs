using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatLibrary;
using System.Net;
using GameChat;

namespace ConsoleForServer
{
    /// <summary>
    /// Class to run the console
    /// </summary>
    public class ConsoleClass
    {
        /// <summary>
        /// Main method, used to run server and start client
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            ConsoleClass instance = new ConsoleClass();
            string tempArg = string.Empty;
            foreach (string s in args)
            {
                if (s.Equals("-server")) { tempArg = s; }
            }


            //If they sent "-server" they wanted a server so use the server code
            if (tempArg.Equals("-server"))
            {

                IPAddress IP = IPAddress.Parse("127.0.0.1");
                Int32 Port = 13000;

                Server server = new Server();
                Console.Write("Waiting for connection... ");

                Server.Connect(IP, Port);                   

                String tempInput = String.Empty; //Variable used to store user input

                while (true) {
                    String temp = null;
                    
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo key = Console.ReadKey(true);

                        if (key.Key == ConsoleKey.I)
                        {

                            Console.Write(">> ");
                            tempInput = Console.ReadLine();
                            Server.Send(tempInput);
                            if (tempInput.Equals("quit")) { Console.WriteLine("Exiting..."); break; } //If the user types "quit" Break out of the loop and end the program
                        }
                    }

                    temp = Server.SendBackRecieved();
                    if (temp == null) { }//If you recieved nothing Do nothing
                    else if (temp.Equals("quit")) { Console.WriteLine("Client has disconnected. Exiting...");  return; } //If the partner quit. Do the same
                    else { Console.WriteLine(temp); } //else just print out what the user sent
                }//
            }
            else //Else they wanted a client
            {
                Driver.Main(args);
            }

            Server.CloseServer();

         }//end main

        
    }
}

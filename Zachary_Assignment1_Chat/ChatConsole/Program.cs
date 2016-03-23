using System;
using ChatLibrary;


namespace ChatConsole
{

    class Program
    {

        static void Main(string[] args)
        {
            String IP = "127.0.0.1";
            Int32 Port = 13000;

            Program instance = new Program();

            //Check for Command Line Arguments specifically "-server"
            String tempArg = String.Empty;
            foreach (string s in args)
            {
                if (s.Equals("-server")) { tempArg = s; }
            }


            //If they sent "-server" they wanted a server so use the server code
            if (tempArg.Equals("-server")) 
            {
                Console.Write("Waiting for connection... ");
                while (!Server.Connect(IP, Port)) { }
                Console.WriteLine("Connected!");

                //Run the main loop and give it the server parameter
                instance.HomeLoop("server");

                Server.CloseServer();
            }
            else //Else they wanted a client
            {
                Console.Write("Attempting to connect to server... ");
                while (!Client.Connect(IP, Port)) { }
                Console.WriteLine("Connected!");

                //Run the main loop and give it the client parameter
                instance.HomeLoop("client");

                Client.CloseClient();

            }//End if/else

            
        }//end main

        //This method contains the main code that needs to be looped
        public void HomeLoop(String type)
        {
            String tempInput = String.Empty; //Variable used to store user input

            while (true)
            {
                //This is checking for key presses
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.I)
                    {
                        if (type.Equals("server"))
                        {
                            Console.Write(">> ");
                            tempInput = Console.ReadLine();
                            Server.Send(tempInput);
                            if (tempInput.Equals("quit")) { break; } //If the user types "quit" Break out of the loop and end the program
                        }
                        else if (type.Equals("client"))
                        {
                            Console.Write(">> ");
                            tempInput = Console.ReadLine();
                            Client.Send(tempInput);
                            if (tempInput.Equals("quit")) { break; } //If the user types "quit" Break out of the loop and end the program
                        }
                    }
                }//end keyavailable 
                
                   
                String temp = null;

                if (type.Equals("server"))
                {
                    temp = Server.Receive();
                    if (temp == null) { }//If you recieved nothing Do nothing
                    else if (temp.Equals("quit")) { return; } //If the partner quit. Do the same
                    else { Console.WriteLine(temp); } //else just print out what the user sent
                }
                else if (type.Equals("client"))
                {
                    temp = Client.Receive();
                    if (temp == null) { }
                    else if(temp.Equals("quit")) { return; }
                    else { Console.WriteLine(temp); }
                }

            }//end while

        }//end HomeLoop

    }
}

using System;
using System.IO;


namespace Logging
{
    /// <summary>
    /// Class for logging
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// Method to write to log file
        /// </summary>
        /// <param name="message"></param>
        public void WriteLog(string message)
        {
            String name = "Logs\\";
            name += (DateTime.Now.ToFileTime()).ToString();
            name += ".txt";
            File.WriteAllText(name, message); //writing
        }

    }
}

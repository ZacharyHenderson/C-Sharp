using System;
using System.Windows.Forms;
using ChatLibrary;
using System.Net;

namespace GameChat
{
    /// <summary>
    /// Class to init Form1 when run
    /// </summary>
    public class Driver
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        [STAThread]
        public static void Main(string[] args)
        {
            
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
                  
        }
        
    }
}

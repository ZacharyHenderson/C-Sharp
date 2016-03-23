using System;
using System.Windows.Forms;
using ChatLibrary;
using System.Net;
using Logging;

namespace GameChat
{
    public partial class Form1 : Form
    {

        static IPAddress IP = IPAddress.Parse("127.0.0.1");
        static Int32 Port = 13000;
        Client client = new Client();
        private string chatLog;
        private readonly Timer timer = new Timer();
        private bool enable = false;


        /// <summary>
        /// Init Form
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            
        }

        /// <summary>
        /// Create a timer tick to execute code with
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            
            timer.Interval = 10;
            timer.Tick += TimerTick;
            timer.Enabled = true;
        }

        /// <summary>
        /// Check for new messages every timer tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimerTick(object sender, EventArgs e)
        {
            GetNewMessages();
            if (!enable) { btnSend.Enabled = false; }
            else { btnSend.Enabled = true; }
        }

        /// <summary>
        /// Connect to server when they click "Connect"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Client.Connect(IP, Port);
            enable = true;
        }

        /// <summary>
        /// Close the app when they click "disconnect" via method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
           CloseClient();
        }

        /// <summary>
        /// Send the message when they click "Send"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if(txtMessage.Text == "quit") {CloseClient(); }
                Client.Send(txtMessage.Text);
                DisplayMessage(">>" + txtMessage.Text);
                txtMessage.Text = null;
            }
            catch (Exception)
            {
               
            }
        }

        /// <summary>
        /// When the form closes for whatever reason, Execute close method
        /// </summary>
        private void Form1_formclosing()
        {
            CloseClient();
            timer.Dispose();
        }

        /// <summary>
        /// Adds the current message from either the client or server
        /// To the ChatLog and the txtConsole for the user to view history
        /// </summary>
        /// <param name="message"></param>
        public void DisplayMessage(string message)
        {
            if(message == "quit") { CloseClient(); message = "Disconnected From Server"; }

            txtConvo.Text += message + Environment.NewLine;
            char[] ary = message.ToCharArray();
            if (ary[0] == '>') {
                String[] formattedMessage = message.Split('>');
                chatLog += DateTime.Now.ToString() + " Client: " + formattedMessage[2] + "\r\n";
            }
            else
            {
                chatLog += DateTime.Now.ToString() + " Server: " + message + "\r\n";
            }
        }

        /// <summary>
        /// Method to get new messages
        /// </summary>
        private void GetNewMessages()
        {
            String temp = Client.SendBackRecieved();
            if (temp != null) { DisplayMessage(temp); }
        }

        /// <summary>
        /// Method that closes everything down: kills timer, writes log
        /// informs the server to quit, Closes the client, exits the app
        /// </summary>
        private void CloseClient()
        {
            enable = false;
            WriteLog();
            Client.Send("quit");
            Client.CloseClient();
            
        }

        /// <summary>
        /// Uses the writelog library to write to a log
        /// </summary>
        private void WriteLog()
        {
            Logger log = new Logger();
            log.WriteLog(chatLog);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseClient();
            timer.Dispose();
            Application.Exit();
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Collections;
using System.Threading;

using IrcDotNet;

namespace NerdChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// 
    /// https://tools.ietf.org/html/rfc1459
    /// 
    /// </summary>
    public partial class MainWindow : Window
    {
        public IRCSocket irc;
        public List<String> m_BlackListHosts = new List<string>();
      

        public MainWindow()
        {
            InitializeComponent();
            this.messageField.KeyDown += textBoxEnter;

            //Instantiate an IRC client session
            irc = new IRCSocket(System.Net.Dns.Resolve("irc.freenode.net").AddressList[0].ToString(), 6667, "tummcid");

            //IrcRegistrationInfo info = new IrcUserRegistrationInfo()
            //{
            //    NickName = Environment.UserName, // "NerdChat",
            //    UserName = Environment.UserName + "NerdChat",
            //    RealName = "NerdChat"
            //};

            //Open IRC client connection
            irc.doWork();
            //irc.RawMessageReceived += IrcClient_Receive;

            // Add server to treeview
            TreeViewItem serverTreeItem = new TreeViewItem();
            serverTreeItem.Header = "irc.freenode.net";
            channelTree.Items.Add(serverTreeItem);

            // Add some dummy channels for testing
            for (int i = 0; i < 10; i++) {
                TreeViewItem dummyItem = new TreeViewItem();
                dummyItem.Header = "#dummy" + i;
                serverTreeItem.Items.Add(dummyItem);
            }
        }
        /// <summary>
        /// Service routine for recieving data from the IRC server.  Keep this routine simple and jump to another routine as soon as the nature of the message is known.  This should run in the main server thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IrcClient_Receive(object sender, IrcRawMessageEventArgs e)
        {
            //Check for a prefix indicator.  If this isnt present the message cannot be processed
            if (!e.RawContent.Contains(':'))
                throw new ArgumentOutOfRangeException("Unable to parse message. " + e.RawContent);
            String[] parsedPrefix = e.RawContent.Split(':')[1].Split(' ');

            //Check if prefix is well formed
            if (parsedPrefix.Count() < 2)
                throw new ArgumentOutOfRangeException("Unable to parse prefix. " + e.RawContent);

            string userHost = parsedPrefix[0];
            string command = parsedPrefix[1];
            string userName = parsedPrefix[2];
            string logText = "";

            //Check if source is blacklisted
            if (m_BlackListHosts.Contains(userHost))
            {
                //TODO log this?
                return;
            }
            //Examine the command
            switch (command)
            {
                case "PRIVMSG":
                    logText = "PRIVMSG from " + userName;
                    //HandlePrivMsg(userName, e.RawContent.Substring(e.RawContent.IndexOf(':', 1)+1)); //skip the first ':' and grab everything after the second
                    break;

                default:
                    logText = "Unknown Message " + e.RawContent;
                    break;
            }
                   
            //var client = (IrcClient)sender;
            //Log this in the main window
            chatBox.Dispatcher.Invoke(delegate
            {
                chatBox.AppendText(logText + "\n");
                chatBox.ScrollToEnd();
            });
        }
        /// <summary>
        /// Place functionality here that will respond to private messages.  This should exist outside of the main thread and so we can take our time here.
        /// </summary>
        /// <param name="fromNick">nickname of the user that sent this private message.  RFC specifies this can be a list of names or channels separated with commas</param>
        /// <param name="message">content of this message.  RFC 1459 sets a hard limit here around ~512 characters</param>
        public void HandlePrivMsg(String message)
        {
            //TODO write a privmsg handler
            //TODO Pass privmsg to handler
            chatBox.Dispatcher.Invoke(delegate
            {
                chatBox.AppendText(message + "\n");
                chatBox.ScrollToEnd();
            });
        }

        private void textBoxEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                String dest = destField.Text;
                String text = messageField.Text;

                chatBox.AppendText(dest + " > " + text + "\n");
                chatBox.ScrollToEnd();

                irc.SendString(text);
                messageField.Clear();
            }
        }
    }
}

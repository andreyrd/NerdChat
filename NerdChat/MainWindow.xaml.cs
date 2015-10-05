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
    /// </summary>
    public partial class MainWindow : Window
    {
        public StandardIrcClient irc;
      

        public MainWindow()
        {
            InitializeComponent();

            this.messageField.KeyDown += textBoxEnter;


            irc = new StandardIrcClient();
            IrcRegistrationInfo info = new IrcUserRegistrationInfo()
            {
                NickName = "NerdChat",
                UserName = "NerdChat",
                RealName = "NerdChat"
            };

            irc.Connect("irc.freenode.net", false, info);
            irc.RawMessageReceived += IrcClient_Receive;
        }

        private void IrcClient_Receive(object sender, IrcRawMessageEventArgs e)
        {
            var client = (IrcClient)sender;
            chatBox.Dispatcher.Invoke(delegate
            {
                chatBox.AppendText(e.RawContent + "\n");
                chatBox.ScrollToEnd();
            });
        }

        private void textBoxEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                String dest = destField.Text;
                String text = messageField.Text;

                chatBox.AppendText(dest + ": " + text + "\n");
                chatBox.ScrollToEnd();

                irc.LocalUser.SendMessage(dest, text);
                messageField.Clear();
            }
        }
    }
}

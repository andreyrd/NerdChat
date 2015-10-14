using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NerdChat
{
    class IRCSocket
    {
        const int IN_BUFFER = 512;
        static Socket m_Con;
        static Thread listenThread;
        String m_userName;
        String m_Password;
        String m_ServerAddress;
        int m_Port;

        public Queue<string> m_Inbound;
        public Queue<string> m_Outbound;

        public IRCSocket(String server, int port, String username, String pass = "")
        {
            m_userName = username;
            m_Password = pass;
            m_ServerAddress = server;
            m_Port = port;

            m_Con = new Socket(AddressFamily.InterNetwork,
          SocketType.Stream, ProtocolType.Tcp);

            m_Inbound = new Queue<string>();
            m_Outbound = new Queue<string>();

        }
        public void doWork()
        {

            //              m_Con.Connect(System.Net.Dns.Resolve("irc.freenode.net").AddressList[0], 6667);              
            m_Con.Connect(System.Net.IPAddress.Parse(m_ServerAddress), m_Port);
            SendString("USER " + m_userName);
            listenThread = new Thread(() =>
            {
                byte[] inData = new byte[IN_BUFFER];
                StringBuilder inbound = new StringBuilder();

                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"bouncer.txt", true))
                while (true)
                {
                    Array.Clear(inData, 0, IN_BUFFER);
                    int inSize = m_Con.Receive(inData, IN_BUFFER, SocketFlags.None);
                    //file.WriteLine(Encoding.ASCII.GetString(inData));
                    inbound.Append(Encoding.ASCII.GetString(inData).Substring(0, inSize));
                    if ((inSize == IN_BUFFER) || (!Encoding.ASCII.GetString(inData).Substring(inSize - 2, 2).Equals("\r\n")))
                        continue;

                    foreach (String line in inbound.ToString().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        String extra;
                        String target;

                        //Check for a prefix indicator.  If this isnt present the message cannot be processed.  We should probably throw an exception.
                        if (!line.Contains(':'))
                            continue;
                        if (line.Substring(0, 4).Equals("PING"))
                        {//This command is parsed in a special way.
                            SendString("PONG " + line.Split(':')[1]);
                            continue;
                        }
                        String prefix = line.Substring(line.IndexOf(':')).Split(' ')[0];

                        //Check if prefix is well formed
                        if (prefix.Length == 0)
                            throw new Exception("unable to parse prefix");

                        string command = line.Substring(prefix.Length).Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[0];
                        string logText = "";

                        //Check if source is blacklisted
                        //if (m_BlackListHosts.Contains(prefix))
                        //{
                        //    //TODO log this?
                        //    return;
                        //}

                        try
                        {
                            extra = line.Substring(command.Length + prefix.Length);
                            if (extra.Length > 0)
                                extra = extra.Substring(1); //Remove the ':' cos who needs it?
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("pasring error", ex);
                        }
                        //Examine the command
                        if ((command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0].Length == 3) &&
                            (!command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("WHO")))//duurrrrr?
                        {
                            logText = " number ";
                        }
                        else
                            switch (command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0])
                            {
                                case "NOTICE":
                                case "PRIVMSG":
                                    target = command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1];
                                    if (target.Equals("AUTH"))
                                    {
                                        SendString("PASS " + System.Configuration.ConfigurationManager.AppSettings["Auth"]);
                                        SendString("NICK " + m_userName);
                                    }
                                    logText = target + prefix.Split('!')[0] + "> " + extra;
                                    break;
                                default:
                                    logText = "Unknown Command " + command;
                                    break;
                            }
                        Console.WriteLine(logText);
                        m_Inbound.Enqueue(logText);
                    }

                    inbound.Clear();
                }//Read socket again
            });
            listenThread.Start();
            // Convert the string data to byte data using ASCII encoding.
            Console.ReadLine();
            listenThread.Abort();
            m_Con.Close();
        }
        private static void SendString(string data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data + "\r\n");

            // Begin sending the data to the remote device.
            m_Con.Send(byteData);
        }
    }
}

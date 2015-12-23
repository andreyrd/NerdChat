using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NerdChat
{
    public class IRCSocket
    {
        const int IN_BUFFER = 512;
        Socket m_Con;
        Thread listenThread;
        Thread sendThread;
        String m_userName;
        String m_Password;
        String m_ServerAddress;
        int m_Port;

        public Queue<IRCMessage> m_Inbound;
        public Queue<IRCMessage> m_Outbound; //IRCMessage

        public IRCSocket(String server, int port, String username, String pass = "")
        {
            m_userName = username;
            m_Password = pass;
            m_ServerAddress = server;
            m_Port = port;

            m_Con = new Socket(AddressFamily.InterNetwork,
          SocketType.Stream, ProtocolType.Tcp);

            m_Inbound = new Queue<IRCMessage>();
            m_Outbound = new Queue<IRCMessage>();

            sendThread = new Thread(() =>
            {
                while (true)
                {
                    if (m_Outbound.Count == 0)
                        continue;

                    IRCMessage sendData = m_Outbound.Dequeue();

                    m_Con.Send(Encoding.ASCII.GetBytes(sendData.command + " " + sendData.payload + "\r\n"));
                }

            });
                listenThread = new Thread(() =>
            {
                byte[] inData = new byte[IN_BUFFER];
                StringBuilder inbound = new StringBuilder();

                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"bouncer.txt", true))
                while (true)
                {
                    Array.Clear(inData, 0, IN_BUFFER);
                    int inSize = m_Con.Receive(inData, IN_BUFFER, SocketFlags.None);
                    if (inSize < 1)
                        break;
                    inbound.Append(Encoding.ASCII.GetString(inData).Substring(0, inSize));
                    if ((inSize == IN_BUFFER) || (!Encoding.ASCII.GetString(inData).Substring(inSize - 2, 2).Equals("\r\n")))
                        continue;

                    foreach (String line in inbound.ToString().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        //Check for a prefix indicator.  If this isnt present the message cannot be processed.  We should probably throw an exception.
                        if (!line.Contains(':'))
                            continue;
                        if (line.Substring(0, 4).Equals("PING"))
                        {//This command is parsed in a special way.
                            SendString("PONG " + line.Split(':')[1]);
                            continue;
                        }
                            string logText = "";
                        IRCMessage inM = new IRCMessage();
                        try
                        {
                            inM.host = line.Substring(line.IndexOf(':')).Split(' ')[0];
                            //Check if source is blacklisted
                            //if (m_BlackListHosts.Contains(inM.host))
                            //{
                            //    //TODO log this?
                            //    return;
                            //}

                            //Check if prefix is well formed
                            if (inM.host.Length == 0)
                                throw new FormatException("unable to parse prefix");

                            inM.command = line.Substring(inM.host.Length).Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                            if (inM.command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Count() > 1)
                            {
                                inM.dest = inM.command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1];
                                inM.command = inM.command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];
                                if (inM.command == "JOIN")
                                    continue;
                                //inM.payload = line.Substring(inM.command.Length + inM.host.Length + inM.command.Length);
                                inM.payload = line.Split(new String[] { inM.dest } ,StringSplitOptions.RemoveEmptyEntries)[1];
                                if (inM.payload.Length > 0)
                                    inM.payload = inM.payload.Substring(1); //Remove the ':' cos who needs it?
                            }

                        }
                        catch (Exception ex)
                        {
                            throw new Exception("pasring error", ex);
                        }
                        //Examine the command
                        if ((inM.command.Length == 3) &&
                            (!inM.command.Equals("WHO")))//duurrrrr?
                        {
                            logText = " number ";
                        }
                        else
                            switch (inM.command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0])
                            {
                                case "NOTICE":
                                case "PRIVMSG":
                                    
                                    if (inM.dest.Equals("AUTH"))
                                    {
                                        SendString("PASS " + m_userName + ":" + m_Password);// System.Configuration.ConfigurationManager.AppSettings["Auth"]);
                                        SendString("NICK " + m_userName);
                                    }
                                    logText = inM.dest + "||" + inM.host + "> " + inM.payload;
                                    break;
                                default:
                                    logText = "Unknown Command " + inM.command;
                                    break;
                            }

                        //Console.WriteLine(logText);
                        m_Inbound.Enqueue(inM);
                    }
                    inbound.Clear();
                }//Read socket again
                m_Con.Close();
                
            });

        }
        public void doWork()
        {

            //              m_Con.Connect(System.Net.Dns.Resolve("irc.freenode.net").AddressList[0], 6667);              
            m_Con.Connect(System.Net.IPAddress.Parse(m_ServerAddress), m_Port);

            listenThread.Start();
            m_Outbound.Enqueue(new IRCMessage("NICK", m_userName));
            m_Outbound.Enqueue(new IRCMessage("USER" , m_userName + " 8 * : username"));
            //m_Outbound.Enqueue("JOIN #nerdchat");
            sendThread.Start();
            //SendString("USER " + m_userName + " 8 * : name"); 
            // Convert the string data to byte data using ASCII encoding.
            //Console.ReadLine();
            //listenThread.Abort();
            //m_Con.Close();
        }
        public void SendString(string data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data + "\r\n");

            // Begin sending the data to the remote device.
            m_Con.Send(byteData);
        }
    }
    public class IRCMessage
    {
        public String dest;
        public String host = "localhost";
        public String payload;
        public String command;

        public IRCMessage()
        { }

        public IRCMessage(string Command)
        {
            command = Command;
        }

        public IRCMessage(string Command, string Payload)
        {
            command = Command;
            payload = Payload;
        }
    }

    public class PrivMessage : IRCMessage
    {
        public String target;
        public String message;

    }
}

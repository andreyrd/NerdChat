using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdChat
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc1459
    /// </summary>
    class RFC1459
    {
        public static readonly IEnumerable<string> commands = new List<string>()
            {
                "ADMIN",
                "AWAY",
                "CONNECT",
                "ERROR",
                "INFO",
                "INVITE",
                "ISON",
                "JOIN",
                "KICK",
                "KILL",
                "LINKS",
                "LIST",
                "MODE",
                "NAMES",
                "NICK",
                "NOTICE",
                "OPER",
                "PART",
                "PASS",
                "PING",
                "PONG",
                "PRIVMSG",
                "QUIT",
                "REHASH",
                "RESTART",
                "SERVER",
                "SQUIT",
                "STATS",
                "SUMMON",
                "TIME",
                "TOPIC",
                "TRACE",
                "USER",
                "USERHOST",
                "USERS",
                "VERSION",
                "WALLOPS",
                "WHO",
                "WHOIS",
                "WHOWAS"
        };
    }
}

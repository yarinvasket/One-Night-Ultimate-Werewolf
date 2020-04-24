using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace One_Night_Ultimate_Werewolf
{
    class Player
    {
        public string name, card, role;
        public TcpClient client;
        public NetworkStream stream;

        public Player(TcpClient client, NetworkStream stream, string name)
        {
            this.name = name;
            this.client = client;
            this.stream = stream;
        }
    }
}

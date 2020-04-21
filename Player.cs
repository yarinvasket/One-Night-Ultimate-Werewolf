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
        private string name, card, role;
        public TcpClient client;
        public NetworkStream stream;

        public Player(TcpClient client, NetworkStream stream, string name)
        {
            this.name = name;
            this.client = client;
            this.stream = stream;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return name;
        }

        public void SetCard(string card)
        {
            this.card = card;
        }

        public string GetCard()
        {
            return card;
        }

        public void SetRole(string role)
        {
            this.role = role;
        }

        public string GetRole()
        {
            return role;
        }
    }
}

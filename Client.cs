using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace One_Night_Ultimate_Werewolf
{
    public partial class Client : Form
    {
        private string username;
        private string ip;
        private Label card;
        private int clientid;
        private TcpClient client;

        public Client(string username, string ip)
        {
            this.username = username;
            this.ip = ip;
            client = new TcpClient(ip, One_Night_Ultimate_Werewolf.Menu.port);
            NetworkStream stream = client.GetStream();
            stream.Write(new byte[] { 8 }, 0, 1);
        }

        private void Client_Load(object sender, EventArgs e)
        {

        }
        public void SetUsername(string username)
        {
            this.username = username;
        }
    }
}

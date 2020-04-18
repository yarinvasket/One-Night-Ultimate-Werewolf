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
            try
            {
                client = new TcpClient(ip, One_Night_Ultimate_Werewolf.Menu.port);
            }
            catch
            {
                DialogResult response = MessageBox.Show("Invalid IP", "Error", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);
                if (response.ToString() == "No")
                {
                    DialogResult response2 = MessageBox.Show("The IP is invalid, don't no me.", "What do you mean no?", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Exclamation);
                    if (response2.ToString() == "Ignore")
                    {
                        Ohno connectForm = new Ohno();
                        this.Hide();
                        Controls.Clear();
                        connectForm.Show();
                        return;
                    }
                }
                One_Night_Ultimate_Werewolf.Menu.OnClose(null, null);
            }
            NetworkStream stream = client.GetStream();
            byte[] bytes = BitConverter.GetBytes(username.Length);
            stream.Write(bytes, 0, bytes.Length);
            byte[] name = Encoding.ASCII.GetBytes(username);
            stream.Write(name, 0, name.Length);
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

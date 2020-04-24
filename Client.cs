using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace One_Night_Ultimate_Werewolf
{
    public partial class Client : Form
    {
        private string username;
        private string ip;
        List<string> players;
        private Label connectedPlayers;
        private TcpClient client;
        private NetworkStream stream;
        private string role;

        public Client(string username, string ip)
        {
            this.username = username;
            this.ip = ip;
            Label waiting = new Label
            {
                Text = "Waiting for the game to start...\nConnected players:",
                Font = new Font("Arial", 24),
                Location = new Point(20, 20),
                Size = new Size(500, 75)
            };
            Controls.Add(waiting);

            connectedPlayers = new Label
            {
                Font = new Font("Arial", 16),
                Location = new Point(20, 125),
                Size = new Size(100, 600)
            };
            Controls.Add(connectedPlayers);

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
                        Ohno connectForm = new Ohno
                        {
                            StartPosition = FormStartPosition.Manual,
                            Location = this.Location
                        };
                        this.Hide();
                        this.WindowState = FormWindowState.Minimized;
                        this.ShowInTaskbar = false;
                        Controls.Clear();
                        connectForm.Show();
                        return;
                    }
                }
                One_Night_Ultimate_Werewolf.Menu.OnClose(null, null);
            }
            stream = client.GetStream();
            byte[] name = Encoding.ASCII.GetBytes(username);
            stream.Write(name, 0, name.Length);

            byte[] bytes = new byte[1024];
            int length = stream.Read(bytes, 0, bytes.Length);
            string str = System.Text.Encoding.UTF8.GetString(bytes, 0, length);
            str = str.Substring(1);
            players = str == "" ? new List<string>() : str.Split('\0').ToList<string>();
            for (int i = 0; i < players.Count; i++)
            {
                Host.AddText(connectedPlayers, players[i]);
            }

            Thread waitUntilGameStarts = new Thread(WaitUntilGameStarts);
            waitUntilGameStarts.Start();
        }

        private void WaitUntilGameStarts()
        {
            while (true)
            {
                byte[] bytes = new byte[64];
                int length = stream.Read(bytes, 0, bytes.Length);
                string str = System.Text.Encoding.UTF8.GetString(bytes, 0, length);
                if (str[0] == '\0')
                {
                    role = str.Substring(1);
                    byte[] bytes2 = new byte[1024];
                    int length2 = stream.Read(bytes2, 0, bytes2.Length);
                    players = System.Text.Encoding.UTF8.GetString(bytes2, 0, length2).Split('\0').ToList<string>();
                    players.Remove(username);
                    break;
                }
                players.Add(str);
                this.Invoke(new AddTextDelegate(Host.AddText), connectedPlayers, str);
            }
        }

        public void SetUsername(string username)
        {
            this.username = username;
        }
    }
}

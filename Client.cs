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
            Label you = new Label
            {
                Text = "YOU - "+username,
                ForeColor = Color.Red,
                Font = new Font("Arial", 20),
                Location = new Point(20,100),
                Size = new Size(500, 75)
            };
            Controls.Add(you);

            connectedPlayers = new Label
            {
                Font = new Font("Arial", 16),
                Location = new Point(20, 125),
                Size = new Size(300, 600)
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
            WriteString(username);

            string str = ReadString(1024);
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
                string str = ReadString(64);
                if (str[0] == '\0')
                {
                    role = str.Substring(1);
                    players = ReadString(1024).Split('\0').ToList<string>();
                    players.Remove(username);
                    Controls.Clear();
                    FormBorderStyle = FormBorderStyle.None;
                    WindowState = FormWindowState.Maximized;
                    break;
                }
                players.Add(str);
                this.Invoke(new AddTextDelegate(Host.AddText), connectedPlayers, str);
            }
        }

        public string ReadString(int byteLength)
        {
            byte[] bytes = new byte[byteLength];
            int length = stream.Read(bytes, 0, bytes.Length);
            return System.Text.Encoding.UTF8.GetString(bytes, 0, length);
        }

        public void WriteString(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}

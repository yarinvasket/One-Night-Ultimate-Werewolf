using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace One_Night_Ultimate_Werewolf
{
    public delegate void AddTextDelegate(Control control, string text);

    public partial class Host : Form
    {
        private string ip = new WebClient().DownloadString("http://icanhazip.com");
        private TcpListener listener;
        private List<Player> players = new List<Player>();
        private string[] deck = {"Hunter", "Tanner", "Insomniac", "Drunk", "Troublemaker", "Robber", "Sear", "Mason", "Mason", "Minion", "Werewolf", "Werewolf", "Doppelganger"};
        private TextBox console;
        private Thread playerReciever;

        public Host()
        {
            this.FormClosing += One_Night_Ultimate_Werewolf.Menu.OnClose;
            listener = new TcpListener(IPAddress.Parse("0.0.0.0"), One_Night_Ultimate_Werewolf.Menu.port);
            Shuffle<string>(deck);
            console = new TextBox
            {
                Location = new Point(60, 25),
                Size = new Size(600, 750),
                Enabled = false,
                Multiline = true,
                Font = new Font("Arial", 21)
            };
            Controls.Add(console);

            Label IPLabel = new Label
            {
                Size = new Size(250, 40),
                Text = "Server IP:",
                Location = new Point(60, 805),
                Font = new Font("Arial", 32)
            };
            Controls.Add(IPLabel);

            TextBox IPBox = new TextBox
            {
                Text = ip,
                Location = new Point(313, 805),
                Font = new Font("Arial", 32),
                Size = new Size(350, 40)
            };
            Controls.Add(IPBox);

            Button startGame = new Button();
            startGame.Text = "Start Game";
            startGame.Location = new Point(325, 875);
            startGame.Size = new Size(100, 50);
            startGame.Click += StartGame_Click;
            Controls.Add(startGame);

            playerReciever = new Thread(WaitForPlayers);
            playerReciever.Start();
        }

        private void StartGame_Click(object sender, EventArgs e)
        {
            if (players.Count < 4)
            {
                MessageBox.Show("Needs at least 4 players to start", "Can't start game!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            playerReciever.Suspend();
            Controls.RemoveAt(Controls.Count - 1);
            int offset = 0;
            for (int i = 0; i < players.Count; i++)
            {
                byte[] bytes = Encoding.ASCII.GetBytes('\0' + deck[i]);
                try
                {
                    players[i].stream.Write(bytes, 0, bytes.Length);
                }
                catch
                {
                    this.Invoke(new AddTextDelegate(AddText), console, players[i].name + " disconnected");
                    players.RemoveAt(i);
                    i--;
                }
            }

            string str = "";
            for (int i = 0; i < players.Count; i++)
            {
                str += players[i].name + '\0';
            }
            str = str.Substring(0, str.Length - 1);

            for (int i = 0; i < players.Count; i++)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(str);
                players[i].stream.Write(bytes, offset, bytes.Length);
            }

            AddText(console, "Game started");
        }

        public static void Shuffle<T>(T[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                int idx = One_Night_Ultimate_Werewolf.Menu.random.Next(i, arr.Length);
                T temp = arr[i];
                arr[i] = arr[idx];
                arr[idx] = temp;
            }
        }

        public void WaitForPlayers()
        {
            listener.Start();
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                byte[] data = new byte[64];
                int bytes = stream.Read(data, 0, data.Length);
                string name = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
                this.Invoke(new AddTextDelegate(AddText), console, name + " joined");
                string names = players.Count == 0 ? "\0" : "";
                for (int i = 0; i < players.Count; i++)
                {
                    names += '\0' + players[i].name;
                    try
                    {
                        players[i].stream.Write(data, 0, bytes);
                    }
                    catch
                    {
                        this.Invoke(new AddTextDelegate(AddText), console, players[i].name + " disconnected");
                        players.RemoveAt(i);
                        i--;
                    }
                }
                byte[] bytesNames = Encoding.ASCII.GetBytes(names);
                stream.Write(bytesNames, 0, bytesNames.Length);
                players.Add(new Player(client, stream, name));
            }
        }

        public static void AddText(Control control, string text)
        {
            control.Text += text + Environment.NewLine;
        }

        public static byte[] ObjectToByteArray(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }
    }
}

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
        private TextBox console;
        private Thread playerReciever;

        public Host()
        {
            this.FormClosing += One_Night_Ultimate_Werewolf.Menu.OnClose;
            listener = new TcpListener(IPAddress.Parse("0.0.0.0"), One_Night_Ultimate_Werewolf.Menu.port);
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
            Controls.RemoveAt(Controls.Count - 1);
        }

        public void WaitForPlayers()
        {
            listener.Start();
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                byte[] data = new byte[256];
                int bytes = stream.Read(data, 0, data.Length);
                string name = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
                this.Invoke(new AddTextDelegate(AddText), console, name + " joined");
                players.Add(new Player(client, stream, name));
            }
        }

        public void AddText(Control control, string text)
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

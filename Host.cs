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
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), One_Night_Ultimate_Werewolf.Menu.port);

            playerReciever = new Thread(WaitForPlayers);
            playerReciever.Start();
            console = new TextBox
            {
                Location = new Point(25, 25),
                Size = new Size(200, 400),
                Enabled = false
            };
            Controls.Add(console);
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
                players.Add(new Player(client, stream, name));
            }
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

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
using Timer = System.Windows.Forms.Timer;

namespace One_Night_Ultimate_Werewolf
{
    public delegate void AddTextDelegate(Control control, string text);

    public partial class Host : Form
    {
        private string ip = new WebClient().DownloadString("http://icanhazip.com");
        private TcpListener listener;
        private List<Player> players = new List<Player>();
        private string[] middleCards = new string[3];
        private string[] deck = { "Doppelganger", "Hunter", "Tanner", "Insomniac", "Drunk", "Troublemaker", "Robber", "Seer", "Mason", "Mason", "Minion", "Werewolf", "Werewolf" };
        private TextBox console;
        private Thread playerReciever;
        private int ind = 0;
        private Timer turn;
        private Thread t;
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
            startGame.Location = new Point(325, 879);
            startGame.Size = new Size(100, 50);
            startGame.Click += StartGame_Click;
            Controls.Add(startGame);

            playerReciever = new Thread(WaitForPlayers);
            playerReciever.Start();
        }

        private void StartGame_Click(object sender, EventArgs e)
        {
            /*if (players.Count < 4)
            {
                MessageBox.Show("Needs at least 4 players to start", "Can't start game!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }*/
            playerReciever.Suspend();
            Controls.RemoveAt(Controls.Count - 1);
            for (int i = 0; i < players.Count; i++)
            {
                players[i].card = deck[i];
                players[i].role = deck[i];
                byte[] bytes = Encoding.UTF8.GetBytes('\0' + deck[i]);
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

            for (int i = 10; i < deck.Length; i++)
            {
                middleCards[i - 10] = deck[i];
            }

            string str = "";
            for (int i = 0; i < players.Count; i++)
            {
                str += players[i].name + '\0';
            }
            str = str.Substring(0, str.Length - 1);

            for (int i = 0; i < players.Count; i++)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                players[i].stream.Write(bytes, 0, bytes.Length);
            }

            AddText(console, "Game started");

            Thread t = new Thread(() =>
            {
                Thread.Sleep(10000);
                for (int i = 0; i < players.Count; i++)
                {
                    try
                    {
                        players[i].stream.Write(new byte[] { 0 }, 0, 1);
                    }
                    catch { }
                }

                deck = new string[] { "Doppelganger", "Werewolf", "Minion", "Mason", "Seer", "Robber", "Troublemaker", "Drunk", "Insomniac" };

                Invoke(new InvokeDelegate(() =>
                {
                    turn = new Timer();
                    turn.Tick += DoTurn;
                    turn.Interval = 8000;
                    turn.Start();
                }));

            });
            t.Start();
        }

        public void DoTurn(object sender, EventArgs args)
        {
            //if (ind == 2)
            //{
            //    //t.Abort();
            //}
            for (int j = 0; j < players.Count; j++)
            {
                if (players[j].role == deck[ind])
                {
                    try
                    {
                        players[j].stream.Write(new byte[] { 0 }, 0, 1);
                    }
                    catch { }
                    if (ind == 0)
                    {
                        Doppelganger(j);
                    }
                    else if (ind == 1 || ind == 2)
                    {
                        Werewolf(j);
                    }
                    else if (ind == 3)
                    {
                        Mason(j);
                    }
                    else if (ind == 5)
                    {
                        Robber(j);
                    }
                    else if (ind == 8)
                    {
                        Insomniac(j);
                    }
                    else if (ind == 7)
                    {
                        Drunk(j);
                    }
                    else if (ind == 4)
                    {
                        Seer(j);
                    }
                }
            }
            if (ind == 8)
            {
                Thread.Sleep(8000);
                turn.Stop();
                for (int j = 0; j < players.Count; j++)
                {
                    try
                    {
                        players[j].stream.Write(new byte[] { 0 }, 0, 1);
                    }
                    catch { }
                }
            }
            else
            {
                ind++;
            }

        }
        public void Doppelganger(int clientind)
        {
            byte player = ReceiveByte(clientind);
            if (player >= clientind)
            {
                player++;
            }
            string role = players[player].role;
            WriteString(role, clientind);

            players[clientind].role = "Doppel" + role;
        }

        public void Werewolf(int clientind)
        {
            string werewolves = " ";
            for (int i = 0; i < players.Count; i++)
            {
                if (i != clientind && players[i].role == "Werewolf")
                {
                    if (i > clientind)
                    {
                        werewolves += i - 1 + " ";
                    }
                    else
                    {
                        werewolves += i + " ";
                    }
                }
            }

            WriteString(werewolves, clientind);

            //if (players[clientind].role == "Werewolf" && werewolves == " ") 
            //{
            //    //t = new  Thread(() =>
            //    //{
            //    //    Invoke(new InvokeDelegate(()=>
            //    //    {
            //            byte ind = ReceiveByte(1);
            //            WriteString(middleCards[ind], clientind);
            //    //    }));
            //    //});
            //    //t.Start();
                
            //}
        }
        public void Minion()
        {

        }
        public void Mason(int clientind)
        {
            string masons = " ";
            for (int i = 0; i < players.Count; i++)
            {
                if (i != clientind && players[i].role == "Mason")
                {
                    if (i > clientind)
                    {
                        masons += i - 1 + " ";
                    }
                    else
                    {
                        masons += i + " ";
                    }
                }
            }

            WriteString(masons, clientind);
        }
        public void Seer(int clientind)
        {
            string read = ReadString(10 , clientind); 
            
            
            if (read[0].Equals('a'))
            {
                int player = int.Parse(read.Substring(1));

                if (player >= clientind)
                {
                    player++;
                }
                string card = players[player].card;
                WriteString(card, clientind);
            }
            else
            {
                int cardind = int.Parse(read);
                string midcard = middleCards[cardind];
                WriteString(midcard, clientind);

                cardind = int.Parse(ReadString(10, clientind)); 
                midcard = middleCards[cardind];
                WriteString(midcard, clientind);
            }
        }
        public void Robber(int clientind)
        {
            int ind = ReceiveInt(10);
            players[ind].card = "Robber";

            if (ind >= clientind)
            {
                ind++;
            }
            string role = players[ind].role;

            WriteString(role, clientind);

            players[clientind].card = role;
        }
        public void Troublemaker()
        {

        }
        public void Drunk(int clientind)
        {
            int ind = ReceiveInt(10); 
            
            string role = middleCards[ind];
            middleCards[ind] = "Drunk";

            string card = players[clientind].card;
            players[clientind].card = role;
            middleCards[ind] = card;
        }
        public void Insomniac(int clientind)
        {
            WriteString(players[clientind].card, clientind);
        }

        //public void CheckRole(object roleobj)
        //{
        //    string role = (string)roleobj;

        //    if (role == "Werewolf")
        //    {
        //        //Werewolf();
        //    }
        //    else
        //    {
        //        if (role == "Mason")
        //        {
        //            //Mason();
        //        }
        //        else
        //        {
        //            if (role == "Doppelganger")
        //            {
        //                Doppelganger(0);
        //            }
        //            else
        //            {
        //                if (role == "Minion")
        //                {
        //                    Minion();
        //                }
        //                else
        //                {
        //                    if (role == "Troublemaker")
        //                    {
        //                        Troublemaker();
        //                    }
        //                    else
        //                    {
        //                        if (role == "Drunk")
        //                        {
        //                           // Drunk();
        //                        }
        //                        else
        //                        {
        //                            if (role == "Seer")
        //                            {
        //                                Seer();
        //                            }
        //                            else
        //                            {
        //                                if (role == "Robber")
        //                                {
        //                                    //Robber();
        //                                }
        //                                else
        //                                {
        //                                   // Insomniac();
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
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
                byte[] data = new byte[24];
                int bytes = stream.Read(data, 0, data.Length);
                string name = RemoveCharacters(System.Text.Encoding.UTF8.GetString(data, 0, bytes));
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

                players.Add(new Player(client, stream, name));
                WriteString(names, players.Count - 1);
            }
        }

        public string RemoveCharacters(string str)
        {
            string output = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != '\n' && str[i] != '\0') output += str[i];
            }
            return output;
        }
        public byte ReceiveByte(int clientid)
        {
            byte[] buffer = new byte[1];
            try
            {
                players[clientid].stream.Read(buffer, 0, 1);
            }
            catch { }
            return buffer[0];
        }

        public int ReceiveInt(int clientid)
        {
            byte[] buffer = new byte[1];
            try
            {
                players[clientid].stream.Read(buffer, 0, 1);
            }
            catch { }
            return (int)buffer[0];
        }

        public string ReadString(int byteLength, int clientind)
        {
            byte[] bytes = new byte[byteLength];
            int length = players[clientind].stream.Read(bytes, 0, bytes.Length);
            return System.Text.Encoding.UTF8.GetString(bytes, 0, length);
        }

        public void WriteString(string str, int clientid)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            players[clientid].stream.Write(buffer, 0, buffer.Length);
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Host
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "Host";
            this.Load += new System.EventHandler(this.Host_Load);
            this.ResumeLayout(false);

        }

        private void Host_Load(object sender, EventArgs e)
        {

        }
    }
}

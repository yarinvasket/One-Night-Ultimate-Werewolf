using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace One_Night_Ultimate_Werewolf
{
    public delegate void InvokeDelegate();
    public partial class Client : Form
    {
        private string username;
        private string ip;
        List<string> players;
        private Label connectedPlayers;
        private TcpClient client;
        private NetworkStream stream;
        private string role;
        private PictureBox[] pcards;
        private Label[] pnames;
        private PictureBox card;
        private Label name;
        private PictureBox[] midcards;
        private int w;
        private int h;
        private int sec;
        private Label in10 = new Label();
        private Label night;
        private Label yourTurn;
        private Timer timer;
        private bool iscurrentturn;
        private Image back;
        private Timer t;
        private int index;
        private int seercount = 0;
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
                Text = "YOU - " + username,
                ForeColor = Color.Red,
                Font = new Font("Arial", 20),
                Location = new Point(20, 100),
                Size = new Size(500, 35)
            };
            Controls.Add(you);

            connectedPlayers = new Label
            {
                Font = new Font("Arial", 16),
                Location = new Point(20, 140),
                Size = new Size(500, 600)
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
                        Ohno OhnoForm = new Ohno
                        {
                            StartPosition = FormStartPosition.Manual,
                            Location = this.Location
                        };
                        this.Hide();
                        this.WindowState = FormWindowState.Minimized;
                        this.ShowInTaskbar = false;
                        Controls.Clear();
                        OhnoForm.Show();
                        return;
                    }
                }

                Connect connectForm = new Connect
                {
                    StartPosition = FormStartPosition.Manual,
                    Location = this.Location,
                    Size = new Size(500, 500),
                    Text = One_Night_Ultimate_Werewolf.Menu.game + " - Connect to server"
                };
                connectForm.FormClosing += One_Night_Ultimate_Werewolf.Menu.OnClose;
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
                Controls.Clear();
                connectForm.Show();
                return;
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
                string str = ReadString(24);
                if (str[0] == '\0')
                {
                    role = str.Substring(1);
                    players = ReadString(1024).Split('\0').ToList<string>();
                    players.Remove(username);
                    Invoke(new InvokeDelegate(GameStarts));
                    break;
                }
                players.Add(str);
                this.Invoke(new AddTextDelegate(Host.AddText), connectedPlayers, str);
            }
        }

        protected virtual void GameStarts()
        {
            Controls.Clear();
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            w = this.Width;
            h = this.Height;

            night = new Label();
            night.Size = new Size(1920, 1080);
            night.BackColor = Color.FromArgb(0, 0, 0, 0);
            night.ForeColor = Color.White;
            night.Text = "Night has fallen over the city...";
            night.Location = new Point(w / 2 - 20 * night.Text.Length, h / 2 - 25);
            night.Font = new Font(FontFamily.GenericMonospace, 50);

            card = new PictureBox();
            pnames = new Label[players.Count];
            Image img = StrToImg(role);
            img = Resize(img, 2 * img.Width / 3, 2 * img.Height / 3);
            card.Image = img;
            card.Size = img.Size;
            card.Location = new Point(w / 2 - card.Size.Width / 2, (5 * h) / 6 - card.Size.Height / 2);
            Controls.Add(card);

            name = new Label();
            name.Text = username;
            name.Font = new Font(FontFamily.GenericMonospace, 14);
            name.Size = new Size(username.Length * 16, 25);
            name.Location = new Point((int)(card.Location.X - 5.5 * username.Length + img.Width / 2), card.Location.Y + img.Height + 5);
            Controls.Add(name);

            img = Properties.Resources.Back;
            img = Resize(img, w / 16, h * 41 / 270);
            int p = players.Count;
            pcards = new PictureBox[players.Count];
            midcards = new PictureBox[3];
            back = img;
            for (int i = 0; i < players.Count; i++)
            {
                pcards[i] = new PictureBox
                {
                    Image = img,
                    Size = img.Size,
                    Tag = (byte)i
                };
                pcards[i].Location = new Point(w / 2 - pcards[i].Width / 2 + (int)(h * Math.Sin(2 * Math.PI * (i + 1) / (p + 1)) / 3), h / 2 - pcards[i].Height / 2 + (int)(h * Math.Cos(2 * Math.PI * (i + 1) / (p + 1)) / 3));
                if (pcards[i].Location.X > w / 2 - pcards[i].Width / 2)
                {
                    pcards[i].Location = new Point(pcards[i].Location.X + w / 6, pcards[i].Location.Y);
                }
                else if (pcards[i].Location.X < w / 2 - pcards[i].Width / 2)
                {
                    pcards[i].Location = new Point(pcards[i].Location.X - w / 6, pcards[i].Location.Y);
                }
                Controls.Add(pcards[i]);

                pnames[i] = new Label();
                pnames[i].Text = players[i];
                pnames[i].Font = new Font(FontFamily.GenericMonospace, 14);
                pnames[i].Size = new Size(players[i].Length * 16, 25);
                pnames[i].Location = new Point((int)(pcards[i].Location.X - 5.5 * players[i].Length + img.Width / 2), pcards[i].Location.Y + img.Height + 5);
                Controls.Add(pnames[i]);
            }

            for (int i = 0; i < midcards.Length; i++)
            {
                midcards[i] = new PictureBox();
                midcards[i].Image = img;
                midcards[i].Size = img.Size;
                midcards[i].Location = new Point(w / 2 - midcards[i].Width / 2 - w / 13 + i * w / 13, h / 2 - midcards[i].Height / 2);
                midcards[i].Tag = (byte)i;
                Controls.Add(midcards[i]);
            }
            SecondsToStart();
        }
        public void SecondsToStart()
        {
            in10.Size = new Size(100, 50);
            in10.Location = new Point(49 * w / 100, h / 3);
            sec = 10;
            in10.Font = new Font("Arial", 24);
            in10.Text = sec.ToString();
            Controls.Add(in10);

            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Waiting10Secs;
            timer.Start();
        }
        public void GotoSleep()
        {
            BackgroundImage = Properties.Resources.Sleep;
            Invoke(new InvokeDelegate(() =>
            {
                for (int i = 0; i < Controls.Count; i++)
                {
                    Controls[i].Hide();
                }
                Controls.Add(night);
            }));
        }
        public void WakeUp()
        {
            BackgroundImage = null;
            if (iscurrentturn)
            {
                for (int i = 0; i < Controls.Count; i++)
                {
                    if (Controls[i].Text != "Night has fallen over the city...")
                    {
                        Controls[i].Show();
                    }
                    else
                    {
                        Controls[i].Text = "It is your turn";
                        Controls[i].Show();
                    }
                }
            }
            else
            {
                for (int i = 0; i < Controls.Count; i++)
                {
                    if (Controls[i].Text == "Night has fallen over the city..." || Controls[i].Text == "It is your turn" || Controls[i].Text == "1")
                    {
                        Controls[i].Hide();
                    }
                    else
                    {
                        Controls[i].Show();
                    }
                }
            }

            //BackgroundImage = null;
            //for (int i = 0; i < Controls.Count; i++)
            //{
            //    Controls[i].Show();
            //}
            //in10.Text = 8.ToString();
            //sec = 8;
            //in10.Show();
            //timer.Start();



        }
        private void Waiting10Secs(object sender, EventArgs args)
        {

            if (sec > 1)
            {
                sec--;
                if (sec == 3)
                {
                    in10.ForeColor = Color.Red;
                }
                in10.Text = sec.ToString();
            }
            else
            {
                if (iscurrentturn)
                {
                    iscurrentturn = false;
                    if (role == "Werewolf")
                    {
                        for (int i = 0; i < players.Count; i++)
                        {

                            if (pcards[i].Image != Properties.Resources.Back)
                            {
                                pcards[i].Image = back;
                                pcards[i].Size = back.Size;
                            }
                            if (i < 3)
                            {
                                if (midcards[i].Image != Properties.Resources.Back)
                                {
                                    midcards[i].Image = back;
                                    midcards[i].Size = back.Size;
                                }
                                midcards[i].Click -= WatchCard;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < players.Count; i++)
                        {
                            if (pcards[i].Image != Properties.Resources.Back)
                            {
                                pcards[i].Image = back;
                                pcards[i].Size = back.Size;
                            }
                            if (i < 3 && midcards[i].Image != Properties.Resources.Back)
                            {
                                midcards[i].Image = back;
                                midcards[i].Size = back.Size;
                            }
                        }
                    }
                    if (card.Image != Properties.Resources.Back)
                    {
                        card.Image = back;
                        card.Size = back.Size;
                    }
                    GotoSleep();

                    Thread t = new Thread(() =>
                    {

                        stream.Read(new byte[] { 0 }, 0, 1);
                        this.Invoke(new InvokeDelegate(WakeUp));

                    });
                    t.Start();
                }
                else
                {
                    stream.Read(new byte[] { 0 }, 0, 1);
                    //this.Invoke(new InvokeDelegate(
                    StartGame();
                    // ));
                }
                in10.Hide();
                timer.Stop();


            }
        }
        public void StartGame()
        {
            Image img = Properties.Resources.Back;
            img = Resize(img, w / 16, h * 41 / 270);
            card.Size = back.Size;
            card.Image = back;

            GotoSleep();


            Thread t = new Thread(() =>
            {
                stream.Read(new byte[] { 0 }, 0, 1);//client's turn 


                Invoke(new InvokeDelegate(() =>
                {
                    WakeUp();
                    if (role != "Tanner" && role != "Hunter")
                    {
                        in10.Text = 8.ToString();
                        sec = 8;
                        in10.Show();
                        CheckRole();
                        timer.Start();
                        night.Hide();
                        iscurrentturn = true;
                    }
                }));
            });
            t.Start();
        }
        public Image Resize(Image image, int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h);
            Graphics grp = Graphics.FromImage(bmp);
            grp.DrawImage(image, 0, 0, w, h);
            grp.Dispose();

            return bmp;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F11)
            {
                if (FormBorderStyle == FormBorderStyle.Sizable)
                {
                    FormBorderStyle = FormBorderStyle.None;
                    WindowState = FormWindowState.Maximized;
                }
                else
                {
                    FormBorderStyle = FormBorderStyle.Sizable;
                    WindowState = FormWindowState.Normal;
                }
            }
            return false;
        }
        public void CheckRole()
        {
            if (role == "Werewolf")
            {
                WereWolf();
            }
            else
            {
                if (role == "Mason")
                {
                    Mason();
                }
                else
                {
                    if (role == "Doppelganger")
                    {
                        Doppelganger();
                    }
                    else
                    {
                        if (role == "Troublemaker")
                        {
                            TroubleMaker();
                        }
                        else
                        {
                            if (role == "Minion")
                            {
                                Minion();
                            }
                            else
                            {
                                if (role == "Drunk")
                                {
                                    Drunk();
                                }
                                else
                                {
                                    if (role == "Seer")
                                    {
                                        Seer();
                                    }
                                    else
                                    {
                                        if (role == "Robber")
                                        {
                                            Robber();
                                        }
                                        else
                                        {
                                            Insomniac();
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }
        public void Insomniac()
        {
            string card = ReadString(20);
            Image img = StrToImg(card);
            img = Resize(img, 2 * img.Width / 3, 2 * img.Height / 3);
            this.card.Image = img;
            this.card.Size = img.Size;
        }
        public void Robber()
        {
            for (int i = 0; i < players.Count; i++)
            {
                pcards[i].Click += RobCard;
            }
        }
        public void RobCard(object sender, EventArgs args)
        {
            PictureBox robbedcard = (PictureBox)sender;
            for (int i = 0; i < players.Count; i++)
            {
                if (pcards[i].Location.X == robbedcard.Location.X)
                {
                    SendInt(i);
                }
                pcards[i].Click -= RobCard;
            }

            string card = ReadString(20);

            Image img = StrToImg(card);
            img = Resize(img, 2 * img.Width / 3, 2 * img.Height / 3);
            this.card.Image = img;
            this.card.Size = img.Size;
        }
        public void Doppelganger()
        {
            for (int i = 0; i < players.Count; i++)
            {
                pcards[i].Click += DoppelgangerClick;
            }
        }
        public void DoppelgangerClick(object sender, EventArgs args)
        {
            PictureBox card = (PictureBox)sender;
            byte i = (byte)card.Tag;

            SendByte(i);

            role = ReadString(20);

            Image newcard = StrToImg(role);
            newcard = Resize(newcard, 2 * newcard.Width / 3, 2 * newcard.Height / 3);
            pcards[i].Image = newcard;

            for (int j = 0; j < players.Count; j++)
            {
                pcards[j].Click -= DoppelgangerClick;
            }

        }
        public void SendInt(int num)
        {
            this.stream.Write(new byte[1] { (byte)num }, 0, 1);
        }
        public void SendByte(byte num)
        {
            this.stream.Write(new byte[1] { (byte)num }, 0, 1);
        }
        public void Minion()
        {
            Image img = StrToImg("Werewolf");
            img = Resize(img, 2 * img.Width / 3, 2 * img.Height / 3);

            string werewolves = ReadString(15);
            string[] index = werewolves.Split(' ');
            for (int i = 1; i < index.Length - 1; i++)
            {
                pcards[int.Parse(index[i])].Image = img;
            }
        }
        public void TroubleMaker()
        {

        }
        public void Drunk()
        {
            for (int i = 0; i < 3; i++)
            {
                midcards[i].Click += DrunkCard;
            }
        }
        public void DrunkCard(object sender, EventArgs args)
        {
            PictureBox clicked = (PictureBox)sender;
            for (int i = 0; i < 3; i++)
            {
                if (midcards[i].Location.X == clicked.Location.X)
                {
                    SendInt(i);
                    index = i;
                }
                midcards[i].Click -= DrunkCard;
            }

            t = new Timer();
            t.Interval = 25;
            t.Tick += Animation;
            t.Start();

        }

        public void Animation(object sender, EventArgs args)
        {
            //card.Location = new Point(w / 2 - card.Size.Width / 2, (5 * h) / 6 - card.Size.Height / 2);

            if (midcards[index].Location.Y < ((5 * h) / 6 - card.Size.Height / 2))
            {
                int x = midcards[index].Location.X < w / 2 - card.Size.Width / 2 ? Math.Abs(midcards[index].Location.X - (w / 2 - card.Size.Width / 2)) / 20 : -Math.Abs(midcards[index].Location.X - (w / 2 - card.Size.Width / 2)) / 20;
                int y = Math.Abs(midcards[index].Location.Y - ((5 * h) / 6 - card.Size.Height / 2)) / 20 + 1;
                midcards[index].Location = new Point(midcards[index].Location.X + x, midcards[index].Location.Y + y);
                card.Location = new Point(card.Location.X - x, card.Location.Y - y);
            }
            else
            {

                t.Stop();
            }
        }
        public void Seer()
        {
            for (int i = 0; i < pcards.Length; i++)
            {
                pcards[i].Click += SeerClick;
            }
            for (int i = 0; i < 3; i++)
            {
                midcards[i].Click += SeerClickMid;
            }
        }
        public void SeerClick(object sender, EventArgs args)
        {
            PictureBox card = (PictureBox)sender;
            byte i = (byte)card.Tag;

            WriteString("a" + i);

            role = ReadString(20);

            Image newcard = StrToImg(role);
            newcard = Resize(newcard, 2 * newcard.Width / 3, 2 * newcard.Height / 3);
            pcards[i].Image = newcard;

            for (int j = 0; j < players.Count; j++)
            {
                pcards[j].Click -= SeerClick;
            }
            for (int j = 0; j < 3; j++)
            {
                midcards[j].Click -= SeerClickMid;
            }

        }
        public void SeerClickMid(object sender, EventArgs args)
        {   
            if (seercount == 0)
            {
                for (int j = 0; j < players.Count; j++)
                {
                    pcards[j].Click -= SeerClick;
                }
            }
            
            PictureBox card = (PictureBox)sender;
            byte i = (byte)card.Tag;

            WriteString(i.ToString());

            role = ReadString(20);

            Image newcard = StrToImg(role);
            newcard = Resize(newcard, 2 * newcard.Width / 3, 2 * newcard.Height / 3);
            midcards[i].Image = newcard;

           
            if (seercount == 1)
            {
                for (int j = 0; j < 3; j++)
                {
                    midcards[j].Click -= SeerClickMid;
                }
            }
            else
            {
                card.Click -= SeerClickMid;
                seercount++;
            }
        }
        public void Mason()
        {
            Image img = StrToImg("Mason");
            img = Resize(img, 2 * img.Width / 3, 2 * img.Height / 3);
            card.Image = img;
            card.Size = img.Size;

            string Masons = ReadString(15);
            string[] index = Masons.Split(' ');
            for (int i = 1; i < index.Length - 1; i++)
            {
                pcards[int.Parse(index[i])].Image = img;
            }
        }

        public void WereWolf()
        {
            Image img = StrToImg("Werewolf");
            img = Resize(img, 2 * img.Width / 3, 2 * img.Height / 3);
            card.Image = img;
            card.Size = img.Size;

            string werewolves = ReadString(15);
            string[] index = werewolves.Split(' ');
            for (int i = 1; i < index.Length - 1; i++)
            {
                pcards[int.Parse(index[i])].Image = img;
            }

            if (index.Length == 2)
            {
                for (int i = 0; i < 3; i++)
                {
                    midcards[i].Click += WatchCard;
                }
            }
            //if (index.Length == 2)
            //{
            //    for (int i = 0; i < 3; i++)
            //    {
            //        midcards[i].Click += WatchCard;
            //    }
            //}
        }
        public void WatchCard(object sender, EventArgs args)
        {
            PictureBox card = (PictureBox)sender;
            byte ind = (byte)card.Tag;

            SendByte(ind);

            role = ReadString(20);

            Image newcard = StrToImg(role);
            newcard = Resize(newcard, 2 * newcard.Width / 3, 2 * newcard.Height / 3);
            midcards[ind].Image = newcard;

            //PictureBox card = (PictureBox)sender;

            //byte ind = (byte)card.Tag;
            //SendByte(ind);

            //string role = ReadString(20);
            //Image img = StrToImg(role);
            //img = Resize(img, 2 * img.Width / 3, 2 * img.Height / 3);

            //midcards[ind].Image = img;

            for (int i = 0; i < 3; i++)
            {
                midcards[i].Click -= WatchCard;
            }
        }
        public Image StrToImg(string str)
        {

            if (str == "Werewolf")
            {
                return Properties.Resources.Werewolf;
            }

            if (str == "Mason")
            {
                return Properties.Resources.Mason;
            }

            if (str == "Drunk")
            {
                return Properties.Resources.Drunk;
            }

            if (str == "Hunter")
            {
                return Properties.Resources.Hunter;
            }

            if (str == "Insomniac")
            {
                return Properties.Resources.Insomniac;
            }

            if (str == "Minion")
            {
                return Properties.Resources.Minion;
            }

            if (str == "Robber")
            {
                return Properties.Resources.Robber;
            }

            if (str == "Seer")
            {
                return Properties.Resources.Seer;
            }

            if (str == "Tanner")
            {
                return Properties.Resources.Tanner;
            }
            if (str == "Doppelganger")
            {
                return Properties.Resources.Doppelganger;
            }

            return Properties.Resources.Troublemaker;
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Client
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "Client";
            this.Load += new System.EventHandler(this.Client_Load);
            this.ResumeLayout(false);

        }

        private void Client_Load(object sender, EventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace One_Night_Ultimate_Werewolf
{
    public partial class Menu : Form
    {
        public static string game = "One Night Ultimate Werewolf";
        private Button connect;
        private Button host;
        private LinkLabel howto;
        public static int port = 192;
        public static Random random = new Random();

        public Menu()
        {
            this.Size = new Size(500, 500);
            this.Text = game + " - Menu";
            this.FormClosing += OnClose;

            connect = new Button
            {
                Size = new Size(200, 100),
                Location = new Point(150, 100),
                Text = "Connect to server"
            };
            connect.Click += ConnectClick;
            Controls.Add(connect);

            host = new Button
            {
                Size = new Size(200, 100),
                Location = new Point(150, 300),
                Text = "Host a server"
            };
            host.Click += HostClick;
            Controls.Add(host);

            howto = new LinkLabel
            {
                Size = new Size(100, 50),
                Location = new Point(220, 415),
                Text = "How to play"
            };
            howto.Click += HowtoClick;
            Controls.Add(howto);
        }

        public static void OnClose(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        public void ConnectClick(object sender, EventArgs e)
        {
            Connect connectForm = new Connect
            {
                StartPosition = FormStartPosition.Manual,
                Location = this.Location,
                Size = this.Size,
                Text = game + " - Connect to server"
            };
            connectForm.FormClosing += OnClose;
            this.Hide();
            Controls.Clear();
            connectForm.Show();
        }

        public void HostClick(object sender, EventArgs e)
        {
            Host hostForm = new Host
            {
                StartPosition = FormStartPosition.Manual,
                Location = this.Location,
                Size = this.Size,
                Text = game + " - Server"
            };
            this.Hide();
            Controls.Clear();
            hostForm.Show();
        }

        public void HowtoClick(object sender, EventArgs e)
        {
            try
            {
                howto.LinkVisited = true;
                System.Diagnostics.Process.Start("https://youtu.be/XsP6LvZQpLk");
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to open link.");
            }
        }
    }
}

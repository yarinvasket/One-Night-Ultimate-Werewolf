﻿using System;
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
        public Menu()
        {
            this.Size = new Size(500, 500);
            this.Text = game + " - Menu";

            Button connect = new Button
            {
                Size = new Size(200, 100),
                Location = new Point(150, 100),
                Text = "Connect to server"
            };
            connect.Click += ConnectClick;
            Controls.Add(connect);

            Button host = new Button
            {
                Size = new Size(200, 100),
                Location = new Point(150, 300),
                Text = "Host a server"
            };
            host.Click += HostClick;
            Controls.Add(host);
        }

        public void ConnectClick(object sender, EventArgs e)
        {
            Connect connectForm = new Connect();
            connectForm.StartPosition = FormStartPosition.Manual;
            connectForm.Location = this.Location;
            connectForm.Size = this.Size;
            connectForm.Text = game + " - Connect to server";
            this.Hide();
            Controls.Clear();
            connectForm.Show();
        }

        public void HostClick(object sender, EventArgs e)
        {
            Host hostForm = new Host();
            hostForm.StartPosition = FormStartPosition.Manual;
            hostForm.Location = this.Location;
            hostForm.Size = this.Size;
            hostForm.Text = game + " - Server";
            this.Hide();
            Controls.Clear();
            hostForm.Show();
        }
    }
}

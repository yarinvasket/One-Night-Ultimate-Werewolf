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
    public partial class Connect : Form
    {
        private Label usernameask;
        private TextBox username;

        public Connect()
        {
            usernameask = new Label();
            usernameask.Font = new Font("Arial",18);
            usernameask.Location = new Point(115, 110);
            usernameask.Size = new Size(1000,30);
            usernameask.Text = "Please enter username";
            Controls.Add(usernameask);

            username = new TextBox();
            username.Font = new Font("Arial", 18);
            username.Text = "Enter username here";
            username.ForeColor = Color.Gray;
            username.Size = new Size(300, 0);
            username.Location = new Point(100, 200);
            username.Click += UsernameClick;
            Controls.Add(username);

            Button sub = new Button();
            sub.Text = "Submit";
            sub.Location = new Point(190, 275);
            sub.Font = new Font("Arial", 18);
            sub.Size = new Size(100, 70);
            sub.Click += IP;
            Controls.Add(sub);
        }
        public void IP(object sender, EventArgs args)
        {
            //set username
            Controls.Remove((Button)sender);


            username.Text = "Enter IP here";
            usernameask.Location = new Point(145, 110);
            usernameask.Text = "Please enter IP";

            Button sub = new Button();            
            sub.Text = "Submit";
            sub.Location = new Point(190, 275);
            sub.Font = new Font("Arial", 18);
            sub.Size = new Size(100, 70);
            sub.Click += IP;
            Controls.Add(sub);
        }
        public void UsernameClick(object sender, EventArgs args)
        {
            TextBox username = (TextBox)sender;
            if (username.Text== "Enter username here"|| username.Text == "Enter IP here")
            {
                username.Text = "";
                username.ForeColor = Color.Black;
            }
            
        }
        private void InitializeComponent()
        {
           
        }

        private void Connect_Load(object sender, EventArgs e)
        {

        }
    }
}

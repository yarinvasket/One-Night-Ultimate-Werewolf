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
        private TextBox usernametext;
        private string username;
        public Connect()
        {
            usernameask = new Label();
            usernameask.Font = new Font("Arial",18);
            usernameask.Location = new Point(115, 110);
            usernameask.Size = new Size(1000,30);
            usernameask.Text = "Please enter username";
            Controls.Add(usernameask);

            usernametext = new TextBox();
            usernametext.Font = new Font("Arial", 18);
            usernametext.Text = "Enter username here";
            usernametext.ForeColor = Color.Gray;
            usernametext.Size = new Size(300, 0);
            usernametext.Location = new Point(100, 200);
            usernametext.Click += UsernameClick;
            Controls.Add(usernametext);

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
            username = usernametext.Text;
            Controls.Remove((Button)sender);

            usernametext.Text = "Enter IP here";
            usernametext.ForeColor = Color.Gray;
            usernameask.Location = new Point(145, 110);
            usernameask.Text = "Please enter IP";

            Button sub = new Button();            
            sub.Text = "Submit";
            sub.Location = new Point(190, 275);
            sub.Font = new Font("Arial", 18);
            sub.Size = new Size(100, 70);
            sub.Click += GotIP;
            Controls.Add(sub);
        }
        public void GotIP(object sender, EventArgs args)
        {
            
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
       
        private void Connect_Load(object sender, EventArgs e)
        {

        }
    }
}

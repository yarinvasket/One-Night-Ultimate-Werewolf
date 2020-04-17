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
    public partial class Client : Form
    {
        private string username;
        private Label card;
        private int clientid;
        public Client()
        {
            
        }

        private void Client_Load(object sender, EventArgs e)
        {

        }
        public void SetUsername(string username)
        {
            this.username = username;
        }
    }
}

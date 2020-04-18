using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace One_Night_Ultimate_Werewolf
{
    class Player
    {
        private string name, card, role;

        public Player(string name)
        {
            this.name = name;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return name;
        }

        public void SetCard(string card)
        {
            this.card = card;
        }

        public string GetCard()
        {
            return card;
        }
    }
}

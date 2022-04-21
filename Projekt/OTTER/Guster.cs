using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    public abstract class Guster : Sprite
    {
        private int brzina;
        public virtual int Brzina
        {
            get { return brzina; }
            set { this.brzina = value; }
        }

        public Guster(string path, int x, int y,int b) : base(path, x, y)
        {
            this.Brzina = b;
        }
    }

    public class Guster1 : Guster
    {
        private int brzina;
        public override int Brzina
        {
            get { return brzina; }
            set { this.brzina = value; }
        }

        public Guster1(string path, int x, int y, int brz) : base(path, x, y,brz)
        {
            this.Brzina = brz;
        }
    }
}

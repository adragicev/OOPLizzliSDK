using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    public abstract class Kamenje:Sprite
    {
        public Kamenje(string path,int x,int y) : base(path, x, y)
        {

        }
    }
    public class Kamen1 : Kamenje
    {
        public Kamen1(string p,int x,int y) : base(p, x, y)
        {

        }
    }
    public class Kamen2 : Kamenje
    {
        private int brzina;
        public int Brzina
        {
            get { return brzina; }
            set { this.brzina = value; }
        }
        public Kamen2(string p, int x, int y,int brz) : base(p, x, y)
        {
            this.Brzina = brz;
        }
    }
    
}

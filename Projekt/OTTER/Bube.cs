using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    public abstract class Bube :Sprite
    {
        public Bube(string path,int x,int y) : base(path, x, y)
        {

        }
    }
    public class Bube1 : Bube
    {
        public Bube1(string path,int x,int y) : base(path, x, y)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class CE_FakePlayer
{
    public byte ColorId = 0;
    public string Name = "Bob";

    CE_FakePlayer()
    {

    }

    CE_FakePlayer(byte color, string name)
    {
        ColorId = color;
        Name = name;
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class CE_LuaDeadBody : CE_LuaSpawnableObject
{

    public byte OwnerID;
    public bool Anon;
    public CE_LuaDeadBody()
    {
        ClientSide = false;
        ID = 0;
        Anon = false;
        OwnerID = 0;
    }

}


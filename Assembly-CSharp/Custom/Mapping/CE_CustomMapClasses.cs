using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class CEM_Point
{
    public CEM_Point()
    {

    }
    public CEM_Point(Vector3 v)
    {
        Values[0] = v.x;
        Values[1] = v.y;
        Values[2] = v.z;
    }
    public float[] Values = new float[3];
}

public class CEM_WallLine
{
    public List<CEM_Point> Points = new List<CEM_Point>();
    public bool ObscureVision = true;

    public CEM_WallLine()
    {

    }
    public CEM_WallLine(List<CEM_Point> points)
    {
        Points = points;
    }
}
public class CEM_Sprite
{
    public CEM_Point Position;
    public string ImageLocal = "???";
    public bool Fullbright = false;

    public CEM_Sprite()
    {

    }
    public CEM_Sprite(Vector3 position, string ImageLocation, bool IsFull)
    {
        Position = new CEM_Point(position);
        ImageLocal = ImageLocation;
        Fullbright = IsFull;
    }
}

public class CEM_Console
{
    public CEM_Point Position;
  //  public string ImageLocal = "???";
    public int TaskType;
    public int Room;
    public int MinStep;
    public int MaxStep;
    public int ConsoleID;

    public CEM_Console()
    {

    }


    public CEM_Console(Vector3 position, string ImageLocation, int tt, int r, int mis, int mas, int ci)
    {
        Position = new CEM_Point(position);
      //  ImageLocal = ImageLocation;
        TaskType = tt;
        Room = r;
        MaxStep = mis;
        MaxStep = mas;
        ConsoleID = ci;
    }
}

/*public class CEM_PC
{
    public CEM_Point Position;
  //  public string ImageLocal = "???";
 //   public int Usable;

    public CEM_PC()
    {

    }
*/

    public CEM_Console(Vector3 position, string ImageLocation, int tt, int r, int mis, int mas, int ci)
    {
        Position = new CEM_Point(position);
      //  ImageLocal = ImageLocation;
        TaskType = tt;
        Room = r;
        MaxStep = mis;
        MaxStep = mas;
        ConsoleID = ci;
    }
}

public class CEM_Task
{
    public int Room;
    public int TaskType;
    public int MaxStep;
    public string Name;

    public CEM_Task(int r, int tt, int ms, string name)
    {
        Room = r;
        TaskType = tt;
        MaxStep = ms;
        Name = name;
    }
}

public class CEM_Vent
{
    public string Name;
    public string LeftName;
    public string RightName;
    public CEM_Point Position;
    public CEM_Vent(string n, string l, string r, Vector3 v)
    {
        Name = n;
        LeftName = l;
        RightName = r;
        Position = new CEM_Point(v);
    }
}

public class CEM_Room
{
    public CEM_Point Position;
    public CEM_Point Scale;
    public int RoomType;
    public int Ambience;
    public int Footsteps;
    public bool CustomAmbience;
    public bool CustomFootsteps;
    public string AmbienceLocal;
    public List<string> AudioLocals;

    public CEM_Room(Vector3 Pos, Vector3 Sca, int Amb, int Foot, int rt)
    {
        Ambience = Amb;
        Footsteps = Foot;
        CustomAmbience = false;
        CustomFootsteps = false;
        Position = new CEM_Point(Pos);
        Scale = new CEM_Point(Sca);
        RoomType = rt;
    }
}

public class CEM_TaskList
{
    public List<CEM_Task> ShortTasks = new List<CEM_Task>();
    public List<CEM_Task> LongTasks = new List<CEM_Task>();
    public List<CEM_Task> CommonTasks = new List<CEM_Task>();
}


public class CEM_Map
{
    public string Name = "Undefined";
    public byte Version = 0;
    public List<CEM_WallLine> Walls = new List<CEM_WallLine>();
    public List<CEM_Sprite> Sprites = new List<CEM_Sprite>();
    public List<CEM_Vent> Vents = new List<CEM_Vent>();
    public CEM_TaskList TaskList = new CEM_TaskList();
    public List<CEM_Console> Consoles = new List<CEM_Console>();
    public List<CEM_Room> Rooms = new List<CEM_Room>();
    public CEM_Point SpawnLocation;
}

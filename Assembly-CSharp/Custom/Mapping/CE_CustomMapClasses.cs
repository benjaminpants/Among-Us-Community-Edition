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

public class CEM_Map
{
    public string Name = "Undefined";
    public byte Version = 0;
    public List<CEM_WallLine> Walls = new List<CEM_WallLine>();
    public List<CEM_Sprite> Sprites = new List<CEM_Sprite>();
    public CEM_Point SpawnLocation;
}

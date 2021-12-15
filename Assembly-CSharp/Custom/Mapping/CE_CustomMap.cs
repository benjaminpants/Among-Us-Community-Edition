using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using InnerNet;
using Newtonsoft.Json;

public class CEM_TaskData
{
    // minigame snizzle!
    public Type minigametype;
    public Type tasktype = typeof(NormalPlayerTask);
    public string MinigameName = "Invalid"; // the name of the minigame in this case "LookForLoaf"

    public CEM_TaskData(Type mgt, Type tt, string mn)
    {
        minigametype = mgt;
        tasktype = tt;
        MinigameName = mn;
    }
    public CEM_TaskData(Type mgt, string mn)
    {
        minigametype = mgt;
        MinigameName = mn;
    }
}

public static class CE_CustomMapManager
{
    public static List<CE_MapInfo> MapInfos = new List<CE_MapInfo>(); // makes map infos i think? idk

    public static void Initialize()
    {
        //  MapInfos.Add(new CE_MapInfo("Skeld")); // adds a dummy map named Skeld
        MapInfos.Add(new CE_MapInfo("Testing lol",new string[21]{
            "Happy Place",
            "Sad Place",
            "Mad Place",
            "Bad Place",
            "Good Place",
            "Evil Place", // demons!
            "Holy Place",
            "Sans Undertale", // undertale map?!?!?!
            "Lovely Day Outside",
            "The Forest",
            "Dumb Place",
            "Memey Place",
            "suloP", // inverted polus
            "ehT Dleks", // inverted skeld
            "Toon land",
            "Pizzaria",
            "Airship",
            "Polus",
            "Hopeful Place",
            "Colorful Place",
            "Playground" // birds are chirping on days like this kids like you GO TO DOG WATER LAND!
        }));
    }

    public static CE_MapInfo GetCurrentMap()
    {
        return MapId[2]; // map id 2 equals custom map testing lol
     //   return MapInfos[2]; // map infos test code
        return MapInfos[PlayerControl.GameOptions.MapId];
    }
}

 

public class CE_CustomMap
{
    private static bool MapTestingActive = true;
    public bool CustomTasksEnabled = true;
    public bool MiniMapEnabled = false;
    public bool CustomVentsEnabled = true;
    
    

    /*public static NormalPlayerTask CreateTask(Type tasktype, SystemTypes systype, int maxstep)
    {
        GameObject task = UnityEngine.GameObject.Instantiate(new GameObject());
        NormalPlayerTask datatask = task.AddComponent(tasktype) as NormalPlayerTask;
        datatask.StartAt = systype;
        datatask.MaxStep = maxstep;
        return GameObject.Instantiate(task).GetComponent(tasktype) as NormalPlayerTask;
    */}
    
    private static void ClearMapCollision(ShipStatus map)
    {
        Collider2D[] colids = map.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colids)
        {
            UnityEngine.Object.Destroy(col.gameObject);
        }
    }

         GetCurrentMap() // Gets what map your playing on. and then code below checks it.
         If CurrentMap() = MapId[2] return; // checks what map id it is
         MapTestingActive = true //enables the bools
         CustomTasksEnabled = true;
         If CurrentMap() = MapId[1] return;
         MapTestingActive = false;
         CustomTasksEnabled = false;
         If CurrentMap() = MapId[0] return;
         MapTestingActive = false;
         CustomTasksEnabled = false;

    // CreateSystemConsole(typeof(TaskAdderGame), new Vector3(5f, 5f, 0.5f), "TaskAddMinigame", sprite);

    public static void SpawnSprite(int x, int y, bool Solid)
    {
        Texture2D texture;

        if (Solid)
        {
            string path = System.IO.Path.Combine(CE_Extensions.GetTexturesDirectory("Mapping"), "TileTest2.png");
            texture = CE_TextureNSpriteExtensions.LoadPNG(path);
            texture.filterMode = FilterMode.Point;
        }
        else
        {
            string path = System.IO.Path.Combine(CE_Extensions.GetTexturesDirectory("Mapping"), "TileTest.png");
            texture = CE_TextureNSpriteExtensions.LoadPNG(path);
            texture.filterMode = FilterMode.Point;
        }


        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.65f));
        GameObject go = new GameObject("TestObject1");
        go.layer = LayerMask.NameToLayer("Ship");
        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        var position = renderer.transform.position;
        position.x = 0.5f * x;
        position.y = 0.5f * y;
        position.z = (position.y / 1000f) + 0.5f;
        renderer.transform.position = position;
        renderer.sprite = sprite;

        if (Solid)
        {
            BoxCollider2D boxCollider = go.AddComponent<BoxCollider2D>();
            boxCollider.transform.position = renderer.transform.position;
            boxCollider.size = new Vector3(0.5f, 0.5f);
        }
    }

        var sprite2 = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.6f, 0.6f)); // the sprite of go alt
        GameObject goalt = new GameObject("Venttest"); // name of the object
        goalt.layer = LayerMask.NameToLayer("Player"); // which layer it is on so default is "Ship"
        SpriteRenderer renderer = goalt.AddComponent<SpriteRenderer>(); // sprtite rendition
        var position = renderer.transform.position; // position
        position.x = 0.6f * x;
        position.y = 0.6f * y;
        position.z = (position.y / 1000f) + 0.6f; 
        renderer.transform.position = position; // position again?
        renderer.sprite = sprite2; // then the sprite varibile where it is called at.

    public static void MapTest(ShipStatus map)
    {
        if (!MapTestingActive) return;
        ClearMapCollision(map);
        for (int x = -24; x < 29; x++)
        {
            for (int y = -24; y < 29; y++)
            {
                bool isSolid = (x == -24 || y == -24 || y == 24 || x == 24);
                SpawnSprite(x, y, isSolid); // spawns the sprite into the map.
            }
        }
    }
}

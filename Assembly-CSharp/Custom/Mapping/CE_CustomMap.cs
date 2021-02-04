using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using InnerNet;

public static class CE_CustomMapManager
{
    public static List<CE_MapInfo> MapInfos = new List<CE_MapInfo>();

    public static void Initialize()
    {
        MapInfos.Add(new CE_MapInfo("Skeld")); //adds a dummy map named skeld.
        MapInfos.Add(new CE_MapInfo("Test Map",new string[12]{
            "Happy Place",
            "Sad Place",
            "Mad Place",
            "Bad Place",
            "Good Place",
            "Evil Place",
            "Holy Place",
            "Happy Place",
            "Happy Place",
            "Happy Place",
            "Happy Place",
            "Happy Place"
        }));
    }

    public static CE_MapInfo GetCurrentMap()
    {
        return MapInfos[PlayerControl.GameOptions.MapId];
    }
}

public class CE_CustomMap
{
    public static bool MapTestingActive = false;

    public static Vent ReferenceVent;

    public static ShipStatus stat;

    public static SoundGroup[] SoundGroups;

    public static AudioClip[] AmbienceSounds;

    public static NormalPlayerTask CreateTask(Type tasktype, SystemTypes systype, int maxstep, TaskTypes taskty, Type minigametype, string name)
    {
        GameObject task = UnityEngine.GameObject.Instantiate(new GameObject());
        NormalPlayerTask datatask = task.AddComponent(tasktype) as NormalPlayerTask;
        datatask.TaskType = taskty;
        datatask.StartAt = systype;
        datatask.MaxStep = maxstep;
        Minigame uploaddat = CE_PrefabHelpers.FindPrefab(name, minigametype) as Minigame;
        datatask.MinigamePrefab = uploaddat;
        return GameObject.Instantiate(task).GetComponent(tasktype) as NormalPlayerTask;
    }

    public static Console CreateTaskConsole(Vector3 transf, Sprite sprite, NormalPlayerTask task, IntRange range, SystemTypes room)
    {
        GameObject ins = GameObject.Instantiate(new GameObject());
        BoxCollider2D col2d = ins.AddComponent<BoxCollider2D>();
        col2d.size = Vector2.one / 2f;
        col2d.isTrigger = true;
        Console console = ins.AddComponent<Console>();
        SpriteRenderer img = ins.AddComponent<SpriteRenderer>();
        img.sprite = sprite;
        img.material.shader = Shader.Find("Sprites/Outline");
        console.Image = img;
        console.Room = room;
        TaskSet ts = new TaskSet();
        ts.taskStep = range;
        ts.taskType = task.TaskType;
        console.ValidTasks = new TaskSet[] {
            ts
        };
        console.TaskTypes = new TaskTypes[]
        {
            task.TaskType
        };

        ins.transform.position = transf;
        return console;
    }
    
    public static ShipRoom CreateShipRoom(SystemTypes room, AudioClip ambience, SoundGroup footsteps, Vector2 position, Vector2 scale)
    {
        GameObject ins = GameObject.Instantiate(new GameObject());
        BoxCollider2D col2d = ins.AddComponent<BoxCollider2D>();
        col2d.size = Vector2.one / 2f;
        col2d.isTrigger = true;
        ins.transform.position = position;
        ins.transform.localScale = scale;
        ins.transform.name = "(Custom)" + room;
        ins.layer = 9;
        ShipRoom rom = ins.AddComponent<ShipRoom>();
        rom.roomArea = col2d;
        rom.RoomId = room;
        rom.FootStepSounds = footsteps;
        rom.AmbientSound = ambience;
        return rom;
    }

    public static SystemConsole CreateSystemConsole(Type minigametype, Vector3 transf, string name, Sprite sprite)
    {
        GameObject ins = GameObject.Instantiate(new GameObject());
        BoxCollider2D col2d = ins.AddComponent<BoxCollider2D>();
        col2d.size = Vector2.one / 2f;
        col2d.isTrigger = true;
        SystemConsole console = ins.AddComponent<SystemConsole>();
        SpriteRenderer img = ins.AddComponent<SpriteRenderer>();
        img.sprite = sprite;
        img.material.shader = Shader.Find("Sprites/Outline");
        console.Image = img;
        ins.transform.position = transf;
        Minigame uploaddat = CE_PrefabHelpers.FindPrefab(name, minigametype) as Minigame;
        console.MinigamePrefab = uploaddat;
        return console;
    }

    public static Vent CreateVent(string VentName, Vector2 Pos, Vent Left = null, Vent Right = null)
    {
        Vent V = GameObject.Instantiate(ReferenceVent);
        V.transform.name = "(Custom)" + VentName;
        V.Left = Left;
        V.Right = Right;
        V.gameObject.transform.position = new Vector3(Pos.x,Pos.y,V.gameObject.transform.position.z); //preserve Z
        V.gameObject.SetActive(true);
        V.Id = VersionShower.GetDeterministicHashCode(VentName);
        return V;
    }

    private static void ClearMapCollision(ShipStatus map)
    {
        foreach (Transform col in map.transform)
        {
           UnityEngine.Object.Destroy(col.gameObject);
        }
    }

    public static GameObject SpawnSprite(int x, int y, bool Solid)
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
        GameObject go = new GameObject("Test");
        go.layer = 9;
        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        var position = renderer.transform.position;
        if (!Solid)
        {
            renderer.material = new Material(Shader.Find("Unlit/MaskShader"));
        }
        position.x = 0.5f * x;
        position.y = 0.5f * y;
        position.z = (position.y / 1000f) + 0.5f;
        renderer.transform.position = position;
        renderer.sprite = sprite;

        if (Solid)
        {
            go.layer = LayerMask.NameToLayer("IlluminatedBlocking");
            position.z = 0.5f;
            BoxCollider2D boxCollider = go.AddComponent<BoxCollider2D>();
            boxCollider.transform.position = renderer.transform.position;
            boxCollider.size = new Vector3(0.5f, 0.5f);
        }
        return go;
    }

    public static void LoadDefaultSounds()
    {
        ShipRoom[] TempRoom = GameObject.FindObjectsOfType<ShipRoom>();
        List<SoundGroup> SoundGroupz = new List<SoundGroup>();
        List<AudioClip> AudioClipz = new List<AudioClip>();
        List<string> SoundNames = new List<string>();
        foreach (ShipRoom V in TempRoom)
        {
            if (!V.name.StartsWith("(Custom)"))
            {
                if (!SoundNames.Contains(V.FootStepSounds.Clips[0].name))
                {
                    SoundGroupz.Add(V.FootStepSounds);
                    SoundNames.Add(V.FootStepSounds.Clips[0].name);
                    Debug.Log("New sound detected! \nAdding sound: " + V.FootStepSounds.Clips[0].name);
                }
                if ((bool)V.AmbientSound)
                {
                    AudioClipz.Add(V.AmbientSound);
                    SoundNames.Add(V.AmbientSound.name);
                    Debug.Log("New sound detected! \nAdding sound: " + V.AmbientSound);
                }
            }
        }
        SoundGroups = SoundGroupz.ToArray();
        AmbienceSounds = AudioClipz.ToArray();
    }

    public static void MapTest(ShipStatus map)
    {
        if (!MapTestingActive) return;
        stat = map;
        Texture2D texture;
        string path = System.IO.Path.Combine(CE_Extensions.GetTexturesDirectory("Mapping"), "tasktest.png");
        texture = CE_TextureNSpriteExtensions.LoadPNG(path);
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.65f));




        ReferenceVent = map.GetComponentInChildren<Vent>();
        ReferenceVent.transform.parent = null;
        ReferenceVent.Left = null;
        ReferenceVent.Right = null;
        ReferenceVent.gameObject.SetActive(false);

        Vent v1 = CreateVent("TestVent1", new Vector2(0f,5f));
        v1.Left =  CreateVent("TestVent2", new Vector2(5f,5f),v1);



        Debug.Log("Clearing Tasks...");
        Debug.Log(map.CommonTasks.Length);
        foreach (NormalPlayerTask mp in map.CommonTasks)
        {
            UnityEngine.GameObject.Destroy(mp);
        }
        map.CommonTasks = new NormalPlayerTask[1];
        foreach (NormalPlayerTask mp in map.NormalTasks)
        {
            UnityEngine.GameObject.Destroy(mp);
        }
        map.NormalTasks = new NormalPlayerTask[3];
        foreach (NormalPlayerTask mp in map.LongTasks)
        {
            UnityEngine.GameObject.Destroy(mp);
        }
        map.LongTasks = new NormalPlayerTask[1];

        LoadDefaultSounds();
   

        Debug.Log("Task clearing complete! Adding tasks and their associated consoles...");
        NormalPlayerTask uptask = CreateTask(typeof(NormalPlayerTask),SystemTypes.Custom1,5,TaskTypes.ClearAsteroids, typeof(WeaponsMinigame),"WeaponsMinigame");
        CreateTaskConsole(new Vector3(3f, 3f, (3f / 1000f) + 0.5f), sprite, uptask, new IntRange(0, 5),SystemTypes.Custom1);
        NormalPlayerTask npt = CreateTask(typeof(UploadDataTask), SystemTypes.Custom2, 2, TaskTypes.UploadData, typeof(UploadDataGame), "UploadMinigame");
        CreateTaskConsole(new Vector3(3f, 6f, (3f / 1000f) + 0.5f), sprite, npt, new IntRange(0, 2), SystemTypes.Custom2);
        NormalPlayerTask npt2 = CreateTask(typeof(UploadDataTask), SystemTypes.Custom3, 2, TaskTypes.UploadData, typeof(UploadDataGame), "UploadMinigame");
        CreateTaskConsole(new Vector3(6f, 3f, (3f / 1000f) + 0.5f), sprite, npt2, new IntRange(0, 2), SystemTypes.Custom3);
        NormalPlayerTask npt3 = CreateTask(typeof(UploadDataTask), SystemTypes.Custom4, 2, TaskTypes.UploadData, typeof(UploadDataGame), "UploadMinigame");
        CreateTaskConsole(new Vector3(6f, 6f, (3f / 1000f) + 0.5f), sprite, npt3, new IntRange(0, 2), SystemTypes.Custom4);
        NormalPlayerTask npt4 = CreateTask(typeof(UploadDataTask), SystemTypes.Custom5, 2, TaskTypes.UploadData, typeof(UploadDataGame), "UploadMinigame");
        CreateTaskConsole(new Vector3(3f, 4f, (3f / 1000f) + 0.5f), sprite, npt4, new IntRange(0, 2), SystemTypes.Custom5);
        map.CommonTasks[0] = uptask;
        map.LongTasks[0] = npt;
        map.NormalTasks[0] = npt2;
        map.NormalTasks[1] = npt3;
        map.NormalTasks[2] = npt4;

        map.SpawnCenter = GameObject.Instantiate(new GameObject()).transform;

        AudioClip ambience = CE_WavUtility.ToAudioClip(Path.Combine(Application.dataPath, "CE_Assets", "Audio", "Ambience", "test.wav"));
        SoundGroup sg = ScriptableObject.CreateInstance(typeof(SoundGroup)) as SoundGroup;
        sg.Clips = new AudioClip[]
        {
            CE_WavUtility.ToAudioClip(Path.Combine(Application.dataPath, "CE_Assets", "Audio", "Ambience", "bloop.wav")),
            CE_WavUtility.ToAudioClip(Path.Combine(Application.dataPath, "CE_Assets", "Audio", "Ambience", "e.wav")),
            CE_WavUtility.ToAudioClip(Path.Combine(Application.dataPath, "CE_Assets", "Audio", "Ambience", "blip.wav"))
        };
        CreateShipRoom(SystemTypes.Custom6,AmbienceSounds[4],SoundGroups[2],new Vector2(5f,5f),new Vector2(5f,5f));

        //CreateSystemConsole(typeof(TaskAdderGame), new Vector3(5f, 5f, 0.5f), "TaskAddMinigame", sprite);
        Debug.Log("Clearing collision...");
        ClearMapCollision(map);
        Debug.Log("Spawning Map...");
        for (int x = -5; x < 5; x++)
        {
            for (int y = -5; y < 5; y++)
            {
                bool isSolid = (x == -5 || y == -5 || y == 4 || x == 4);
                SpawnSprite(x, y, BoolRange.Next(0.1f));
            }
        }
        GameObject.Destroy(ReferenceVent);
        ReferenceVent = null;
    }
}

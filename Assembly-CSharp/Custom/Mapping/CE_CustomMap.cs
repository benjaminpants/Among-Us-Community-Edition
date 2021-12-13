using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using InnerNet;
using Newtonsoft.Json;

public class CEM_TaskData
{
    public Type minigametype;
    public Type tasktype = typeof(NormalPlayerTask);
    public string MinigameName = "Invalid";

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
    public static List<CE_MapInfo> MapInfos = new List<CE_MapInfo>();
    public static Dictionary<TaskTypes,CEM_TaskData> TypeToTaskName= new Dictionary<TaskTypes, CEM_TaskData>();

    public static void Initialize()
    {
        /*MapInfos.Add(new CE_MapInfo("Clue")); //adds a dummy map named clue replacing skeld.
        if (!Directory.Exists(Path.Combine(CE_Extensions.GetGameDirectory(), "Maps")))
        {
            //Debug.Log("Release build, disabling CM logic!");
            return;
        }*/
        FileInfo[] files = new DirectoryInfo(Path.Combine(CE_Extensions.GetGameDirectory(), "Maps")).GetFiles("*.json");
        foreach (FileInfo fo in files)
        {
            using StreamReader streamReader = File.OpenText(fo.FullName);
            string json = streamReader.ReadToEnd();
            CEM_Map maptoload = JsonConvert.DeserializeObject<CEM_Map>(json);
            MapInfos.Add(new CE_MapInfo(maptoload.Name,maptoload, Path.Combine(CE_Extensions.GetGameDirectory(), "Maps", fo.Name.Remove(fo.Name.Length - 5))));
            Debug.Log(Path.Combine(CE_Extensions.GetGameDirectory(), "Maps", fo.Name.Remove(fo.Name.Length - 5)));
        }
        

/*        TypeToTaskName.Add(TaskTypes.UploadData, new CEM_TaskData(typeof(UploadDataGame), typeof(UploadDataTask), "UploadMinigame"));
        TypeToTaskName.Add(TaskTypes.ClearAsteroids, new CEM_TaskData(typeof(WeaponsMinigame), "WeaponsMinigame"));
        TypeToTaskName.Add(TaskTypes.AlignEngineOutput, new CEM_TaskData(typeof(AlignGame), "AlignMinigame"));
        TypeToTaskName.Add(TaskTypes.CalibrateDistributor, new CEM_TaskData(typeof(SweepMinigame), "SweepMinigame"));
        TypeToTaskName.Add(TaskTypes.ChartCourse, new CEM_TaskData(typeof(CourseMinigame), "CourseMinigame"));
        TypeToTaskName.Add(TaskTypes.SubmitScan, new CEM_TaskData(typeof(MedScanMinigame), "MedScanMinigame"));
        TypeToTaskName.Add(TaskTypes.PrimeShields, new CEM_TaskData(typeof(ShieldMinigame), "ShieldsMinigame"));
        TypeToTaskName.Add(TaskTypes.UnlockManifolds, new CEM_TaskData(typeof(UnlockManifoldsMinigame), "UnlockManifoldsMinigame"));
        TypeToTaskName.Add(TaskTypes.CleanO2Filter, new CEM_TaskData(typeof(LeafMinigame), "LeafMinigame"));
        TypeToTaskName.Add(TaskTypes.EmptyGarbage, new CEM_TaskData(typeof(EmptyGarbageMinigame), "EmptyGarbageMinigame"));
        TypeToTaskName.Add(TaskTypes.DivertPower, new CEM_TaskData(typeof(DivertPowerMinigame), "EmptyGarbageMinigame")); //use different system for divert power as its a bit weird
        TypeToTaskName.Add(TaskTypes.SwipeCard, new CEM_TaskData(typeof(CardSlideGame), "CardMinigame"));
        TypeToTaskName.Add(TaskTypes.FuelEngines, new CEM_TaskData(typeof(RefuelMinigame), "RefuelMinigame"));
        TypeToTaskName.Add(TaskTypes.InspectSample, new CEM_TaskData(typeof(SampleMinigame), "SampleMinigame"));
        TypeToTaskName.Add(TaskTypes.StartReactor, new CEM_TaskData(typeof(SimonSaysGame), "SimonSaysGame"));
        TypeToTaskName.Add(TaskTypes.EmptyChute, new CEM_TaskData(typeof(EmptyGarbageMinigame), "EmptyGarbageMinigame"));
        TypeToTaskName.Add(TaskTypes.FixWiring, new CEM_TaskData(typeof(WireMinigame), "WireMingame"));
    }
    */


    public static CE_MapInfo GetCurrentMap()
    {
        Debug.Log(PlayerControl.GameOptions.MapId);
        if (DestroyableSingleton<TutorialManager>.InstanceExists)
        {
            return MapInfos[0];
        }
        return MapInfos[PlayerControl.GameOptions.MapId];
    }
}

public class CE_CustomMap
{
    public static bool MapTestingActive = true;

    public static Vent ReferenceVent;
	
	public static Vent ReferenceVent2;

    public DivertPowerMetagame DivertMeta;

    public float usableDistance = 1f;

	public SpriteRenderer Image;

	public float UsableDistance => usableDistance;

	public float PercentCool => 0f;

    public static ShipStatus stat;

    public static SoundGroup[] SoundGroups;

    public static AudioClip[] AmbienceSounds;

    public enum FootstepSounds
    {
        Tile,
        Metal,
        Carpet
    }

    public enum Ambience
    {
        Medbay,
        Security,
        Comms,
        Engine,
        Reactor,
        Electrical,
        Shield,
        O2,
        Weapons
    }

    public static NormalPlayerTask CreateTask(Type tasktype, SystemTypes systype, int maxstep, TaskTypes taskty, Type minigametype, string name)
    {
        GameObject task = new GameObject();
        NormalPlayerTask datatask = task.AddComponent(tasktype) as NormalPlayerTask;
        datatask.TaskType = taskty;
        datatask.StartAt = systype;
        datatask.MaxStep = maxstep;
        Minigame uploaddat = CE_PrefabHelpers.FindPrefab(name, minigametype) as Minigame;
        datatask.MinigamePrefab = uploaddat;
        if (maxstep == 0 || maxstep == 1)
        {
            datatask.ShowTaskStep = false;
        }
        if (taskty == TaskTypes.InspectSample)
        {
            datatask.ShowTaskStep = false;
            datatask.ShowTaskTimer = true;
        }
        return GameObject.Instantiate(task).GetComponent(tasktype) as NormalPlayerTask;
    }
    
    public float CanUse(GameData.PlayerInfo pc, out bool canUse, out bool couldUse)
	{
		float num = float.MaxValue;
		PlayerControl @object = pc.Object;
		couldUse = pc.Object.CanMove;
		canUse = couldUse;
		if (canUse)
		{
			num = Vector2.Distance(@object.GetTruePosition(), base.transform.position);
			canUse &= num <= UsableDistance;
		}
		return num;
	}

	public void Use()
	{
		CanUse(PlayerControl.LocalPlayer.Data, out var canUse, out var _);
		if (canUse)
		{
			PlayerControl.LocalPlayer.NetTransform.Halt();
			DestroyableSingleton<HudManager>.Instance.ShowMap(delegate(MapBehaviour m)
			{
				m.ShowCountOverlay();
			});
		}
	}
}

    public static Console CreateTaskConsole(Vector3 transf, Sprite sprite, TaskTypes tt, IntRange range, SystemTypes room, int consoleid)
    {
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.65f));
        GameObject ins = new GameObject();
        BoxCollider2D col2d = ins.AddComponent<BoxCollider2D>();
        col2d.size = Vector2.one / 2f;
        col2d.isTrigger = true;
        ins.transform.position = transf;
        Console console = ins.AddComponent<Console>();
        SpriteRenderer img = ins.AddComponent<SpriteRenderer>();
        img.sprite = sprite;
        img.material.shader = Shader.Find("Sprites/Outline");
        console.Image = img;
        console.Room = room;
        TaskSet ts = new TaskSet();
        ts.taskStep = range;
        ts.taskType = tt;
        console.ValidTasks = new TaskSet[] {
            ts
        };
        console.TaskTypes = new TaskTypes[]
        {
            tt
        };
        console.ConsoleId = consoleid;
        return console;
    }
    
    public static ShipRoom CreateShipRoom(SystemTypes room, AudioClip ambience, SoundGroup footsteps, Vector2 position, Vector2 scale)
    {
        GameObject ins = new GameObject();
        BoxCollider2D col2d = ins.AddComponent<BoxCollider2D>();
        col2d.isTrigger = true;
        ins.transform.localScale = scale;
        ins.transform.position = position;
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
        GameObject ins = new GameObject();
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

    public static Vent CreateVent(string VentName, Vector3 Pos, Vent Left = null, Vent Right = null)
    {
        Vent V = GameObject.Instantiate(ReferenceVent);
        V.transform.name = "(Custom)" + VentName;
        V.Left = Left;
        V.Right = Right;
        Debug.Log(V.gameObject.transform.position.z);
        V.gameObject.transform.position = new Vector3(Pos.x,Pos.y,Pos.z);
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


   /*     var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.65f));
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
*/
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

    public static NormalPlayerTask ProcessCEMTask(CEM_Task ct)
    {
        TaskTypes tt = (TaskTypes)ct.TaskType;
        if (tt != TaskTypes.DivertPower || true) // TODO: PROPERLY IMPLEMENT DIVERT POWER
        {
            if (!CE_CustomMapManager.TypeToTaskName.TryGetValue(tt, out CEM_TaskData td))
            {
                throw new Exception("Unknown Task Type:" + tt.ToString());
            }
            return CreateTask(td.tasktype, (SystemTypes)ct.Room, ct.MaxStep, tt, td.minigametype, td.MinigameName);
        }
        else
        {
            
        }
        throw new Exception("Somehow reached end of ProcessCEMTask! Something has went horribly wrong!");
    }


    public static void MapTest(ShipStatus map)
    {
        if (!CE_CustomMapManager.GetCurrentMap().IsCustom) return;
        stat = map;



        ReferenceVent = map.GetComponentInChildren<Vent>();
        ReferenceVent.transform.parent = null;
        ReferenceVent.Left = RefrenceVent2;
        ReferenceVent.Right = null;
        ReferenceVent.gameObject.SetActive(true);
	    
	ReferenceVent2 = map.GetComponentInChildren<Vent>();
        ReferenceVent2.transform.parent = null;
        ReferenceVent2.Left = RefrenceVent;
        ReferenceVent2.Right = null;
        ReferenceVent2.gameObject.SetActive(true);

       // Vent v1 = CreateVent("TestVent1", new Vector2(0f,5f));
       // v1.Left =  CreateVent("TestVent2", new Vector2(5f,5f),v1);
        Debug.Log("Test Vents Active");



        Debug.Log("Clearing Tasks...");
        LoadDefaultSounds();
   

        Debug.Log("Task clearing complete! Adding tasks and their associated consoles...");
       /* NormalPlayerTask uptask = CreateTask(typeof(NormalPlayerTask),SystemTypes.Custom1,5,TaskTypes.ClearAsteroids, typeof(WeaponsMinigame),"WeaponsMinigame");
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
        map.NormalTasks[2] = npt4;*/
        /*AudioClip ambience = CE_WavUtility.ToAudioClip(Path.Combine(Application.dataPath, "CE_Assets", "Audio", "Ambience", "test.wav"));
        SoundGroup sg = ScriptableObject.CreateInstance(typeof(SoundGroup)) as SoundGroup;
        sg.Clips = new AudioClip[]
        {
            CE_WavUtility.ToAudioClip(Path.Combine(Application.dataPath, "CE_Assets", "Audio", "Ambience", "bloop.wav")),
            CE_WavUtility.ToAudioClip(Path.Combine(Application.dataPath, "CE_Assets", "Audio", "Ambience", "e.wav")),
            CE_WavUtility.ToAudioClip(Path.Combine(Application.dataPath, "CE_Assets", "Audio", "Ambience", "blip.wav"))
        };*/

        CreateSystemConsole(typeof(TaskAdderGame), new Vector3(5f, 5f, 0.5f), "TaskAddMinigame", Vector3);
        Debug.Log("Clearing collision...");
        ClearMapCollision(map);
        Debug.Log("Spawning Map...");
        //TODO: once the binary version of the map format gets implemented, avoid using the CEM classes.
        CEM_Map maptospawn = CE_CustomMapManager.GetCurrentMap().Map;
        string contentlocal = CE_CustomMapManager.GetCurrentMap().ContentFolder;
        if (maptospawn.TaskList.CommonTasks.Count == 0 || maptospawn.TaskList.ShortTasks.Count == 0 || maptospawn.TaskList.LongTasks.Count == 0)
        {
            Debug.LogWarning("Not Enough Tasks included in loaded map!\nUsing default skeld tasks...");

        }
        else
        {
            map.CommonTasks = new NormalPlayerTask[maptospawn.TaskList.CommonTasks.Count];
            map.NormalTasks = new NormalPlayerTask[maptospawn.TaskList.ShortTasks.Count];
            map.LongTasks = new NormalPlayerTask[maptospawn.TaskList.LongTasks.Count];
            foreach (NormalPlayerTask mp in map.CommonTasks)
            {
                UnityEngine.GameObject.Destroy(mp);
            }
            foreach (NormalPlayerTask mp in map.NormalTasks)
            {
                UnityEngine.GameObject.Destroy(mp);
            }
            foreach (NormalPlayerTask mp in map.LongTasks)
            {
                UnityEngine.GameObject.Destroy(mp);
            }
            for (int i = 0; i < maptospawn.TaskList.CommonTasks.Count; i++)
            {
                map.CommonTasks[i] = CE_CustomMap.ProcessCEMTask(maptospawn.TaskList.CommonTasks[i]);
            }
            for (int i = 0; i < maptospawn.TaskList.LongTasks.Count; i++)
            {
                map.LongTasks[i] = CE_CustomMap.ProcessCEMTask(maptospawn.TaskList.LongTasks[i]);
            }
            for (int i = 0; i < maptospawn.TaskList.ShortTasks.Count; i++)
            {
                map.NormalTasks[i] = CE_CustomMap.ProcessCEMTask(maptospawn.TaskList.ShortTasks[i]);
            }
        }
        foreach (CEM_Console C in maptospawn.Consoles)
        {
            if (File.Exists(Path.Combine(contentlocal, C.ImageLocal)))
            {
                byte[] data = File.ReadAllBytes(Path.Combine(contentlocal, C.ImageLocal));
                Texture2D texture2D = new Texture2D(2, 2);
                texture2D.LoadImage(data);
                CreateTaskConsole(new Vector3(C.Position.Values[0],C.Position.Values[1],C.Position.Values[2]), Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f)),(TaskTypes)C.TaskType,new IntRange(C.MinStep,C.MaxStep),(SystemTypes)C.Room,C.ConsoleID);
                
            }
        }

        foreach (CEM_Room R in maptospawn.Rooms)
        {
            CreateShipRoom((SystemTypes)R.RoomType, AmbienceSounds[R.Ambience], SoundGroups[R.Footsteps], new Vector3(R.Position.Values[0], R.Position.Values[1], R.Position.Values[2]), new Vector3(R.Scale.Values[0], R.Scale.Values[1], R.Scale.Values[2]));
        }
        foreach (CEM_Sprite SPR in maptospawn.Sprites)
        {
            if (File.Exists(Path.Combine(contentlocal, SPR.ImageLocal)))
            {
                byte[] data = File.ReadAllBytes(Path.Combine(contentlocal, SPR.ImageLocal));
                Texture2D texture2D = new Texture2D(2, 2);
                texture2D.LoadImage(data);
                GameObject to = new GameObject();
                to.name = "Sprite" + SPR.ImageLocal.GetHashCode();
                SpriteRenderer spir = to.AddComponent<SpriteRenderer>();
                spir.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0, 0));
                Debug.Log(spir.transform);
                if (!SPR.Fullbright)
                {
                    spir.material = new Material(Shader.Find("Unlit/MaskShader"));
                }
                to.layer = 9;
                to.transform.position = new Vector3(SPR.Position.Values[0], SPR.Position.Values[1], SPR.Position.Values[2]);
            }
        }
        foreach (CEM_WallLine Wall in maptospawn.Walls)
        {
            GameObject EdgeWall = new GameObject();
            EdgeCollider2D WallCol = EdgeWall.AddComponent<EdgeCollider2D>();
            List<Vector2> Pt = new List<Vector2>();
            foreach (CEM_Point Point in Wall.Points)
            {
                Pt.Add(new Vector2(Point.Values[0], Point.Values[1]));
            }
            WallCol.points = Pt.ToArray();
            if (Wall.ObscureVision)
            {
                EdgeWall.layer = LayerMask.NameToLayer("IlluminatedBlocking");
            }
        }
        Dictionary<Vent, CEM_Vent> CV = new Dictionary<Vent, CEM_Vent>();
        foreach (CEM_Vent V in maptospawn.Vents)
        {
            CV.Add(CreateVent(V.Name, new Vector3(V.Position.Values[0], V.Position.Values[1], V.Position.Values[2])),V);
        }
        GameObject.Destroy(ReferenceVent);
        ReferenceVent = null;
	ReferenceVent2 = null;
        foreach (Vent V in GameObject.FindObjectsOfType<Vent>())
        {
            if (V.name.StartsWith("(Custom)"))
            {
                CEM_Vent cv;
                if (CV.TryGetValue(V,out cv))
                {
                    if (cv.LeftName != "")
                    {
                        V.Left = GameObject.Find("(Custom)" + cv.LeftName).GetComponent<Vent>();
                    }
                    if (cv.RightName != "")
                    {
                        V.Right = GameObject.Find("(Custom)" + cv.RightName).GetComponent<Vent>();
                    }
                }
            }
        }

        GameObject newgam = new GameObject();
        newgam.transform.position = new Vector3(maptospawn.SpawnLocation.Values[0], maptospawn.SpawnLocation.Values[1], maptospawn.SpawnLocation.Values[2]);
        map.SpawnCenter = newgam.transform;
        /*for (int x = -5; x < 5; x++)
        {
            for (int y = -5; y < 5; y++)
            {
                bool isSolid = (x == -5 || y == -5 || y == 4 || x == 4);
                SpawnSprite(x, y, BoolRange.Next(0.1f));
            }
        }*/
    }
}

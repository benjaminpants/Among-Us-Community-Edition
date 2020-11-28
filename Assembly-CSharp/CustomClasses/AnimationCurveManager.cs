using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AnimationCurveManager : MonoBehaviour
{
    [Serializable]
    public sealed class ClipInfo
    {
        public int ClipInstanceID;
        public List<CurveInfo> CurveInfos = new List<CurveInfo>();

        // default constructor is sometimes required for (de)serialization
        public ClipInfo() { }

        public ClipInfo(Object clip, List<CurveInfo> curveInfos)
        {
            ClipInstanceID = clip.GetInstanceID();
            CurveInfos = curveInfos;
        }
    }

    [Serializable]
    public sealed class CurveInfo
    {
        public string PathKey;

        public List<KeyFrameInfo> Keys = new List<KeyFrameInfo>();
        public WrapMode PreWrapMode;
        public WrapMode PostWrapMode;

        // default constructor is sometimes required for (de)serialization
        public CurveInfo() { }

        public CurveInfo(string pathKey, AnimationCurve curve)
        {
            PathKey = pathKey;

            foreach (var keyframe in curve.keys)
            {
                Keys.Add(new KeyFrameInfo(keyframe));
            }

            PreWrapMode = curve.preWrapMode;
            PostWrapMode = curve.postWrapMode;
        }
    }

    [Serializable]
    public sealed class KeyFrameInfo
    {
        public float Value;
        public float InTangent;
        public float InWeight;
        public float OutTangent;
        public float OutWeight;
        public float Time;
        public WeightedMode WeightedMode;

        // default constructor is sometimes required for (de)serialization
        public KeyFrameInfo() { }

        public KeyFrameInfo(Keyframe keyframe)
        {
            Value = keyframe.value;
            InTangent = keyframe.inTangent;
            InWeight = keyframe.inWeight;
            OutTangent = keyframe.outTangent;
            OutWeight = keyframe.outWeight;
            Time = keyframe.time;
            WeightedMode = keyframe.weightedMode;
        }
    }

    // I know ... singleton .. but what choices do we have? ;)
    private static AnimationCurveManager _instance;
    public static AnimationCurveManager Instance
    {
        get
        {
            // lazy initialization/instantiation
            if (_instance) return _instance;

            _instance = FindObjectOfType<AnimationCurveManager>();

            if (_instance) return _instance;

            _instance = new GameObject("AnimationCurveManager").AddComponent<AnimationCurveManager>();

            return _instance;
        }
    }

    // Clips to manage e.g. reference these via the Inspector
    public List<AnimationClip> clips = new List<AnimationClip>();

    // every animation curve belongs to a specific clip and 
    // a specific property of a specific component on a specific object
    // for making this easier lets simply use a combined string as key
    private string CurveKey(string pathToObject, Type type, string propertyName)
    {
        return $"{pathToObject}:{type.FullName}:{propertyName}";
    }

    public List<ClipInfo> ClipCurves = new List<ClipInfo>();

    private void Awake()
    {
        if (_instance && _instance != this)
        {
            Debug.LogWarning("Multiple Instances of AnimationCurveManager! Will ignore this one!", this);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(gameObject);

        // load infos on runtime
        LoadClipCurves();
    }

#if UNITY_EDITOR
    // Call this from the ContextMenu (or later via editor script)
    [ContextMenu("Save Animation Curves")]
    private void SaveAnimationCurves()
    {
        ClipCurves.Clear();

        foreach (var clip in clips)
        {
            var curveInfos = new List<CurveInfo>();
            ClipCurves.Add(new ClipInfo(clip, curveInfos));

            foreach (var binding in AnimationUtility.GetCurveBindings(clip))
            {
                var key = CurveKey(binding.path, binding.type, binding.propertyName);
                var curve = AnimationUtility.GetEditorCurve(clip, binding);

                curveInfos.Add(new CurveInfo(key, curve));
            }
        }

        // create the StreamingAssets folder if it does not exist
        try
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }
        }
        catch (IOException ex)
        {
            Debug.LogError(ex.Message);
        }

        // create a new file e.g. AnimationCurves.dat in the StreamingAssets folder
        var fileStream = new FileStream(Path.Combine(Application.streamingAssetsPath, "AnimationCurves.dat"), FileMode.Create);

        // Construct a BinaryFormatter and use it to serialize the data to the stream.
        var formatter = new BinaryFormatter();
        try
        {
            formatter.Serialize(fileStream, ClipCurves);
        }
        catch (SerializationException e)
        {
            Debug.LogErrorFormat(this, "Failed to serialize. Reason: {0}", e.Message);
        }
        finally
        {
            fileStream.Close();
        }

        AssetDatabase.Refresh();
    }
#endif

    private void LoadClipCurves()
    {
        var filePath = Path.Combine(Application.streamingAssetsPath, "AnimationCurves.dat");

        if (!File.Exists(filePath))
        {
            Debug.LogErrorFormat(this, "File \"{0}\" not found!", filePath);
            return;
        }

        var fileStream = new FileStream(filePath, FileMode.Open);

        try
        {
            var formatter = new BinaryFormatter();

            // Deserialize the hashtable from the file and 
            // assign the reference to the local variable.
            ClipCurves = (List<ClipInfo>)formatter.Deserialize(fileStream);
        }
        catch (SerializationException e)
        {
            Debug.LogErrorFormat(this, "Failed to deserialize. Reason: {0}", e.Message);
        }
        finally
        {
            fileStream.Close();
        }
    }

    // now for getting a specific clip's curves
    public AnimationCurve GetCurve(AnimationClip clip, string pathToObject, Type type, string propertyName)
    {
        // either not loaded yet or error -> try again
        if (ClipCurves == null || ClipCurves.Count == 0) LoadClipCurves();
        // still null? -> error
        if (ClipCurves == null || ClipCurves.Count == 0)
        {
            Debug.LogError("Apparantly no clipCurves loaded!");
            return null;
        }

        var clipInfo = ClipCurves.FirstOrDefault(ci => ci.ClipInstanceID == clip.GetInstanceID());

        // does this clip exist in the dictionary?
        if (clipInfo == null)
        {
            Debug.LogErrorFormat(this, "The clip \"{0}\" was not found in clipCurves!", clip.name);
            return null;
        }

        var key = CurveKey(pathToObject, type, propertyName);

        var curveInfo = clipInfo.CurveInfos.FirstOrDefault(c => string.Equals(c.PathKey, key));

        // does the curve key exist for the clip?
        if (curveInfo == null)
        {
            Debug.LogErrorFormat(this, "The key \"{0}\" was not found for clip \"{1}\"", key, clip.name);
            return null;
        }

        var keyframes = new Keyframe[curveInfo.Keys.Count];

        for (var i = 0; i < curveInfo.Keys.Count; i++)
        {
            var keyframe = curveInfo.Keys[i];

            keyframes[i] = new Keyframe(keyframe.Time, keyframe.Value, keyframe.InTangent, keyframe.OutTangent, keyframe.InWeight, keyframe.OutWeight)
            {
                weightedMode = keyframe.WeightedMode
            };
        }

        var curve = new AnimationCurve(keyframes)
        {
            postWrapMode = curveInfo.PostWrapMode,
            preWrapMode = curveInfo.PreWrapMode
        };

        // otherwise finally return the AnimationCurve
        return curve;
    }
}
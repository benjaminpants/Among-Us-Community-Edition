using System.Linq;
using UnityEngine;

public class ProgressTracker : MonoBehaviour
{
	public MeshRenderer TileParent;

	private float curValue;

	public void Start()
	{
		TileParent.material.SetFloat("_Buckets", 1f);
		TileParent.material.SetFloat("_FullBuckets", 0f);
	}

	public void FixedUpdate()
	{
		if (PlayerTask.PlayerHasHudTask(PlayerControl.LocalPlayer))
		{
			TileParent.enabled = false;
			return;
		}
		if (!TileParent.enabled)
		{
			TileParent.enabled = true;
		}
		GameData instance = GameData.Instance;
		if ((bool)instance && instance.TotalTasks > 0)
		{
			int num = (DestroyableSingleton<TutorialManager>.InstanceExists ? 1 : (instance.AllPlayers.Count - PlayerControl.GameOptions.NumImpostors));
			num -= instance.AllPlayers.Count((GameData.PlayerInfo p) => p.Disconnected);
			float b = (float)instance.CompletedTasks / (float)instance.TotalTasks * (float)num;
			curValue = Mathf.Lerp(curValue, b, Time.fixedDeltaTime * 2f);
			TileParent.material.SetFloat("_Buckets", num);
			TileParent.material.SetFloat("_FullBuckets", curValue);
		}
	}
}

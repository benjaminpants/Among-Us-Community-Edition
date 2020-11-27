using System;
using System.Linq;
using UnityEngine;

// Token: 0x020000F9 RID: 249
public class ProgressTracker : MonoBehaviour
{
	// Token: 0x06000551 RID: 1361 RVA: 0x000055C4 File Offset: 0x000037C4
	public void Start()
	{
		this.TileParent.material.SetFloat("_Buckets", 1f);
		this.TileParent.material.SetFloat("_FullBuckets", 0f);
	}

	// Token: 0x06000552 RID: 1362 RVA: 0x00022AB0 File Offset: 0x00020CB0
	public void FixedUpdate()
	{
		if (PlayerTask.PlayerHasHudTask(PlayerControl.LocalPlayer))
		{
			this.TileParent.enabled = false;
			return;
		}
		if (!this.TileParent.enabled)
		{
			this.TileParent.enabled = true;
		}
		GameData instance = GameData.Instance;
		if (instance && instance.TotalTasks > 0)
		{
			int num = DestroyableSingleton<TutorialManager>.InstanceExists ? 1 : (instance.AllPlayers.Count - PlayerControl.GameOptions.NumImpostors);
			num -= instance.AllPlayers.Count((GameData.PlayerInfo p) => p.Disconnected);
			float b = (float)instance.CompletedTasks / (float)instance.TotalTasks * (float)num;
			this.curValue = Mathf.Lerp(this.curValue, b, Time.fixedDeltaTime * 2f);
			this.TileParent.material.SetFloat("_Buckets", (float)num);
			this.TileParent.material.SetFloat("_FullBuckets", this.curValue);
		}
	}

	// Token: 0x0400051B RID: 1307
	public MeshRenderer TileParent;

	// Token: 0x0400051C RID: 1308
	private float curValue;
}

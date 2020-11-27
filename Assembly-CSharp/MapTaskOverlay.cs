using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000E1 RID: 225
public class MapTaskOverlay : MonoBehaviour
{
	// Token: 0x060004C2 RID: 1218 RVA: 0x0001B9AC File Offset: 0x00019BAC
	public void Show()
	{
		base.gameObject.SetActive(true);
		if (PlayerTask.PlayerHasHudTask(PlayerControl.LocalPlayer))
		{
			return;
		}
		for (int i = 0; i < PlayerControl.LocalPlayer.myTasks.Count; i++)
		{
			PlayerTask playerTask = PlayerControl.LocalPlayer.myTasks[i];
			if (playerTask.HasLocation && !playerTask.IsComplete)
			{
				PooledMapIcon pooledMapIcon = this.icons.Get<PooledMapIcon>();
				pooledMapIcon.transform.localScale = new Vector3(pooledMapIcon.NormalSize, pooledMapIcon.NormalSize, pooledMapIcon.NormalSize);
				if (PlayerTask.TaskIsEmergency(playerTask))
				{
					pooledMapIcon.rend.color = Color.red;
					pooledMapIcon.alphaPulse.enabled = true;
					pooledMapIcon.rend.material.SetFloat("_Outline", 1f);
				}
				else
				{
					pooledMapIcon.rend.color = Color.yellow;
				}
				MapTaskOverlay.SetIconLocation(playerTask, pooledMapIcon);
				this.data.Add(playerTask, pooledMapIcon);
			}
		}
	}

	// Token: 0x060004C3 RID: 1219 RVA: 0x0001BAAC File Offset: 0x00019CAC
	public void Update()
	{
		if (PlayerTask.PlayerHasHudTask(PlayerControl.LocalPlayer))
		{
			return;
		}
		for (int i = 0; i < PlayerControl.LocalPlayer.myTasks.Count; i++)
		{
			PlayerTask playerTask = PlayerControl.LocalPlayer.myTasks[i];
			if (playerTask.HasLocation && !playerTask.IsComplete && playerTask.LocationDirty)
			{
				PooledMapIcon pooledMapIcon;
				if (!this.data.TryGetValue(playerTask, out pooledMapIcon))
				{
					pooledMapIcon = this.icons.Get<PooledMapIcon>();
					pooledMapIcon.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
					if (PlayerTask.TaskIsEmergency(playerTask))
					{
						pooledMapIcon.rend.color = Color.red;
						pooledMapIcon.alphaPulse.enabled = true;
						pooledMapIcon.rend.material.SetFloat("_Outline", 1f);
					}
					else
					{
						pooledMapIcon.rend.color = Color.yellow;
					}
					this.data.Add(playerTask, pooledMapIcon);
				}
				MapTaskOverlay.SetIconLocation(playerTask, pooledMapIcon);
			}
		}
	}

	// Token: 0x060004C4 RID: 1220 RVA: 0x0001BBBC File Offset: 0x00019DBC
	private static void SetIconLocation(PlayerTask task, PooledMapIcon mapIcon)
	{
		if (mapIcon.lastMapTaskStep != task.TaskStep)
		{
			mapIcon.lastMapTaskStep = task.TaskStep;
			Vector3 vector = task.Location;
			vector /= ShipStatus.Instance.MapScale;
			vector.z = -1f;
			mapIcon.name = task.name;
			mapIcon.transform.localPosition = vector;
			if (task.TaskStep > 0)
			{
				mapIcon.alphaPulse.enabled = true;
				mapIcon.rend.material.SetFloat("_Outline", 1f);
			}
		}
	}

	// Token: 0x060004C5 RID: 1221 RVA: 0x0001BC54 File Offset: 0x00019E54
	public void Hide()
	{
		foreach (KeyValuePair<PlayerTask, PooledMapIcon> keyValuePair in this.data)
		{
			keyValuePair.Value.OwnerPool.Reclaim(keyValuePair.Value);
		}
		this.data.Clear();
		base.gameObject.SetActive(false);
	}

	// Token: 0x04000498 RID: 1176
	public ObjectPoolBehavior icons;

	// Token: 0x04000499 RID: 1177
	private Dictionary<PlayerTask, PooledMapIcon> data = new Dictionary<PlayerTask, PooledMapIcon>();
}

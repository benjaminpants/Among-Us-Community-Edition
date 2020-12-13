using System.Collections.Generic;
using UnityEngine;

public class MapTaskOverlay : MonoBehaviour
{
	public ObjectPoolBehavior icons;

	private Dictionary<PlayerTask, PooledMapIcon> data = new Dictionary<PlayerTask, PooledMapIcon>();

	public void Show()
	{
		base.gameObject.SetActive(value: true);
		if (PlayerTask.PlayerHasHudTask(PlayerControl.LocalPlayer))
		{
			return;
		}
		for (int i = 0; i < PlayerControl.LocalPlayer.myTasks.Count; i++)
		{
			PlayerTask playerTask = PlayerControl.LocalPlayer.myTasks[i];
			if (playerTask.HasLocation && !playerTask.IsComplete)
			{
				PooledMapIcon pooledMapIcon = icons.Get<PooledMapIcon>();
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
				SetIconLocation(playerTask, pooledMapIcon);
				data.Add(playerTask, pooledMapIcon);
			}
		}
	}

	public void Update()
	{
		if (PlayerTask.PlayerHasHudTask(PlayerControl.LocalPlayer))
		{
			return;
		}
		for (int i = 0; i < PlayerControl.LocalPlayer.myTasks.Count; i++)
		{
			PlayerTask playerTask = PlayerControl.LocalPlayer.myTasks[i];
			if (!playerTask.HasLocation || playerTask.IsComplete || !playerTask.LocationDirty)
			{
				continue;
			}
			if (!data.TryGetValue(playerTask, out var value))
			{
				value = icons.Get<PooledMapIcon>();
				value.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
				if (PlayerTask.TaskIsEmergency(playerTask))
				{
					value.rend.color = Color.red;
					value.alphaPulse.enabled = true;
					value.rend.material.SetFloat("_Outline", 1f);
				}
				else
				{
					value.rend.color = Color.yellow;
				}
				data.Add(playerTask, value);
			}
			SetIconLocation(playerTask, value);
		}
	}

	private static void SetIconLocation(PlayerTask task, PooledMapIcon mapIcon)
	{
		if (mapIcon.lastMapTaskStep != task.TaskStep)
		{
			mapIcon.lastMapTaskStep = task.TaskStep;
			Vector3 localPosition = task.Location;
			localPosition /= ShipStatus.Instance.MapScale;
			localPosition.z = -1f;
			mapIcon.name = task.name;
			mapIcon.transform.localPosition = localPosition;
			if (task.TaskStep > 0)
			{
				mapIcon.alphaPulse.enabled = true;
				mapIcon.rend.material.SetFloat("_Outline", 1f);
			}
		}
	}

	public void Hide()
	{
		foreach (KeyValuePair<PlayerTask, PooledMapIcon> datum in data)
		{
			datum.Value.OwnerPool.Reclaim(datum.Value);
		}
		data.Clear();
		base.gameObject.SetActive(value: false);
	}
}

using System;
using Assets.CoreScripts;
using UnityEngine;

public class Console : MonoBehaviour, IUsable
{
	public float usableDistance = 1f;

	public int ConsoleId;

	public bool onlyFromBelow;

	public bool GhostsIgnored;

	public SystemTypes Room;

	public TaskTypes[] TaskTypes;

	public TaskSet[] ValidTasks;

	public SpriteRenderer Image;

	public float UsableDistance => usableDistance;

	public float PercentCool => 0f;

	public void SetOutline(bool on, bool mainTarget)
	{
		if ((bool)Image)
		{
			Image.material.SetFloat("_Outline", on ? 1 : 0);
			Image.material.SetColor("_OutlineColor", Color.yellow);
			Image.material.SetColor("_AddColor", mainTarget ? Color.yellow : Color.clear);
		}
	}

	public float CanUse(GameData.PlayerInfo pc, out bool canUse, out bool couldUse)
	{
		float num = float.MaxValue;
		PlayerControl @object = pc.Object;
		couldUse = (!pc.IsDead || (PlayerControl.GameOptions.GhostsDoTasks && !GhostsIgnored)) && @object.CanMove && !RestrictedByBeingImposter(pc, @object) && (!onlyFromBelow || @object.transform.position.y < base.transform.position.y) && (bool)FindTask(@object);
		canUse = couldUse;
		if (canUse)
		{
			num = Vector2.Distance(@object.GetTruePosition(), base.transform.position);
			canUse &= num <= UsableDistance;
		}
		return num;
	}

	private PlayerTask FindTask(PlayerControl pc)
	{
		for (int i = 0; i < pc.myTasks.Count; i++)
		{
			PlayerTask playerTask = pc.myTasks[i];
			if (!playerTask.IsComplete && playerTask.ValidConsole(this))
			{
				return playerTask;
			}
		}
		return null;
	}

	public void Use()
	{
		CanUse(PlayerControl.LocalPlayer.Data, out var canUse, out var _);
		if (canUse)
		{
			PlayerControl localPlayer = PlayerControl.LocalPlayer;
			PlayerTask playerTask = FindTask(localPlayer);
			localPlayer.moveable = false;
			localPlayer.NetTransform.Halt();
            Minigame prefab = playerTask.MinigamePrefab;
			if ((bool)prefab)
			{
				Minigame minigame = UnityEngine.Object.Instantiate(prefab);
				minigame.transform.SetParent(Camera.main.transform, worldPositionStays: false);
				minigame.transform.localPosition = new Vector3(0f, 0f, -50f);
				minigame.Console = this;
				minigame.Begin(playerTask);
				DestroyableSingleton<Telemetry>.Instance.WriteUse(localPlayer.PlayerId, playerTask.TaskType, base.transform.position);
			}
		}
	}

	public bool RestrictedByBeingImposter(GameData.PlayerInfo pc, PlayerControl @object)
	{
		try
		{
			switch (FindTask(@object).TaskType)
			{
			case global::TaskTypes.ResetReactor:
				return false;
			case global::TaskTypes.FixLights:
				return false;
			case global::TaskTypes.FixComms:
				return false;
			case global::TaskTypes.RestoreOxy:
				return false;
			case global::TaskTypes.CleanO2Filter:
				break;
			}
		}
		catch (Exception)
		{
		}
		return pc.IsImpostor || CE_RoleManager.GetRoleFromID(pc.role).DoesNotDoTasks();
	}
}

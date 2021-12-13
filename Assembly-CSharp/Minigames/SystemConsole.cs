using UnityEngine;

public class SystemConsole : MonoBehaviour, IUsable
{
	public float usableDistance = 1f;

	public bool FreeplayOnly;

	public SpriteRenderer Image;

	public Minigame MinigamePrefab;

	public float UsableDistance => usableDistance;

	public float PercentCool => 0f;

	public void Start()
	{
		if (FreeplayOnly && !DestroyableSingleton<TutorialManager>.InstanceExists)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void SetOutline(bool on, bool mainTarget)
	{
		if ((bool)Image)
		{
			Image.material.SetFloat("_Outline", on ? 1 : 0);
			Image.material.SetColor("_OutlineColor", Color.white);
			Image.material.SetColor("_AddColor", mainTarget ? Color.white : Color.clear);
		}
	}

	public float CanUse(GameData.PlayerInfo pc, out bool canUse, out bool couldUse)
	{
		float num = float.MaxValue;
		PlayerControl @object = pc.Object;
		couldUse = pc.Object.CanMove && (!pc.IsDead || !(MinigamePrefab is EmergencyMinigame));
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
			Minigame minigame = Object.Instantiate(MinigamePrefab);
			minigame.transform.SetParent(Camera.main.transform, worldPositionStays: false);
			minigame.transform.localPosition = new Vector3(0f, 0f, -50f);
			if (!IsCustom)
			{
				minigame.Begin(null);
			}
			else
            {
				minigame.Begin(TaskOverride);
            }
		}
	}
}

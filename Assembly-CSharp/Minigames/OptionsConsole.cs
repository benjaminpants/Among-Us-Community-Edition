using UnityEngine;

public class OptionsConsole : MonoBehaviour, IUsable
{
	public CustomPlayerMenu MenuPrefab;

	public SpriteRenderer Outline;

	public float UsableDistance => 1f;

	public float PercentCool => 0f;

	public float CanUse(GameData.PlayerInfo pc, out bool canUse, out bool couldUse)
	{
		float num = float.MaxValue;
		PlayerControl @object = pc.Object;
		couldUse = @object.CanMove;
		canUse = couldUse;
		if (canUse)
		{
			num = Vector2.Distance(@object.GetTruePosition(), base.transform.position);
			canUse &= num <= UsableDistance;
		}
		return num;
	}

	public void SetOutline(bool on, bool mainTarget)
	{
		if ((bool)Outline)
		{
			Outline.material.SetFloat("_Outline", on ? 1 : 0);
			Outline.material.SetColor("_OutlineColor", Color.white);
			Outline.material.SetColor("_AddColor", mainTarget ? Color.white : Color.clear);
		}
	}

	public void Use()
	{
		CanUse(PlayerControl.LocalPlayer.Data, out var canUse, out var _);
		if (canUse)
		{
			PlayerControl.LocalPlayer.NetTransform.Halt();
			CustomPlayerMenu customPlayerMenu = Object.Instantiate(MenuPrefab);
			customPlayerMenu.transform.SetParent(Camera.main.transform, worldPositionStays: false);
			customPlayerMenu.transform.localPosition = new Vector3(0f, 0f, -20f);
		}
	}
}

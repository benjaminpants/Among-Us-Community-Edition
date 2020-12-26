using UnityEngine;

public class CE_PlayerOptions : MonoBehaviour
{
	public static CustomPlayerMenu MenuPrefabPermanent;
	private static void FindPrefab()
	{
		if (MenuPrefabPermanent) return;
		var resources = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(CustomPlayerMenu));
		if (resources != null)
		{
			foreach (var item in resources)
			{
				if (item.name == "PlayerOptionsMenu")
				{
					MenuPrefabPermanent = Object.Instantiate<CustomPlayerMenu>(item as CustomPlayerMenu, Camera.current.transform);
					return;
				}
			}
		}
	}
	public static void Open()
	{
		FindPrefab();
		if (!MenuPrefabPermanent)
		{
			PlayerControl.LocalPlayer.NetTransform.Halt();
			CustomPlayerMenu customPlayerMenu = MenuPrefabPermanent;
			customPlayerMenu.transform.localPosition = new Vector3(0f, 0f, -20f);
		}
	}
}

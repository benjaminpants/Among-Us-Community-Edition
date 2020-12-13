using UnityEngine;

public class GameSettingMenu : MonoBehaviour
{
	public Transform[] AllItems;

	public float YStart;

	public float YOffset;

	public Transform[] HideForOnline;

	private void OnEnable()
	{
		int num = 0;
		for (int i = 0; i < AllItems.Length; i++)
		{
			Transform transform = AllItems[i];
			if (transform.gameObject.activeSelf)
			{
				if ((AmongUsClient.Instance.GameMode == GameModes.OnlineGame && HideForOnline.IndexOf(transform) != -1) || (transform.name == "MapName" && !TempData.IsDo2Enabled))
				{
					transform.gameObject.SetActive(value: false);
					continue;
				}
				Vector3 localPosition = transform.localPosition;
				localPosition.y = YStart - (float)num * YOffset;
				transform.localPosition = localPosition;
				num++;
			}
		}
		GetComponent<Scroller>().YBounds.max = (float)num * YOffset / 2f + 0.1f;
	}
}

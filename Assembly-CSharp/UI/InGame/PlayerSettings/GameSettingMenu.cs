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
				if (i != 0)
				{
					transform.gameObject.SetActive(value: false);
					continue;
				}
				else
                {
					Vector3 localPosition = transform.localPosition;
					localPosition.y = YStart - (float)num * YOffset;
					transform.localPosition = localPosition;
					num++;
				}
			}
		}
		GetComponent<Scroller>().allowY = false;
	}
}

using UnityEngine;

public class ConditionalHide : MonoBehaviour
{
	public RuntimePlatform[] HideForPlatforms = new RuntimePlatform[1]
	{
		RuntimePlatform.WindowsPlayer
	};

	private void Awake()
	{
		for (int i = 0; i < HideForPlatforms.Length; i++)
		{
			if (HideForPlatforms[i] == RuntimePlatform.WindowsPlayer)
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}
}

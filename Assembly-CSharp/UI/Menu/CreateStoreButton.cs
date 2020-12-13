using UnityEngine;

public class CreateStoreButton : MonoBehaviour
{
	public Transform Target;

	public StoreMenu StorePrefab;

	public void Click()
	{
		StoreMenu storeMenu = Object.Instantiate(StorePrefab, Target);
		storeMenu.transform.localPosition = new Vector3(0f, 0f, -100f);
		storeMenu.transform.localScale = Vector3.zero;
	}
}

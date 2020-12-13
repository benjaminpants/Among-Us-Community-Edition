using UnityEngine;

public class DotAligner : MonoBehaviour
{
	public float Width = 2f;

	public bool Even;

	public void Start()
	{
		int num = 0;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			if (base.transform.GetChild(i).gameObject.activeSelf)
			{
				num++;
			}
		}
		float num2;
		float num3;
		if (Even)
		{
			num2 = (0f - Width) * (float)(num - 1) / 2f;
			num3 = Width;
		}
		else
		{
			num2 = (0f - Width) / 2f;
			num3 = Width / (float)(num - 1);
		}
		int num4 = 0;
		for (int j = 0; j < base.transform.childCount; j++)
		{
			Transform child = base.transform.GetChild(j);
			if (child.gameObject.activeSelf)
			{
				child.transform.localPosition = new Vector3(num2 + (float)num4 * num3, 0f, 0f);
				num4++;
			}
		}
	}
}

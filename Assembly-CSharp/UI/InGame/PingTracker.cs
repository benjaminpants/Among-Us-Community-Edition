using UnityEngine;

public class PingTracker : MonoBehaviour
{
	public TextRenderer text;

	public GameObject ugh;

	private static Sprite assmark;

	private void Update()
	{
		if (ugh == null)
        {
			ugh = GameObject.Instantiate<GameObject>(GameObject.Find("MenuButton"));
			ugh.name = "WatermarkOfPain";
            ugh.transform.parent = GameObject.Find("Hud").transform;
			AspectPosition pos = ugh.GetComponent<AspectPosition>();
			pos.DistanceFromEdge = new Vector3(pos.DistanceFromEdge.x,pos.DistanceFromEdge.y * 5f, pos.DistanceFromEdge.z);
            Destroy(ugh.GetComponent<ButtonBehavior>());
			Destroy(ugh.GetComponent<Collider>());
			if (!assmark)
            {
				assmark = CE_TextureNSpriteExtensions.ConvertToSpriteAutoPivot(CE_CommonUI.STUPIDASSWATERMARK);
			}
			ugh.GetComponent<SpriteRenderer>().sprite = assmark;
            ugh.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 50);
			ugh.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			pos.AdjustPosition();
        }
		if ((bool)AmongUsClient.Instance)
		{
			if (AmongUsClient.Instance.GameMode == GameModes.FreePlay)
			{
				text.Text = "> Among Us: CE <\n> " + VersionShower.BuildID + " <\n[FFFF00FF]> Alpha Build <[]";
				//text.Text = ((int)(1.0f / Time.smoothDeltaTime)).ToString();
			}
			else
			{
				text.Text = $"Ping: {AmongUsClient.Instance.Ping} ms\n> " + VersionShower.BuildID + " <\n[FFFF00FF]> Alpha Build <[]";
			}
		}
	}
}

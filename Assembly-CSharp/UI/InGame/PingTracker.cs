using UnityEngine;

public class PingTracker : MonoBehaviour
{
	public TextRenderer text;

	public GameObject watermark;

	private static Sprite watermark2;

	private void Update()
	{
		if (watermark == null)
        {
			watermark = GameObject.Instantiate<GameObject>(GameObject.Find("MenuButton"));
			watermark.name = "Watermark";
            watermark.transform.parent = GameObject.Find("Hud").transform;
			AspectPosition pos = watermark.GetComponent<AspectPosition>();
			pos.DistanceFromEdge = new Vector3(pos.DistanceFromEdge.x,pos.DistanceFromEdge.y * 5f, pos.DistanceFromEdge.z);
            Destroy(watermark.GetComponent<ButtonBehavior>());
			Destroy(watermark.GetComponent<Collider>());
			if (!watermark2)
            {
				watermark2 = CE_TextureNSpriteExtensions.ConvertToSpriteAutoPivot(CE_CommonUI.Watermark);
			}
			watermark.GetComponent<SpriteRenderer>().sprite = watermark2;
            watermark.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 50);
			watermark.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			pos.AdjustPosition();
        }
		if ((bool)AmongUsClient.Instance)
		{
			if (AmongUsClient.Instance.GameMode == GameModes.FreePlay)
			{
				text.Text = "> Among Us: CEC REVIVED <\n> " + VersionShower.BuildID + " <\n[FF009F]> Alpha Build <[]";
				//text.Text = ((int)(1.0f / Time.smoothDeltaTime)).ToString();
			}
			else
			{
				text.Text = $"Ping: {AmongUsClient.Instance.Ping} ms\n> " + VersionShower.BuildID + " <\n[FF009F]> Alpha Build <[]";
			}
		}
	}
}

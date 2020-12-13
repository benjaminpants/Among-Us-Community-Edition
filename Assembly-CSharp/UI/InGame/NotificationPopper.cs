using System.Text;
using UnityEngine;

public class NotificationPopper : MonoBehaviour
{
	public TextRenderer TextArea;

	public float zPos = -350f;

	private float alphaTimer;

	public float ShowDuration = 5f;

	public float FadeDuration = 1f;

	public Color textColor = Color.white;

	private StringBuilder builder = new StringBuilder();

	public AudioClip NotificationSound;

	public void Update()
	{
		if (alphaTimer > 0f)
		{
			float num = Camera.main.orthographicSize * Camera.main.aspect;
			if (!DestroyableSingleton<HudManager>.Instance.TaskText.isActiveAndEnabled)
			{
				float height = DestroyableSingleton<HudManager>.Instance.GameSettings.Height;
				Transform transform = DestroyableSingleton<HudManager>.Instance.GameSettings.transform;
				base.transform.localPosition = new Vector3(0f - num + 0.1f, transform.localPosition.y - height, zPos);
			}
			else
			{
				float height2 = DestroyableSingleton<HudManager>.Instance.TaskText.Height;
				Transform parent = DestroyableSingleton<HudManager>.Instance.TaskText.transform.parent;
				base.transform.localPosition = new Vector3(0f - num + 0.1f, parent.localPosition.y - height2 - 0.2f, zPos);
			}
			alphaTimer -= Time.deltaTime;
			textColor.a = Mathf.Clamp(alphaTimer / FadeDuration, 0f, 1f);
			TextArea.Color = textColor;
			if (alphaTimer <= 0f)
			{
				builder.Clear();
				TextArea.Text = string.Empty;
			}
		}
	}

	public void AddItem(string item)
	{
		builder.AppendLine(item);
		TextArea.Text = builder.ToString();
		alphaTimer = ShowDuration;
		SoundManager.Instance.PlaySound(NotificationSound, loop: false);
	}
}

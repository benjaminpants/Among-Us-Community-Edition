using System;
using System.Text;
using UnityEngine;

// Token: 0x020000F4 RID: 244
public class NotificationPopper : MonoBehaviour
{
	// Token: 0x06000538 RID: 1336 RVA: 0x0002243C File Offset: 0x0002063C
	public void Update()
	{
		if (this.alphaTimer > 0f)
		{
			float num = Camera.main.orthographicSize * Camera.main.aspect;
			if (!DestroyableSingleton<HudManager>.Instance.TaskText.isActiveAndEnabled)
			{
				float height = DestroyableSingleton<HudManager>.Instance.GameSettings.Height;
				Transform transform = DestroyableSingleton<HudManager>.Instance.GameSettings.transform;
				base.transform.localPosition = new Vector3(-num + 0.1f, transform.localPosition.y - height, this.zPos);
			}
			else
			{
				float height2 = DestroyableSingleton<HudManager>.Instance.TaskText.Height;
				Transform parent = DestroyableSingleton<HudManager>.Instance.TaskText.transform.parent;
				base.transform.localPosition = new Vector3(-num + 0.1f, parent.localPosition.y - height2 - 0.2f, this.zPos);
			}
			this.alphaTimer -= Time.deltaTime;
			this.textColor.a = Mathf.Clamp(this.alphaTimer / this.FadeDuration, 0f, 1f);
			this.TextArea.Color = this.textColor;
			if (this.alphaTimer <= 0f)
			{
				this.builder.Clear();
				this.TextArea.Text = string.Empty;
			}
		}
	}

	// Token: 0x06000539 RID: 1337 RVA: 0x00022598 File Offset: 0x00020798
	public void AddItem(string item)
	{
		this.builder.AppendLine(item);
		this.TextArea.Text = this.builder.ToString();
		this.alphaTimer = this.ShowDuration;
		SoundManager.Instance.PlaySound(this.NotificationSound, false, 1f);
	}

	// Token: 0x04000501 RID: 1281
	public TextRenderer TextArea;

	// Token: 0x04000502 RID: 1282
	public float zPos = -350f;

	// Token: 0x04000503 RID: 1283
	private float alphaTimer;

	// Token: 0x04000504 RID: 1284
	public float ShowDuration = 5f;

	// Token: 0x04000505 RID: 1285
	public float FadeDuration = 1f;

	// Token: 0x04000506 RID: 1286
	public Color textColor = Color.white;

	// Token: 0x04000507 RID: 1287
	private StringBuilder builder = new StringBuilder();

	// Token: 0x04000508 RID: 1288
	public AudioClip NotificationSound;
}

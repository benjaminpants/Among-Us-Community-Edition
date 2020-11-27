using System;
using System.Collections.Generic;
using InnerNet;
using UnityEngine;

// Token: 0x020000B9 RID: 185
public class BanMenu : MonoBehaviour
{
	// Token: 0x060003DC RID: 988 RVA: 0x000047BA File Offset: 0x000029BA
	public void ShowButton(bool show)
	{
		show &= AmongUsClient.Instance.CanKick();
		show &= (MeetingHud.Instance || !ShipStatus.Instance);
		base.GetComponent<SpriteRenderer>().enabled = show;
	}

	// Token: 0x060003DD RID: 989 RVA: 0x000047F6 File Offset: 0x000029F6
	public void SetButtonActive(bool show)
	{
		show &= AmongUsClient.Instance.CanKick();
		show &= (MeetingHud.Instance || !ShipStatus.Instance);
		base.GetComponent<PassiveButton>().enabled = show;
	}

	// Token: 0x060003DE RID: 990 RVA: 0x00018CF4 File Offset: 0x00016EF4
	public void Show()
	{
		if (this.ContentParent.activeSelf)
		{
			this.Hide();
			return;
		}
		this.selected = -1;
		this.KickButton.color = Color.gray;
		this.BanButton.color = Color.gray;
		this.ContentParent.SetActive(true);
		int num = 1;
		if (AmongUsClient.Instance)
		{
			this.UpdateKicksLeft();
			if (AmongUsClient.Instance.CanKick())
			{
				List<ClientData> allClients = AmongUsClient.Instance.allClients;
				for (int i = 0; i < allClients.Count; i++)
				{
					ClientData clientData = allClients[i];
					if (clientData.Id != AmongUsClient.Instance.ClientId && clientData.Character)
					{
						GameData.PlayerInfo data = clientData.Character.Data;
						if (!string.IsNullOrWhiteSpace(data.PlayerName))
						{
							BanButton banButton = UnityEngine.Object.Instantiate<BanButton>(this.BanButtonPrefab, this.ContentParent.transform);
							banButton.transform.localPosition = new Vector3(-0.2f, -0.15f - 0.4f * (float)num, -1f);
							banButton.Parent = this;
							banButton.NameText.Text = data.PlayerName;
							banButton.TargetClientId = clientData.Id;
							banButton.Unselect();
							this.allButtons.Add(banButton);
							num++;
						}
					}
				}
			}
		}
		this.BanLeftText.transform.localPosition = new Vector3(-0.78f, 0.049999997f, -1f);
		this.KickButton.transform.localPosition = new Vector3(-0.8f, -0.15f - 0.4f * (float)num - 0.1f, -1f);
		this.BanButton.transform.localPosition = new Vector3(0.3f, -0.15f - 0.4f * (float)num - 0.1f, -1f);
		float num2 = 0.3f + (float)(num + 1) * 0.4f;
		this.Background.size = new Vector2(3f, num2);
		this.Background.GetComponent<BoxCollider2D>().size = new Vector2(3f, num2);
		this.Background.transform.localPosition = new Vector3(0f, -num2 / 2f + 0.15f, 0.1f);
	}

	// Token: 0x060003DF RID: 991 RVA: 0x00018F5C File Offset: 0x0001715C
	public void Hide()
	{
		this.selected = -1;
		this.ContentParent.SetActive(false);
		for (int i = 0; i < this.allButtons.Count; i++)
		{
			UnityEngine.Object.Destroy(this.allButtons[i].gameObject);
		}
		this.allButtons.Clear();
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x00018FB4 File Offset: 0x000171B4
	public void Select(int client)
	{
		this.selected = client;
		for (int i = 0; i < this.allButtons.Count; i++)
		{
			if (this.allButtons[i].TargetClientId != client)
			{
				this.allButtons[i].Unselect();
			}
		}
		this.KickButton.color = Color.white;
		this.BanButton.color = Color.white;
	}

	// Token: 0x060003E1 RID: 993 RVA: 0x00004832 File Offset: 0x00002A32
	public void Kick(bool ban)
	{
		if (this.selected >= 0 && AmongUsClient.Instance.CanKick())
		{
			AmongUsClient.Instance.KickPlayer(this.selected, ban);
			this.UpdateKicksLeft();
			this.Hide();
		}
	}

	// Token: 0x060003E2 RID: 994 RVA: 0x00019024 File Offset: 0x00017224
	private void UpdateKicksLeft()
	{
		string str = (AmongUsClient.Instance.KicksLeft < 0) ? "∞" : AmongUsClient.Instance.KicksLeft.ToString();
		this.BanLeftText.Text = "Kicks Left: " + str;
	}

	// Token: 0x040003CB RID: 971
	public BanButton BanButtonPrefab;

	// Token: 0x040003CC RID: 972
	public SpriteRenderer Background;

	// Token: 0x040003CD RID: 973
	public SpriteRenderer BanButton;

	// Token: 0x040003CE RID: 974
	public SpriteRenderer KickButton;

	// Token: 0x040003CF RID: 975
	public TextRenderer BanLeftText;

	// Token: 0x040003D0 RID: 976
	public GameObject ContentParent;

	// Token: 0x040003D1 RID: 977
	public int selected = -1;

	// Token: 0x040003D2 RID: 978
	[HideInInspector]
	public List<BanButton> allButtons = new List<BanButton>();
}

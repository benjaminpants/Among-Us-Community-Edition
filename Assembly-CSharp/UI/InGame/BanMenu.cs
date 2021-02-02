using System.Collections.Generic;
using InnerNet;
using UnityEngine;

public class BanMenu : MonoBehaviour
{
	public BanButton BanButtonPrefab;

	public SpriteRenderer Background;

	public SpriteRenderer BanButton;

	public SpriteRenderer KickButton;

	public TextRenderer BanLeftText;

	public GameObject ContentParent;

	public int selected = -1;

	[HideInInspector]
	public List<BanButton> allButtons = new List<BanButton>();

	public void ShowButton(bool show)
	{
		show &= AmongUsClient.Instance.CanKick();
		show &= (bool)MeetingHud.Instance || !ShipStatus.Instance;
		GetComponent<SpriteRenderer>().enabled = show;
	}

	public void SetButtonActive(bool show)
	{
		show &= AmongUsClient.Instance.CanKick();
		show &= (bool)MeetingHud.Instance || !ShipStatus.Instance;
		GetComponent<PassiveButton>().enabled = show;
	}

	public void Show()
	{
		if (ContentParent.activeSelf)
		{
			Hide();
			return;
		}
		selected = -1;
		KickButton.color = Color.gray;
		BanButton.color = Color.gray;
		ContentParent.SetActive(value: true);
		int num = 1;
		if ((bool)AmongUsClient.Instance)
		{
			UpdateKicksLeft();
			if (AmongUsClient.Instance.CanKick())
			{
				List<ClientData> allClients = AmongUsClient.Instance.allClients;
				for (int i = 0; i < allClients.Count; i++)
				{
					ClientData clientData = allClients[i];
					if (clientData.Id != AmongUsClient.Instance.ClientId && (bool)clientData.Character)
					{
						GameData.PlayerInfo data = clientData.Character.Data;
						if (!string.IsNullOrWhiteSpace(data.PlayerName))
						{
							BanButton banButton = Object.Instantiate(BanButtonPrefab, ContentParent.transform);
							banButton.transform.localPosition = new Vector3(-0.2f, (-0.15f - 0.4f * (float)num) / 2f, -1f);
							banButton.Parent = this;
							banButton.gameObject.transform.localScale *= new Vector2(1f,0.5f);
                            banButton.NameText.Text = data.PlayerName;
							banButton.NameText.gameObject.transform.localScale *= new Vector2(0.5f, 1f);
							banButton.TargetClientId = clientData.Id;
							banButton.Unselect();
							allButtons.Add(banButton);
							num++;
						}
					}
				}
			}
		}
		BanLeftText.transform.localPosition = new Vector3(-0.78f, (0.049999997f) / 2f, -1f);
		KickButton.transform.localPosition = new Vector3(-0.8f, (-0.15f - 0.4f * (float)num - 0.1f) / 2f, -1f);
		BanButton.transform.localPosition = new Vector3(0.3f, (-0.15f - 0.4f * (float)num - 0.1f) / 2f, -1f);
		float num2 = 0.3f + (float)(num + 1) * 0.4f;
		Background.size = new Vector2(3f, (num2 / 2f));
		Background.GetComponent<BoxCollider2D>().size = new Vector2(3f, (num2 / 2f));
		Background.transform.localPosition = new Vector3(0f, ((0f - num2) / 2f + 0.15f) / 2f, 0.1f);
	}

	public void Hide()
	{
		selected = -1;
		ContentParent.SetActive(value: false);
		for (int i = 0; i < allButtons.Count; i++)
		{
			Object.Destroy(allButtons[i].gameObject);
		}
		allButtons.Clear();
	}

	public void Select(int client)
	{
		selected = client;
		for (int i = 0; i < allButtons.Count; i++)
		{
			if (allButtons[i].TargetClientId != client)
			{
				allButtons[i].Unselect();
			}
		}
		KickButton.color = Color.white;
		BanButton.color = Color.white;
	}

	public void Kick(bool ban)
	{
		if (selected >= 0 && AmongUsClient.Instance.CanKick())
		{
			AmongUsClient.Instance.KickPlayer(selected, ban);
			UpdateKicksLeft();
			Hide();
		}
	}

	private void UpdateKicksLeft()
	{
		string str = ((AmongUsClient.Instance.KicksLeft < 0) ? "∞" : AmongUsClient.Instance.KicksLeft.ToString());
		BanLeftText.Text = "Kicks Left: " + str;
	}
}

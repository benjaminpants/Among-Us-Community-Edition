using System.Net;
using UnityEngine;

public class ServerSelector : MonoBehaviour
{
	public ServerInfo MyServer = new ServerInfo();

	public TextRenderer Text;

	public ButtonRolloverHandler Background;

	public TextBox ipInput;

	public ServerSelectUi Parent
	{
		get;
		set;
	}

	public void Start()
	{
		if ((bool)ipInput)
		{
			ipInput.SetText(MyServer.Ip);
			ipInput.OnChange.AddListener(OnIpChange);
		}
		else
		{
			Text.Text = MyServer.Name;
		}
	}

	private void OnIpChange()
	{
		if (IPAddress.TryParse(ipInput.text, out var _))
		{
			MyServer.Name = "Custom";
			MyServer.Ip = ipInput.text;
			Select();
		}
	}

	public void Select()
	{
		Background.OutColor = Color.green;
		Background.DoMouseOut();
		Parent.SelectServer(this);
	}

	internal void Unselect()
	{
		Background.OutColor = Color.white;
		Background.DoMouseOut();
	}
}

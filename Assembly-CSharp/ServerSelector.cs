using System;
using System.Net;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001B7 RID: 439
public class ServerSelector : MonoBehaviour
{
	// Token: 0x1700017C RID: 380
	// (get) Token: 0x06000982 RID: 2434 RVA: 0x00007C7C File Offset: 0x00005E7C
	// (set) Token: 0x06000983 RID: 2435 RVA: 0x00007C84 File Offset: 0x00005E84
	public ServerSelectUi Parent { get; set; }

	// Token: 0x06000984 RID: 2436 RVA: 0x000319F0 File Offset: 0x0002FBF0
	public void Start()
	{
		if (this.ipInput)
		{
			this.ipInput.SetText(this.MyServer.Ip, "");
			this.ipInput.OnChange.AddListener(new UnityAction(this.OnIpChange));
			return;
		}
		this.Text.Text = this.MyServer.Name;
	}

	// Token: 0x06000985 RID: 2437 RVA: 0x00031A58 File Offset: 0x0002FC58
	private void OnIpChange()
	{
		IPAddress ipaddress;
		if (!IPAddress.TryParse(this.ipInput.text, out ipaddress))
		{
			return;
		}
		this.MyServer.Name = "Custom";
		this.MyServer.Ip = this.ipInput.text;
		this.Select();
	}

	// Token: 0x06000986 RID: 2438 RVA: 0x00007C8D File Offset: 0x00005E8D
	public void Select()
	{
		this.Background.OutColor = Color.green;
		this.Background.DoMouseOut();
		this.Parent.SelectServer(this);
	}

	// Token: 0x06000987 RID: 2439 RVA: 0x00007CB6 File Offset: 0x00005EB6
	internal void Unselect()
	{
		this.Background.OutColor = Color.white;
		this.Background.DoMouseOut();
	}

	// Token: 0x04000911 RID: 2321
	public ServerInfo MyServer = new ServerInfo();

	// Token: 0x04000912 RID: 2322
	public TextRenderer Text;

	// Token: 0x04000913 RID: 2323
	public ButtonRolloverHandler Background;

	// Token: 0x04000914 RID: 2324
	public TextBox ipInput;
}

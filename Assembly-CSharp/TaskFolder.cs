using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200022D RID: 557
public class TaskFolder : MonoBehaviour
{
	// Token: 0x06000BEA RID: 3050 RVA: 0x0000924E File Offset: 0x0000744E
	public void Start()
	{
		this.Text.Text = this.FolderName;
	}

	// Token: 0x06000BEB RID: 3051 RVA: 0x00009261 File Offset: 0x00007461
	public void OnClick()
	{
		this.Parent.ShowFolder(this);
	}

	// Token: 0x06000BEC RID: 3052 RVA: 0x00002732 File Offset: 0x00000932
	internal List<TaskFolder> OrderBy()
	{
		throw new NotImplementedException();
	}

	// Token: 0x04000B86 RID: 2950
	public string FolderName;

	// Token: 0x04000B87 RID: 2951
	public TextRenderer Text;

	// Token: 0x04000B88 RID: 2952
	public TaskAdderGame Parent;

	// Token: 0x04000B89 RID: 2953
	public List<TaskFolder> SubFolders = new List<TaskFolder>();

	// Token: 0x04000B8A RID: 2954
	public List<PlayerTask> Children = new List<PlayerTask>();
}

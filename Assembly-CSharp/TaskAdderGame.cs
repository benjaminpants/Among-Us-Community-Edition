using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// Token: 0x0200022B RID: 555
public class TaskAdderGame : Minigame
{
	// Token: 0x06000BDF RID: 3039 RVA: 0x0003A718 File Offset: 0x00038918
	public override void Begin(PlayerTask t)
	{
		base.Begin(t);
		this.Root = UnityEngine.Object.Instantiate<TaskFolder>(this.RootFolderPrefab, base.transform);
		this.Root.gameObject.SetActive(false);
		Dictionary<SystemTypes, TaskFolder> folders = new Dictionary<SystemTypes, TaskFolder>();
		this.PopulateRoot(this.Root, folders, ShipStatus.Instance.CommonTasks);
		this.PopulateRoot(this.Root, folders, ShipStatus.Instance.LongTasks);
		this.PopulateRoot(this.Root, folders, ShipStatus.Instance.NormalTasks);
		this.Root.SubFolders = (from f in this.Root.SubFolders
		orderby f.FolderName
		select f).ToList<TaskFolder>();
		this.ShowFolder(this.Root);
	}

	// Token: 0x06000BE0 RID: 3040 RVA: 0x0003A7EC File Offset: 0x000389EC
	private void PopulateRoot(TaskFolder rootFolder, Dictionary<SystemTypes, TaskFolder> folders, NormalPlayerTask[] taskList)
	{
		foreach (NormalPlayerTask normalPlayerTask in taskList)
		{
			SystemTypes systemTypes = normalPlayerTask.StartAt;
			if (normalPlayerTask is DivertPowerTask)
			{
				systemTypes = ((DivertPowerTask)normalPlayerTask).TargetSystem;
			}
			if (systemTypes == SystemTypes.LowerEngine)
			{
				systemTypes = SystemTypes.UpperEngine;
			}
			TaskFolder taskFolder;
			if (!folders.TryGetValue(systemTypes, out taskFolder))
			{
				taskFolder = (folders[systemTypes] = UnityEngine.Object.Instantiate<TaskFolder>(this.RootFolderPrefab, base.transform));
				taskFolder.gameObject.SetActive(false);
				if (systemTypes == SystemTypes.UpperEngine)
				{
					taskFolder.FolderName = "Engines";
				}
				else
				{
					taskFolder.FolderName = SystemTypeHelpers.StringNames[(int)systemTypes];
				}
				rootFolder.SubFolders.Add(taskFolder);
			}
			taskFolder.Children.Add(normalPlayerTask);
		}
	}

	// Token: 0x06000BE1 RID: 3041 RVA: 0x000091F0 File Offset: 0x000073F0
	public void GoToRoot()
	{
		this.Heirarchy.Clear();
		this.ShowFolder(this.Root);
	}

	// Token: 0x06000BE2 RID: 3042 RVA: 0x0003A8A0 File Offset: 0x00038AA0
	public void GoUpOne()
	{
		if (this.Heirarchy.Count > 1)
		{
			TaskFolder taskFolder = this.Heirarchy[this.Heirarchy.Count - 2];
			this.Heirarchy.RemoveAt(this.Heirarchy.Count - 1);
			this.Heirarchy.RemoveAt(this.Heirarchy.Count - 1);
			this.ShowFolder(taskFolder);
		}
	}

	// Token: 0x06000BE3 RID: 3043 RVA: 0x0003A90C File Offset: 0x00038B0C
	public void ShowFolder(TaskFolder taskFolder)
	{
		StringBuilder stringBuilder = new StringBuilder(64);
		this.Heirarchy.Add(taskFolder);
		for (int i = 0; i < this.Heirarchy.Count; i++)
		{
			stringBuilder.Append(this.Heirarchy[i].FolderName);
			stringBuilder.Append("\\");
		}
		this.PathText.Text = stringBuilder.ToString();
		for (int j = 0; j < this.ActiveItems.Count; j++)
		{
			UnityEngine.Object.Destroy(this.ActiveItems[j].gameObject);
		}
		this.ActiveItems.Clear();
		float num = 0f;
		float num2 = 0f;
		for (int k = 0; k < taskFolder.SubFolders.Count; k++)
		{
			TaskFolder taskFolder2 = UnityEngine.Object.Instantiate<TaskFolder>(taskFolder.SubFolders[k], this.TaskParent);
			taskFolder2.gameObject.SetActive(true);
			taskFolder2.Parent = this;
			taskFolder2.transform.localPosition = new Vector3(num, num2, 0f);
			taskFolder2.transform.localScale = Vector3.one;
			num += this.folderWidth;
			if (num > this.lineWidth)
			{
				num = 0f;
				num2 += this.lineHeight;
			}
			this.ActiveItems.Add(taskFolder2.transform);
		}
		List<PlayerTask> list = (from t in taskFolder.Children
		orderby t.TaskType.ToString()
		select t).ToList<PlayerTask>();
		for (int l = 0; l < list.Count; l++)
		{
			TaskAddButton taskAddButton = UnityEngine.Object.Instantiate<TaskAddButton>(this.TaskPrefab);
			taskAddButton.MyTask = list[l];
			if (taskAddButton.MyTask.TaskType == TaskTypes.DivertPower && (((DivertPowerTask)taskAddButton.MyTask).TargetSystem == SystemTypes.LowerEngine || ((DivertPowerTask)taskAddButton.MyTask).TargetSystem == SystemTypes.UpperEngine))
			{
				taskAddButton.Text.Text = TaskTypesHelpers.StringNames[(int)((byte)taskAddButton.MyTask.TaskType)] + " (" + SystemTypeHelpers.StringNames[(int)((DivertPowerTask)taskAddButton.MyTask).TargetSystem] + ")";
			}
			else
			{
				taskAddButton.Text.Text = TaskTypesHelpers.StringNames[(int)((byte)taskAddButton.MyTask.TaskType)];
			}
			this.AddFileAsChild(taskAddButton, ref num, ref num2);
		}
		if (this.Heirarchy.Count == 1)
		{
			TaskAddButton taskAddButton2 = UnityEngine.Object.Instantiate<TaskAddButton>(this.InfectedButton);
			taskAddButton2.Text.Text = "Be_Impostor.exe";
			this.AddFileAsChild(taskAddButton2, ref num, ref num2);
		}
	}

	// Token: 0x06000BE4 RID: 3044 RVA: 0x0003ABBC File Offset: 0x00038DBC
	private void AddFileAsChild(TaskAddButton item, ref float xCursor, ref float yCursor)
	{
		item.transform.SetParent(this.TaskParent);
		item.transform.localPosition = new Vector3(xCursor, yCursor, 0f);
		item.transform.localScale = Vector3.one;
		xCursor += this.fileWidth;
		if (xCursor > this.lineWidth)
		{
			xCursor = 0f;
			yCursor -= this.lineHeight;
		}
		this.ActiveItems.Add(item.transform);
	}

	// Token: 0x04000B77 RID: 2935
	public TextRenderer PathText;

	// Token: 0x04000B78 RID: 2936
	public TaskFolder RootFolderPrefab;

	// Token: 0x04000B79 RID: 2937
	public TaskAddButton TaskPrefab;

	// Token: 0x04000B7A RID: 2938
	public Transform TaskParent;

	// Token: 0x04000B7B RID: 2939
	public List<TaskFolder> Heirarchy = new List<TaskFolder>();

	// Token: 0x04000B7C RID: 2940
	public List<Transform> ActiveItems = new List<Transform>();

	// Token: 0x04000B7D RID: 2941
	public TaskAddButton InfectedButton;

	// Token: 0x04000B7E RID: 2942
	public float folderWidth;

	// Token: 0x04000B7F RID: 2943
	public float fileWidth;

	// Token: 0x04000B80 RID: 2944
	public float lineWidth;

	// Token: 0x04000B81 RID: 2945
	public float lineHeight;

	// Token: 0x04000B82 RID: 2946
	private TaskFolder Root;
}

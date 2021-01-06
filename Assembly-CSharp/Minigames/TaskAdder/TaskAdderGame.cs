using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TaskAdderGame : Minigame
{
	public TextRenderer PathText;

	public TaskFolder RootFolderPrefab;

	public TaskAddButton TaskPrefab;

	public Transform TaskParent;

	public List<TaskFolder> Heirarchy = new List<TaskFolder>();

	public List<Transform> ActiveItems = new List<Transform>();

	public TaskAddButton InfectedButton;

	public float folderWidth;

	public float fileWidth;

	public float lineWidth;

	public float lineHeight;

	private TaskFolder Root;

	public override void Begin(PlayerTask t)
	{
		base.Begin(t);
		Root = Object.Instantiate(RootFolderPrefab, base.transform);
		Root.gameObject.SetActive(value: false);
		Dictionary<SystemTypes, TaskFolder> folders = new Dictionary<SystemTypes, TaskFolder>();
		PopulateRoot(Root, folders, ShipStatus.Instance.CommonTasks);
		PopulateRoot(Root, folders, ShipStatus.Instance.LongTasks);
		PopulateRoot(Root, folders, ShipStatus.Instance.NormalTasks);
		Root.SubFolders = Root.SubFolders.OrderBy((TaskFolder f) => f.FolderName).ToList();
		ShowFolder(Root);
	}

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
			if (!folders.TryGetValue(systemTypes, out var value))
			{
				TaskFolder taskFolder2 = (folders[systemTypes] = Object.Instantiate(RootFolderPrefab, base.transform));
				value = taskFolder2;
				value.gameObject.SetActive(value: false);
				if (systemTypes == SystemTypes.UpperEngine)
				{
					value.FolderName = "Engines";
				}
				else
				{
					value.FolderName = SystemTypeHelpers.StringNames[(uint)systemTypes];
				}
				rootFolder.SubFolders.Add(value);
			}
			value.Children.Add(normalPlayerTask);
		}
	}

	public void GoToRoot()
	{
		Heirarchy.Clear();
		ShowFolder(Root);
	}

	public void GoUpOne()
	{
		if (Heirarchy.Count > 1)
		{
			TaskFolder taskFolder = Heirarchy[Heirarchy.Count - 2];
			Heirarchy.RemoveAt(Heirarchy.Count - 1);
			Heirarchy.RemoveAt(Heirarchy.Count - 1);
			ShowFolder(taskFolder);
		}
	}

	public void ShowFolder(TaskFolder taskFolder)
	{
		StringBuilder stringBuilder = new StringBuilder(64);
		Heirarchy.Add(taskFolder);
		for (int i = 0; i < Heirarchy.Count; i++)
		{
			stringBuilder.Append(Heirarchy[i].FolderName);
			stringBuilder.Append("\\");
		}
		PathText.Text = stringBuilder.ToString();
		for (int j = 0; j < ActiveItems.Count; j++)
		{
			Object.Destroy(ActiveItems[j].gameObject);
		}
		ActiveItems.Clear();
		float xCursor = 0f;
		float yCursor = 0f;
		for (int k = 0; k < taskFolder.SubFolders.Count; k++)
		{
			TaskFolder taskFolder2 = Object.Instantiate(taskFolder.SubFolders[k], TaskParent);
			taskFolder2.gameObject.SetActive(value: true);
			taskFolder2.Parent = this;
			taskFolder2.transform.localPosition = new Vector3(xCursor, yCursor, 0f);
			taskFolder2.transform.localScale = Vector3.one;
			xCursor += folderWidth;
			if (xCursor > lineWidth)
			{
				xCursor = 0f;
				yCursor += lineHeight;
			}
			ActiveItems.Add(taskFolder2.transform);
		}
		List<PlayerTask> list = taskFolder.Children.OrderBy((PlayerTask t) => t.TaskType.ToString()).ToList();
		for (int l = 0; l < list.Count; l++)
		{
			TaskAddButton taskAddButton = Object.Instantiate(TaskPrefab);
			taskAddButton.MyTask = list[l];
			if (taskAddButton.MyTask.TaskType == TaskTypes.DivertPower && (((DivertPowerTask)taskAddButton.MyTask).TargetSystem == SystemTypes.LowerEngine || ((DivertPowerTask)taskAddButton.MyTask).TargetSystem == SystemTypes.UpperEngine))
			{
				taskAddButton.Text.Text = TaskTypesHelpers.StringNames[(byte)taskAddButton.MyTask.TaskType] + " (" + SystemTypeHelpers.StringNames[(uint)((DivertPowerTask)taskAddButton.MyTask).TargetSystem] + ")";
			}
			else
			{
				taskAddButton.Text.Text = TaskTypesHelpers.StringNames[(byte)taskAddButton.MyTask.TaskType];
			}
			AddFileAsChild(taskAddButton, ref xCursor, ref yCursor);
		}
		if (Heirarchy.Count == 1)
		{
			TaskAddButton taskAddButton3 = Object.Instantiate(InfectedButton);
			taskAddButton3.Text.Text = "Settings.exe";
			taskAddButton3.OptionsTask = true;
			taskAddButton3.ImpostorTask = false;
			taskAddButton3.OnOptionsTask += TaskAddButton3_OnOptionsTask;
			AddFileAsChild(taskAddButton3, ref xCursor, ref yCursor, true);
			taskAddButton3.SetOptionsTaskColor(Color.gray);

			TaskAddButton taskAddButton4 = Object.Instantiate(InfectedButton);
			taskAddButton4.Text.Text = "TODO.txt";
			taskAddButton4.OptionsTask = true;
			taskAddButton4.ImpostorTask = false;
			taskAddButton4.OnOptionsTask += TaskAddButton4_OnOptionsTask;
			AddFileAsChild(taskAddButton4, ref xCursor, ref yCursor, true);
			taskAddButton4.SetOptionsTaskColor(Color.white);

			TaskAddButton taskAddButton2 = Object.Instantiate(InfectedButton);
			taskAddButton2.Text.Text = "Impostor.exe";
			AddFileAsChild(taskAddButton2, ref xCursor, ref yCursor, true);
		}
	}

	private void TaskAddButton4_OnOptionsTask(object sender, System.EventArgs e)
	{
		CE_DevMinigame.IsShown = true;
		base.Close();
	}

	private void TaskAddButton3_OnOptionsTask(object sender, System.EventArgs e)
    {
		CE_PlayerOptions.Open();
		base.Close();
	}

	private void AddFileAsChild(TaskAddButton item, ref float xCursor, ref float yCursor, bool addInstead = false)
	{
		item.transform.SetParent(TaskParent);
		item.transform.localPosition = new Vector3(xCursor, yCursor, 0f);
		item.transform.localScale = Vector3.one;
		xCursor += fileWidth;
		if (xCursor > lineWidth)
		{
			xCursor = 0f;
			if (addInstead) yCursor += lineHeight;
			else yCursor -= lineHeight;
		}
		ActiveItems.Add(item.transform);
	}
}

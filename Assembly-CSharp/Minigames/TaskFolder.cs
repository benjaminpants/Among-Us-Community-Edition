using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskFolder : MonoBehaviour
{
	public string FolderName;

	public TextRenderer Text;

	public TaskAdderGame Parent;

	public List<TaskFolder> SubFolders = new List<TaskFolder>();

	public List<PlayerTask> Children = new List<PlayerTask>();

	public void Start()
	{
		Text.Text = FolderName;
	}

	public void OnClick()
	{
		Parent.ShowFolder(this);
	}

	internal List<TaskFolder> OrderBy()
	{
		throw new NotImplementedException();
	}
}

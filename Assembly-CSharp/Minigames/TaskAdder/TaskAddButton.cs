using System;
using UnityEngine;

// Token: 0x0200022A RID: 554
public class TaskAddButton : MonoBehaviour
{
	// Token: 0x06000BDB RID: 3035 RVA: 0x0003A4E0 File Offset: 0x000386E0
	public void Start()
	{
		if (this.ImpostorTask)
		{
			GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
			this.Overlay.enabled = data.IsImpostor;
			this.Overlay.sprite = this.CheckImage;
			return;
		}
		PlayerTask playerTask = this.FindTaskByType();
		if (playerTask)
		{
			this.Overlay.enabled = true;
			this.Overlay.sprite = (playerTask.IsComplete ? this.CheckImage : this.ExImage);
			return;
		}
		this.Overlay.enabled = false;
	}

	// Token: 0x06000BDC RID: 3036 RVA: 0x0003A56C File Offset: 0x0003876C
	public void AddTask()
	{
		if (this.ImpostorTask)
		{
			GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
			if (data.IsImpostor)
			{
				PlayerControl.LocalPlayer.RemoveInfected();
				this.Overlay.enabled = false;
				return;
			}
			PlayerControl.LocalPlayer.RpcSetInfected(new GameData.PlayerInfo[]
			{
				data
			});
			this.Overlay.enabled = true;
			return;
		}
		else
		{
			PlayerTask playerTask = this.FindTaskByType();
			if (!playerTask)
			{
				PlayerTask playerTask2 = UnityEngine.Object.Instantiate<PlayerTask>(this.MyTask, PlayerControl.LocalPlayer.transform);
				PlayerTask playerTask3 = playerTask2;
				PlayerControl localPlayer = PlayerControl.LocalPlayer;
				uint taskIdCount = localPlayer.TaskIdCount;
				localPlayer.TaskIdCount = taskIdCount + 1U;
				playerTask3.Id = taskIdCount;
				playerTask2.Owner = PlayerControl.LocalPlayer;
				playerTask2.Initialize();
				PlayerControl.LocalPlayer.myTasks.Add(playerTask2);
				GameData.Instance.TutOnlyAddTask(PlayerControl.LocalPlayer.PlayerId, playerTask2.Id);
				this.Overlay.sprite = this.ExImage;
				this.Overlay.enabled = true;
				return;
			}
			PlayerControl.LocalPlayer.RemoveTask(playerTask);
			this.Overlay.enabled = false;
			return;
		}
	}

	// Token: 0x06000BDD RID: 3037 RVA: 0x0003A680 File Offset: 0x00038880
	private PlayerTask FindTaskByType()
	{
		for (int i = PlayerControl.LocalPlayer.myTasks.Count - 1; i > -1; i--)
		{
			PlayerTask playerTask = PlayerControl.LocalPlayer.myTasks[i];
			if (playerTask.TaskType == this.MyTask.TaskType)
			{
				if (playerTask.TaskType == TaskTypes.DivertPower)
				{
					if (((DivertPowerTask)playerTask).TargetSystem == ((DivertPowerTask)this.MyTask).TargetSystem)
					{
						return playerTask;
					}
				}
				else
				{
					if (playerTask.TaskType != TaskTypes.UploadData)
					{
						return playerTask;
					}
					if (playerTask.StartAt == this.MyTask.StartAt)
					{
						return playerTask;
					}
				}
			}
		}
		return null;
	}

	// Token: 0x04000B71 RID: 2929
	public TextRenderer Text;

	// Token: 0x04000B72 RID: 2930
	public SpriteRenderer Overlay;

	// Token: 0x04000B73 RID: 2931
	public Sprite CheckImage;

	// Token: 0x04000B74 RID: 2932
	public Sprite ExImage;

	// Token: 0x04000B75 RID: 2933
	public PlayerTask MyTask;

	// Token: 0x04000B76 RID: 2934
	public bool ImpostorTask;
}

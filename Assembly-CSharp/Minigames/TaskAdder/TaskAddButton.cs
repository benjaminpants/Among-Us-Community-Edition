using UnityEngine;

public class TaskAddButton : MonoBehaviour
{
	public TextRenderer Text;

	public SpriteRenderer Overlay;

	public Sprite CheckImage;

	public Sprite ExImage;

	public PlayerTask MyTask;

	public bool ImpostorTask;

	public bool OptionsTask;

	public event System.EventHandler OnOptionsTask;

	public void SetOptionsTaskColor(Color color)
    {
		var renders = this.gameObject.GetComponents<SpriteRenderer>();
		foreach (var render in renders)
        {
			Debug.Log(render.name);
			render.color = color;
        }

	}

	public void Start()
	{
		if (ImpostorTask)
		{
            Text.scale -= 0.2f;
			Text.maxWidth += 0.4f;
			GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
			Overlay.enabled = data.IsImpostor;
			Overlay.sprite = CheckImage;
			return;
		}
		if (OptionsTask)
        {
			Text.scale -= 0.2f;
			Text.maxWidth += 0.4f;
			Overlay.enabled = false;
			Overlay.sprite = CheckImage;
			return;
		}
		PlayerTask playerTask = FindTaskByType();
		if ((bool)playerTask)
		{
			Overlay.enabled = true;
			Overlay.sprite = (playerTask.IsComplete ? CheckImage : ExImage);
		}
		else
		{
			Overlay.enabled = false;
		}
	}

	public void AddTask()
	{
		if (ImpostorTask)
		{
			GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
			if (data.IsImpostor)
			{
				PlayerControl.LocalPlayer.RemoveInfected();
				Overlay.enabled = false;
			}
			else
			{
				PlayerControl.LocalPlayer.RpcSetInfected(new GameData.PlayerInfo[1]
				{
					data
				});
				Overlay.enabled = true;
			}
			return;
		}
		if (OptionsTask)
        {
			OnOptionsTask.Invoke(this, null);
			return;
        }
		PlayerTask playerTask = FindTaskByType();
		if (!playerTask)
		{
			PlayerTask playerTask2 = Object.Instantiate(MyTask, PlayerControl.LocalPlayer.transform);
			playerTask2.Id = PlayerControl.LocalPlayer.TaskIdCount++;
			playerTask2.Owner = PlayerControl.LocalPlayer;
			playerTask2.Initialize();
			PlayerControl.LocalPlayer.myTasks.Add(playerTask2);
			GameData.Instance.TutOnlyAddTask(PlayerControl.LocalPlayer.PlayerId, playerTask2.Id);
			Overlay.sprite = ExImage;
			Overlay.enabled = true;
		}
		else
		{
			PlayerControl.LocalPlayer.RemoveTask(playerTask);
			Overlay.enabled = false;
		}
	}

	private PlayerTask FindTaskByType()
	{
		for (int num = PlayerControl.LocalPlayer.myTasks.Count - 1; num > -1; num--)
		{
			PlayerTask playerTask = PlayerControl.LocalPlayer.myTasks[num];
			if (playerTask.TaskType == MyTask.TaskType)
			{
				if (playerTask.TaskType == TaskTypes.DivertPower)
				{
					if (((DivertPowerTask)playerTask).TargetSystem == ((DivertPowerTask)MyTask).TargetSystem)
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
					if (playerTask.StartAt == MyTask.StartAt)
					{
						return playerTask;
					}
				}
			}
		}
		return null;
	}
}

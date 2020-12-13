using UnityEngine;

public class DialogueBox : MonoBehaviour
{
	public TextRenderer target;

	public void Show(string dialogue)
	{
		target.Text = dialogue;
		if ((bool)Minigame.Instance)
		{
			Minigame.Instance.Close();
			Minigame.Instance.Close();
		}
		PlayerControl.LocalPlayer.moveable = false;
		PlayerControl.LocalPlayer.NetTransform.Halt();
		base.gameObject.SetActive(value: true);
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
		PlayerControl.LocalPlayer.moveable = true;
		Camera.main.GetComponent<FollowerCamera>().Locked = false;
	}
}

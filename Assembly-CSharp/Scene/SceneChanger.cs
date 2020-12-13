using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
	public string TargetScene;

	public Button.ButtonClickedEvent BeforeSceneChange;

	public void Click()
	{
		BeforeSceneChange.Invoke();
		ChangeScene(TargetScene);
	}

	public static void ChangeScene(string target)
	{
		SceneManager.LoadScene(target);
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020000A9 RID: 169
public class HowToPlayController : MonoBehaviour
{
	// Token: 0x0600038A RID: 906 RVA: 0x00018124 File Offset: 0x00016324
	public void Start()
	{
		this.Scenes[2] = this.PCMove;
		this.PCMove.gameObject.SetActive(false);
		for (int i = 1; i < this.Scenes.Length; i++)
		{
			this.Scenes[i].gameObject.SetActive(false);
		}
		for (int j = 0; j < this.DotParent.childCount; j++)
		{
			this.DotParent.GetChild(j).localScale = Vector3.one;
		}
		this.ChangeScene(0);
	}

	// Token: 0x0600038B RID: 907 RVA: 0x000044CE File Offset: 0x000026CE
	public void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			this.Close();
		}
	}

	// Token: 0x0600038C RID: 908 RVA: 0x000044DF File Offset: 0x000026DF
	public void NextScene()
	{
		this.ChangeScene(1);
	}

	// Token: 0x0600038D RID: 909 RVA: 0x000044E8 File Offset: 0x000026E8
	public void PreviousScene()
	{
		this.ChangeScene(-1);
	}

	// Token: 0x0600038E RID: 910 RVA: 0x000044F1 File Offset: 0x000026F1
	public void Close()
	{
		SceneManager.LoadScene("MainMenu");
	}

	// Token: 0x0600038F RID: 911 RVA: 0x000181AC File Offset: 0x000163AC
	private void ChangeScene(int del)
	{
		this.Scenes[this.SceneNum].gameObject.SetActive(false);
		this.DotParent.GetChild(this.SceneNum).localScale = Vector3.one;
		this.SceneNum = Mathf.Clamp(this.SceneNum + del, 0, this.Scenes.Length - 1);
		this.Scenes[this.SceneNum].gameObject.SetActive(true);
		this.DotParent.GetChild(this.SceneNum).localScale = new Vector3(1.5f, 1.5f, 1.5f);
		this.leftButton.gameObject.SetActive(this.SceneNum > 0);
		this.rightButton.gameObject.SetActive(this.SceneNum < this.Scenes.Length - 1);
	}

	// Token: 0x04000375 RID: 885
	public Transform DotParent;

	// Token: 0x04000376 RID: 886
	public SpriteRenderer leftButton;

	// Token: 0x04000377 RID: 887
	public SpriteRenderer rightButton;

	// Token: 0x04000378 RID: 888
	public SceneController PCMove;

	// Token: 0x04000379 RID: 889
	public SceneController[] Scenes;

	// Token: 0x0400037A RID: 890
	public int SceneNum;
}

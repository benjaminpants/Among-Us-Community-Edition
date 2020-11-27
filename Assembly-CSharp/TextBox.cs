using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000108 RID: 264
public class TextBox : MonoBehaviour, IFocusHolder
{
	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x060005A5 RID: 1445 RVA: 0x0000590E File Offset: 0x00003B0E
	public float TextHeight
	{
		get
		{
			return this.outputText.Height;
		}
	}

	// Token: 0x060005A6 RID: 1446 RVA: 0x0000591B File Offset: 0x00003B1B
	public void Start()
	{
		this.colliders = base.GetComponents<Collider2D>();
		DestroyableSingleton<PassiveButtonManager>.Instance.RegisterOne(this);
		if (this.Pipe)
		{
			this.Pipe.enabled = false;
		}
	}

	// Token: 0x060005A7 RID: 1447 RVA: 0x0000594D File Offset: 0x00003B4D
	public void OnDestroy()
	{
		if (this.keyboard != null)
		{
			this.keyboard.active = false;
			this.keyboard = null;
		}
		if (DestroyableSingleton<PassiveButtonManager>.InstanceExists)
		{
			DestroyableSingleton<PassiveButtonManager>.Instance.RemoveOne(this);
		}
	}

	// Token: 0x060005A8 RID: 1448 RVA: 0x0000597C File Offset: 0x00003B7C
	public void Clear()
	{
		this.SetText(string.Empty, string.Empty);
	}

	// Token: 0x060005A9 RID: 1449 RVA: 0x000238F8 File Offset: 0x00021AF8
	public void Update()
	{
		if (!this.hasFocus)
		{
			return;
		}
		string inputString = Input.inputString;
		if (inputString.Length > 0 || this.compoText != Input.compositionString)
		{
			if (this.text == null || this.text == "Enter Name")
			{
				this.text = "";
			}
			this.SetText(this.text + inputString, Input.compositionString);
		}
		if (this.Pipe && this.hasFocus)
		{
			this.pipeBlinkTimer += Time.deltaTime * 2f;
			this.Pipe.enabled = ((int)this.pipeBlinkTimer % 2 == 0);
		}
	}

	// Token: 0x060005AA RID: 1450 RVA: 0x000239B0 File Offset: 0x00021BB0
	public void GiveFocus()
	{
		Input.imeCompositionMode = IMECompositionMode.On;
		if (this.hasFocus)
		{
			return;
		}
		if (this.ClearOnFocus)
		{
			this.text = string.Empty;
			this.compoText = string.Empty;
			this.outputText.Text = string.Empty;
		}
		this.hasFocus = true;
		if (TouchScreenKeyboard.isSupported)
		{
			this.keyboard = TouchScreenKeyboard.Open(this.text);
		}
		if (this.Background)
		{
			this.Background.color = Color.green;
		}
		this.pipeBlinkTimer = 0f;
		if (this.Pipe)
		{
			this.Pipe.transform.localPosition = this.outputText.CursorPos;
		}
	}

	// Token: 0x060005AB RID: 1451 RVA: 0x00023A6C File Offset: 0x00021C6C
	public void LoseFocus()
	{
		if (!this.hasFocus)
		{
			return;
		}
		Input.imeCompositionMode = IMECompositionMode.Off;
		if (this.compoText.Length > 0)
		{
			this.SetText(this.text + this.compoText, "");
			this.compoText = string.Empty;
		}
		this.hasFocus = false;
		if (this.keyboard != null)
		{
			this.keyboard.active = false;
			this.keyboard = null;
		}
		if (this.Background)
		{
			this.Background.color = Color.white;
		}
		if (this.Pipe)
		{
			this.Pipe.enabled = false;
		}
		this.OnFocusLost.Invoke();
	}

	// Token: 0x060005AC RID: 1452 RVA: 0x00023B20 File Offset: 0x00021D20
	public bool CheckCollision(Vector2 pt)
	{
		for (int i = 0; i < this.colliders.Length; i++)
		{
			if (this.colliders[i].OverlapPoint(pt))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060005AD RID: 1453 RVA: 0x00023B54 File Offset: 0x00021D54
	public void SetText(string input, string inputCompo = "")
	{
		bool flag = false;
		char c = ' ';
		this.tempTxt.Clear();
		for (int i = 0; i < input.Length; i++)
		{
			char c2 = input[i];
			if (c != ' ' || c2 != ' ')
			{
				if (c2 == '\r' || c2 == '\n')
				{
					flag = true;
				}
				if (c2 == '\b')
				{
					this.tempTxt.Length = Math.Max(this.tempTxt.Length - 1, 0);
				}
				if (this.ForceUppercase)
				{
					c2 = char.ToUpperInvariant(c2);
				}
				if (this.IsCharAllowed(c2))
				{
					this.tempTxt.Append(c2);
					c = c2;
				}
			}
		}
		if (this.characterLimit > 0)
		{
			this.tempTxt.Length = Math.Min(this.tempTxt.Length, this.characterLimit);
		}
		input = this.tempTxt.ToString();
		if (!input.Equals(this.text) || !inputCompo.Equals(this.compoText))
		{
			this.text = input;
			this.compoText = inputCompo;
			this.outputText.Text = this.text + "[FF0000FF]" + this.compoText + "[]";
			this.outputText.RefreshMesh();
			if (this.keyboard != null)
			{
				this.keyboard.text = this.text;
			}
			this.OnChange.Invoke();
		}
		if (flag)
		{
			this.OnEnter.Invoke();
		}
		if (this.Pipe)
		{
			this.Pipe.transform.localPosition = this.outputText.CursorPos;
		}
	}

	// Token: 0x060005AE RID: 1454 RVA: 0x00023CD8 File Offset: 0x00021ED8
	public bool IsCharAllowed(char i)
	{
		if (this.IpMode)
		{
			return (i >= '0' && i <= '9') || i == '.';
		}
		return i == ' ' || (i >= 'A' && i <= 'Z') || (i >= 'a' && i <= 'z') || (i >= '0' && i <= '9') || (i >= 'À' && i <= 'ÿ') || (i >= 'А' && i <= 'я') || (i >= 'ㄱ' && i <= 'ㆎ') || (i >= '가' && i <= '힣') || (this.AllowSymbols && TextBox.SymbolChars.Contains(i));
	}

	// Token: 0x04000573 RID: 1395
	public static readonly HashSet<char> SymbolChars = new HashSet<char>
	{
		'?',
		'!',
		',',
		'.',
		'\'',
		':',
		';',
		'(',
		')',
		'/',
		'\\',
		'%',
		'^',
		'&',
		'-',
		'='
	};

	// Token: 0x04000574 RID: 1396
	public string text;

	// Token: 0x04000575 RID: 1397
	private string compoText = "";

	// Token: 0x04000576 RID: 1398
	public int characterLimit = -1;

	// Token: 0x04000577 RID: 1399
	[SerializeField]
	private TextRenderer outputText;

	// Token: 0x04000578 RID: 1400
	public SpriteRenderer Background;

	// Token: 0x04000579 RID: 1401
	public MeshRenderer Pipe;

	// Token: 0x0400057A RID: 1402
	private float pipeBlinkTimer;

	// Token: 0x0400057B RID: 1403
	public bool ClearOnFocus;

	// Token: 0x0400057C RID: 1404
	public bool ForceUppercase;

	// Token: 0x0400057D RID: 1405
	public Button.ButtonClickedEvent OnEnter;

	// Token: 0x0400057E RID: 1406
	public Button.ButtonClickedEvent OnChange;

	// Token: 0x0400057F RID: 1407
	public Button.ButtonClickedEvent OnFocusLost;

	// Token: 0x04000580 RID: 1408
	private TouchScreenKeyboard keyboard;

	// Token: 0x04000581 RID: 1409
	public bool AllowSymbols;

	// Token: 0x04000582 RID: 1410
	public bool IpMode;

	// Token: 0x04000583 RID: 1411
	private Collider2D[] colliders;

	// Token: 0x04000584 RID: 1412
	private bool hasFocus;

	// Token: 0x04000585 RID: 1413
	private StringBuilder tempTxt = new StringBuilder();
}

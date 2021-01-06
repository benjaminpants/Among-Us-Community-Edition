using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TextBox : MonoBehaviour, IFocusHolder
{
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

	public string text;

	private string compoText = "";

	public int characterLimit = -1;

	[SerializeField]
	private TextRenderer outputText;

	public SpriteRenderer Background;

	public MeshRenderer Pipe;

	private float pipeBlinkTimer;

	public bool ClearOnFocus;

	public bool ForceUppercase;

	public Button.ButtonClickedEvent OnEnter;

	public Button.ButtonClickedEvent OnChange;

	public Button.ButtonClickedEvent OnFocusLost;

	private TouchScreenKeyboard keyboard;

	public bool AllowSymbols;

	public bool IpMode;

	private Collider2D[] colliders;

	private bool hasFocus;

	private StringBuilder tempTxt = new StringBuilder();

	public float TextHeight => outputText.Height;

	public void Start()
	{
		colliders = GetComponents<Collider2D>();
		DestroyableSingleton<PassiveButtonManager>.Instance.RegisterOne(this);
		if ((bool)Pipe)
		{
			Pipe.enabled = false;
		}
	}

	public void OnDestroy()
	{
		if (keyboard != null)
		{
			keyboard.active = false;
			keyboard = null;
		}
		if (DestroyableSingleton<PassiveButtonManager>.InstanceExists)
		{
			DestroyableSingleton<PassiveButtonManager>.Instance.RemoveOne(this);
		}
	}

	public void Clear()
	{
		SetText(string.Empty, string.Empty);
	}

	public void Update()
	{
		if (!hasFocus)
		{
			return;
		}

		string inputString = Input.inputString;
		if (Input.GetKey(KeyCode.LeftControl) && CE_Input.CE_GetKeyDown(KeyCode.V))
		{
			inputString += GUIUtility.systemCopyBuffer;
		}
		if (inputString.Length > 0 || compoText != Input.compositionString)
		{
			if (text == null || text == "Enter Name")
			{
				text = "";
			}
			SetText(text + inputString, Input.compositionString);
		}
		if ((bool)Pipe && hasFocus)
		{
			pipeBlinkTimer += Time.deltaTime * 2f;
			Pipe.enabled = (int)pipeBlinkTimer % 2 == 0;
		}
	}

	public void GiveFocus()
	{
		Input.imeCompositionMode = IMECompositionMode.On;
		if (!hasFocus)
		{
			if (ClearOnFocus)
			{
				text = string.Empty;
				compoText = string.Empty;
				outputText.Text = string.Empty;
			}
			hasFocus = true;
			if (TouchScreenKeyboard.isSupported)
			{
				keyboard = TouchScreenKeyboard.Open(text);
			}
			if ((bool)Background)
			{
				Background.color = Color.green;
			}
			pipeBlinkTimer = 0f;
			if ((bool)Pipe)
			{
				Pipe.transform.localPosition = outputText.CursorPos;
			}
		}
	}

	public void LoseFocus()
	{
		if (hasFocus)
		{
			Input.imeCompositionMode = IMECompositionMode.Off;
			if (compoText.Length > 0)
			{
				SetText(text + compoText);
				compoText = string.Empty;
			}
			hasFocus = false;
			if (keyboard != null)
			{
				keyboard.active = false;
				keyboard = null;
			}
			if ((bool)Background)
			{
				Background.color = Color.white;
			}
			if ((bool)Pipe)
			{
				Pipe.enabled = false;
			}
			OnFocusLost.Invoke();
		}
	}

	public bool CheckCollision(Vector2 pt)
	{
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].OverlapPoint(pt))
			{
				return true;
			}
		}
		return false;
	}

	public void SetText(string input, string inputCompo = "")
	{
		bool flag = false;
		char c = ' ';
		tempTxt.Clear();
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
					tempTxt.Length = Math.Max(tempTxt.Length - 1, 0);
				}
				if (ForceUppercase)
				{
					c2 = char.ToUpperInvariant(c2);
				}
				if (IsCharAllowed(c2))
				{
					tempTxt.Append(c2);
					c = c2;
				}
			}
		}
		if (characterLimit > 0)
		{
			tempTxt.Length = Math.Min(tempTxt.Length, characterLimit);
		}
		input = tempTxt.ToString();
		if (!input.Equals(text) || !inputCompo.Equals(compoText))
		{
			text = input;
			compoText = inputCompo;
			outputText.Text = text + "[FF0000FF]" + compoText + "[]";
			outputText.RefreshMesh();
			if (keyboard != null)
			{
				keyboard.text = text;
			}
			OnChange.Invoke();
		}
		if (flag)
		{
			OnEnter.Invoke();
		}
		if ((bool)Pipe)
		{
			Pipe.transform.localPosition = outputText.CursorPos;
		}
	}

	public bool IsCharAllowed(char i)
	{
		if (IpMode)
		{
			if (i >= '0' && i <= '9')
			{
				return true;
			}
			if (i == '.')
			{
				return true;
			}
			return false;
		}
		switch (i)
		{
		case ' ':
			return true;
		case 'A':
		case 'B':
		case 'C':
		case 'D':
		case 'E':
		case 'F':
		case 'G':
		case 'H':
		case 'I':
		case 'J':
		case 'K':
		case 'L':
		case 'M':
		case 'N':
		case 'O':
		case 'P':
		case 'Q':
		case 'R':
		case 'S':
		case 'T':
		case 'U':
		case 'V':
		case 'W':
		case 'X':
		case 'Y':
		case 'Z':
			return true;
		default:
			if (i >= 'a' && i <= 'z')
			{
				return true;
			}
			if (i >= '0' && i <= '9')
			{
				return true;
			}
			if (i >= 'À' && i <= 'ÿ')
			{
				return true;
			}
			if (i >= 'А' && i <= 'я')
			{
				return true;
			}
			if (i >= 'ㄱ' && i <= 'ㆎ')
			{
				return true;
			}
			if (i >= '가' && i <= '힣')
			{
				return true;
			}
			if (AllowSymbols && SymbolChars.Contains(i))
			{
				return true;
			}
			return false;
		}
	}
}

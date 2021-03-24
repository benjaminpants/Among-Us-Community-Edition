using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class TextRenderer : MonoBehaviour
{
	public TextAsset FontData;

	public float scale = 1f;

	public float TabWidth = 0.5f;

	public bool Centered;

	public bool RightAligned;

	public TextLink textLinkPrefab;

	[HideInInspector]
	private Mesh mesh;

	[HideInInspector]
	private MeshRenderer render;

	[Multiline]
	public string Text;

	private string lastText;

    private Color[] topcolors = new Color[]
    {
        Color.yellow,
		Color.white,
		Color.white,
		Color.white,
		Color.blue,
		Color.white,
		Color.red,
		Color.black,
		Color.red,
		Color.cyan
	};

    private Color[] bottomcolors = new Color[]
    {
        Color.red,
		Color.yellow,
		Color.red,
		Color.gray,
		Color.blue,
		Color.white,
		Color.black,
		Color.black,
		Color.blue,
		Color.green
	};

    private Color[] righttopcolors = new Color[]
    {
        Color.green,
		Color.white,
		Color.white,
		Color.gray,
		Color.red,
		Color.white,
		Color.red,
		Color.black,
		Color.white,
		Color.blue
	};

	private Color[] rightbotcolors = new Color[]
	{
		Color.blue,
		Color.yellow,
		Color.red,
		Color.black,
		Color.red,
		Color.magenta,
		Color.black,
		Color.magenta,
		Color.blue,
		Color.green
	};

	public Color Color = Color.white;

	private Color lastColor = Color.white;

	public Color OutlineColor = Color.black;

	private Color lastOutlineColor = Color.white;

	public float maxWidth = -1f;

	private Vector2 cursorLocation;

	public float Width
	{
		get;
		private set;
	}

	public float Height
	{
		get;
		private set;
	}

	public Vector3 CursorPos => new Vector3(cursorLocation.x / 100f * scale, cursorLocation.y / 100f * scale, -0.001f);

	public void Start()
	{
		render = GetComponent<MeshRenderer>();
		MeshFilter component = GetComponent<MeshFilter>();
		if (!component.mesh)
		{
			mesh = new Mesh();
			mesh.name = "Text" + base.name;
			component.mesh = mesh;
			render.material.SetColor("_OutlineColor", OutlineColor);
		}
		else
		{
			mesh = component.mesh;
		}
	}

	[ContextMenu("Generate Mesh")]
	public void GenerateMesh()
	{
		render = GetComponent<MeshRenderer>();
		MeshFilter component = GetComponent<MeshFilter>();
		if (!component.sharedMesh)
		{
			mesh = new Mesh();
			mesh.name = "Text" + base.name;
			component.mesh = mesh;
		}
		else
		{
			mesh = component.sharedMesh;
		}
		lastText = null;
		lastOutlineColor = OutlineColor;
		Update();
	}

	private void Update()
	{
		if (lastOutlineColor != OutlineColor)
		{
			lastOutlineColor = OutlineColor;
			render.material.SetColor("_OutlineColor", OutlineColor);
		}
		if (lastText != Text || lastColor != Color)
		{
			RefreshMesh();
		}
	}

	public void RefreshMesh()
	{
		if (render == null)
		{
			Start();
		}
		if (Text != null && Text.Any((char c) => c > '✐'))
		{
			FontCache.Instance.SetFont(this, "Korean");
		}
		FontData fontData = FontCache.Instance.LoadFont(FontData);
		lastText = Text;
		lastColor = Color;
		if (maxWidth > 0f)
		{
			lastText = (Text = WrapText(fontData, lastText, maxWidth));
		}
		List<Vector3> list = new List<Vector3>(lastText.Length * 4);
		List<Vector2> list2 = new List<Vector2>(lastText.Length * 4);
		List<Vector4> list3 = new List<Vector4>(lastText.Length * 4);
		List<Color> list4 = new List<Color>(lastText.Length * 4);
		int[] array = new int[lastText.Length * 6];
		Width = 0f;
		cursorLocation.x = (cursorLocation.y = 0f);
		char previous_char = '\0';
		Vector2 from = default(Vector2);
		string text = null;
		int lineStart = 0;
		int num2 = 0;
        Color item = Color;
        Color item_2 = Color;
        Color item_3 = Color;
		Color item_4 = Color;
		int? num3 = null;
		for (int i = 0; i < lastText.Length; i++)
		{
			char current_char = lastText[i];
			if (current_char == '[')
			{
				num3 = 0;
				previous_char = current_char;
				continue;
			}
			if (num3.HasValue)
			{
				switch (current_char)
				{
				case ']':
					if (previous_char != '[')
					{
						item = new Color32((byte)((num3 >> 24) & 0xFF).Value, (byte)((num3 >> 16) & 0xFF).Value, (byte)((num3 >> 8) & 0xFF).Value, (byte)(num3 & 0xFF).Value);
                        item.a *= Color.a;
						item_2 = new Color32((byte)((num3 >> 24) & 0xFF).Value, (byte)((num3 >> 16) & 0xFF).Value, (byte)((num3 >> 8) & 0xFF).Value, (byte)(num3 & 0xFF).Value);
                        item_2.a *= Color.a;
						item_3 = new Color32((byte)((num3 >> 24) & 0xFF).Value, (byte)((num3 >> 16) & 0xFF).Value, (byte)((num3 >> 8) & 0xFF).Value, (byte)(num3 & 0xFF).Value);
						item_3.a *= Color.a;
						item_4 = new Color32((byte)((num3 >> 24) & 0xFF).Value, (byte)((num3 >> 16) & 0xFF).Value, (byte)((num3 >> 8) & 0xFF).Value, (byte)(num3 & 0xFF).Value);
						item_4.a *= Color.a;
					}
					else
					{
                        item = Color;
                        item_2 = Color;
                        item_3 = Color;
						item_4 = Color;
					}
					previous_char = '\0';
					num3 = null;
					if (text != null)
					{
						TextLink textLink = Object.Instantiate(textLinkPrefab, base.transform);
						textLink.transform.localScale = Vector3.one;
						Vector3 v = list.Last();
						textLink.Set(from, v, text);
						text = null;
					}
					break;
				case 'h':
				{
					int num5 = lastText.IndexOf(']', i);
					text = lastText.Substring(i, num5 - i);
					from = list[list.Count - 2];
                    item = new Color(0.5f, 0.5f, 1f);
                    item_2 = new Color(0.5f, 0.5f, 1f);
                    item_3 = new Color(0.5f, 0.5f, 1f);
					item_4 = new Color(0.5f, 0.5f, 1f);
					previous_char = '\0';
					num3 = null;
					i = num5; //skips to the next bracket
					break;
				}
				case 'r': //attempt at custom text thing
				{
                    int num5 = lastText.IndexOf(']', i);
					int id = int.Parse(lastText[i + 1].ToString());
					item = bottomcolors[id];
					item.a *= Color.a;
					item_2 = topcolors[id];
					item_2.a *= Color.a;
					item_3 = righttopcolors[id];
					item_3.a *= Color.a;
					item_4 = rightbotcolors[id];
					item_4.a *= Color.a;
					previous_char = '\0';
					num3 = null;
					//Debug.Log("found and processed custom text thing!");
					i = num5;
					break;
				}
				default:
					num3 = (num3 << 4) | CharToInt(current_char);
					break;
				}
				previous_char = current_char;
				continue;
			}
			switch (current_char)
			{
			case '\n':
				if (Centered)
				{
					CenterVerts(list, cursorLocation.x, lineStart);
				}
				else if (RightAligned)
				{
					RightAlignVerts(list, cursorLocation.x, lineStart);
				}
				cursorLocation.x = 0f;
				cursorLocation.y -= fontData.LineHeight;
				lineStart = list.Count;
				continue;
			case '	': //possiblty a tab character?
			{
				float num6 = cursorLocation.x / 100f;
				num6 = Mathf.Ceil(num6 / TabWidth) * TabWidth;
				cursorLocation.x = num6 * 100f;
				continue;
			}
			case (char)13: //cant figure out this character
				continue;
			}
			if (!fontData.charMap.TryGetValue(current_char, out var value))
			{
				Debug.Log("Missing char :" + current_char);
				current_char = '\0'; //please tell me this doesn't break anything
				value = fontData.charMap[-1];
			}
			Vector4 vector = fontData.bounds[value];
			Vector2 textureSize = fontData.TextureSize;
			Vector3 vector2 = fontData.offsets[value];
			float kerning = fontData.GetKerning(previous_char, current_char);
			float num7 = cursorLocation.x + vector2.x + kerning;
			float num8 = cursorLocation.y - vector2.y;
			list.Add(new Vector3(num7, num8 - vector.w) / 100f * scale);
			list.Add(new Vector3(num7, num8) / 100f * scale);
			list.Add(new Vector3(num7 + vector.z, num8) / 100f * scale);
			list.Add(new Vector3(num7 + vector.z, num8 - vector.w) / 100f * scale);
			list4.Add(item);
			list4.Add(item_2);
			list4.Add(item_3);
			list4.Add(item_4);
			list2.Add(new Vector2(vector.x / textureSize.x, 1f - (vector.y + vector.w) / textureSize.y));
			list2.Add(new Vector2(vector.x / textureSize.x, 1f - vector.y / textureSize.y));
			list2.Add(new Vector2((vector.x + vector.z) / textureSize.x, 1f - vector.y / textureSize.y));
			list2.Add(new Vector2((vector.x + vector.z) / textureSize.x, 1f - (vector.y + vector.w) / textureSize.y));
			Vector4 item2 = fontData.Channels[value];
			list3.Add(item2);
			list3.Add(item2);
			list3.Add(item2);
			list3.Add(item2);
			array[num2 * 6] = num2 * 4;
			array[num2 * 6 + 1] = num2 * 4 + 1;
			array[num2 * 6 + 2] = num2 * 4 + 2;
			array[num2 * 6 + 3] = num2 * 4;
			array[num2 * 6 + 4] = num2 * 4 + 2;
			array[num2 * 6 + 5] = num2 * 4 + 3;
			cursorLocation.x += vector2.z + kerning;
			float num9 = cursorLocation.x / 100f * scale;
			if (Width < num9)
			{
				Width = num9;
			}
			previous_char = current_char;
			num2++;
		}
		if (Centered)
		{
			CenterVerts(list, cursorLocation.x, lineStart);
			cursorLocation.x /= 2f;
			Width /= 2f;
		}
		else if (RightAligned)
		{
			RightAlignVerts(list, cursorLocation.x, lineStart);
		}
		Height = (0f - (cursorLocation.y - fontData.LineHeight)) / 100f * scale;
		mesh.Clear();
		if (list.Count > 0)
		{
			mesh.SetVertices(list);
			mesh.SetColors(list4);
			mesh.SetUVs(0, list2);
			mesh.SetUVs(1, list3);
			mesh.SetIndices(array, MeshTopology.Triangles, 0);
		}
	}

	private void RightAlignVerts(List<Vector3> verts, float baseX, int lineStart)
	{
		for (int i = lineStart; i < verts.Count; i++)
		{
			Vector3 value = verts[i];
			value.x -= baseX / 100f * scale;
			verts[i] = value;
		}
	}

	private void CenterVerts(List<Vector3> verts, float baseX, int lineStart)
	{
		for (int i = lineStart; i < verts.Count; i++)
		{
			Vector3 value = verts[i];
			value.x -= baseX / 200f * scale;
			verts[i] = value;
		}
	}

	private int CharToInt(int c)
	{
		if (c < 65)
		{
			return c - 48;
		}
		if (c < 97)
		{
			return 10 + (c - 65);
		}
		return 10 + (c - 97);
	}

	public static string WrapText(FontData data, string displayTxt, float maxWidth)
	{
		float num = 0f;
		int num2 = -1;
		int last = -1;
		bool flag = false;
		int num3 = 0;
		for (int i = 0; i < displayTxt.Length && num3++ <= 1000; i++)
		{
			int num4 = displayTxt[i];
			switch (num4)
			{
			case 91:
				flag = true;
				break;
			case 93:
				flag = false;
				continue;
			}
			if (flag)
			{
				continue;
			}
			switch (num4)
			{
			case 10:
				num2 = -1;
				last = -1;
				num = 0f;
				continue;
			case 9:
				num = Mathf.Ceil(num / 100f / 0.5f) * 0.5f * 100f;
				continue;
			case 13:
				continue;
			}
			if (!data.charMap.TryGetValue(num4, out var value))
			{
				Debug.Log("Missing char :" + num4);
				num4 = -1;
				value = data.charMap[-1];
			}
			if (num4 == 32)
			{
				num2 = i;
			}
			num += data.offsets[value].z + data.GetKerning(last, num4);
			if (num > maxWidth * 100f)
			{
				if (num2 != -1)
				{
					displayTxt = displayTxt.Substring(0, num2) + "\n" + displayTxt.Substring(num2 + 1);
					i = num2;
				}
				else
				{
					displayTxt = displayTxt.Substring(0, i) + "\n" + displayTxt.Substring(i);
				}
				num2 = -1;
				last = -1;
				num = 0f;
			}
			last = num4;
		}
		return displayTxt;
	}
}

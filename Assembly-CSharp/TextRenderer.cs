using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200005C RID: 92
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class TextRenderer : MonoBehaviour
{
	// Token: 0x1700005F RID: 95
	// (get) Token: 0x060001F5 RID: 501 RVA: 0x000033F6 File Offset: 0x000015F6
	// (set) Token: 0x060001F6 RID: 502 RVA: 0x000033FE File Offset: 0x000015FE
	public float Width { get; private set; }

	// Token: 0x17000060 RID: 96
	// (get) Token: 0x060001F7 RID: 503 RVA: 0x00003407 File Offset: 0x00001607
	// (set) Token: 0x060001F8 RID: 504 RVA: 0x0000340F File Offset: 0x0000160F
	public float Height { get; private set; }

	// Token: 0x17000061 RID: 97
	// (get) Token: 0x060001F9 RID: 505 RVA: 0x00003418 File Offset: 0x00001618
	public Vector3 CursorPos
	{
		get
		{
			return new Vector3(this.cursorLocation.x / 100f * this.scale, this.cursorLocation.y / 100f * this.scale, -0.001f);
		}
	}

	// Token: 0x060001FA RID: 506 RVA: 0x00011260 File Offset: 0x0000F460
	public void Start()
	{
		this.render = base.GetComponent<MeshRenderer>();
		MeshFilter component = base.GetComponent<MeshFilter>();
		if (!component.mesh)
		{
			this.mesh = new Mesh();
			this.mesh.name = "Text" + base.name;
			component.mesh = this.mesh;
			this.render.material.SetColor("_OutlineColor", this.OutlineColor);
			return;
		}
		this.mesh = component.mesh;
	}

	// Token: 0x060001FB RID: 507 RVA: 0x000112E8 File Offset: 0x0000F4E8
	[ContextMenu("Generate Mesh")]
	public void GenerateMesh()
	{
		this.render = base.GetComponent<MeshRenderer>();
		MeshFilter component = base.GetComponent<MeshFilter>();
		if (!component.sharedMesh)
		{
			this.mesh = new Mesh();
			this.mesh.name = "Text" + base.name;
			component.mesh = this.mesh;
		}
		else
		{
			this.mesh = component.sharedMesh;
		}
		this.lastText = null;
		this.lastOutlineColor = this.OutlineColor;
		this.Update();
	}

	// Token: 0x060001FC RID: 508 RVA: 0x00011370 File Offset: 0x0000F570
	private void Update()
	{
		if (this.lastOutlineColor != this.OutlineColor)
		{
			this.lastOutlineColor = this.OutlineColor;
			this.render.material.SetColor("_OutlineColor", this.OutlineColor);
		}
		if (this.lastText != this.Text || this.lastColor != this.Color)
		{
			this.RefreshMesh();
		}
	}

	// Token: 0x060001FD RID: 509 RVA: 0x000113E4 File Offset: 0x0000F5E4
	public void RefreshMesh()
	{
		if (this.render == null)
		{
			this.Start();
		}
		if (this.Text != null)
		{
			if (this.Text.Any((char c) => c > '✐'))
			{
				FontCache.Instance.SetFont(this, "Korean");
			}
		}
		FontData fontData = FontCache.Instance.LoadFont(this.FontData);
		this.lastText = this.Text;
		this.lastColor = this.Color;
		if (this.maxWidth > 0f)
		{
			this.lastText = (this.Text = TextRenderer.WrapText(fontData, this.lastText, this.maxWidth));
		}
		List<Vector3> list = new List<Vector3>(this.lastText.Length * 4);
		List<Vector2> list2 = new List<Vector2>(this.lastText.Length * 4);
		List<Vector4> list3 = new List<Vector4>(this.lastText.Length * 4);
		List<Color> list4 = new List<Color>(this.lastText.Length * 4);
		int[] array = new int[this.lastText.Length * 6];
		this.Width = 0f;
		this.cursorLocation.x = (this.cursorLocation.y = 0f);
		int num = -1;
		Vector2 from = default(Vector2);
		string text = null;
		int lineStart = 0;
		int num2 = 0;
		Color item = this.Color;
		int? num3 = null;
		for (int i = 0; i < this.lastText.Length; i++)
		{
			int num4 = (int)this.lastText[i];
			if (num4 == 91)
			{
				num3 = new int?(0);
				num = num4;
			}
			else if (num3 != null)
			{
				if (num4 == 93)
				{
					if (num != 91)
					{
						int? num5 = num3;
						byte r = (byte)((num5 != null) ? new int?(num5.GetValueOrDefault() >> 24 & 255) : null).Value;
						int? num6 = num3;
						byte g = (byte)((num6 != null) ? new int?(num6.GetValueOrDefault() >> 16 & 255) : null).Value;
						int? num7 = num3;
						item = new Color32(r, g, (byte)((num7 != null) ? new int?(num7.GetValueOrDefault() >> 8 & 255) : null).Value, (byte)(num3 & 255).Value);
						item.a *= this.Color.a;
					}
					else
					{
						item = this.Color;
					}
					num3 = null;
					if (text != null)
					{
						TextLink textLink = UnityEngine.Object.Instantiate<TextLink>(this.textLinkPrefab, base.transform);
						textLink.transform.localScale = Vector3.one;
						Vector3 v = list.Last<Vector3>();
						textLink.Set(from, v, text);
						text = null;
					}
				}
				else if (num4 == 104)
				{
					int num8 = this.lastText.IndexOf(']', i);
					text = this.lastText.Substring(i, num8 - i);
					from = list[list.Count - 2];
					item = new Color(0.5f, 0.5f, 1f);
					num3 = null;
					i = num8;
				}
				else
				{
					num3 = (num3 << 4 | this.CharToInt(num4));
				}
				num = num4;
			}
			else if (num4 != 13)
			{
				if (num4 == 10)
				{
					if (this.Centered)
					{
						this.CenterVerts(list, this.cursorLocation.x, lineStart);
					}
					else if (this.RightAligned)
					{
						this.RightAlignVerts(list, this.cursorLocation.x, lineStart);
					}
					this.cursorLocation.x = 0f;
					this.cursorLocation.y = this.cursorLocation.y - fontData.LineHeight;
					lineStart = list.Count;
				}
				else if (num4 == 9)
				{
					float num9 = this.cursorLocation.x / 100f;
					num9 = Mathf.Ceil(num9 / this.TabWidth) * this.TabWidth;
					this.cursorLocation.x = num9 * 100f;
				}
				else
				{
					int index;
					if (!fontData.charMap.TryGetValue(num4, out index))
					{
						Debug.Log("Missing char :" + num4);
						num4 = -1;
						index = fontData.charMap[-1];
					}
					Vector4 vector = fontData.bounds[index];
					Vector2 textureSize = fontData.TextureSize;
					Vector3 vector2 = fontData.offsets[index];
					float kerning = fontData.GetKerning(num, num4);
					float num10 = this.cursorLocation.x + vector2.x + kerning;
					float num11 = this.cursorLocation.y - vector2.y;
					list.Add(new Vector3(num10, num11 - vector.w) / 100f * this.scale);
					list.Add(new Vector3(num10, num11) / 100f * this.scale);
					list.Add(new Vector3(num10 + vector.z, num11) / 100f * this.scale);
					list.Add(new Vector3(num10 + vector.z, num11 - vector.w) / 100f * this.scale);
					list4.Add(item);
					list4.Add(item);
					list4.Add(item);
					list4.Add(item);
					list2.Add(new Vector2(vector.x / textureSize.x, 1f - (vector.y + vector.w) / textureSize.y));
					list2.Add(new Vector2(vector.x / textureSize.x, 1f - vector.y / textureSize.y));
					list2.Add(new Vector2((vector.x + vector.z) / textureSize.x, 1f - vector.y / textureSize.y));
					list2.Add(new Vector2((vector.x + vector.z) / textureSize.x, 1f - (vector.y + vector.w) / textureSize.y));
					Vector4 item2 = fontData.Channels[index];
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
					this.cursorLocation.x = this.cursorLocation.x + (vector2.z + kerning);
					float num12 = this.cursorLocation.x / 100f * this.scale;
					if (this.Width < num12)
					{
						this.Width = num12;
					}
					num = num4;
					num2++;
				}
			}
		}
		if (this.Centered)
		{
			this.CenterVerts(list, this.cursorLocation.x, lineStart);
			this.cursorLocation.x = this.cursorLocation.x / 2f;
			this.Width /= 2f;
		}
		else if (this.RightAligned)
		{
			this.RightAlignVerts(list, this.cursorLocation.x, lineStart);
		}
		this.Height = -(this.cursorLocation.y - fontData.LineHeight) / 100f * this.scale;
		this.mesh.Clear();
		if (list.Count > 0)
		{
			this.mesh.SetVertices(list);
			this.mesh.SetColors(list4);
			this.mesh.SetUVs(0, list2);
			this.mesh.SetUVs(1, list3);
			this.mesh.SetIndices(array, MeshTopology.Triangles, 0);
		}
	}

	// Token: 0x060001FE RID: 510 RVA: 0x00011C94 File Offset: 0x0000FE94
	private void RightAlignVerts(List<Vector3> verts, float baseX, int lineStart)
	{
		for (int i = lineStart; i < verts.Count; i++)
		{
			Vector3 value = verts[i];
			value.x -= baseX / 100f * this.scale;
			verts[i] = value;
		}
	}

	// Token: 0x060001FF RID: 511 RVA: 0x00011CDC File Offset: 0x0000FEDC
	private void CenterVerts(List<Vector3> verts, float baseX, int lineStart)
	{
		for (int i = lineStart; i < verts.Count; i++)
		{
			Vector3 value = verts[i];
			value.x -= baseX / 200f * this.scale;
			verts[i] = value;
		}
	}

	// Token: 0x06000200 RID: 512 RVA: 0x00011D24 File Offset: 0x0000FF24
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

	// Token: 0x06000201 RID: 513 RVA: 0x00011D54 File Offset: 0x0000FF54
	public static string WrapText(FontData data, string displayTxt, float maxWidth)
	{
		float num = 0f;
		int num2 = -1;
		int last = -1;
		bool flag = false;
		int num3 = 0;
		int num4 = 0;
		while (num4 < displayTxt.Length && num3++ <= 1000)
		{
			int num5 = (int)displayTxt[num4];
			if (num5 == 91)
			{
				flag = true;
				goto IL_49;
			}
			if (num5 != 93)
			{
				goto IL_49;
			}
			flag = false;
			IL_155:
			num4++;
			continue;
			IL_49:
			if (flag || num5 == 13)
			{
				goto IL_155;
			}
			if (num5 == 10)
			{
				num2 = -1;
				last = -1;
				num = 0f;
				goto IL_155;
			}
			if (num5 == 9)
			{
				num = Mathf.Ceil(num / 100f / 0.5f) * 0.5f * 100f;
				goto IL_155;
			}
			int index;
			if (!data.charMap.TryGetValue(num5, out index))
			{
				Debug.Log("Missing char :" + num5);
				num5 = -1;
				index = data.charMap[-1];
			}
			if (num5 == 32)
			{
				num2 = num4;
			}
			Vector3 vector = data.offsets[index];
			num += vector.z + data.GetKerning(last, num5);
			if (num > maxWidth * 100f)
			{
				if (num2 != -1)
				{
					displayTxt = displayTxt.Substring(0, num2) + "\n" + displayTxt.Substring(num2 + 1);
					num4 = num2;
				}
				else
				{
					displayTxt = displayTxt.Substring(0, num4) + "\n" + displayTxt.Substring(num4);
				}
				num2 = -1;
				num = 0f;
			}
			last = num5;
			goto IL_155;
		}
		return displayTxt;
	}

	// Token: 0x040001E3 RID: 483
	public TextAsset FontData;

	// Token: 0x040001E4 RID: 484
	public float scale = 1f;

	// Token: 0x040001E5 RID: 485
	public float TabWidth = 0.5f;

	// Token: 0x040001E6 RID: 486
	public bool Centered;

	// Token: 0x040001E7 RID: 487
	public bool RightAligned;

	// Token: 0x040001E8 RID: 488
	public TextLink textLinkPrefab;

	// Token: 0x040001E9 RID: 489
	[HideInInspector]
	private Mesh mesh;

	// Token: 0x040001EA RID: 490
	[HideInInspector]
	private MeshRenderer render;

	// Token: 0x040001EB RID: 491
	[Multiline]
	public string Text;

	// Token: 0x040001EC RID: 492
	private string lastText;

	// Token: 0x040001ED RID: 493
	public Color Color = Color.white;

	// Token: 0x040001EE RID: 494
	private Color lastColor = Color.white;

	// Token: 0x040001EF RID: 495
	public Color OutlineColor = Color.black;

	// Token: 0x040001F0 RID: 496
	private Color lastOutlineColor = Color.white;

	// Token: 0x040001F1 RID: 497
	public float maxWidth = -1f;

	// Token: 0x040001F4 RID: 500
	private Vector2 cursorLocation;
}

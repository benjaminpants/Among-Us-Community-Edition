using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TextController : MonoBehaviour
{
	public float Scale = 1f;

	[Multiline]
	public string Text;

	private string displaying;

	[HideInInspector]
	private Texture2D texture;

	[HideInInspector]
	private Texture2D colorTexture;

	private MeshRenderer rend;

	private float _scale = float.NegativeInfinity;

	public Color Color = Color.white;

	private Color lastColor;

	public Vector3 Offset;

	public bool topAligned;

	public void Update()
	{
		if (!rend)
		{
			rend = GetComponent<MeshRenderer>();
		}
		if (string.IsNullOrEmpty(Text))
		{
			rend.enabled = false;
			return;
		}
		if (displaying == null || displaying.GetHashCode() != Text.GetHashCode() || Color != lastColor)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 1;
			for (int i = 0; i < Text.Length; i++)
			{
				if (Text[i] == '\n')
				{
					num2 = 0;
					num3++;
					continue;
				}
				num2++;
				if (num2 > num)
				{
					num = num2;
				}
			}
			if (!texture || !colorTexture)
			{
				if (!texture)
				{
					texture = new Texture2D(num, num3, TextureFormat.ARGB32, mipChain: false);
					texture.filterMode = FilterMode.Point;
					texture.wrapMode = TextureWrapMode.Clamp;
				}
				if (!colorTexture)
				{
					colorTexture = new Texture2D(num, num3, TextureFormat.ARGB32, mipChain: false);
					colorTexture.filterMode = FilterMode.Point;
					colorTexture.wrapMode = TextureWrapMode.Clamp;
				}
			}
			else if (texture.width != num || texture.height != num3)
			{
				texture.Resize(num, num3, TextureFormat.ARGB32, hasMipMap: false);
				colorTexture.Resize(num, num3, TextureFormat.ARGB32, hasMipMap: false);
			}
			Color[] array = new Color[num * num3];
			array.SetAll(Color);
			colorTexture.SetPixels(array);
			array.SetAll(new Color(0.125f, 0f, 0f));
			texture.SetPixels(array);
			int num4 = 0;
			int num5 = texture.height - 1;
			Color color = Color;
			for (int j = 0; j < Text.Length; j++)
			{
				char c = Text[j];
				switch (c)
				{
				case '\n':
					num4 = 0;
					num5--;
					break;
				default:
					texture.SetPixel(num4, num5, new Color((float)(int)c / 256f, 0f, 0f));
					colorTexture.SetPixel(num4, num5, color);
					num4++;
					break;
				case '\r':
					break;
				}
			}
			texture.Apply(updateMipmaps: false, makeNoLongerReadable: false);
			colorTexture.Apply(updateMipmaps: false, makeNoLongerReadable: false);
			rend.enabled = true;
			rend.material.SetTexture("_InputTex", texture);
			rend.material.SetTexture("_ColorTex", colorTexture);
			_scale = float.NegativeInfinity;
			displaying = Text;
			lastColor = Color;
		}
		if (_scale != Scale)
		{
			_scale = Scale;
			base.transform.localScale = new Vector3(texture.width, texture.height, 1f) * Scale;
			if (topAligned)
			{
				base.transform.localPosition = Offset + new Vector3((float)texture.width * Scale / 2f, (float)(-texture.height) * Scale / 2f, 0f);
			}
		}
	}
}

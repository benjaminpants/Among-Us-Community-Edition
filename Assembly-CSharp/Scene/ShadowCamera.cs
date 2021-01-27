using UnityEngine;

public class ShadowCamera : MonoBehaviour
{
	public Shader Shadozer;

	public bool updated;

	private int LastRecordedRes;

	private int LastRecordedBright;

	public void OnEnable()
	{
        GetComponent<Camera>().SetReplacementShader(Shadozer, "RenderType");
	}


	private int GetBiggerScreenDimension()
    {
		return Screen.width >= Screen.height ? Screen.width : Screen.height;
    }

	public void Update()
    {
		if (SaveManager.UseHDSHadows || true)
		{
			if (LastRecordedRes != GetBiggerScreenDimension() || PlayerControl.GameOptions.Brightness != LastRecordedBright)
			{
				updated = false;
			}
			if ((bool)GetComponent<Camera>().activeTexture && !updated)
			{
				updated = true;
				int Res = GetBiggerScreenDimension();
				LastRecordedRes = Res;
				RenderTexture Tex = new RenderTexture(Res, Res, 16);
				GetComponent<Camera>().targetTexture.Release();
				GetComponent<Camera>().targetTexture = Tex;
				GetComponent<Camera>().SetReplacementShader(Shadozer, "RenderType");
				GameObject shadquad = GameObject.Find("ShadowQuad");
                shadquad.GetComponent<MeshRenderer>().material.mainTexture = Tex;
				LastRecordedBright = PlayerControl.GameOptions.Brightness;
				shadquad.GetComponent<MeshRenderer>().material.color = new Color32(PlayerControl.GameOptions.Brightness, PlayerControl.GameOptions.Brightness, PlayerControl.GameOptions.Brightness, 255);
			}
		}
	}

	public void OnDisable()
	{
		GetComponent<Camera>().ResetReplacementShader();
	}
}

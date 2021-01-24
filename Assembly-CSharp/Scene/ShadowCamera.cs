using UnityEngine;

public class ShadowCamera : MonoBehaviour
{
	public Shader Shadozer;

	public bool updated;

	public int LastRecordedRes;

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
		if (SaveManager.UseHDSHadows)
		{
			if (LastRecordedRes != GetBiggerScreenDimension())
			{
				updated = false;
			}
			if ((bool)GetComponent<Camera>().activeTexture && !updated)
			{
				updated = true;
				Debug.Log(GetComponent<Camera>().activeTexture.name);
				int Res = GetBiggerScreenDimension();
				LastRecordedRes = Res;
				RenderTexture Tex = new RenderTexture(Res, Res, 16);
				GetComponent<Camera>().targetTexture.Release();
				GetComponent<Camera>().targetTexture = Tex;
				GetComponent<Camera>().SetReplacementShader(Shadozer, "RenderType");
				GameObject shadquad = GameObject.Find("ShadowQuad");
				shadquad.GetComponent<MeshRenderer>().material.mainTexture = Tex;
			}
		}
	}

	public void OnDisable()
	{
		GetComponent<Camera>().ResetReplacementShader();
	}
}

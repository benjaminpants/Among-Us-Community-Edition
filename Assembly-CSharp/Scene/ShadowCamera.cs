using UnityEngine;

public class ShadowCamera : MonoBehaviour
{
	public Shader Shadozer;

	public void OnEnable()
	{
		GetComponent<Camera>().SetReplacementShader(Shadozer, "RenderType");
	}

	public void OnDisable()
	{
		GetComponent<Camera>().ResetReplacementShader();
	}
}

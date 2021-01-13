using UnityEngine;

public class ShadowCamera : MonoBehaviour
{
	public Shader Shadozer;

	public bool updated;

	public void OnEnable()
	{
        GetComponent<Camera>().SetReplacementShader(Shadozer, "RenderType");
	}

	public void Update()
    {
		if ((bool)GetComponent<Camera>().activeTexture && !updated)
        {
			updated = true;
		}
	}

	public void OnDisable()
	{
		GetComponent<Camera>().ResetReplacementShader();
	}
}

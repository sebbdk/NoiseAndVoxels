using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TextureCreator : MonoBehaviour {

	[Range(2, 512)]
	public int resolution = 256;

	public float frequency = 1f;

	[Range(1, 3)]
	public int dimensions = 3;

	public NoiseMethodType type;

	private Texture2D texture;

	private void Awake () {
		UpdateNoise ();
	}

	private void Update() {
		if (Application.isEditor) {
			UpdateNoise ();
		}
	}

	private void OnEnable() {
		if (Application.isEditor) {
			UpdateNoise ();
		}	
	}

	private void UpdateNoise() {
		texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, false);
		texture.name = "Procedural Texture"; 
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Trilinear;

		var tempMaterial = new Material(GetComponent<MeshRenderer> ().sharedMaterial);
		tempMaterial.mainTexture = texture;

		GetComponent<MeshRenderer> ().material = tempMaterial;
		FillTexture();	
	}

	public void FillTexture () {
		if (texture.width != resolution) {
			texture.Resize(resolution, resolution);
		}

		Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f,-0.5f));
		Vector3 point10 = transform.TransformPoint(new Vector3( 0.5f,-0.5f));
		Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f, 0.5f));
		Vector3 point11 = transform.TransformPoint(new Vector3( 0.5f, 0.5f));

		NoiseMethod method = Noise.noiseMethods[(int)type][dimensions - 1];

		float stepSize = 1f / resolution;
		Random.seed = 42;
		for (int y = 0; y < resolution; y++) {
			Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
			Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
			for (int x = 0; x < resolution; x++) {
				Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
				float sample = method(point, frequency);
				if (type != NoiseMethodType.Value) {
					sample = sample * 0.5f + 0.5f;
				}
				texture.SetPixel(x, y, Color.white * sample);
			}
		}


/*
		Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f,-0.5f));
		Vector3 point10 = transform.TransformPoint(new Vector3( 0.5f,-0.5f));
		Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f, 0.5f));
		Vector3 point11 = transform.TransformPoint(new Vector3( 0.5f, 0.5f));

		float stepSize = 1f / resolution;
		for (int y = 0; y < resolution; y++) {

			Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
			Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);

			for (int x = 0; x < resolution; x++) {
				//texture.SetPixel(x, y, new Color((x+0.5f) * stepSize, (y+0.5f) * stepSize, 0f));
				//texture.SetPixel(x, y, new Color((x + 0.5f) * stepSize % 0.1f, (y + 0.5f) * stepSize % 0.1f, 0f) * 10f);

				Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
				texture.SetPixel(x, y, new Color(point.x, point.y, point.z));

			}
		}*/



		texture.Apply();
	}
}

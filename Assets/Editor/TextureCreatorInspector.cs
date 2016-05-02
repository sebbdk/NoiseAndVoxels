using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextureCreator))]
public class TextureCreatorInspector : Editor {

	public override void OnInspectorGUI () {
		EditorGUI.BeginChangeCheck();
		DrawDefaultInspector();
		if (EditorGUI.EndChangeCheck() && Application.isPlaying) {
			(target as TextureCreator).FillTexture();
		}
	}
}
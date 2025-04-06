using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class ImageRenderer : EditorWindow
{
	Camera cam;

	int resolutionX = 265;
	int resolutionY = 265;

	string outputPath = "Rendering/output.png";

	[MenuItem("Image Rendering/Menu")]
	static void GetMe()
	{
		GetWindow<ImageRenderer>();
	}

	void OnGUI()
	{
		cam = (Camera)EditorGUILayout.ObjectField("Rendering Camera", cam, typeof(Camera), true);
		resolutionX = EditorGUILayout.IntField("resolution X", resolutionX);
		resolutionY = EditorGUILayout.IntField("resolution Y", resolutionY);
		outputPath = GUILayout.TextField(outputPath);

        if (GUILayout.Button("Shot")) TakeAShot();
	}

	void TakeAShot()
	{
		if (cam == null)
		{
			EditorUtility.DisplayDialog("Bruh.", "Camera is not set.", "lol");
			return;
		}

        RenderTexture rTex = new RenderTexture(resolutionX, resolutionY, 24);

        cam.targetTexture = rTex;
		// CameraClearFlags ogFlags = cam.clearFlags;
		// // cam.clearFlags = CameraClearFlags.Depth;

        Texture2D shot = new Texture2D(resolutionX, resolutionY, TextureFormat.RGBA32, false);

        cam.Render();

        RenderTexture.active = rTex;

        shot.ReadPixels(new Rect(0, 0, resolutionX, resolutionY), 0, 0);

        cam.targetTexture = null;
        RenderTexture.active = null;

		byte[] b = shot.EncodeToPNG();

		string path = Path.Combine(Application.dataPath, outputPath);

        if (!Directory.Exists(Path.GetDirectoryName(path)))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(path));
		}

		File.WriteAllBytes(path, b);

        // destroy to not cause memory leaks
        DestroyImmediate(rTex);
        DestroyImmediate(shot);

        // cam.clearFlags = ogFlags;

        AssetDatabase.Refresh();
	}
}

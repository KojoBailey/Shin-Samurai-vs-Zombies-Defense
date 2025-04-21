using UnityEngine;

public class SUIScreen : WeakGlobalInstance<SUIScreen>, SUIProcess {
	public const System.UInt16 Width = 1920;
	public const System.UInt16 Height = 1080;
	public AutoScaler AutoScaler;
	public static bool AspectRatioStandard {
		get {
			return Width / Height == Screen.width / Screen.height;
		}
	}
	
	public SUIInputManager Inputs;
	private static double previousTime;
	private static double deltaTime = 0.0;
	public static float DeltaTime {
		get {
			return Mathf.Min((float)deltaTime, 0.1f);
		}
	}

	public SUIFader Fader;

	public SUIScreen() {
		SetUniqueInstance(this);
		previousTime = Time.realtimeSinceStartup;
		Time.timeScale = 1f;
		AutoScaler = new AutoScaler(new Vector2(Width, Height));
		Inputs = new SUIInputManager();
		Fader = new SUIFader();
	}

	public void Update() {
		UpdateTime();
		Inputs.Update();
		Fader.Update();
	}

	public void UpdateTime() {
		deltaTime = (double)Time.realtimeSinceStartup - previousTime;
		previousTime = Time.realtimeSinceStartup;
	}

	public void Destroy() {
		Fader.Destroy();
		Fader = null;
	}

	public void EditorRenderOnGUI() {}
}

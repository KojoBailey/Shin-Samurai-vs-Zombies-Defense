using UnityEngine;

public class SceneBehaviour : MonoBehaviour {
	private float playHavenTimer = 1f;
	protected bool justShownPlayHaven;

	private SUIScreen screen;

	private int skipNumUpdate = 3;

	private static SceneBehaviour uniqueInstance;

	public static SceneBehaviour SceneBehaviourInstance {
		get {
			return uniqueInstance;
		}
	}

	public bool JustShownPlayHaven {
		get {
			return justShownPlayHaven;
		}
		set {
			justShownPlayHaven = value;
		}
	}

	private void Awake() {
		uniqueInstance = this;
		Singleton<PlayHavenTowerControl>.instance.ClearHistory();
		SingletonMonoBehaviour<Achievements>.instance.Init();
		SingletonMonoBehaviour<ResourcesManager>.instance.Init();
		ApplicationUtilities.instance.Init();
		screen = new SUIScreen();
	}

	protected bool SceneBehaviourUpdate() {
		if (skipNumUpdate > 0) {
			screen.UpdateTime();
			skipNumUpdate--;
			if (skipNumUpdate == 0) {
				SingletonMonoBehaviour<WaitingIconBetweenScenes>.instance.visible = false;
			}
			return true;
		}
		UpdatePlayHavenGameLaunch();
		screen.Update();
		return false;
	}

	private void UpdatePlayHavenGameLaunch() {
		if (justShownPlayHaven) {
			justShownPlayHaven = false;
			ApplicationUtilities.instance.mustShowGameLauchPlayHavenAds = false;
		} else if (playHavenTimer > 0f) {
			playHavenTimer -= Time.deltaTime;
		} else if (ApplicationUtilities.instance.mustShowGameLauchPlayHavenAds) {
			Debug.Log("**** DISPLAY PLAYHAVEN ****");
			ApplicationUtilities.instance.mustShowGameLauchPlayHavenAds = false;
			Singleton<PlayHavenTowerControl>.instance.InvokeContent("game_launch");
		}
	}
}

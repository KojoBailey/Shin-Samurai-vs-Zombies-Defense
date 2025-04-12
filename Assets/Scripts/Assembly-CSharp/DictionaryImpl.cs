using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictionaryImpl : SceneBehaviour {
    private bool start_finished;
    private SUILayout layout;

	void Start () {
        // Load layout data.
		string layout_path = Singleton<PlayModesManager>.instance.selectedModeData["layout_Dictionary"];
        layout = new SUILayout(layout_path);

        // Transition into scene.
		WeakGlobalInstance<SUIScreen>.instance.fader.speed = 1f;
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeFromBlack();
		layout.AnimateIn();

        // Assign button functions.
		((SUIButton)layout["back"]).onButtonPressed = delegate { GoToMenu("Store"); };

        start_finished = true;
	}
	
	void Update () {
        if (!start_finished || SceneBehaviourUpdate()) {
			return;
		}
		layout.Update();
        if (Input.GetKeyUp(KeyCode.Escape)) {
			GoToMenu("Store");
		}
	}

    private void GoToMenu(string sceneName) {
		layout.AnimateOut(WeakGlobalInstance<SUIScreen>.instance.fader.speed);
		WeakGlobalInstance<SUIScreen>.instance.fader.onFadingDone = delegate {
			Singleton<MenusFlow>.instance.LoadScene(sceneName);
		};
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeToBlack();
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = false;
	}
}

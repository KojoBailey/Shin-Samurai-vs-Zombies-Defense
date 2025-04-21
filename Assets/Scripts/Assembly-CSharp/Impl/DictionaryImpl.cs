using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictionaryImpl : SceneBehaviour {
    private bool start_finished;

    private SUILayout layout;

	// List SUI
	private CardListController card_list_controller;
	private SUIScrollList card_list;
	private Rect card_list_area = new Rect(110f, 130f, 880f, 508f);
	private Vector2 card_size = new Vector2(220f, 270f);

	void Start () {
        // Load layout data.
		string layout_path = Singleton<PlayModesManager>.instance.selectedModeData["layout_Dictionary"];
        layout = new SUILayout(layout_path);

        // Transition into scene.
		WeakGlobalInstance<SUIScreen>.instance.Fader.speed = 1f;
		WeakGlobalInstance<SUIScreen>.instance.Fader.FadeFromBlack();
		layout.AnimateIn();

        // Assign button functions.
		((SUIButton)layout["back"]).onButtonPressed = delegate { GoToMenu("Store"); };

		// Load cards.
		card_list_controller = new CardListController(card_list_area, card_size);
		card_list = new SUIScrollList(card_list_controller, card_list_area, card_size, SUIScrollList.ScrollDirection.Vertical, 4);
		card_list.TransitInFromBelow();
		card_list.onItemTouched = OnItemTouched;

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
		layout.AnimateOut(WeakGlobalInstance<SUIScreen>.instance.Fader.speed);
		WeakGlobalInstance<SUIScreen>.instance.Fader.onFadingDone = delegate {
			Singleton<MenusFlow>.instance.LoadScene(sceneName);
		};
		WeakGlobalInstance<SUIScreen>.instance.Fader.FadeToBlack();
		WeakGlobalInstance<SUIScreen>.instance.Inputs.processInputs = false;
	}

	private void OnItemTouched(int card_index) {
		
	}
}

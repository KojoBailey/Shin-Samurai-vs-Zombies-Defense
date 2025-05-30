using UnityEngine;

public class OptionsImpl : SceneBehaviour {
	private SUILayout mLayout;

	private YesNoDialog mConfirmDialog;

	private SUILayout mAboutDialog;

	private void Start() {
		WeakGlobalInstance<SUIScreen>.instance.Fader.FadeFromBlack();
		mLayout = new SUILayout("Layouts/Options");
		mLayout.AnimateIn();
		mLayout.GetButton("backBtn").onButtonPressed = delegate {
			GotoScene("TitleScreen");
		};
		mLayout.GetButton("credits").onButtonPressed = delegate {
			GotoScene("Credits");
		};
		mLayout.GetButton("about").onButtonPressed = PopAboutInfo;
		mLayout.GetButton("language").onButtonPressed = PopLanguageDialog;
		mLayout.GetButton("privacy").onButtonPressed = ShowPrivacy;
		mLayout.GetButton("resetSave").onButtonPressed = PopResetDataConfirmDialog;
		mLayout.GetButton("resetSave").visible = false;
	}

	private void Update() {
		if (SceneBehaviourUpdate()) {
			return;
		}
		if (mAboutDialog != null) {
			mAboutDialog.Update();
			if (Input.GetKeyUp(KeyCode.Escape)) {
				mAboutDialog.Destroy();
				mAboutDialog = null;
			}
		}
		else if (mConfirmDialog != null) {
			mConfirmDialog.Update();
			if (mConfirmDialog.isDone) {
				mConfirmDialog.Destroy();
				mConfirmDialog = null;
			}
		}
		else {
			mLayout.Update();
			if (Input.GetKeyUp(KeyCode.Escape)) {
				GotoScene("TitleScreen");
			}
		}
	}

	private void GotoScene(string sceneName) {
		mLayout.GetButton("backBtn").onButtonPressed = null;
		mLayout.GetButton("credits").onButtonPressed = null;
		mLayout.GetButton("about").onButtonPressed = null;
		mLayout.GetButton("language").onButtonPressed = null;
		mLayout.GetButton("resetSave").onButtonPressed = null;
		WeakGlobalInstance<SUIScreen>.instance.Fader.onFadingDone = delegate {
			Singleton<MenusFlow>.instance.LoadScene(sceneName);
		};
		WeakGlobalInstance<SUIScreen>.instance.Fader.FadeToBlack();
		mLayout.AnimateOut();
		WeakGlobalInstance<SUIScreen>.instance.Inputs.processInputs = false;
	}

	private void PopResetDataConfirmDialog() {
		mConfirmDialog = new YesNoDialog(Singleton<Localizer>.instance.Get("options_confirmreset"), false, delegate {
			Singleton<Profile>.instance.ResetData();
		}, delegate {
		});
		mConfirmDialog.priority = 500f;
	}

	private void PopLanguageDialog() {
		if (Singleton<GameVersion>.instance.language == "Default") {
			mConfirmDialog = new YesNoDialog("Switch to Japanese?", false, delegate {
				Singleton<GameVersion>.instance.language = "Japanese";
				GotoScene("TitleScreen");
			}, delegate {});
		} else if (Singleton<GameVersion>.instance.language == "Japanese") {
			mConfirmDialog = new YesNoDialog("英語を切り替わるか？", false, delegate {
				Singleton<GameVersion>.instance.language = "English";
				GotoScene("TitleScreen");
			}, delegate {});
		}
		mConfirmDialog.priority = 500f;
	}

	private void PopAboutInfo() {
		string text = string.Empty;
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Text/" + Singleton<Localizer>.instance.Get("legal_file"));
		for (int i = 0; i < sDFTreeNode.attributeCount; i++) {
			string text2 = sDFTreeNode[i];
			if (text2 == "*version") {
				text2 = "v" + Singleton<GameVersion>.instance.ToString();
			}
			if (text != string.Empty) {
				text += "\n";
			}
			text += text2;
		}
		mAboutDialog = new SUILayout("Layouts/AboutDialog");
		((SUISprite)mAboutDialog["panel"]).scale = new Vector2(1f, 1.1f);
		((SUILabel)mAboutDialog["text"]).text = text;
		mAboutDialog.basePriority = 500f;
		((SUIButton)mAboutDialog["back"]).onButtonPressed = delegate {
			if (mAboutDialog != null) {
				mAboutDialog.Destroy();
				mAboutDialog = null;
			}
		};
		mAboutDialog.AnimateIn();
	}

	private void ShowPrivacy() {
		Application.OpenURL("http://www.glu.com/legal");
	}
}

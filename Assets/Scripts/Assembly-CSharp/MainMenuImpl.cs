using System.Collections;
using UnityEngine;

public class MainMenuImpl : SceneBehaviour
{
	private SUILayout mLayout;
	private DialogHandler dialog_handler;
	public AudioClip click_sfx;
	private YesNoDialog quit_dialog;
	private SUIButton cheats_button;

	// Scrolling blood
	private SpriteScreenScroller scrolling_blood_top;
	private SpriteScreenScroller scrolling_blood_bottom;
	private float blood_speed = 1000f;

	private void Start() {
		// Set up layout.
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeFromBlack();
		mLayout = new SUILayout("Layouts/StartScreen");
		mLayout.AnimateIn();

		// Version text (top-right).
		mLayout.GetLabel("version").text = "version " + Singleton<GameVersion>.instance.ToString() + ((!SingletonMonoBehaviour<ResourcesManager>.instance.hasOnlineBundleLoaded) ? string.Empty : "+");
		
		// Cheats - debug only.
		cheats_button = mLayout.GetButton("cheats");
		if (Debug.isDebugBuild)
			cheats_button.onButtonPressed = ShowCheatsScreen;
		else
			cheats_button.visible = false;

		// Scrolling blood UI.
		scrolling_blood_top = new SpriteScreenScroller(mLayout.GetSprite("bloodTop"));
		scrolling_blood_top.ScrollHorizontally(blood_speed);
		scrolling_blood_bottom = new SpriteScreenScroller(mLayout.GetSprite("bloodBottom"));
		scrolling_blood_bottom.ScrollHorizontally(0f - blood_speed);

		EnableButtons();

		// Welcome back and daily rewards.
		IDialog special_dialog = null;
		special_dialog = Singleton<FreebiesManager>.instance.CheckForWelcomeBackGift();
		if (special_dialog != null) {
			((DailyRewardDialog)special_dialog).onYesPressed = delegate {
				GoToScene(delegate {
					Singleton<MenusFlow>.instance.LoadScene("BoosterPackStore");
				});
			};
		} else {
			special_dialog = DailyRewardsManager.CheckForDailyReward();
		}
		if (special_dialog == null) {
			special_dialog = Singleton<FreebiesManager>.instance.CheckForGiveOnceGift();
		}
		if (special_dialog != null) {
			StartDialog(special_dialog);
		}
	}

	private IEnumerator CoQuit() {
		yield return new WaitForSeconds(0.5f);
		Application.Quit();
		if (Debug.isDebugBuild) {
			YesNoDialog yesNoDialog = new YesNoDialog(
			"The game has been quit, although will continue to run in the Unity editor.", false, delegate{}, null);
			yesNoDialog.priority = 500f;
			StartDialog(yesNoDialog);
		}
	}

	private void Update() {
		if (SceneBehaviourUpdate()) return;

		// Update blood positions.
		scrolling_blood_top.Update();
		scrolling_blood_bottom.Update();
		if (blood_speed > 100f) {
			blood_speed = (blood_speed - 100f) * Mathf.Clamp(1f - SUIScreen.deltaTime * 2f, 0f, 0.98f) + 100f;
			scrolling_blood_top.speed = blood_speed;
			scrolling_blood_bottom.speed = 0f - blood_speed;
		}

		// Dialog handler.
		if (dialog_handler != null) {
			dialog_handler.Update();
			if (dialog_handler.isDone) {
				dialog_handler.Destroy();
				dialog_handler = null;
			}
			return;
		}

		// Refresh layout.
		mLayout.Update();

		// Check for quitting.
		if (Input.GetKeyUp(KeyCode.Escape)) {
			quit_dialog = new YesNoDialog(Singleton<Localizer>.instance.Get("quit_confirm"), false, delegate {
				StartCoroutine(CoQuit());
			}, delegate{});
			quit_dialog.priority = 500f;
			StartDialog(quit_dialog);
		}
	}

	private void EnableButtons() {
		// The two heroes. Click to select a mode.
			// Classic Mode
		mLayout.GetButton("samurai").onButtonPressed = delegate {
			if ((Singleton<Profile>.instance.waveToBeat == 1 && Singleton<Profile>.instance.GetWaveLevel(2) == 0) || Singleton<Profile>.instance.readyToStartBonusWave) {
				GoToScene(delegate {
					WaveManager.LoadNextWaveLevel();
				});
			}
			else {
				Singleton<Profile>.instance.tapFeatureAdsCnt++;
				Singleton<Profile>.instance.Save();
				GoToScene(delegate {
					Singleton<PlayModesManager>.instance.selectedMode = "classic";
					Singleton<MenusFlow>.instance.LoadScene("Store");
				});
			}
		};
			// Zombies Rising
		mLayout.GetButton("zombiehero").onButtonPressed = delegate {
			Singleton<Profile>.instance.tapFeatureAdsCnt++;
			Singleton<Profile>.instance.Save();
			GoToScene(delegate {
				Singleton<PlayModesManager>.instance.selectedMode = "zombies";
				Singleton<MenusFlow>.instance.LoadScene("Store");
			});
		};

		// Options.
		mLayout.GetButton("options").onButtonPressed = delegate {
			GoToScene(delegate {
				Singleton<MenusFlow>.instance.LoadScene("Options");
			});
		};

		// Other games.
		mLayout.GetButton("otherGames").onButtonPressed = delegate {
			YesNoDialog yesNoDialog = new YesNoDialog(
				"Check out Zweronz's various projects via his GitHub and YouTube profiles!", false, delegate{}, null);
			yesNoDialog.priority = 500f;
			StartDialog(yesNoDialog);
		};
	}
	private void DisableButtons() {
		mLayout.GetButton("zombiehero").onButtonPressed = null;
		mLayout.GetButton("samurai").onButtonPressed = null;
		mLayout.GetButton("options").onButtonPressed = null;
		mLayout.GetButton("otherGames").onButtonPressed = null;
	}

	private void StartDialog(IDialog d) {
		// Remove existing dialog if  open.
		if (dialog_handler != null) {
			dialog_handler.Destroy();
			dialog_handler = null;
		}
		// Open new dialog with provided data.
		dialog_handler = new DialogHandler(499f, d);
	}

	private void GoToScene(OnSUIGenericCallback menuJump) {
		ApplicationUtilities.instance.HideAds();
		DisableButtons();
		WeakGlobalInstance<SUIScreen>.instance.fader.onFadingDone = delegate {
			menuJump();
		};
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeToBlack();
		base.GetComponent<AudioSource>().PlayOneShot(click_sfx);
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = false;
	}

	private void ShowCheatsScreen() {
		GoToScene(delegate {
			Singleton<MenusFlow>.instance.LoadScene("cheats");
		});
	}
}

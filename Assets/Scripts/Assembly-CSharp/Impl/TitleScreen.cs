using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : SceneBehaviour {
	private SUILayout layout;
	private DialogHandler dialogHandler;
	public AudioClip ClickSFX;
	private YesNoDialog quitDialog;
	private SUIButton cheatsButton;

	// Scrolling blood
	private SpriteScreenScroller scrollingBloodTop;
	private SpriteScreenScroller scrollingBloodBottom;
	private float bloodSpeed = 1000f;

	private void Start() {
		// Set up layout.
		WeakGlobalInstance<SUIScreen>.instance.Fader.FadeFromBlack();
		layout = new SUILayout("Layouts/StartScreen");
		layout.AnimateIn();

		// Version text (top-right).
		layout.GetLabel("version").text = "Version " + Singleton<GameVersion>.instance.ToString();
		
		// Cheats - debug only.
		cheatsButton = layout.GetButton("cheats");
		if (Debug.isDebugBuild)
			cheatsButton.onButtonPressed = ShowCheatsScreen;
		else
			cheatsButton.visible = false;

		// Scrolling blood UI.
		scrollingBloodTop = new SpriteScreenScroller(layout.GetSprite("bloodTop"));
		scrollingBloodTop.ScrollHorizontally(bloodSpeed);
		scrollingBloodBottom = new SpriteScreenScroller(layout.GetSprite("bloodBottom"));
		scrollingBloodBottom.ScrollHorizontally(0f - bloodSpeed);

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
		scrollingBloodTop.Update();
		scrollingBloodBottom.Update();
		if (bloodSpeed > 100f) {
			bloodSpeed = (bloodSpeed - 100f) * Mathf.Clamp(1f - SUIScreen.DeltaTime * 2f, 0f, 0.98f) + 100f;
			scrollingBloodTop.speed = bloodSpeed;
			scrollingBloodBottom.speed = 0f - bloodSpeed;
		}

		// Dialog handler.
		if (dialogHandler != null) {
			dialogHandler.Update();
			if (dialogHandler.isDone) {
				dialogHandler.Destroy();
				dialogHandler = null;
			}
			return;
		}

		// Refresh layout.
		layout.Update();

		// Check for quitting.
		if (Input.GetKeyUp(KeyCode.Escape)) {
			quitDialog = new YesNoDialog(Singleton<Localizer>.instance.Get("quit_confirm"), false, delegate {
				StartCoroutine(CoQuit());
			}, delegate{});
			quitDialog.priority = 500f;
			StartDialog(quitDialog);
		}
	}

	private void EnableButtons() {
		// The two heroes. Click to select a mode.
			// Classic Mode
		layout.GetButton("samurai").onButtonPressed = delegate {
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
		layout.GetButton("zombiehero").onButtonPressed = delegate {
			Singleton<Profile>.instance.tapFeatureAdsCnt++;
			Singleton<Profile>.instance.Save();
			GoToScene(delegate {
				Singleton<PlayModesManager>.instance.selectedMode = "zombies";
				Singleton<MenusFlow>.instance.LoadScene("Store");
			});
		};

		// Options.
		layout.GetButton("options").onButtonPressed = delegate {
			GoToScene(delegate {
				Singleton<MenusFlow>.instance.LoadScene("Options");
			});
		};

		// Other games.
		layout.GetButton("otherGames").onButtonPressed = delegate {
			YesNoDialog yesNoDialog = new YesNoDialog(
				"Check out Zweronz's various projects via his GitHub and YouTube profiles!", false, delegate{}, null);
			yesNoDialog.priority = 500f;
			StartDialog(yesNoDialog);
		};
	}
	private void DisableButtons() {
		layout.GetButton("zombiehero").onButtonPressed = null;
		layout.GetButton("samurai").onButtonPressed = null;
		layout.GetButton("options").onButtonPressed = null;
		layout.GetButton("otherGames").onButtonPressed = null;
	}

	private void StartDialog(IDialog d) {
		// Remove existing dialog if  open.
		if (dialogHandler != null) {
			dialogHandler.Destroy();
			dialogHandler = null;
		}
		// Open new dialog with provided data.
		dialogHandler = new DialogHandler(499f, d);
	}

	private void GoToScene(OnSUIGenericCallback menuJump) {
		ApplicationUtilities.instance.HideAds();
		DisableButtons();
		WeakGlobalInstance<SUIScreen>.instance.Fader.onFadingDone = delegate {
			menuJump();
		};
		WeakGlobalInstance<SUIScreen>.instance.Fader.FadeToBlack();
		base.GetComponent<AudioSource>().PlayOneShot(ClickSFX);
		WeakGlobalInstance<SUIScreen>.instance.Inputs.processInputs = false;
	}

	private void ShowCheatsScreen() {
		GoToScene(delegate {
			Singleton<MenusFlow>.instance.LoadScene("cheats");
		});
	}
}

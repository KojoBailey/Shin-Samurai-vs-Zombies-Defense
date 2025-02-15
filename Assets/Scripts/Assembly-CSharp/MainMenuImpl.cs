using System.Collections;
using UnityEngine;

public class MainMenuImpl : SceneBehaviour
{
	private SUILayout mLayout;
	private DialogHandler mDialogHandler;
	public AudioClip ClickSFX;
	private YesNoDialog m_quitDialog;
	private SUIButton m_cheatsBtn;

	// Scrolling blood
	private SpriteScreenScroller mScrollBloodTop;
	private SpriteScreenScroller mScrollBloodBottom;
	private float mBloodSpeed = 1000f;

	private void Start() {
		// Set up layout.
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeFromBlack();
		mLayout = new SUILayout("Layouts/StartScreen");
		mLayout.AnimateIn();

		// Version text (top-right).
		mLayout.GetLabel("version").text = Singleton<GameVersion>.instance.ToString() + ((!SingletonMonoBehaviour<ResourcesManager>.instance.hasOnlineBundleLoaded) ? string.Empty : "+");
		
		// Cheats - debug only.
		m_cheatsBtn = mLayout.GetButton("cheats");
		m_cheatsBtn.scale = new Vector2(2f, 2f);
		if (Debug.isDebugBuild)
			m_cheatsBtn.onButtonPressed = ShowCheatsScreen;
		else
			m_cheatsBtn.visible = false;

		// Scrolling blood UI.
		mScrollBloodTop = new SpriteScreenScroller(mLayout.GetSprite("bloodTop"));
		mScrollBloodTop.ScrollHorizontally(mBloodSpeed);
		mScrollBloodBottom = new SpriteScreenScroller(mLayout.GetSprite("bloodBottom"));
		mScrollBloodBottom.ScrollHorizontally(0f - mBloodSpeed);

		EnableButtons();

		// Welcome back and daily rewards.
		IDialog specialDialog2 = null;
		specialDialog2 = Singleton<FreebiesManager>.instance.CheckForWelcomeBackGift();
		if (specialDialog2 != null) {
			((DailyRewardDialog)specialDialog2).onYesPressed = delegate {
				GoToScene(delegate {
					Singleton<MenusFlow>.instance.LoadScene("BoosterPackStore");
				});
			};
		} else {
			specialDialog2 = DailyRewardsManager.CheckForDailyReward();
		}
		if (specialDialog2 == null) {
			specialDialog2 = Singleton<FreebiesManager>.instance.CheckForGiveOnceGift();
		}
		if (specialDialog2 != null) {
			StartDialog(specialDialog2);
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
		mScrollBloodTop.Update();
		mScrollBloodBottom.Update();
		if (mBloodSpeed > 100f) {
			mBloodSpeed = (mBloodSpeed - 100f) * Mathf.Clamp(1f - SUIScreen.deltaTime * 2f, 0f, 0.98f) + 100f;
			mScrollBloodTop.speed = mBloodSpeed;
			mScrollBloodBottom.speed = 0f - mBloodSpeed;
		}

		// Dialog handler.
		if (mDialogHandler != null) {
			mDialogHandler.Update();
			if (mDialogHandler.isDone) {
				mDialogHandler.Destroy();
				mDialogHandler = null;
			}
			return;
		}

		// Refresh layout.
		mLayout.Update();

		// Check for quitting.
		if (Input.GetKeyUp(KeyCode.Escape)) {
			m_quitDialog = new YesNoDialog(Singleton<Localizer>.instance.Get("quit_confirm"), false, delegate {
				StartCoroutine(CoQuit());
			}, delegate{});
			m_quitDialog.priority = 500f;
			StartDialog(m_quitDialog);
		}
	}

	private void EnableButtons() {
		// The two heroes. Click to select a mode.
			// Classic Mode
		mLayout.GetButton("samurai").onButtonPressed = delegate
		{
			if ((Singleton<Profile>.instance.waveToBeat == 1 && Singleton<Profile>.instance.GetWaveLevel(2) == 0) || Singleton<Profile>.instance.readyToStartBonusWave) {
				GoToScene(delegate
				{
					WaveManager.LoadNextWaveLevel();
				});
			}
			else {
				Singleton<Profile>.instance.tapFeatureAdsCnt = Singleton<Profile>.instance.tapFeatureAdsCnt + 1;
				Singleton<Profile>.instance.Save();
				GoToScene(delegate
				{
					Singleton<MenusFlow>.instance.LoadScene("Store");
				});
			}
		};
			// Zombies Rising
		mLayout.GetButton("zombiehero").onButtonPressed = delegate
		{
			Singleton<Profile>.instance.tapFeatureAdsCnt = Singleton<Profile>.instance.tapFeatureAdsCnt + 1;
			Singleton<Profile>.instance.Save();
			GoToScene(delegate
			{
				Singleton<PlayModesManager>.instance.selectedMode = "zombies";
				Singleton<MenusFlow>.instance.LoadScene("Store");
			});
		};

		// Options.
		mLayout.GetButton("options").onButtonPressed = delegate
		{
			GoToScene(delegate
			{
				Singleton<MenusFlow>.instance.LoadScene("Options");
			});
		};

		// Other games.
		mLayout.GetButton("otherGames").onButtonPressed = onOtherGames;
	}
	private void DisableButtons() {
		mLayout.GetButton("zombiehero").onButtonPressed = null;
		mLayout.GetButton("samurai").onButtonPressed = null;
		mLayout.GetButton("options").onButtonPressed = null;
		mLayout.GetButton("otherGames").onButtonPressed = null;
	}

	private void onOtherGames() {
		YesNoDialog yesNoDialog = new YesNoDialog(
			"Check out Zweronz's various projects via his GitHub and YouTube profiles!", false, delegate{}, null);
		yesNoDialog.priority = 500f;
		StartDialog(yesNoDialog);
	}

	private void StartDialog(IDialog d) {
		// Remove existing dialog if  open.
		if (mDialogHandler != null) {
			mDialogHandler.Destroy();
			mDialogHandler = null;
		}
		// Open new dialog with provided data.
		mDialogHandler = new DialogHandler(499f, d);
	}

	private void GoToScene(OnSUIGenericCallback menuJump) {
		ApplicationUtilities.instance.HideAds();
		DisableButtons();
		WeakGlobalInstance<SUIScreen>.instance.fader.onFadingDone = delegate {
			menuJump();
		};
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeToBlack();
		base.GetComponent<AudioSource>().PlayOneShot(ClickSFX);
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = false;
	}

	private void ShowCheatsScreen() {
		GoToScene(delegate {
			Singleton<MenusFlow>.instance.LoadScene("cheats");
		});
	}
}

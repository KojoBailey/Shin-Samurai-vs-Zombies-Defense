using UnityEngine;

public class PlayModeMenuImpl : SceneBehaviour
{
	private SUILayout mLayout;

	private TutorialPopup mUnlockDialog;

	private void Start()
	{
		WeakGlobalInstance<SUIScreen>.instance.Fader.FadeFromBlack();
		mLayout = new SUILayout("Layouts/PlayModesLayout");
		mLayout.AnimateIn();
		((SUIButton)mLayout["backBtn"]).onButtonPressed = delegate
		{
			GotoScene(delegate
			{
				Singleton<MenusFlow>.instance.LoadScene("TitleScreen");
			});
		};
		((SUITouchArea)mLayout["mode_classic"]).onAreaTouched = delegate
		{
			SelectMode("classic");
		};
		((SUITouchArea)mLayout["mode_zombies"]).onAreaTouched = delegate
		{
			SelectMode("zombies");
		};
		if (!Singleton<Profile>.instance.zombieModeUnlockedMessageShown)
		{
			Singleton<Profile>.instance.zombieModeUnlockedMessageShown = true;
			Singleton<Profile>.instance.Save();
			ShowUnlockMessage();
		}
	}

	private void Update()
	{
		if (SceneBehaviourUpdate())
		{
			return;
		}
		if (mUnlockDialog != null)
		{
			mUnlockDialog.Update();
			if (WeakGlobalInstance<SUIScreen>.instance.Inputs.justTouched)
			{
				mUnlockDialog.Destroy();
				mUnlockDialog = null;
			}
			return;
		}
		mLayout.Update();
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			GotoScene(delegate
			{
				Singleton<MenusFlow>.instance.LoadScene("TitleScreen");
			});
		}
	}

	private void GotoScene(OnSUIGenericCallback menuJump)
	{
		WeakGlobalInstance<SUIScreen>.instance.Fader.onFadingDone = delegate
		{
			menuJump();
		};
		WeakGlobalInstance<SUIScreen>.instance.Fader.FadeToBlack();
		mLayout.AnimateOut();
		WeakGlobalInstance<SUIScreen>.instance.Inputs.processInputs = false;
	}

	private void SelectMode(string mode)
	{
		Singleton<PlayModesManager>.instance.selectedMode = mode;
		((SUIButton)mLayout["backBtn"]).onButtonPressed = null;
		((SUITouchArea)mLayout["mode_classic"]).onAreaTouched = null;
		((SUITouchArea)mLayout["mode_zombies"]).onAreaTouched = null;
		if (mode == "classic" && ((Singleton<Profile>.instance.waveToBeat == 1 && Singleton<Profile>.instance.GetWaveLevel(2) == 0) || Singleton<Profile>.instance.readyToStartBonusWave))
		{
			GotoScene(delegate
			{
				WaveManager.LoadNextWaveLevel();
			});
		}
		else
		{
			GotoScene(delegate
			{
				Singleton<MenusFlow>.instance.LoadScene("Store");
			});
		}
	}

	private void ShowUnlockMessage()
	{
		mUnlockDialog = new TutorialPopup();
		mUnlockDialog.ShowPanel(Singleton<Localizer>.instance.Get("unlocked_zombies"));
		mUnlockDialog.SetPanelPosition(new Vector2(SUIScreen.Width / 2f, SUIScreen.Height / 2f));
	}
}

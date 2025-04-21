public class MoviePlayerImpl : SceneBehaviour
{
	private SUILayout mLayout;

	private void Start()
	{
		WeakGlobalInstance<SUIScreen>.instance.Fader.FadeFromBlack();
		mLayout = new SUILayout("Layouts/MoviePlayer");
		mLayout.AnimateIn();
		((SUIButton)mLayout["continue"]).onButtonPressed = onContinueButton;
	}

	private void Update()
	{
		if (!SceneBehaviourUpdate())
		{
			mLayout.Update();
		}
	}

	private void onContinueButton()
	{
		WeakGlobalInstance<SUIScreen>.instance.Fader.onFadingDone = delegate
		{
			if (Singleton<MenusFlow>.instance.previousScene == "Options")
			{
				Singleton<MenusFlow>.instance.LoadPreviousScene();
			}
			else
			{
				Singleton<MenusFlow>.instance.LoadScene("Store");
			}
		};
		WeakGlobalInstance<SUIScreen>.instance.Fader.FadeToBlack();
		mLayout.AnimateOut();
		WeakGlobalInstance<SUIScreen>.instance.Inputs.processInputs = false;
	}
}

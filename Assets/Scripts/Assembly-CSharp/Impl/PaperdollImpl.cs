public class PaperdollImpl : SceneBehaviour
{
	private SUILayout mLayout;

	private void Start()
	{
		WeakGlobalInstance<SUIScreen>.instance.Fader.FadeFromBlack();
		mLayout = new SUILayout("Layouts/PaperdollLayout");
		mLayout.AnimateIn();
	}

	private void Update()
	{
		if (!SceneBehaviourUpdate())
		{
			mLayout.Update();
		}
	}

	private void Quit()
	{
		WeakGlobalInstance<SUIScreen>.instance.Fader.onFadingDone = delegate
		{
			Singleton<MenusFlow>.instance.LoadPreviousScene();
		};
		WeakGlobalInstance<SUIScreen>.instance.Fader.FadeToBlack();
		mLayout.AnimateOut();
		WeakGlobalInstance<SUIScreen>.instance.Inputs.processInputs = false;
	}
}

using UnityEngine;

public class MenuWith3DLevelImpl : SceneBehaviour
{
	public BoxCollider spawnArea;

	private MenuWith3DLevelManager mManager;

	private void Start()
	{
		WeakGlobalInstance<SUIScreen>.instance.Fader.speed = 1f;
		WeakGlobalInstance<SUIScreen>.instance.Fader.FadeFromBlack();
		mManager = new MenuWith3DLevelManager(spawnArea);
		ApplicationUtilities.instance.ShowAds((ApplicationUtilities.AdPosition)17);
	}

	private void OnDestroy()
	{
		ApplicationUtilities.instance.HideAds();
	}

	private void Update()
	{
		if (!SceneBehaviourUpdate())
		{
			mManager.Update();
		}
	}

	private void OnGUI()
	{
	}
}

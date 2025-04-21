using UnityEngine;

public class WaitingIconBetweenScenes : SingletonMonoBehaviour<WaitingIconBetweenScenes>
{
	private SUISprite mIcon;

	public bool visible
	{
		get
		{
			return mIcon != null;
		}
		set
		{
			if (value)
			{
				if (mIcon == null)
				{
					mIcon = new SUISprite("Sprites/Localized/loading_timer");
					mIcon.priority = 5000f;
					mIcon.position = new Vector2(SUIScreen.Width / 2f, SUIScreen.Height / 2f);
					Object.DontDestroyOnLoad(mIcon.gameObject);
				}
			}
			else if (mIcon != null)
			{
				mIcon.Destroy();
				mIcon = null;
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}

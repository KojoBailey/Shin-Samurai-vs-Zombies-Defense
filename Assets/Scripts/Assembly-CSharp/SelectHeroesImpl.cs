using UnityEngine;

public class SelectHeroesImpl : MenuWith3DLevel
{
	private const int kNumListColumns = 3;

	private Rect kListArea = new Rect(34f, 116f, 492f, 340f);

	private Vector2 kCellSize = new Vector2(164f, 212f);

	private SUILayout mLayout;

	private SUIScrollList mList;

	private SelectHeroesController mListController;

	private SlotGroup mSlotGroup;

	private SUIButton mResetSlotButton;

	private int initTimer = 3;

	public SelectHeroesImpl()
	{
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = true;
		WeakGlobalInstance<MenuWith3DLevelManager>.instance.description = string.Empty;
		mLayout = new SUILayout(Singleton<PlayModesManager>.instance.selectedModeData["layout_SelectHeroesMenu"]);
		mLayout.AnimateIn();
		((SUIButton)mLayout["continue"]).onButtonPressed = OnNextMenu;
		((SUIButton)mLayout["back"]).onButtonPressed = OnPreviousMenu;
		mSlotGroup = new SlotGroup(mLayout);
		mSlotGroup.onSelectionChanged = OnSlotSelectionChanged;
		mListController = new SelectHeroesController(kListArea, kCellSize);
		mList = new SUIScrollList(mListController, kListArea, kCellSize, SUIScrollList.ScrollDirection.Vertical, 3);
		mList.TransitInFromBelow();
		mList.onSelectionChanged = OnListSelectionChanged;
		string selectedHero = Singleton<Profile>.instance.heroID;
		if (selectedHero != string.Empty)
		{
			mSlotGroup.SetSlot(0, selectedHero, mListController.GetIcon(mListController.FindIndex(selectedHero)));
			mList.selection = mListController.FindIndex(selectedHero);
		}
		mList.Update();
		SUILayout.ObjectData od = new SUILayout.ObjectData
		{
			animAlpha = new SUILayoutAnim.AnimFloat(1f, 0f, new SUILayout.NormalRange(0f, 1f), Ease.Linear),
			obj = mList
		};
		mLayout.Add("mainList", od);
		mList.alpha = 0f;
	}

	public void Destroy()
	{
		mLayout.Destroy();
		mLayout = null;
		mSlotGroup = null;
		mList.Destroy();
		mList = null;
		mListController = null;
	}

	public void Update()
	{
		if (initTimer > 0)
		{
			initTimer--;
			return;
		}
		mLayout.Update();
		if (mLayout != null)
		{
			mList.Update();
			mSlotGroup.Update();
			mResetSlotButton.visible = mSlotGroup.GetSlotValue(0) != string.Empty;
			if (Input.GetKeyUp(KeyCode.Escape))
			{
				OnPreviousMenu();
			}
		}
	}

	public void SaveChanges()
	{
		Singleton<Profile>.instance.heroID = mSlotGroup.GetSlotValue(0);
		// Singleton<Analytics>.instance.LogEvent("CharmEquiped", Singleton<Profile>.instance.selectedHero, Singleton<Profile>.instance.waveToBeat);
	}

	private void OnNextMenu()
	{
		SaveChanges();
		mLayout.onTransitionOver = delegate
		{
			WeakGlobalInstance<MenuWith3DLevelManager>.instance.LoadMenu("SelectHelpersImpl");
		};
		mLayout.AnimateOut();
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = false;
	}

	private void OnPreviousMenu()
	{
		SaveChanges();
		WeakGlobalInstance<MenuWith3DLevelManager>.instance.GoToMenu("Store");
	}

	private void OnListSelectionChanged(int index)
	{
		string text = mListController.FindID(index);
		WeakGlobalInstance<MenuWith3DLevelManager>.instance.description = mListController.GetDescription(index);
		WeakGlobalInstance<MenuWith3DLevelManager>.instance.descriptionIcon = mListController.GetIcon(index);
		mSlotGroup.SetSlot(mSlotGroup.selected, text, mListController.GetIcon(index));
	}

	private void OnSlotSelectionChanged(int index)
	{
		WeakGlobalInstance<MenuWith3DLevelManager>.instance.description = string.Empty;
		mList.selection = mListController.FindIndex(mSlotGroup.GetSlotValue(index));
	}
}

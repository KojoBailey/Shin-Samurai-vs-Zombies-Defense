using UnityEngine;

// Hero Select menu before a Wave begins
public class SelectHeroesImpl : MenuWith3DLevel {
	SUILayout layout;
	SUIScrollList list;
	SelectHeroesController list_controller;
	SlotGroup slot_group;
	
	Rect list_area = new Rect(34f, 116f, 492f, 340f); // 3 columns
	Vector2 cell_size = new Vector2(164f, 212f);

	public SelectHeroesImpl() {
		// Enable global screen inputs.
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = true;

		// Load layout data in.
		layout = new SUILayout(Singleton<PlayModesManager>.instance.selectedModeData["layout_SelectHeroesMenu"]);
		layout.AnimateIn();

		// Assign functions to buttons.
		((SUIButton)layout["continue"]).onButtonPressed = OnNextMenu;
		((SUIButton)layout["back"]).onButtonPressed = OnPreviousMenu;
		slot_group = new SlotGroup(layout);
		slot_group.onSelectionChanged = OnSlotSelectionChanged;
		list_controller = new SelectHeroesController(list_area, cell_size);
		list = new SUIScrollList(list_controller, list_area, cell_size, SUIScrollList.ScrollDirection.Vertical, 3);
		list.TransitInFromBelow();
		list.onSelectionChanged = OnListSelectionChanged;

		// Set default selected Hero.
		string selectedHero = Singleton<Profile>.instance.heroID;
		if (selectedHero != string.Empty) {
			slot_group.SetSlot(0, selectedHero, list_controller.GetIcon(list_controller.FindIndex(selectedHero)));
			list.selection = list_controller.FindIndex(selectedHero);
		}
		list.Update();

		// Opacity fade transition effect.
		SUILayout.ObjectData object_data = new SUILayout.ObjectData {
			animAlpha = new SUILayoutAnim.AnimFloat(1f, 0f, new SUILayout.NormalRange(0f, 1f), Ease.Linear),
			obj = list
		};
		layout.Add("mainList", object_data);
		list.alpha = 0f;
	}

	public void Destroy() {
		layout.Destroy();
		layout = null;
		slot_group = null;
		list.Destroy();
		list = null;
		list_controller = null;
	}

	public void Update() {
		layout.Update();
		if (layout != null) {
			list.Update();
			slot_group.Update();
			if (Input.GetKeyUp(KeyCode.Escape)) {
				OnPreviousMenu();
			}
		}
	}

	public void SaveChanges() {
		Singleton<Profile>.instance.heroID = slot_group.GetSlotValue(0);
		Singleton<Analytics>.instance.LogEvent("HeroEquipped", Singleton<Profile>.instance.heroID, Singleton<Profile>.instance.waveToBeat);
	}

	void OnNextMenu() {
		SaveChanges();
		layout.onTransitionOver = delegate {
			WeakGlobalInstance<MenuWith3DLevelManager>.instance.LoadMenu("SelectHelpersImpl");
		};
		layout.AnimateOut();

		// Disable global inputs.
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = false;
	}

	void OnPreviousMenu() {
		SaveChanges();
		WeakGlobalInstance<MenuWith3DLevelManager>.instance.GoToMenu("Store");
	}

	void OnListSelectionChanged(int index) {
		// Update information on right for the selected Hero.
		string text = list_controller.FindID(index);
		WeakGlobalInstance<MenuWith3DLevelManager>.instance.description = list_controller.GetDescription(index);
		WeakGlobalInstance<MenuWith3DLevelManager>.instance.descriptionIcon = list_controller.GetIcon(index);
		slot_group.SetSlot(slot_group.selected, text, list_controller.GetIcon(index));
	}

	void OnSlotSelectionChanged(int index) {
		WeakGlobalInstance<MenuWith3DLevelManager>.instance.description = string.Empty;
		list.selection = list_controller.FindIndex(slot_group.GetSlotValue(index));
	}
}

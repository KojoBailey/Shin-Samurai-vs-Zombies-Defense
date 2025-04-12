using System.Collections.Generic;
using Debug = UnityEngine.Debug;

public class StoreAvailability_Heroes {
	public static void Get(List<StoreData.Item> items) {
		string[] allIDs = Singleton<HeroesDatabase>.instance.allIDs;
		foreach (string text in allIDs) {
			bool locked = false;
			int num = Singleton<Profile>.instance.GetHeroLevel(text) + 1;
			SDFTreeNode hero_registry = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Heroes/" + text);
			if (num == 1 && Singleton<Profile>.instance.highestUnlockedWave < int.Parse(hero_registry["waveToUnlock"])) {
				locked = true;
			} else {
				EnsureProperInitialHeroLevel(text);
			}
			SDFTreeNode hero_level_registry = hero_registry.to(string.Format("{0:000}", num));
				string delegateArg = text;
				bool isLastUpgrade = hero_registry.childCount == num;
				StoreData.Item item = new StoreData.Item(delegate {
					LevelUpHero(delegateArg, isLastUpgrade);
				});
				if (hero_registry.hasChild(num)) {
					SDFTreeNode hero_upgrade_registry = hero_registry.to(num);
					item.cost = new Cost(hero_upgrade_registry["cost"], Singleton<SalesDatabase>.instance.GetSaleFor("hero", num.ToString()));
				} else {
					item.cost = new Cost(hero_registry["infiniteUpgradeCost"], Singleton<SalesDatabase>.instance.GetSaleFor("hero", num.ToString()));
				}
				item.id = text;
				item.icon = hero_registry["icon"];
				item.isEvent = hero_registry["eventID"] != string.Empty;
				item.locked = locked;
				item.unlockAtWave = int.Parse(hero_registry["waveToUnlock"]);
				if (locked) {
					item.unlockCondition = string.Format(Singleton<Localizer>.instance.Get("store_unlockatwave"), item.unlockAtWave);
				}
				item.title = string.Format(Singleton<Localizer>.instance.Parse(hero_registry["store_levelup"]), num);
				item.unlockTitle = Singleton<Localizer>.instance.Parse(hero_registry["displayName"]);
				item.details.AddSmallDescription((!hero_registry.hasAttribute("desc")) ? string.Empty : hero_registry["desc"]);
				items.Add(item);
		}
	}

	private static void FillInfinityPath(string heroID, SDFTreeNode mainData, List<StoreData.Item> items) {}

	private static void EnsureProperInitialHeroLevel(string heroID) {
		int heroLevel = Singleton<Profile>.instance.GetHeroLevel(heroID);
		if (heroLevel == 0) {
			Singleton<Profile>.instance.SetHeroLevel(heroID, heroLevel + 1);
			Singleton<Profile>.instance.Save();
		}
	}

	private static void LevelUpHero(string heroID, bool isLastUpgrade) {
		Singleton<Profile>.instance.SetHeroLevel(heroID, Singleton<Profile>.instance.GetHeroLevel(heroID) + 1);
		Singleton<Profile>.instance.Save();
		Singleton<Analytics>.instance.LogEvent("HeroUpgradePurchased", heroID, Singleton<Profile>.instance.GetHeroLevel(heroID));
		if (!isLastUpgrade) {
			return;
		}
		SDFTreeNode hero_registry = SDFTree.LoadFromResources("Registry/Heroes/" + heroID);
		if (hero_registry == null) {
			Debug.Log("ERROR: Could not find Hero data to retrieve Achievements infos: " + heroID);
			return;
		}
		string text = hero_registry["achievementGC"];
		if (text != string.Empty) {
			SingletonMonoBehaviour<Achievements>.instance.Award(text);
		}
	}
}

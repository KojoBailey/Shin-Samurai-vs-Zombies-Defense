using System;
using System.Collections.Generic;
using UnityEngine;

public class HeroesDatabase : Singleton<HeroesDatabase>
{
	private class HeroData
	{
		public string id;

		public SDFTreeNode registryData;

		public HeroData(string abId)
		{
			id = abId;
		}

		public void EnsureDataCached()
		{
			if (registryData == null)
			{
				registryData = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Heroes/" + id);
			}
		}
	}

	private List<HeroData> mData = new List<HeroData>();

	private string[] mAllIDs;

	public string[] allIDs
	{
		get
		{
			return mAllIDs;
		}
	}

	public HeroesDatabase()
	{
		ResetCachedData();
		SingletonMonoBehaviour<ResourcesManager>.instance.onInvalidateCache += ResetCachedData;
	}

	public void ResetCachedData()
	{
		mData = new List<HeroData>();
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Heroes/AllHeroes");
		if (sDFTreeNode == null || sDFTreeNode.attributeCount == 0)
		{
			Debug.Log("ERROR: Cannot find the list of Heroes (AllHeroes.txt)");
		}
		for (int i = 0; sDFTreeNode.hasAttribute(i); i++)
		{
			mData.Add(new HeroData(sDFTreeNode[i]));
		}
		CacheSimpleIDList();
	}

	public bool Contains(string id)
	{
		string[] array = mAllIDs;
		foreach (string strA in array)
		{
			if (string.Compare(strA, id, true) == 0)
			{
				return true;
			}
		}
		return false;
	}

	public string GetAttribute(string heroID, string attributeName)
	{
		return GetAttribute(heroID, attributeName, false);
	}

	public int GetMaxLevel(string heroID)
	{
		HeroData heroData = Seek(heroID);
		if (heroData != null)
		{
			heroData.EnsureDataCached();
			for (int i = 1; i < 1000; i++)
			{
				if (heroData.registryData.to(i) == null)
				{
					return i - 1;
				}
			}
		}
		Debug.Log("ERROR: Unknown hero ID, or hero data malformed: " + heroID);
		return -1;
	}

	private void CacheSimpleIDList()
	{
		mAllIDs = new string[mData.Count];
		int num = 0;
		foreach (HeroData mDatum in mData)
		{
			mAllIDs[num++] = mDatum.id;
		}
	}

	private HeroData Seek(string heroID)
	{
		foreach (HeroData mDatum in mData)
		{
			if (mDatum.id.Equals(heroID, StringComparison.OrdinalIgnoreCase))
			{
				return mDatum;
			}
		}
		return null;
	}

	private string GetAttribute(string heroID, string attributeName, bool noWarningIfNotFound)
	{
		HeroData heroData = Seek(heroID);
		if (heroData == null)
		{
			Debug.Log("WARNING: Could not find hero: " + heroID);
			return string.Empty;
		}
		heroData.EnsureDataCached();
		if (heroData.registryData.hasAttribute(attributeName))
		{
			return Singleton<Localizer>.instance.Parse(heroData.registryData[attributeName]);
		}
		// level = Mathf.Max(level, 1);
		// SDFTreeNode sDFTreeNode = heroData.registryData.to(level);
		// if (sDFTreeNode != null && sDFTreeNode.hasAttribute(attributeName))
		// {
		// 	return Singleton<Localizer>.instance.Parse(sDFTreeNode[attributeName]);
		// }
		if (!noWarningIfNotFound)
		{
			Debug.Log("WARNING: Could not find hero attribute: " + attributeName);
		}
		return string.Empty;
	}

	// private int GetHeroLevel(string heroID)
	// {
	// 	HeroData heroData = Seek(heroID);
	// 	string text = heroData.registryData["levelMatchOtherHero"];
	// 	if (text != string.Empty)
	// 	{
	// 		return Singleton<Profile>.instance.GetHeroLevel(text);
	// 	}
	// 	return Singleton<Profile>.instance.GetHeroLevel(heroID);
	// }
}

using System.Collections.Generic;
using UnityEngine;

public class SelectHeroesController : SUIScrollList.IController
{
	public class HeroCell : ListCells.IconNameCounter
	{
		public HeroCell(SUISprite cursor)
			: base("default18", cursor, true)
		{
			Vector2 vector = new Vector2(0f, 10f);
			iconOffset += vector;
			labelOffset += vector;
			counterOffset += vector;
			cursorOffset = iconOffset;
		}
	}

	private class Data
	{
		public string id = string.Empty;

		public string displayName = string.Empty;

		public string iconFile = string.Empty;

		public string desc = string.Empty;
	}

	private const float kCellPriority = 1f;

	private const float kCursorPriority = 0.5f;

	private const int kTitleMaxWidth = 156;

	private const float kSelectedCellAlpha = 0.4f;

	private List<Data> mData = new List<Data>();

	private IconGlowWidget mListCursor;

	public SelectHeroesController(Rect listArea, Vector2 cellSize)
	{
		mListCursor = new IconGlowWidget();
		mListCursor.priority = 0.5f;
		mListCursor.visible = false;
		string[] allIDs = Singleton<HeroesDatabase>.instance.allIDs;
		foreach (string text in allIDs)
		{
			Data item = new Data
			{
				id = text,
				displayName = Singleton<HeroesDatabase>.instance.GetAttribute(text, "displayName"),
				iconFile = Singleton<HeroesDatabase>.instance.GetAttribute(text, "icon"),
				desc = Singleton<HeroesDatabase>.instance.GetAttribute(text, "desc")
			};
			mData.Add(item);
		}
	}

	public void Update()
	{
	}

	public void Destroy()
	{
		mListCursor.Destroy();
		mListCursor = null;
	}

	public int FindIndex(string charmID)
	{
		int num = 0;
		foreach (Data mDatum in mData)
		{
			if (mDatum.id == charmID)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public string FindID(int index)
	{
		try
		{
			return mData[index].id;
		}
		catch
		{
			return string.Empty;
		}
	}

	public string GetIcon(int index)
	{
		try
		{
			return mData[index].iconFile;
		}
		catch
		{
			return string.Empty;
		}
	}

	public string GetDescription(int index)
	{
		try
		{
			return mData[index].desc;
		}
		catch
		{
			return string.Empty;
		}
	}

	public int ScrollList_NumEntries()
	{
		return mData.Count;
	}

	public SUIScrollList.Cell ScrollList_CreateCell()
	{
		HeroCell heroCell = new HeroCell(mListCursor);
		heroCell.priority = 1f;
		heroCell.title.maxWidth = 156;
		return heroCell;
	}

	public void ScrollList_DrawCellContent(SUIScrollList.Cell c, int dataIndex, bool isSelected)
	{
		HeroCell heroCell = (HeroCell)c;
		if (isSelected)
		{
			heroCell.cursorRef.visible = true;
		}
		else if (heroCell.isSelected)
		{
			heroCell.cursorRef.visible = false;
		}
		heroCell.isSelected = isSelected;
		heroCell.icon.texture = mData[dataIndex].iconFile;
		heroCell.title.text = mData[dataIndex].displayName;
		heroCell.CheckIfRecommended(mData[dataIndex].id);
	}
}

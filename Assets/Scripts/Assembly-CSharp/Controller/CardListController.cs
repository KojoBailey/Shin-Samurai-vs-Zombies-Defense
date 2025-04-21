using System.Collections.Generic;
using UnityEngine;

public class CardListController : SUIScrollList.IController {
	private List<CardData.Card> m_cards;

	public List<CardData.Card> cards {
		get {
			return m_cards;
		}
	}

	public List<CardData.Card> data {
		get {
			return m_cards;
		}
	}

	public CardListController(Rect listArea, Vector2 cellSize) {
		m_cards = new List<CardData.Card>();
		m_cards.Add(new CardData.Card {id = "hero_samurai", texture = "Samurai", title = "Samurai"});
        m_cards.Add(new CardData.Card {id = "hero_ronin", texture = "Ronin", title = "Ronin"});
	}

	public void Update() {}

	public void Destroy() {}

	public int ScrollList_NumEntries() {
		return m_cards.Count;
	}

	public SUIScrollList.Cell ScrollList_CreateCell() {
		return new StoreListCell();
	}

	public void ScrollList_DrawCellContent(SUIScrollList.Cell c, int dataIndex, bool isSelected) {
		CardData.Card card_data = m_cards[dataIndex];
	}
}

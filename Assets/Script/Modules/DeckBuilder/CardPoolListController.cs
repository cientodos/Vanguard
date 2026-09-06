using EnumData;
using PolyAndCode.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Vanguard.CardSystem.UI;
using Vanguard.Data.Databases;
using Vanguard.Data.DataModels;

namespace Vanguard.DeckBuilder.UI
{
    public class CardPoolFilterCriteria
    {
        public string searchText = "";
        public CardCategoryType category = CardCategoryType.All;
        public CountryType? selectedCountry = null;
    }

    public class CardPoolListController : MonoBehaviour, IRecyclableScrollRectDataSource
    {
        #region Header
        [Header("팝업 및 상세 패널 연결")]
        [SerializeField] private CardContextMenu contextMenu;
        [SerializeField] private CardDisplayPanel cardDisPlayPanel;
        [SerializeField] private CardReader cardReader;
        [SerializeField] private RecyclableScrollRect recyclableScrollRect;

        private List<CardData> displayCardList = new List<CardData>();
        private CardPoolFilterCriteria currentFilter = new CardPoolFilterCriteria();
        private bool isInitialized = false;
        #endregion
        private void OnEnable()
        {
            CardItemCell.OnCellClicked += HandleCellClick;
        }
        private void OnDisable()
        {
            CardItemCell.OnCellClicked -= HandleCellClick; 
        }

        private void Awake()
        {
            recyclableScrollRect.DataSource = this;
            if (recyclableScrollRect != null)
            {
                recyclableScrollRect.DataSource = this;
            }
        }

        private void Start()
        {
            SetCardDataList(CardDatabase.GetAll());
        }

        public void SetCardDataList(List<CardData> rawCardList)
        {
            displayCardList = rawCardList != null ? new List<CardData>(rawCardList) : new List<CardData>();
            UpdateScrollRect();
        }

        public void RefreshCardPoolUI()
        {
           
            var allCards = CardDatabase.GetAll();

            var filteredCards = allCards.Where(card =>
            {
                bool matchesSearch = string.IsNullOrEmpty(currentFilter.searchText) ||
                                     card.cardName.Contains(currentFilter.searchText) ||
                                     card.id.ToString().Contains(currentFilter.searchText);

                bool matchesCountry = !currentFilter.selectedCountry.HasValue ||
                                      (card.countryList != null && card.countryList.Contains(currentFilter.selectedCountry.Value));

                bool matchesCategory = false;

                switch (currentFilter.category)
                {
                    case CardCategoryType.All: matchesCategory = true; break;
                    case CardCategoryType.Grade0: matchesCategory = (card.grade == 0); break;
                    case CardCategoryType.Grade1: matchesCategory = (card.grade == 1); break;
                    case CardCategoryType.Grade2: matchesCategory = (card.grade == 2); break;
                    case CardCategoryType.Grade3: matchesCategory = (card.grade == 3); break;
                    case CardCategoryType.Order: matchesCategory = (card.cardType == CardType.Order); break;
                }

                return matchesSearch && matchesCountry && matchesCategory;
            });

            displayCardList = filteredCards.ToList();
            UpdateScrollRect();
        }

        private void UpdateScrollRect()
        {
            if (recyclableScrollRect == null) return;

            if (!isInitialized)
            {
                recyclableScrollRect.Initialize(this);
                isInitialized = true;
            }
            else
            {
                recyclableScrollRect.ReloadData();
            }
        }

        public void SetCategory(int categoryIndex)
        {
            currentFilter.category = (CardCategoryType)categoryIndex;
            RefreshCardPoolUI();
        }

        public void SetCountry(int countryIndex)
        {
            if (countryIndex == 8) currentFilter.selectedCountry = null;
            else currentFilter.selectedCountry = (CountryType)(countryIndex - 1);

            RefreshCardPoolUI();
        }

        public void SetSearchText(string text)
        {
            currentFilter.searchText = text.Trim();
            RefreshCardPoolUI();
        }

        private void HandleCellClick(PointerEventData.InputButton button, CardData data, Vector2 mousePos, Sprite cardSprite)
        {
            if (button == PointerEventData.InputButton.Left && cardDisPlayPanel != null)
            {
                cardDisPlayPanel.UpdateDetailView(data, cardSprite);
            }
            else if (button == PointerEventData.InputButton.Right && contextMenu != null)
            {
                contextMenu.ShowMenu(data, mousePos);
            }
        }

        #region IRecyclableScrollRectDataSource
        public int GetItemCount() => displayCardList != null ? displayCardList.Count : 0;

        public void SetCell(ICell cell, int index)
        {
            if (cell is CardItemCell itemCell && index >= 0 && index < displayCardList.Count)
            {
                itemCell.ConfigureCell(displayCardList[index]);
            }
        }
        #endregion
    }
}
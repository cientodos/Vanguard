using EnumData;
using Google.GData.Extensions;
using PolyAndCode.UI;
using System;
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
        public CountryType? selectedCountry = null; // 프로젝트의 Country Enum 타입명 적용
    }
    public class CardPoolListController : MonoBehaviour, IRecyclableScrollRectDataSource
    {

        [Header("팝업 및 상세 패널 연결")]
        [SerializeField] private CardContextMenu contextMenu;
        [SerializeField] private CardDisplayPanel cardDisPlayPanel;
        [SerializeField] private CardReader cardReader;
        [SerializeField] private RecyclableScrollRect recyclableScrollRect;
        private List<CardData> displayCardList = new List<CardData>();
        private bool isInitialized = false;

        private void OnEnable()
        {
            CardItemCell.OnCellClicked += HandleCellClick;
        }
        private void OnDisable()
        {
            CardItemCell.OnCellClicked -= HandleCellClick;
        }

        private CardPoolFilterCriteria currentFilter = new CardPoolFilterCriteria();

        // 💡 Inspector에서 버튼/InputField에 직접 연결할 이벤트 핸들러들
        public void SetCategory(int categoryIndex)
        {
            currentFilter.category = (CardCategoryType)categoryIndex;
            RefreshCardPoolUI();
        }

        public void SetCountry(int countryIndex)
        {
            if (countryIndex == 8)
            {
                currentFilter.selectedCountry = null;
            }
            else
            {
                currentFilter.selectedCountry = (CountryType)(countryIndex - 1);
            }

            RefreshCardPoolUI();
        }

        public void SetSearchText(string text)
        {
            currentFilter.searchText = text.Trim();
            RefreshCardPoolUI();
        }

        // 💡 필터링 및 UI 갱신 로직
        public void RefreshCardPoolUI()
        {
            if (cardReader == null || cardReader.DataList == null) return;

            // 1. LINQ 조건 검색 수행
            var filteredCards = cardReader.DataList.Where(card =>
            {
                // 1-1. 검색어 검사
                bool matchesSearch = string.IsNullOrEmpty(currentFilter.searchText) ||
                                     card.cardName.Contains(currentFilter.searchText) ||
                                     card.id.ToString().Contains(currentFilter.searchText);

                // 1-2. 국가 검사 (countryList에 선택된 국가가 포함되어 있는지)
                bool matchesCountry = !currentFilter.selectedCountry.HasValue ||
                                      (card.countryList != null && card.countryList.Contains(currentFilter.selectedCountry.Value));

                // 1-3. 카테고리(그레이드 0~3, 오더, All) 검사
                bool matchesCategory = false;

                switch (currentFilter.category)
                {
                    case CardCategoryType.All:
                        matchesCategory = true;
                        break;

                    case CardCategoryType.Grade0:
                        // Grade가 0이고, CardType이 Order가 아닌 경우 (또는 card.grade == 0)
                        matchesCategory = (card.grade == 0);
                        break;

                    case CardCategoryType.Grade1:
                        matchesCategory = (card.grade == 1);
                        break;

                    case CardCategoryType.Grade2:
                        matchesCategory = (card.grade == 2);
                        break;

                    case CardCategoryType.Grade3:
                        matchesCategory = (card.grade == 3);
                        break;

                    case CardCategoryType.Order:
                        // 카드 타입이 Order인 경우 (EnumData의 CardType에 맞춰 조건 작성)
                        matchesCategory = (card.cardType == CardType.Order);
                        break;
                }

                return matchesSearch && matchesCountry && matchesCategory;
            });

            // 2. [RecyclableScrollRect 핵심] 필터링된 결과로 displayCardList를 새로 채움
            displayCardList = filteredCards.ToList();

            // 3. 리스트가 완성된 후 ScrollRect 데이터 갱신 알림
            UpdateScrollRect();
        }

        void Awake()
        {
            recyclableScrollRect.DataSource = this;
        }
        void Start()
        {
            if (cardReader != null && cardReader.DataList != null)
            {
                SetCardDataList(cardReader.DataList);
            }
            else
            {
                Debug.LogError("[CardPoolListController] CardReader 또는 DataList가 비어있습니다!");
            }
        }
        public void SetCardDataList(List<CardData> rawCardList)
        {
            Debug.Log($"[CardPoolListController] 전달받은 원본 카드 수: {(rawCardList != null ? rawCardList.Count : 0)}");
            displayCardList.Clear();

            if (rawCardList != null)
            {
                foreach (var rawCard in rawCardList)
                {
                    if (rawCard.spriteAssetKeys.Count <= 1)
                    {
                        displayCardList.Add(rawCard);
                    }
                    else
                    {
                        for (int i = 0; i < rawCard.spriteAssetKeys.Count; i++)
                        {
                            CardData rarityCard = new CardData
                            {
                                id = rawCard.id,
                                cardName = rawCard.cardName,
                                grade = rawCard.grade,
                                power = rawCard.power,
                                shield = rawCard.shield,
                                critical = rawCard.critical,
                                countryList = rawCard.countryList != null ? new List<CountryType>(rawCard.countryList) : new List<CountryType>(),
                                tribeList = rawCard.tribeList != null ? new List<TribeType>(rawCard.tribeList) : new List<TribeType>(),
                                triggerType = rawCard.triggerType,
                                cardType = rawCard.cardType,
                                flavorText = rawCard.flavorText,
                                effectText = rawCard.effectText,
                                spriteAssetKeys = new List<string> { rawCard.spriteAssetKeys[i] },
                            };
                            displayCardList.Add(rarityCard);
                        }
                    }
                }
            }

            UpdateScrollRect();
        }

        private void UpdateScrollRect()
        {
            if (recyclableScrollRect == null) return;

            // 최초 1회만 Initialize 호출
            if (!isInitialized)
            {
                recyclableScrollRect.Initialize(this);
                isInitialized = true;
            }
            else
            {
                // 이미 초기화된 이후에는 ReloadData만 호출
                recyclableScrollRect.ReloadData();
            }
        }


        private void HandleCellClick(PointerEventData.InputButton button, CardData data, Vector2 mousePos, Sprite cardSprite)
        {
            if (button == PointerEventData.InputButton.Left)
            {
                if (cardDisPlayPanel != null)
                {
                    cardDisPlayPanel.UpdateDetailView(data, cardSprite);
                }
            }
            else if (button == PointerEventData.InputButton.Right)
            {
                if (contextMenu != null)
                {
                    contextMenu.ShowMenu(data, mousePos);
                }
            }
        }

        #region IRecyclabeScrollRectDataSource 구현

        public int GetItemCount()
        {
            return displayCardList != null ? displayCardList.Count : 0;
        }
        public void SetCell(ICell cell, int index)
        {
            var itemCell = cell as CardItemCell;
            if (itemCell != null && index >= 0 && index < displayCardList.Count)
            {
                itemCell.ConfigureCell(displayCardList[index]);
            }
        }
        #endregion
    }
}
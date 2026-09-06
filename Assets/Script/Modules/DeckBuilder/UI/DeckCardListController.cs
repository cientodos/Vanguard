using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vanguard.CardSystem.UI;
using Vanguard.Data.Databases;
using Vanguard.Data.DataModels;
using System.Linq;

namespace Vanguard.DeckBuilder.UI
{
    public class DeckCardListController : MonoBehaviour
    {
        #region Header
        [SerializeField] private CardDisplayPanel cardDisPlayPanel;
        [Header("UI요소")]
        [SerializeField] private Transform mainDeckContainer;
        [SerializeField] private Transform extraDeckContainer;
        [SerializeField] private Transform rideDeckContainer;
        [SerializeField] private GameObject cardItemPrefab;
        [SerializeField] private Text mainTitle;
        [SerializeField] private Text deckCount;
        [SerializeField] private Text rideTitle;
        [SerializeField] private Text rideDeckCount;
        [SerializeField] private Text extraTitle;
        [SerializeField] private Text extraDeckCount;
        [SerializeField] private Image cardImage;
        #endregion

        private void OnEnable()
        {
            CardItemCell.OnCellClicked += HandleCellClick;

            if (DeckManager.Instance != null)
            {
                DeckManager.Instance.OnDeckUpdated += OnDeckUpdatedHandler;
            }

            if (CardDatabase.isInitialized)
            {
                RefreshUI();
            }
        }

        private void OnDisable()
        {
            CardItemCell.OnCellClicked -= HandleCellClick;

            if (DeckManager.Instance != null)
            {
                DeckManager.Instance.OnDeckUpdated -= OnDeckUpdatedHandler;
            }
        }
        private void OnDeckUpdatedHandler(DeckData updatedDeck)
        {
            RefreshUI(updatedDeck);
        }

        public void RefreshUI(DeckData targetDeck = null)
        {
            if (targetDeck == null) targetDeck = GetTargetDeck();

            if (targetDeck == null)
            {
                ClearAllContainers();
                return;
            }

            Debug.Log($"[DeckCardListController] 불러온 덱 이름: {targetDeck.deckName}, MainCard 수: {targetDeck.mainDeckCardIds?.Count}");

            ClearAllContainers();

            if (deckCount != null) deckCount.text = $"{targetDeck.mainDeckCardIds.Count} / 50";
            if (rideDeckCount != null) rideDeckCount.text = $"{targetDeck.rideDeckCardIds.Count}";
            if (extraDeckCount != null) extraDeckCount.text = $"{targetDeck.extraDeckCardIds.Count}";

            Canvas.ForceUpdateCanvases();

            DisplayCardsInContainer(targetDeck.rideDeckCardIds, rideDeckContainer);
            DisplayCardsInContainer(targetDeck.mainDeckCardIds, mainDeckContainer);
            DisplayCardsInContainer(targetDeck.extraDeckCardIds, extraDeckContainer);
        }

        private DeckData GetTargetDeck()
        {
            var manager = DeckManager.Instance;
        
            if (manager.deckList == null || manager.deckList.Count == 0)
            {
                return null;
            }

            // 이미 선택된 덱이 있으면 반환
            if (manager.currentSelectedDeck != null)
            {
                return manager.currentSelectedDeck;
            }
            manager.currentSelectedDeck = manager.deckList[0];
            return manager.currentSelectedDeck;
        }


        private void DisplayCardsInContainer(List<int> cardIds, Transform container)
        {
            var sortedCardIds = cardIds.OrderByDescending(id => CardDatabase.Get(id)?.grade ?? 0).ThenBy(id => id).ToList();

            foreach (int cardId in sortedCardIds)
            {
                // DB에서 데이터 꺼내오기
                CardData data = CardDatabase.Get(cardId);
                GameObject cardObj = Instantiate(cardItemPrefab, container);
                cardObj.transform.localScale = Vector3.one;

                CardItemCell itemCell = cardObj.GetComponent<CardItemCell>();
                if (itemCell != null)
                {
                    itemCell.ConfigureCell(data);
                }

                // 💡 Addressables 이미지 키를 $"card_{cardId}"가 아닌 data.spriteAssetKeys[0]으로 로드
                Image img = cardObj.GetComponentInChildren<Image>();

                string addressKey = (data.spriteAssetKeys != null && data.spriteAssetKeys.Count > 0) ? data.spriteAssetKeys[0] : $"card_{cardId}";

                Addressables.LoadAssetAsync<Sprite>(addressKey).Completed += handle =>
                {

                    if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                    {
                        img.sprite = handle.Result;
                    }
                };
            }
        }

        private void HandleCellClick(PointerEventData.InputButton button, CardData data, Vector2 mousePos, Sprite cardSprite)
        {
            if (button == PointerEventData.InputButton.Left && cardDisPlayPanel != null)
            {
                cardDisPlayPanel.UpdateDetailView(data, cardSprite);
            }
        }

        private void ClearAllContainers()
        {
            ClearCardContainer(mainDeckContainer);
            ClearCardContainer(extraDeckContainer);
            ClearCardContainer(rideDeckContainer);
        }

        private void ClearCardContainer(Transform container)
        {
            foreach (Transform child in container)
            {
                if (child.GetComponent<Text>() == null)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using Vanguard.Data.DataModels;

namespace Vanguard.DeckBuilder.UI
{
    public class DeckItemCell : MonoBehaviour
    {
        [SerializeField] private Transform cardContainer;
        [SerializeField] private GameObject cardItemPrefab;
        [SerializeField] private Text mainTitle;
        [SerializeField] private Text deckCount;

        private void OnEnable()
        {
            if (DeckManager.Instance != null)
            {
                DeckManager.Instance.OnDeckUpdated += OnDeckUpdatedHandler;
                // 💡 켜지는 즉시 현재 덱 정보로 UI 초기화 (반응 지연 방지)
                RefreshUI(DeckManager.Instance.currentSelectedDeck);
            }
        }

        private void OnDisable()
        {
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
                if (mainTitle != null) mainTitle.text = "덱이 없습니다.";
                if (deckCount != null) deckCount.text = "0 / 50";
                ClearCardContainer();
                return;
            }

            if (mainTitle != null) mainTitle.text = targetDeck.deckName;
            if (deckCount != null) deckCount.text = $"{targetDeck.mainDeckCardIds.Count} / 50";

            DisplayDeckCards(targetDeck.mainDeckCardIds);
        }

        private DeckData GetTargetDeck()
        {
            var manager = DeckManager.Instance;
            if (manager == null || manager.deckList == null || manager.deckList.Count == 0) return null;

            if (manager.currentSelectedDeck != null && manager.currentSelectedDeck.mainDeckCardIds != null)
            {
                return manager.currentSelectedDeck;
            }

            manager.SelectDeck(manager.deckList[0]);
            return manager.currentSelectedDeck;
        }

        private void DisplayDeckCards(List<int> cardIds)
        {
            ClearCardContainer();

            foreach (int cardId in cardIds)
            {
                GameObject cardObj = Instantiate(cardItemPrefab, cardContainer);
                cardObj.transform.localScale = Vector3.one;

                Image cardImage = cardObj.GetComponentInChildren<Image>();
                string addressKey = $"card_{cardId}";

                Addressables.LoadAssetAsync<Sprite>(addressKey).Completed += handle =>
                {
                    if (cardObj == null || cardImage == null) return; // 비동기 완료 시점 파괴 검사

                    if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                    {
                        cardImage.sprite = handle.Result;
                    }
                    else
                    {
                        Debug.LogWarning($"[Addressables] 로드 실패 Key: {addressKey}");
                    }
                };
            }

            // UI Layout 즉시 재계산
            if (cardContainer is RectTransform rectTransform)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }
        }

        private void ClearCardContainer()
        {
            foreach (Transform child in cardContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
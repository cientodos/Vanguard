using System;
using UnityEngine;
using UnityEngine.UI;
using Vanguard.Data.DataModels;

namespace Vanguard.CardSystem.UI
{
    public class CardContextMenu : MonoBehaviour
    {
        #region
        [Header("UI 컴포넌트")]
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Button addButton;
        [SerializeField] private Button removeButton;
        #endregion
        private CardData targetCardData;
        public static event Action<string> OnMenuErrorOccurred;
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            addButton.onClick.AddListener(OnClickAdd);
            removeButton.onClick.AddListener(OnClickRemove);
        }


        private void Update()
        {
            AutoClose();
        }

        private void AutoClose()
        {
            if (gameObject.activeSelf && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
            {
                if (!RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition))
                {
                    CloseMenu();
                }
            }
        }

        public void ShowMenu(CardData cardData, Vector2 mousePosition)
        {
            targetCardData = cardData;

            rectTransform.position = mousePosition;

            Vector2 pivot = new Vector2(0.5f, 0.5f); 
            if (mousePosition.y - rectTransform.rect.height < 0) pivot.y = 0f; 
            rectTransform.pivot = pivot;

            gameObject.SetActive(true);
        }


        private void OnClickAdd()
        {
            if (targetCardData != null)
            {
                DeckData currentDeck = DeckManager.Instance.currentSelectedDeck;

                if (currentDeck == null)
                {
                    OnMenuErrorOccurred?.Invoke("카드를 추가할 덱이 선택되지 않았습니다.");
                    CloseMenu();
                    return;
                }
                DeckManager.Instance.AddCardToCurrentDeck(targetCardData.id);
            }
            CloseMenu();
        }
        private void OnClickRemove()
        {
            if (targetCardData != null)
            {
                DeckData currentDeck = DeckManager.Instance.currentSelectedDeck;

                if (currentDeck == null)
                {
                    OnMenuErrorOccurred?.Invoke("카드를 제거할 덱이 선택되지 않았습니다.");
                    CloseMenu();
                    return;
                }

                // 💡 3. 덱에서 카드 제거
                if (!DeckManager.Instance.RemoveCardFromDeck(currentDeck, targetCardData.id))
                {
                    OnMenuErrorOccurred?.Invoke("해당 덱에 카드가 존재하지 않습니다.");
                }
            }
            CloseMenu();
        }
        private void CloseMenu()
        {
            gameObject.SetActive(false);
        }
    }
}
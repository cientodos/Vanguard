using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vanguard.CardSystem.UI;
using Vanguard.Data.DataModels;


public class DeckUIController : MonoBehaviour
{   
    #region Header
    [Header("UI 토글")]
    private bool isRideMode = false;
    private bool isDeleteMode = false;

    [Header("UI 컴포넌트")]
    [SerializeField] public Button saveButton;
    [SerializeField] public Button openCardList;
    [SerializeField] public Button toggleRideButton;
    [SerializeField] public Button toggleDeleteButton;
    [SerializeField] private GameObject cardPanel;
    [SerializeField] private GameObject deckPanel;
    #endregion
    DeckManager manager;
    private CardData targetCardData;
    private void OnEnable()
    {
        CardItemCell.OnCellClicked += HandleCellClicked;
    }

    private void OnDisable()
    {
        CardItemCell.OnCellClicked -= HandleCellClicked;
    }
    private void HandleCellClicked(PointerEventData.InputButton button, CardData cardData, Vector2 position, Sprite sprite)
    {
        if (button == PointerEventData.InputButton.Left)
        {
            OnCardClicked(cardData);
        }
    }
    public void OnClickSaveDeck()
    {
        DeckManager.Instance.SaveDecks();
    }

    public void OnClickToggleRideDeck()
    {
        isRideMode = !isRideMode;

        if (isRideMode)
        {
            isDeleteMode = false;
        }
    }

    public void OnClickToggleCardDelete()
    {
        isDeleteMode = !isDeleteMode;

        if (isDeleteMode)
        {
            isRideMode = false;
        }
    }


    public void OnCardClicked(CardData clickedCardData)
    {
        int cardId = clickedCardData.id;

        if (isRideMode)
        {
            // [라이드덱 모드 ON] -> ToggleRideDeckCard 실행
            DeckManager.Instance?.ToggleRideDeckCard(cardId);
        }
        else if (isDeleteMode)
        {
            // [삭제 모드 ON] -> RemoveCardFromDeck 실행
            var currentDeck = DeckManager.Instance?.currentSelectedDeck;
            if (currentDeck != null)
            {
                DeckManager.Instance.RemoveCardFromDeck(currentDeck, cardId);
            }
        }
        else
        {
            // [기본 모드] -> 상세 보기 등
            targetCardData = clickedCardData;
            Debug.Log($"[Normal] 카드 선택: {clickedCardData.cardName}");
        }
    }

    public void DeckListClear()
    {
        if (manager.deckList == null || manager.deckList.Count == 0)
        {
            return;
        }
    }

    public void OnClickOpenMatSettings()
    {
      //매트 세팅창열기
    }

    public void OnClickOpenSleeveSettings()
    {
        //슬리브 세팅창열기
    }

    public void OpenCardPanel()
    {
        cardPanel.SetActive(true);
        deckPanel.SetActive(false);
    }
    public void CloseCardPanel()
    {
        cardPanel.SetActive(false);
        deckPanel.SetActive(true);
    }


}
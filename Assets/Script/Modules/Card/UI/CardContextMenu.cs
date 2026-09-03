using Vanguard.Data.DataModels;
using UnityEngine;
using UnityEngine.UI;

namespace Vanguard.CardSystem.UI { 
public class CardContextMenu : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Button addButton;
    [SerializeField] private Button removeButton;

    private CardData targetCardData;

    private void Awake()
    {
        if (rectTransform != null) rectTransform = GetComponent<RectTransform>();
        if (addButton != null)
            addButton.onClick.AddListener(OnClickAdd);
        if (removeButton != null)
            removeButton.onClick.AddListener(OnClickRemove);

    }


    private void Update()
    {
        if (gameObject.activeSelf && (Input.GetMouseButton(0) || Input.GetMouseButton(1)))
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

        Vector2 pivot = new Vector2(0.5f, 0.5f); // 기본: 마우스 우측 아래로 팝업 생성
        if (mousePosition.x + rectTransform.rect.width > Screen.width) pivot.x = 1f; // 오른쪽 구석이면 왼쪽으로
        if (mousePosition.y - rectTransform.rect.height < 0) pivot.y = 0f; // 아래 구석이면 위로
        rectTransform.pivot = pivot;

        gameObject.SetActive(true);
    }


    private void OnClickAdd()
    {

        
        if (targetCardData != null)
        {
            // 💡 1. 현재 선택된 덱이 존재하는지 검사
            DeckData currentDeck = DeckManager.Instance.currentSelectedDeck;

            if (currentDeck == null)
            {
                Debug.LogWarning("[CardContextMenu] 카드를 추가할 덱이 선택되지 않았습니다!");
                CloseMenu();
                return;
            }

            // 💡 2. 덱에 카드 추가 시도
            if (DeckManager.Instance.AddCardToCurrentDeck(targetCardData.id))
            {
                Debug.Log($"<color=green>[덱에 추가 성공]</color> '{currentDeck.deckName}' 덱에 {targetCardData.cardName}(ID:{targetCardData.id}) 추가됨. (현재 {currentDeck.mainDeckCardIds.Count}/30장)");

                // TODO: 덱 카드 슬롯 UI 리프레시 호출
            }
            else
            {
                Debug.LogWarning($"[덱에 추가 실패] 조건 미충족 (50장 초과 또는 동일 카드 4장 초과)");
            }
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
                Debug.LogWarning("[CardContextMenu] 카드를 제거할 덱이 선택되지 않았습니다!");
                CloseMenu();
                return;
            }

            // 💡 3. 덱에서 카드 제거
            if (DeckManager.Instance.RemoveCardFromDeck(currentDeck, targetCardData.id))
            {
                Debug.Log($"<color=red>[덱에서 제거 성공]</color> '{currentDeck.deckName}' 덱에서 {targetCardData.cardName}(ID:{targetCardData.id}) 제거됨. (남은 카드: {currentDeck.mainDeckCardIds.Count}장)");

                // TODO: 덱 카드 슬롯 UI 리프레시 호출
            }
            else
            {
                Debug.LogWarning($"[덱에서 제거 실패] 해당 덱에 카드가 존재하지 않습니다.");
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
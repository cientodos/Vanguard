using UnityEngine;
using Vanguard.Data.DataModels;


public class DeckUIController : MonoBehaviour
{
    [Header("현재 선택된 덱 ID")]
    public string currentSelectedDeckId;

    [Header("임시 클립보드 (복사/붙여넣기용)")]
    private DeckData copiedDeckData;

    #region 버튼 연동 전용 void 메서드

    // 2. 덱 저장 버튼
    public void OnClick_SaveDeck()
    {
        DeckManager.Instance.SaveDecks();
        Debug.Log("[UI] 현재 덱 정보가 저장되었습니다.");
    }

    // 3. 새로 작성 (새 덱 만들기) 버튼
    public void OnClick_CreateNewDeck()
    {
        if (DeckManager.Instance.CreateNewDeck("새 덱", out DeckData newDeck))
        {
            currentSelectedDeckId = newDeck.deckId;
            Debug.Log($"[UI] 새 덱이 생성되었습니다. (ID: {newDeck.deckId})");
            // TODO: UI 목록 리프레시 / 덱 편집 화면 갱신 로직
        }
        else
        {
            Debug.LogWarning("[UI] 덱 생성 실패: 저장 가능 개수(30개)를 초과했습니다.");
        }
    }

    // 4. 덱 지우기 버튼
    public void OnClick_DeleteDeck()
    {
        if (string.IsNullOrEmpty(currentSelectedDeckId))
        {
            Debug.LogWarning("[UI] 선택된 덱이 없습니다.");
            return;
        }

        if (DeckManager.Instance.DeleteDeck(currentSelectedDeckId))
        {
            Debug.Log($"[UI] 덱이 삭제되었습니다. (ID: {currentSelectedDeckId})");
            currentSelectedDeckId = string.Empty;
            // TODO: UI 목록 리프레시
        }
    }

    // 5. 매트 설정 버튼
    public void OnClick_OpenMatSettings()
    {
        Debug.Log("[UI] 매트 설정 창 열기");
        // TODO: 매트 변경 UI 팝업 토글
    }

    // 6. 슬리브 설정 버튼
    public void OnClick_OpenSleeveSettings()
    {
        Debug.Log("[UI] 슬리브 설정 창 열기");
        // TODO: 카드 뒷면/슬리브 UI 팝업 토글
    }

    // 8. 덱 복사 버튼
    public void OnClick_CopyDeck()
    {
        DeckData targetDeck = DeckManager.Instance.deckList.Find(d => d.deckId == currentSelectedDeckId);
        if (targetDeck == null)
        {
            Debug.LogWarning("[UI] 복사할 덱이 선택되지 않았습니다.");
            return;
        }

        // Deep Copy (깊은 복사)로 복사본 생성
        copiedDeckData = new DeckData(targetDeck.deckName + " - 복사본");
        copiedDeckData.mainDeckCardIds = new System.Collections.Generic.List<int>(targetDeck.mainDeckCardIds);

        Debug.Log($"[UI] 덱 '{targetDeck.deckName}' 복사 완료.");
    }

    // 9. 덱 붙여넣기 버튼
    public void OnClick_PasteDeck()
    {
        if (copiedDeckData == null)
        {
            Debug.LogWarning("[UI] 복사된 덱 데이터가 없습니다.");
            return;
        }

        if (DeckManager.Instance.deckList.Count >= DeckManager.MAX_DECK_COUNT)
        {
            Debug.LogWarning("[UI] 덱 저장 한도(30개)를 초과하여 붙여넣을 수 없습니다.");
            return;
        }

        // 새로운 GUID를 부여받아 덱 목록에 등록
        DeckData pastedDeck = new DeckData(copiedDeckData.deckName);
        pastedDeck.mainDeckCardIds = new System.Collections.Generic.List<int>(copiedDeckData.mainDeckCardIds);

        DeckManager.Instance.deckList.Add(pastedDeck);
        DeckManager.Instance.SaveDecks();

        currentSelectedDeckId = pastedDeck.deckId;
        Debug.Log($"[UI] 덱 붙여넣기 성공! (ID: {pastedDeck.deckId})");
        // TODO: UI 목록 리프레시
    }
    #endregion
}
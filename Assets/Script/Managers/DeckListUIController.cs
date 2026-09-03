using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vanguard.Data.DataModels;
using Vanguard.DeckBuilder.UI;

public class DeckListUIController : MonoBehaviour
{
    [Header("스크롤뷰 연결")]
    [SerializeField] private Transform contentTransform; // Viewport -> Content
    [Header("맨 위 고정 UI")]
    [SerializeField] private Button editModeButton; // 맨 위 '덱 편집' 버튼
    [Header("프리팹 연결")]
    [SerializeField] private GameObject deckSlotPrefab;
    [SerializeField] private GameObject createDeckButtonPrefab;
    [Header("이름 변경 팝업 UI")]
    [SerializeField] private GameObject renamePopupPanel; // 팝업 패널
    [SerializeField] private InputField renameInputField; // 또는 TMP_InputField
    [SerializeField] private Button confirmRenameButton;
    [SerializeField] private Button cancelRenameButton;
    private DeckData targetRenameDeck;
    private bool isEditMode = false; // 현재 편집 모드 상태 플래그

    private void Start()
    {
        if (editModeButton != null)
        {
            editModeButton.onClick.RemoveAllListeners();
            editModeButton.onClick.AddListener(ToggleEditMode);
        }

        RefreshDeckListUI();
    }

    // 💡 덱 편집 버튼 클릭 시 토글
    public void ToggleEditMode()
    {
        isEditMode = !isEditMode;


        RefreshDeckListUI();
    }

    public void RefreshDeckListUI()
    {
        if (contentTransform == null || DeckManager.Instance == null) return;

        // 💡 1. 맨 위 고정 버튼(editModeButton)을 제외한 나머지 동적 자식만 삭제
        for (int i = contentTransform.childCount - 1; i >= 0; i--)
        {
            Transform child = contentTransform.GetChild(i);

            // 맨 위에 고정 배치된 EditModeButton은 파괴하지 않고 스킵
            if (editModeButton != null && child == editModeButton.transform)
                continue;

            Destroy(child.gameObject);
        }

        List<DeckData> allDecks = DeckManager.Instance.deckList;

        // 2. 덱 슬롯 동적 생성 (덱 편집 버튼 바로 아래부터 생성됨)
        foreach (var deck in allDecks)
        {
            if (deckSlotPrefab == null) break;

            GameObject slotObj = Instantiate(deckSlotPrefab, contentTransform);
            DeckSlotItem slotItem = slotObj.GetComponent<DeckSlotItem>();

            if (slotItem != null)
            {
                slotItem.Init(deck, OnSelectDeck, OnDeleteDeck, OnRenameDeck);

                // 현재 편집 모드 상태 적용 (삭제/이름변경 버튼 On/Off)
                slotItem.SetEditModeUI(isEditMode);

                bool isSelected = DeckManager.Instance.currentSelectedDeck != null &&
                                  DeckManager.Instance.currentSelectedDeck.deckId == deck.deckId;
            }
        }

        // 3. [+] 새 덱 만들기 버튼 생성 (맨 아래)
        if (allDecks.Count < DeckManager.MAX_DECK_COUNT)
        {
            if (createDeckButtonPrefab != null)
            {
                GameObject createBtnObj = Instantiate(createDeckButtonPrefab, contentTransform);
                Button createBtn = createBtnObj.GetComponent<Button>();
                if (createBtn == null) createBtn = createBtnObj.GetComponentInChildren<Button>();

                if (createBtn != null)
                {
                    createBtn.onClick.RemoveAllListeners();
                    createBtn.onClick.AddListener(OnClickCreateDeck);
                }
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform as RectTransform);
    }

    public void OnClickCreateDeck()
    {
        int nextIndex = DeckManager.Instance.deckList.Count + 1;
        string newName = $"덱 {nextIndex}";

        if (DeckManager.Instance.CreateNewDeck(newName, out DeckData createdDeck))
        {
            DeckManager.Instance.SelectDeck(createdDeck);
            RefreshDeckListUI();
        }
    }

    private void OnSelectDeck(DeckData selectedDeck)
    {
        DeckManager.Instance.SelectDeck(selectedDeck);
        RefreshDeckListUI();
    }

    private void OnDeleteDeck(DeckData targetDeck)
    {
        if (targetDeck == null) return;

        if (DeckManager.Instance.currentSelectedDeck != null &&
            DeckManager.Instance.currentSelectedDeck.deckId == targetDeck.deckId)
        {
            DeckManager.Instance.currentSelectedDeck = null;
        }

        DeckManager.Instance.DeleteDeck(targetDeck.deckId);
        RefreshDeckListUI();
    }

    private void OnRenameDeck(DeckData targetDeck)
    {
        targetRenameDeck = targetDeck;

        if (renamePopupPanel != null && renameInputField != null)
        {
            renameInputField.text = targetDeck.deckName; // 기존 덱 이름으로 InputField 채우기
            renameInputField.Select();
            renameInputField.ActivateInputField();

            int textLength = renameInputField.text.Length;
            renameInputField.caretPosition = textLength;
            renameInputField.selectionAnchorPosition = textLength;
            renameInputField.selectionFocusPosition = textLength;
            renameInputField.caretPosition = renameInputField.text.Length;
            renameInputField.selectionAnchorPosition = renameInputField.text.Length;
            renameInputField.selectionFocusPosition = renameInputField.text.Length;

            renamePopupPanel.SetActive(true);
            renameInputField.Select();
            renameInputField.ActivateInputField();
        }
    }
    // 💡 2. 팝업에서 [확인] 버튼을 눌렀을 때 입력값 적용
    public void OnConfirmRename()
    {
        if (targetRenameDeck == null || renameInputField == null) return;

        string newName = renameInputField.text;

        if (!string.IsNullOrWhiteSpace(newName))
        {
            // deckName 대신 deckId를 넘겨주어야 식별이 정확합니다.
            DeckManager.Instance.RenameDeck(targetRenameDeck.deckId, newName.Trim());

            renamePopupPanel.SetActive(false); // 팝업 닫기
            targetRenameDeck = null;

            RefreshDeckListUI(); // UI 새로고침
        }
    }

    // 💡 3. 팝업에서 [취소] 버튼을 눌렀을 때 처리
    public void OnCancelRename()
    {
        targetRenameDeck = null;
        if (renamePopupPanel != null)
        {
            renamePopupPanel.SetActive(false);
        }
    }

}
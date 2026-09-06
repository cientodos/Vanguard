using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vanguard.Data.DataModels;
using Vanguard.DeckBuilder.UI;

public class DeckListUIController : MonoBehaviour
{
    #region Header
    [Header("스크롤뷰 연결")]
    [SerializeField] private Transform contentTransform; 
    [Header("맨 위 고정 UI")]
    [SerializeField] private Button editModeButton; 
    [Header("프리팹")]
    [SerializeField] private GameObject deckSlotPrefab;
    [SerializeField] private GameObject createDeckButtonPrefab;
    [Header("이름 변경 팝업 UI")]
    [SerializeField] private GameObject renamePopupPanel; 
    [SerializeField] private InputField renameInputField; 
    [SerializeField] private Button confirmRenameButton;
    [SerializeField] private Button cancelRenameButton;
    private DeckData targetRenameDeck;
    private bool isEditMode = false;
    #endregion
    private void Start()
    {
        if (editModeButton != null)
        {
            editModeButton.onClick.RemoveAllListeners();
            editModeButton.onClick.AddListener(ToggleEditMode);
        }

        RefreshDeckListUI();
    }

   
    public void ToggleEditMode()
    {
        isEditMode = !isEditMode;


        RefreshDeckListUI();
    }

    public void RefreshDeckListUI()
    {
        if (contentTransform == null || DeckManager.Instance == null) return;

      
        for (int i = contentTransform.childCount - 1; i >= 0; i--)
        {
            Transform child = contentTransform.GetChild(i);

           
            if (child == editModeButton.transform)
                continue;

            Destroy(child.gameObject);
        }

        List<DeckData> allDecks = DeckManager.Instance.deckList;

        
        foreach (var deck in allDecks)
        {
            GameObject slotObj = Instantiate(deckSlotPrefab, contentTransform);
            DeckSlotItem slotItem = slotObj.GetComponent<DeckSlotItem>();

            if (slotItem != null)
            {
                slotItem.Init(deck, OnSelectDeck, OnDeleteDeck, OnRenameDeck);

              
                slotItem.SetEditModeUI(isEditMode);

                bool isSelected = DeckManager.Instance.currentSelectedDeck != null &&
                                  DeckManager.Instance.currentSelectedDeck.deckId == deck.deckId;
            }
        }


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
        bool isCurrentDeleted = (DeckManager.Instance.currentSelectedDeck != null && DeckManager.Instance.currentSelectedDeck.deckId == targetDeck.deckId);

        DeckManager.Instance.DeleteDeck(targetDeck.deckId);

        if (isCurrentDeleted)
        {
            DeckData nextDeck = DeckManager.Instance.deckList.Count > 0 ? DeckManager.Instance.deckList[0] : null;
            DeckManager.Instance.SelectDeck(nextDeck);
        }
        RefreshDeckListUI();
    }

    private void OnRenameDeck(DeckData targetDeck)
    {
        targetRenameDeck = targetDeck;

        if (renamePopupPanel != null && renameInputField != null)
        {
            renameInputField.text = targetDeck.deckName; 
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
    public void OnConfirmRename()
    {
        if (targetRenameDeck == null || renameInputField == null) return;

        string newName = renameInputField.text;

        if (!string.IsNullOrWhiteSpace(newName))
        {
          
            DeckManager.Instance.RenameDeck(targetRenameDeck.deckId, newName.Trim());

            renamePopupPanel.SetActive(false); 
            targetRenameDeck = null;

            RefreshDeckListUI(); 
        }
    }

    public void OnCancelRename()
    {
        targetRenameDeck = null;
        if (renamePopupPanel != null)
        {
            renamePopupPanel.SetActive(false);
        }
    }

}
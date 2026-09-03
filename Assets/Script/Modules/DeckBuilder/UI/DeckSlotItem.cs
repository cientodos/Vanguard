using UnityEngine;
using UnityEngine.UI;
using Vanguard.Data.DataModels;

namespace Vanguard.DeckBuilder.UI
{
    public class DeckSlotItem : MonoBehaviour
    {
        [Header("UI 요소 연결")]
        [SerializeField] private Text deckNameText;
        [SerializeField] private Text cardCountText;
        [SerializeField] private Button selectButton;
        [SerializeField] private Button reNameButton;
        [SerializeField] private Button deleteButton;
        private DeckData targetDeck;

        // 💡 string -> DeckData 타입으로 변경하여 타입 일치
        private System.Action<DeckData> onSelectCallback;
        private System.Action<DeckData> onDeleteCallback;
        private System.Action<DeckData> onRenameCallback;

        private void Awake()
        {
            if (selectButton == null) selectButton = GetComponent<Button>();
        }

        public void Init(DeckData deckData, System.Action<DeckData> onSelect, System.Action<DeckData> onDelete, System.Action<DeckData> onRename = null)
        {
            targetDeck = deckData;
            onSelectCallback = onSelect;
            onDeleteCallback = onDelete;
            onRenameCallback = onRename;

            if (deckNameText != null) deckNameText.text = deckData.deckName;
            if (cardCountText != null) cardCountText.text = $"{deckData.mainDeckCardIds.Count} / 30";

            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(() => onSelectCallback?.Invoke(targetDeck));
            }

            if (deleteButton != null)
            {
                deleteButton.onClick.RemoveAllListeners();
                deleteButton.onClick.AddListener(() => onDeleteCallback?.Invoke(targetDeck));
            }

            if (reNameButton != null)
            {
                reNameButton.onClick.RemoveAllListeners();
                reNameButton.onClick.AddListener(() => onRenameCallback?.Invoke(targetDeck));
            }

        }



        public void SetEditModeUI(bool isEditMode)
        {
            if (deleteButton != null) deleteButton.gameObject.SetActive(isEditMode);
            if (reNameButton != null) reNameButton.gameObject.SetActive(isEditMode);
        }

    }
}
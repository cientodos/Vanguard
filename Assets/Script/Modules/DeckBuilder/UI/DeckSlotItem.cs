using UnityEngine;
using UnityEngine.UI;
using Vanguard.Data.DataModels;

namespace Vanguard.DeckBuilder.UI
{
    public class DeckSlotItem : MonoBehaviour
    {
        #region Header
        [Header("UI 컴포넌트")]
        [SerializeField] private Text deckNameText;
        [SerializeField] private Button selectButton;
        [SerializeField] private Button reNameButton;
        [SerializeField] private Button deleteButton;
        private DeckData targetDeck;
        private System.Action<DeckData> onSelectCallback; //덱타입 변환 string > DeckData
        private System.Action<DeckData> onDeleteCallback;
        private System.Action<DeckData> onRenameCallback;
        #endregion
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
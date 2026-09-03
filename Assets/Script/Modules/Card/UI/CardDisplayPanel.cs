using EnumData;
using UnityEngine;
using UnityEngine.UI;
using Vanguard.Data.DataModels;

namespace Vanguard.CardSystem.UI
{
    public class CardDisplayPanel : MonoBehaviour
    {
        [Header("UI 요소 연결")]
        [SerializeField] private Image cardImage;
        [SerializeField] private Text nameText;
        [SerializeField] private Text effectText;
        [SerializeField] private Text flavorText;
        [SerializeField] private Text tribeText;
        [SerializeField] private Text countryText;
        public void UpdateDetailView(CardData data, Sprite loadedSprite)
        {
            if (data == null) return;

            if (nameText != null) nameText.text = data.cardName;
            if (effectText != null) effectText.text = data.effectText;
            if (flavorText != null) flavorText.text = data.flavorText;
            if (countryText != null) countryText.text = data.countryList.ToKoreanString();
            if (tribeText != null) tribeText.text = data.tribeList.ToKoreanString();

            if (cardImage != null && loadedSprite != null)
            {
                cardImage.sprite = loadedSprite;
            }

        }
    }
}
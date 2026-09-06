using EnumData;
using UnityEngine;
using UnityEngine.UI;
using Vanguard.Data.DataModels;

namespace Vanguard.CardSystem.UI
{
    public class CardDisplayPanel : MonoBehaviour
    {
        #region Header
        [Header("UI 컴포넌트")]
        [SerializeField] private Image cardImage;
        [SerializeField] private Text nameText;
        [SerializeField] private Text effectText;
        [SerializeField] private Text flavorText;
        [SerializeField] private Text tribeText;
        [SerializeField] private Text countryText;
        #endregion
        public void UpdateDetailView(CardData data, Sprite loadedSprite)
        {
            if (data == null) return;

            nameText.text = data.cardName;
            effectText.text = data.effectText;
            flavorText.text = data.flavorText;
            countryText.text = data.countryList.ToKoreanString();
            tribeText.text = data.tribeList.ToKoreanString();

            if (loadedSprite != null)
            {
                cardImage.sprite = loadedSprite;
            }

        }
    }
}
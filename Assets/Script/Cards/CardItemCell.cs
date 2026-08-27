using UnityEngine;
using PolyAndCode.UI; 
using UnityEngine.UI;

// ICell 인터페이스 구현 필수
public class CardItemCell : MonoBehaviour, ICell
{
    [SerializeField] private Image cardImage;
   // [SerializeField] private Text cardNameText;

    private CardData currentData;

    public void ConfigureCell(CardData cardData)
    {
        currentData = cardData;
       // cardNameText.text = cardData.cardName;
    }
}
using UnityEngine;
using UnityEngine.UI;
using EnumData;

namespace Vanguard.CardSystem.UI
{
    [RequireComponent(typeof(Button))]
    public class CountryButton : MonoBehaviour
    {
        [Header("연결할 컨트롤러")]
        [SerializeField] private DeckBuilder.UI.CardPoolListController controller;

        [Header("국가 필터 설정")]
        [SerializeField] private bool isAllCountry = false; // '전체 국가' 버튼인지 여부
        [SerializeField] private CountryType countryType = CountryType.None;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            if (controller != null)
            {
                if (isAllCountry)
                {
                    // 0번: 전체 국가(null)
                    controller.SetCountry(0);
                }
                else
                {
                    // 1번부터: Enum 순서 매핑 (None=1, LyricalMonasterio=2 ...)
                    controller.SetCountry((int)countryType + 1);
                }
            }
        }
    }
}
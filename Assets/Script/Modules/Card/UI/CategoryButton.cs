using UnityEngine;
using UnityEngine.UI;
using EnumData;

namespace Vanguard.CardSystem.UI
{
    [RequireComponent(typeof(Button))]
    public class CategoryButton : MonoBehaviour
    {
        [Header("연결할 컨트롤러")]
        [SerializeField] private DeckBuilder.UI.CardPoolListController controller;

        [Header("버튼 필터 설정")]
        [SerializeField] private CardCategoryType categoryType = CardCategoryType.All;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            // 버튼 클릭 이벤트에 컨트롤러의 SetCategory 연결
            button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            if (controller != null)
            {
                // Enum 값을 int로 변환하여 전달
                controller.SetCategory((int)categoryType);
            }
            else
            {
                Debug.LogWarning($"[{gameObject.name}] CardPoolListController가 할당되지 않았습니다!");
            }
        }
    }
}
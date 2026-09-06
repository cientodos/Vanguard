using PolyAndCode.UI;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Vanguard.Data.DataModels;

// ICell 인터페이스 구현
namespace Vanguard.CardSystem.UI
{
    public class CardItemCell : MonoBehaviour, ICell, IPointerClickHandler
    {
        #region Header
        [SerializeField] private Image cardImage;
        private CardData currentData;
        public static event Action<PointerEventData.InputButton, CardData, Vector2, Sprite> OnCellClicked;
        private AsyncOperationHandle<Sprite> spriteHandle;
        #endregion



        // Recyclable Scroll Rect에서 셀이 재사용될 때마다 호출되는 메서드
        public void ConfigureCell(CardData cardData)
        {
            // 1. 셀이 재사용되므로 이전 카드의 이미지 메모리 해제 및 UI 초기화
            ReleaseSprite();

            currentData = cardData;
            if (currentData == null) return;

            // 2. 어드레서블 키 가져오기 (List<string>의 첫 번째 키)
            string key = (currentData.spriteAssetKeys != null && currentData.spriteAssetKeys.Count > 0) ? currentData.spriteAssetKeys[0] : null;

            if (string.IsNullOrEmpty(key)) return;
            CardData requestedData = currentData;

            // 3. 어드레서블 이미지 비동기 로드
            Addressables.LoadAssetAsync<Sprite>(key).Completed += handle =>
            {
                // 로드 도중 스크롤로 인해 셀이 다른 데이터로 재설정되었거나 파괴된 경우 방어
                if (!handle.IsValid()) return;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    spriteHandle = handle;
                    if (cardImage != null)
                    {
                        cardImage.sprite = handle.Result;
                    }
                }
                else
                {
                    Debug.LogWarning($"[CardItemCell] 이미지 로드 실패 - Key: {key}");
                }
            };
        }

        // 어드레서블 메모리 해제 전용 메서드
        private void ReleaseSprite()
        {
            if (spriteHandle.IsValid())
            {
                Addressables.Release(spriteHandle);
            }
            if (cardImage != null)
            {
                cardImage.sprite = null;
            }
        }

        // 씬이 변경되거나 오브젝트가 파괴될 때 메모리 정리
        private void OnDestroy()
        {
            ReleaseSprite();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"[CardItemCell] 클릭 감지됨! 버튼 종류: {eventData.button} / 카드명: {currentData?.cardName}");
            if (currentData == null) return;
            Sprite currentSprite = cardImage != null ? cardImage.sprite : null;
            // 좌클릭/우클릭 정보와 마우스 현재 위치, CardData를 이벤트로 전송
            OnCellClicked?.Invoke(eventData.button, currentData, eventData.position, currentSprite);

        }
    }
}
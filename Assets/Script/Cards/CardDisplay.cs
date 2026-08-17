using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    [Header("카드 면 렌더러")]
    public MeshRenderer cardFrontRenderer; // 카드의 앞면 MeshRenderer

    // 카드를 생성하거나 데이터를 등록할 때 호출하는 함수
    public void SetCardImage(Texture2D newCardTexture)
    {
        // 머티리얼의 텍스처만 실시간으로 변경합니다.
        cardFrontRenderer.material.mainTexture = newCardTexture;

        // URP(Universal Render Pipeline)를 사용 중이라면 아래 줄을 사용합니다.
        // cardFrontRenderer.material.SetTexture("_BaseMap", newCardTexture);
    }
}
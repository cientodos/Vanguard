using UnityEngine;
using DG.Tweening; // DOTween 패키지 사용 시

public class BackgroundLineGlow : MonoBehaviour
{
    [SerializeField] private RectTransform glowLight; // 빛 이미지
    [SerializeField] private Vector3 startPos;       // 네온선 시작 좌표
    [SerializeField] private Vector3 endPos;         // 네온선 끝 좌표
    [SerializeField] private float duration = 2.5f;

    void Start()
    {
        FlowEffect();
    }

    void FlowEffect()
    {
        glowLight.localPosition = startPos;
        // 시작점에서 끝점으로 이동 후 무한 반복
        glowLight.DOLocalMove(endPos, duration)
                 .SetEase(Ease.InQuad)
                 .SetLoops(-1, LoopType.Restart);
    }
}
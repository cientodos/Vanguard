using System.Collections;
using UnityEngine;

public class Animation : MonoBehaviour
{
    [Header("회전 소요 시간 (초)")]
    public float duration = 0.3f;

    private Coroutine rotateCoroutine;


    // 카드를 Y축 90도로 눕히기 (Rest / Tap)
    public void RestCard()
    {
        // Y축 90도 회전 목표값
        Quaternion targetRotation = Quaternion.Euler(0, 90f, 0);
        StartRotate(targetRotation);
    }

    // 카드를 다시 0도로 일으키기 (Stand / Untap)
    public void StandCard()
    {
        // 원래 회전값 (0도)a
        Quaternion targetRotation = Quaternion.Euler(0, 0f, 0);
        StartRotate(targetRotation);
    }

    private void StartRotate(Quaternion targetRotation)
    {
        // 이미 회전 중인 연출이 있다면 중단하고 새로 시작
        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
        }
        rotateCoroutine = StartCoroutine(RotateTo(targetRotation));
    }

    private IEnumerator RotateTo(Quaternion target)
    {
        Quaternion start = transform.localRotation;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;

            // Mathf.SmoothStep을 쓰면 시작과 끝이 더 부드러워집니다.
            float t = Mathf.SmoothStep(0, 1, time / duration);
            transform.localRotation = Quaternion.Slerp(start, target, t);

            yield return null; // 다음 프레임까지 대기
        }

        // 오차 방지를 위해 마지막에 정확한 목표값 설정
        transform.localRotation = target;
    }
}
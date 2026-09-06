using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    public Image sceneImage;
    public Sprite[] sprites;

    public float fadeDuration = 1.0f;
    public float imageDisplayDuration = 8.0f; // 이미지가 유지되는 시간 (전체 10초 중 8초 표시)

    private int index = 0;

    void Start()
    {
        if (sceneImage == null)
            sceneImage = GetComponent<Image>();

        // 최초 0번째 이미지 설정 후 반복 루프 시작
        if (sprites.Length > 0)
        {
            sceneImage.sprite = sprites[index];
            StartCoroutine(ImageChangeRoutine());
        }
    }

    // 10초 주기를 제어하는 메인 코루틴 루프
    IEnumerator ImageChangeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(imageDisplayDuration);
            yield return StartCoroutine(FadeIn());
          
            index = (index + 1) % sprites.Length;
            sceneImage.sprite = sprites[index];

            yield return StartCoroutine(FadeOut());
        }
    }

    // 페이드 아웃 (투명 ➔ 불투명, 화면이 어두워짐)
    IEnumerator FadeOut()
    {
        float timer = 0f;
        Color color = sceneImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            sceneImage.color = color;
            yield return null;
        }

        color.a = 1f;
        sceneImage.color = color;
    }

    // 페이드 인 (불투명 ➔ 투명, 화면이 밝아짐)
    IEnumerator FadeIn()
    {
        float timer = 0f;
        Color color = sceneImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            sceneImage.color = color;
            yield return null;
        }

        color.a = 0f;
        sceneImage.color = color;
    }
}

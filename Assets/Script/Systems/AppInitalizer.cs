using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using Vanguard.Data.Databases;

namespace Vanguard.Initializer
{
    public class AppInitializer : MonoBehaviour
    {
        #region Header
        [Header("UI 연결")]
        [SerializeField] private Slider progressBar;
        [SerializeField] private Text statusText;
        [SerializeField] private string nextSceneName = "MainTitle";

        [Header("ScriptableObjects / Database")]
        [SerializeField] private CardReader cardReader;
        #endregion
        private async void Start()
        {
            try
            {
                // Step 1. Addressables 로컬 에셋 시스템 초기화
                UpdateStatus("에셋 시스템 초기화 중...", 0.2f);
                var initHandle = Addressables.InitializeAsync();
                await initHandle.Task;

                // Step 2. 카드 데이터베이스 로드 (CardReader -> CardDatabase)
                UpdateStatus("카드 데이터베이스 구성 중...", 0.5f);
                await LoadCardDatabaseAsync();

                // Step 3. 유저 덱 데이터 로드
                UpdateStatus("유저 덱 데이터 불러오는 중...", 0.8f);
                await LoadUserDeckDataAsync();

                // Step 4. 준비 완료 -> 메인 화면 진입
                UpdateStatus("로딩 완료!", 1.0f);
                await Task.Delay(300); // UI 연출용 대기

                SceneManager.LoadScene(nextSceneName);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AppInitializer] 로딩 중 오류 발생: {ex.Message}");
                UpdateStatus("로딩 실패. 오류를 확인하세요.", progressBar.value);
            }
        }

        private void UpdateStatus(string message, float progress)
        {
            if (statusText != null) statusText.text = message;
            if (progressBar != null) progressBar.value = progress;
        }

        private async Task LoadCardDatabaseAsync()
        {
            if (cardReader != null && cardReader.DataList != null)
            {
                // ScriptableObject에 저장된 카드 데이터를 CardDatabase Dictionary에 동기화
                CardDatabase.Initialize(cardReader.DataList);
            }
            else
            {
                Debug.LogWarning("[AppInitializer] CardReader가 할당되지 않았거나 DataList가 비어있습니다.");
            }

            await Task.Yield();
        }

        private async Task LoadUserDeckDataAsync()
        {
            if (DeckManager.Instance != null)
            {
                // 유저가 구성해둔 덱 리스트(JSON / PlayerPrefs) 로드
                DeckManager.Instance.LoadDecks();
            }

            await Task.Yield();
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using Vanguard.Data.Databases;
using Vanguard.Data.DataModels;

namespace Vanguard.InGame
{
    public class InGameDeckManager : MonoBehaviour
    {
        public static InGameDeckManager Instance { get; private set; }

        [Header("Runtime Deck Lists")]
        public List<int> mainDeck = new List<int>();       // 런타임 메인 덱
        public List<int> rideDeck = new List<int>();       // 런타임 라이드 덱
        public List<int> hand = new List<int>();           // 손패 (Hand)
        public List<int> dropZone = new List<int>();       // 드롭 존 (묘지)

        // UI 갱신 이벤트 (손패/덱 수량 변경 시 호출)
        public event Action OnHandUpdated;
        public event Action OnDeckUpdated;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            InitializeGameDeck();
        }

        /// <summary>
        /// 덱 데이터 초기화 및 게임 준비
        /// </summary>
        public void InitializeGameDeck()
        {
            var selectedDeck = DeckManager.Instance?.currentSelectedDeck;

            if (selectedDeck == null)
            {
                Debug.LogError("[InGameDeckManager] 선택된 덱 데이터가 없습니다!");
                return;
            }

            // 1. DeckManager 데이터 복사 (원본 데이터 보존)
            mainDeck = new List<int>(selectedDeck.mainDeckCardIds);
            rideDeck = new List<int>(selectedDeck.rideDeckCardIds);
            hand.Clear();
            dropZone.Clear();

            // 2. 메인 덱 셔플
            ShuffleDeck();

            // 3. 초기 손패 5장 드로우
            DrawCards(5);

            Debug.Log($"[InGame] 덱 초기화 완료 - 남은 덱: {mainDeck.Count}장, 손패: {hand.Count}장, 라이드 덱: {rideDeck.Count}장");
        }

        /// <summary>
        /// 피셔-예이츠(Fisher-Yates) 알고리즘 기반 덱 셔플
        /// </summary>
        public void ShuffleDeck()
        {
            for (int i = mainDeck.Count - 1; i > 0; i--)
            {
                int randomIndex = UnityEngine.Random.Range(0, i + 1);
                int temp = mainDeck[i];
                mainDeck[i] = mainDeck[randomIndex];
                mainDeck[randomIndex] = temp;
            }

            Debug.Log("[InGameDeckManager] 메인 덱 셔플 완료");
            OnDeckUpdated?.Invoke();
        }

        /// <summary>
        /// 메인 덱 상단에서 카드를 드로우하여 손패로 이동
        /// </summary>
        public List<int> DrawCards(int count)
        {
            List<int> drawnCards = new List<int>();

            for (int i = 0; i < count; i++)
            {
                if (mainDeck.Count == 0)
                {
                    Debug.LogWarning("[InGameDeckManager] 메인 덱에 카드가 없어 더 이상 드로우할 수 없습니다.");
                    break;
                }

                int drawnCardId = mainDeck[0];
                mainDeck.RemoveAt(0);
                hand.Add(drawnCardId);
                drawnCards.Add(drawnCardId);
            }

            // UI 갱신 이벤트 발행
            OnHandUpdated?.Invoke();
            OnDeckUpdated?.Invoke();

            return drawnCards;
        }

        /// <summary>
        /// 특정 카드를 손패에서 드롭 존으로 보냄
        /// </summary>
        public bool DiscardFromHand(int cardId)
        {
            if (hand.Remove(cardId))
            {
                dropZone.Add(cardId);
                OnHandUpdated?.Invoke();
                return true;
            }
            return false;
        }
    }
}
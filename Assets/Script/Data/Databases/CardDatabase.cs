using System.Collections.Generic;
using UnityEngine;
using Vanguard.Data.DataModels;

namespace Vanguard.Data.Databases
{
    public static class CardDatabase
    {
        private static CardReader cardReader;
        private static readonly Dictionary<int, CardData> cardDict = new();
        private static bool isInitialized = false;

        public static void Initialize()
        {
            if (isInitialized) return;

            // Resources 폴더에서 CardReader.asset 로드
            cardReader = Resources.Load<CardReader>("CardReader");

            if (cardReader == null)
            {
                Debug.LogError("[CardDatabase] Resources 폴더에서 CardReader.asset을 찾을 수 없습니다!");
                return;
            }

            // 빠른 조회를 위해 딕셔너리에 매핑
            cardDict.Clear();
            foreach (var data in cardReader.DataList)
            {
                if (data != null && !cardDict.ContainsKey(data.id))
                {
                    cardDict.Add(data.id, data);
                }
            }

            isInitialized = true;
            Debug.Log($"[CardDatabase] 총 {cardDict.Count}장의 카드가 성공적으로 로드되었습니다.");
        }

        public static CardData Get(int id)
        {
            if (!isInitialized) Initialize();

            if (cardDict.TryGetValue(id, out var card))
            {
                return card;
            }

            Debug.LogError($"[CardDatabase] ID {id}에 해당하는 카드가 없습니다.");
            return null;
        }
    }
}
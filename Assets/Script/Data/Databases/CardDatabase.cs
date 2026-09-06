using System;
using System.Collections.Generic;
using UnityEngine;
using Vanguard.Data.DataModels;

namespace Vanguard.Data.Databases
{
    public static class CardDatabase
    {
        #region Header
        private static readonly Dictionary<int, CardData> cardDict = new();
        public static bool isInitialized = false;
        public static event Action OnDatabaseReady;
        #endregion
        public static void Initialize(List<CardData> rawDataList)
        {
            if (isInitialized) return;

            cardDict.Clear();

            foreach (var rawCard in rawDataList)
            {
                if (rawCard == null) continue;

                // 1. 원본 카드 등록 (기본 레어리티 / 첫 번째 스프라이트)
                CardIdUtil.ApplyCardTypeInfo(rawCard.id, rawCard);
                cardDict[rawCard.id] = rawCard;

                // 2. spriteAssetKeys가 2개 이상일 때 고레어 복사본 등록
                if (rawCard.spriteAssetKeys != null && rawCard.spriteAssetKeys.Count > 1)
                {
                    GenerateRarityVariants(rawCard);
                }
            }

            isInitialized = true;
            Debug.Log($"[CardDatabase] 초기화 완료! 원본 + 고레어 포함 총 {cardDict.Count}장의 카드가 등록되었습니다.");
            OnDatabaseReady?.Invoke();
        }

        private static void GenerateRarityVariants(CardData rawCard)
        {
          
            for (int i = 1; i < rawCard.spriteAssetKeys.Count; i++)
            {
                
                int rarityValue = (rawCard.availableRarities != null && i < rawCard.availableRarities.Count) ? rawCard.availableRarities[i] : i;

               
                int variantId = ChangeRarityDigit(rawCard.id, rarityValue);

                CardData rarityCard = new CardData
                {
                    id = variantId,
                    cardName = rawCard.cardName,
                    grade = rawCard.grade,
                    power = rawCard.power,
                    shield = rawCard.shield,
                    critical = rawCard.critical,
                    countryList = rawCard.countryList != null ? new List<EnumData.CountryType>(rawCard.countryList) : new List<EnumData.CountryType>(),
                    tribeList = rawCard.tribeList != null ? new List<EnumData.TribeType>(rawCard.tribeList) : new List<EnumData.TribeType>(),
                    triggerType = rawCard.triggerType,
                    cardType = rawCard.cardType,
                    flavorText = rawCard.flavorText,
                    effectText = rawCard.effectText,
                    availableRarities = rawCard.availableRarities != null ? new List<int>(rawCard.availableRarities) : new List<int>(),
                    spriteAssetKeys = new List<string> { rawCard.spriteAssetKeys[i] }
                };

                CardIdUtil.ApplyCardTypeInfo(rarityCard.id, rarityCard);

                if (!cardDict.ContainsKey(variantId))
                {
                    cardDict.Add(variantId, rarityCard);
                }
            }
        }

        /// <summary>
        /// ID의 3번째 자리(백만 자리 = 100,000 단위)를 새로운 레어도 값으로 교체합니다.
        /// 예: 62001234 -> 레어도 3(FFR) 대입 -> 62301234
        /// </summary>
        private static int ChangeRarityDigit(int originalId, int newRarity)
        {
            int currentRarityDigit = (originalId / 100000) % 10;
            int baseId = originalId - (currentRarityDigit * 100000);
            return baseId + (newRarity * 100000);
        }

        public static CardData Get(int id)
        {
            return cardDict.TryGetValue(id, out var card) ? card : null;
        }

        public static List<CardData> GetAll()
        {
            return new List<CardData>(cardDict.Values);
        }
    }
}
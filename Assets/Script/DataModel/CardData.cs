using EnumData;
using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#endif
namespace Vanguard.Data.DataModels
{
    [Serializable]
    public class CardData
    {
        [Header("기본 정보")]
        public int id;                          // 63000010 (int 변환)
        public string cardName;                 // 데블스프레스 자객
        public int grade;                       // 3
        public int power;                       // 13000
        public int shield;                      // 0
        public int critical;                    // 1
        public int maxCount = 4;
        public bool isSentinel = false;
        [Header("속성 정보")]
        public List<CountryType> countryList = new List<CountryType>();            // Enum 관리 
        public List<TribeType> tribeList = new List<TribeType>();

        [Header("카드 분류 및 텍스트")]
        public TriggerType triggerType;        // Enum 관리 
        public CardType cardType;              // Enum 관리 
        public string flavorText;              // 플레이버 텍스트
        public string effectText;              // 효과 설명 문구

        [Header("에셋 정보")]
        public List<int> availableRarities;    // [0, 1, 2] 파싱 리스트
        public List<string> spriteAssetKeys;          // 어드레서블 스프라이트 키

        public CardData()
        {
            countryList = new List<CountryType>();
            tribeList = new List<TribeType>();
            availableRarities = new List<int>();
            spriteAssetKeys = new List<string>();
        }

        public CardData(
               int id, string cardName, int grade, int power, int shield, int critical,
               List<CountryType> countryList, List<TribeType> tribeList,
               string flavorText, string effectText, TriggerType triggerType, CardType cardType,
               List<int> availableRarities, List<string> spriteAssetKeys)
        {
            this.id = id;
            this.cardName = cardName;
            this.grade = grade;
            this.power = power;
            this.shield = shield;
            this.critical = critical;
            this.countryList = countryList;
            this.tribeList = tribeList;
            this.flavorText = flavorText;
            this.effectText = effectText;
            this.triggerType = triggerType;
            this.cardType = cardType;
            this.availableRarities = availableRarities;
            this.spriteAssetKeys = spriteAssetKeys;
        }
    }
}
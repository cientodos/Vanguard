using UnityEngine;
using EnumData;
using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
#endif

[Serializable]
public class CardData
{
    public int id;                          // 63000010 (int 변환)
    public string cardName;                 // 데블스프레스 자객
    public int grade;                       // 3
    public int power;                       // 13000
    public int shield;                      // 0
    public int critical;                    // 1
    public CountryType country;            // Enum 관리 (리지컬메소스타지오)
    public TribeType tribe;                // Enum 관리 (휴먼)
    public TriggerType triggerType;        // Enum 관리 (None)
    public CardType cardType;              // Enum 관리 (normal)
    public string flavorText;              // 플레이버 텍스트
    public string effectText;              // 효과 설명 문구
    public List<int> availableRarities;    // [0, 1, 2] 파싱 리스트
    public string spriteAssetKey;          // 어드레서블 스프라이트 키


    public CardData(int id, string cardName, int grade, int power, int shiled, int critical, CountryType country, TribeType tribe, string flavorText, string effectText, TriggerType triggerType, CardType cardType, List<int> availableRarities, string spriteAssetKeys)
    {
        this.id = id;
        this.cardName = cardName;
        this.grade = grade;
        this.power = power;
        this.shield = shiled;
        this.critical = critical;
        this.country = country;
        this.tribe = tribe;
        this.flavorText = flavorText;
        this.effectText = effectText;
        this.availableRarities = availableRarities;
        this.spriteAssetKey = spriteAssetKeys;
    }
}

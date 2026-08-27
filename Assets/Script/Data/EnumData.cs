using GoogleSheetsToUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnumData
{
    public enum CountryType
    {
        None,
        LyricalMonasterio, // 리리컬모나스테리오
        KeterSanctuary,    // 케테르 생츄어리
        DragonEmpire,      // 드래곤 엠파이어
        DarkStates,        // 다크 스테이츠
        BrandtGate,        // 브랜트 게이트
        Stoicheia          // 스토이케이아
    }
    public enum TribeType
    {
        None,
        Human,             // 휴먼
        Ghost,             // 고스트
        Angel,             // 엔젤
        Demon,             // 데몬
        Warbeast,          // 워비스트
        Giant,             // 자이언트
        Vampire,            // 뱀파이어
        Elf,
        Phantom,
        Dragoroid,
        Bioroid,
        Cyberloid,
        Battroid,
        Succubus,
        ForestDragon,

    }
    public enum TriggerType
    {
        None,
        Critical,
        Draw,
        Front,
        Heal,
        Over
    }
    public enum CardType
    {
        Normal,            // normal
        SetOrder,          // setOrder
        BlitzOrder,
        Trigger
    }
    
}

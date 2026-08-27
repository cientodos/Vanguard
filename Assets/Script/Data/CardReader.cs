using UnityEngine;
using System.Collections.Generic;
using GoogleSheetsToUnity;
using EnumData;
using System;

[CreateAssetMenu(fileName = "CardReader", menuName = "Scriptable Object/CardReader")]
public class CardReader : ScriptableObject
{
    [Header("구글 시트 연동 설정")]
    public string associatedSheet = "1Uh8AwERVUNqxEbevHFTfmQTlekXF0bLleCPzgQYrFo8";         // 구글 시트 /d/ 와 /edit 사이의 ID
    public string associatedWorksheet = "CardData"; // 시트 하단 탭 이름
    public int START_ROW_LENGTH = 2;            // 데이터가 시작되는 행 (1행은 헤더이므로 2)

    [Header("저장된 카드 데이터")]
    public List<CardData> DataList = new List<CardData>();

    // 한 행의 Cell 데이터를 CardData 객체로 변환하여 저장하는 메서드
    public void AddDataFromRow(List<GSTU_Cell> list)
    {
        int id = 0; string name = ""; int grade = 0, power = 0, shield = 0, critical = 0;
        List<CountryType> countryList = new List<CountryType>();
        string tribeRaw = "";
        List<string> tribeList = new List<string>();
        TriggerType trigger = TriggerType.None;
        CardType type = CardType.Normal;
        string flavor = "", effect = "";
        List<string> spriteKeys = new List<string>();   
        List<int> rarities = new();

        foreach (var cell in list)
        {
            switch (cell.columnId)
            {
                case "id": int.TryParse(cell.value, out id); break;
                case "cardName": name = cell.value; break;
                case "grade": int.TryParse(cell.value, out grade); break;
                case "power": int.TryParse(cell.value, out power); break;
                case "shield": int.TryParse(cell.value, out shield); break;
                case "critical": int.TryParse(cell.value, out critical); break;
                case "country": ParseCountries(cell.value, countryList); break;
                case "tribe": tribeRaw = cell.value;ParseTribes(cell.value, tribeList); break;
                case "triggerType": Enum.TryParse(cell.value, true, out trigger); break;
                case "cardType": Enum.TryParse(cell.value, true, out type); break;
                case "flavorText": flavor = cell.value; break;
                case "effectText": effect = cell.value; break;
                case "spriteAssetKeys": ParseSpriteKeys(cell.value, spriteKeys); break;
                case "availableRarities":
                    ParseRarities(cell.value, rarities);
                    break;
            }
        }

        // 작성하신 CardData 생성자 인자 순서대로 매핑
        DataList.Add(new CardData(
              id, name, grade, power, shield, critical,
              countryList, tribeRaw, tribeList,
              flavor, effect, trigger, type,
              rarities, spriteKeys
          ));
    }

    private void ParseRarities(string rawValue, List<int> list)
    {
        if (string.IsNullOrEmpty(rawValue)) return;

        if (rawValue.Contains(","))
        {
            foreach (var r in rawValue.Split(','))
                if (int.TryParse(r.Trim(), out int val)) list.Add(val);
        }
        else
        {
            foreach (char c in rawValue)
                if (int.TryParse(c.ToString(), out int val)) list.Add(val);
        }
    }

    private void ParseCountries(string rawValue, List<CountryType> list)
    {
        if (string.IsNullOrWhiteSpace(rawValue)) return;

        string[] names = rawValue.Split('/');
        foreach (var name in names)
        {
            CountryType type = (name.Trim()) switch
            {
                "리리컬모나스테리오" => CountryType.LyricalMonasterio,
                "드래곤엠파이어" => CountryType.DragonEmpire,
                "케테르생츄어리" => CountryType.KeterSanctuary,
                "다크스테이츠" => CountryType.DarkStates,
                "브랜트게이트" => CountryType.BrandtGate,
                "스토이케이아" => CountryType.Stoicheia,
                "국가없음" => CountryType.None,
                _ => CountryType.None
            };

            if (type == CountryType.None)
            {
                // 시트에 있는 텍스트가 안 매핑되었을 때 콘솔에 출력
                Debug.LogWarning($"[Country 파싱 실패] 알 수 없는 국가 텍스트: '...'");
            }
            else if (!list.Contains(type))
            {
                list.Add(type);
            }
        }
    }
    private void ParseTribes(string rawValue, List<string> list)
    {
        if (string.IsNullOrWhiteSpace(rawValue)) return;

        string[] splitTribes = rawValue.Split('/');
        foreach (var t in splitTribes)
        {
            string clean = t.Trim();
            if (!string.IsNullOrEmpty(clean) && !list.Contains(clean))
            {
                list.Add(clean);
            }
        }
    }
    private void ParseSpriteKeys(string rawValue, List<string> list)
    {
        if (string.IsNullOrWhiteSpace(rawValue)) return;

        string[] keys = rawValue.Split('/'); 
        foreach (var k in keys)
        {
            string clean = k.Trim();
            if (!string.IsNullOrEmpty(clean) && !list.Contains(clean))
                list.Add(clean);
        }
    }
}
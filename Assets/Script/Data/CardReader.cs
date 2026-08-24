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
        CountryType country = CountryType.None;
        TribeType tribe = TribeType.None;
        TriggerType trigger = TriggerType.None;
        CardType type = CardType.Normal;
        string flavor = "", effect = "", spriteKey = "";
        List<int> rarities = new List<int>();

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
                case "country": Enum.TryParse(cell.value, true, out country); break;
                case "tribe": Enum.TryParse(cell.value, true, out tribe); break;
                case "triggerType": Enum.TryParse(cell.value, true, out trigger); break;
                case "cardType": Enum.TryParse(cell.value, true, out type); break;
                case "flavorText": flavor = cell.value; break;
                case "effectText": effect = cell.value; break;
                case "spriteAssetKeys": spriteKey = cell.value; break;
                case "availableRarities":
                    ParseRarities(cell.value, rarities);
                    break;
            }
        }

        // 작성하신 CardData 생성자 인자 순서대로 매핑
        DataList.Add(new CardData(
            id, name, grade, power, shield, critical,
            country, tribe, flavor, effect,
            trigger, type, rarities, spriteKey
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
}
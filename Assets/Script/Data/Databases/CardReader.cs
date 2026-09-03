using EnumData;
using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;
using Vanguard.Data.DataModels;

namespace Vanguard.Data.Databases
{
    [CreateAssetMenu(fileName = "CardReader", menuName = "Scriptable Object/CardReader")]
    public class CardReader : ScriptableObject
    {
        [Header("구글 시트 연동 설정")]
        public string associatedSheet = "1Uh8AwERVUNqxEbevHFTfmQTlekXF0bLleCPzgQYrFo8";
        public string associatedWorksheet = "CardData";
        public int START_ROW_LENGTH = 2;

        [Header("저장된 카드 데이터")]
        public List<CardData> DataList = new List<CardData>();

        public void AddDataFromRow(List<GSTU_Cell> list)
        {
            int id = 0; string name = ""; int grade = 0, power = 0, shield = 0, critical = 0;
            List<CountryType> countryList = new List<CountryType>();
            List<TribeType> tribeList = new List<TribeType>();
            TriggerType trigger = TriggerType.None;
            CardType type = CardType.Normal;
            string flavor = "", effect = "";
            List<string> spriteKeys = new List<string>();
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
                    case "country": ParseCountries(cell.value, countryList); break;
                    case "tribe": ParseTribes(cell.value, tribeList); break;
                    case "triggerType": Enum.TryParse(cell.value, true, out trigger); break;
                    case "cardType": Enum.TryParse(cell.value, true, out type); break;
                    case "flavorText": flavor = cell.value; break;
                    case "effectText": effect = cell.value; break;
                    case "spriteAssetKeys": ParseSpriteKeys(cell.value, spriteKeys); break;
                    case "availableRarities": ParseRarities(cell.value, rarities); break;
                }
            }


            {
                // 💡 [수정 4] 최초 등록
                DataList.Add(new CardData(
                    id, name, grade, power, shield, critical,
                    new List<CountryType>(countryList), new List<TribeType>(tribeList),
                    flavor, effect, trigger, type,
                    new List<int>(rarities), new List<string>(spriteKeys)
                ));


            }
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

            foreach (var name in rawValue.Split('/'))
            {
                CountryType type = EnumDataMapper.ParseCountry(name);
                if (type != CountryType.None && !list.Contains(type))
                    list.Add(type);
            }
        }

        private void ParseTribes(string rawValue, List<TribeType> list)
        {
            if (string.IsNullOrWhiteSpace(rawValue)) return;

            foreach (var t in rawValue.Split('/'))
            {
                TribeType type = EnumDataMapper.ParseTribe(t);
                if (type != TribeType.None && !list.Contains(type))
                    list.Add(type);
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
}
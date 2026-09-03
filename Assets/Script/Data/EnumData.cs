using System.Collections.Generic;
using System.Linq;

namespace EnumData
{
    public enum CountryType
    {
        DragonEmpire,
        DarkStates,
        BrandtGate,
        KeterSanctuary,
        Stoicheia,
        LyricalMonasterio,
        None
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
        Mermaid
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
        Trigger,
        Order

    }
    public enum CardCategoryType
    {
        All,
        Grade0,
        Grade1,
        Grade2,
        Grade3,
        Order
    }
    public static class EnumDataMapper
    {
        public static CountryType ParseCountry(string text)
        {
            return text?.Trim() switch
            {
                "드래곤엠파이어" => CountryType.DragonEmpire,
                "다크스테이츠" => CountryType.DarkStates,
                "브랜트게이트" => CountryType.BrandtGate,
                "케테르생츄어리" => CountryType.KeterSanctuary,
                "스토이케이아" => CountryType.Stoicheia,
                "리리컬모나스테리오" => CountryType.LyricalMonasterio,
                "무국가" => CountryType.None
            };
        }
        public static TribeType ParseTribe(string text)
        {
            return text?.Trim() switch
            {
                "휴먼" => TribeType.Human,
                "고스트" => TribeType.Ghost,
                "엔젤" => TribeType.Angel,
                "데몬" => TribeType.Demon,
                "워비스트" => TribeType.Warbeast,
                "자이언트" => TribeType.Giant,
                "뱀파이어" => TribeType.Vampire,
                "엘프" => TribeType.Elf,
                "환영" => TribeType.Phantom,
                "드래고로이드" => TribeType.Dragoroid,
                "바이오로이드" => TribeType.Bioroid,
                "사이버로이드" => TribeType.Cyberloid,
                "배틀로이드" => TribeType.Battroid,
                "서큐버스" => TribeType.Succubus,
                "포레스트드래곤" => TribeType.ForestDragon,
                "머메이드" => TribeType.Mermaid,

                _ => TribeType.None
            };
        }

        public static string ToKorean(this CountryType country) => country switch
        {
            CountryType.DragonEmpire => "드래곤엠파이어",
            CountryType.DarkStates => "다크스테이츠",
            CountryType.BrandtGate => "브랜트게이트",
            CountryType.KeterSanctuary => "케테르생츄어리",
            CountryType.Stoicheia => "스토이케이아",
            CountryType.LyricalMonasterio => "리리컬모나스테리오",
            CountryType.None => "무국가",
        };
        public static string ToKorean(this TribeType tribe) => tribe switch
        {

            TribeType.Human => "휴먼",
            TribeType.Ghost => "고스트",
            TribeType.Angel => "엔젤",
            TribeType.Demon => "데몬",
            TribeType.Warbeast => "워비스트",
            TribeType.Giant => "자이언트",
            TribeType.Vampire => "뱀파이어",
            TribeType.Elf => "엘프",
            TribeType.Phantom => "환영",
            TribeType.Dragoroid => "드래고로이드",
            TribeType.Bioroid => "바이오로이드",
            TribeType.Cyberloid => "사이버로이드",
            TribeType.Battroid => "배틀로이드",
            TribeType.Succubus => "서큐버스",
            TribeType.ForestDragon => "포레스트드래곤",
            TribeType.Mermaid => "머메이드",
            TribeType.None => "-"
        };

        public static string ToKoreanString(this List<CountryType> list)
        {
            if (list == null || list.Count == 0) return "없음";
            return string.Join(" / ", list.Select(c => c.ToKorean()));
        }

        public static string ToKoreanString(this List<TribeType> list)
        {
            if (list == null || list.Count == 0) return "-";
            return string.Join(" / ", list.Select(t => t.ToKorean()));
        }
    }
}

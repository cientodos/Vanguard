using EnumData;
using System.Linq;
using UnityEngine;
public static class DeckValidator
{
    public static bool Validate(DeckData deck, out string errorMessage)
    {
        // 1. 메인 덱 매수 체크 (50장)
        if (deck.mainDeckCardIds.Count != 50)
        {
            errorMessage = $"메인 덱은 정확히 50장이어야 합니다. (현재: {deck.mainDeckCardIds.Count}장)";
            return false;
        }

        // 2. 동명 카드 최대 4장 제한 체크
        var cardCounts = deck.mainDeckCardIds
            .GroupBy(id => id)
            .ToDictionary(g => g.Key, g => g.Count());

        foreach (var pair in cardCounts)
        {
            if (pair.Value > 4)
            {
                errorMessage = $"동명 카드는 최대 4장까지만 투입 가능합니다.";
                return false;
            }
        }

        // 3. 트리거 매수 체크 (정확히 16장)
        int triggerCount = deck.mainDeckCardIds
            .Count(id => CardDatabase.Get(id).triggerType != TriggerType.None);

        if (triggerCount != 16)
        {
            errorMessage = $"트리거 카드는 정확히 16장이어야 합니다. (현재: {triggerCount}장)";
            return false;
        }


        errorMessage = string.Empty;
        return true;
    }
}
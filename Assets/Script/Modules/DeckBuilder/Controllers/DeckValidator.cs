using EnumData;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vanguard.Data.Databases;
using Vanguard.Data.DataModels;

public static class DeckValidator
{
    public static bool Validate(DeckData deck, out string errorMessage)
    {
        errorMessage = string.Empty;

        // 1. 메인 덱 수량 체크 (50장)
        if (deck.mainDeckCardIds == null || deck.mainDeckCardIds.Count != 50)
        {
            errorMessage = $"메인 덱은 정확히 50장이어야 합니다. (현재: {deck.mainDeckCardIds?.Count ?? 0}장)";
            return false;
        }

        // 2. 라이드 덱 수량 체크 (4장)
        if (deck.rideDeckCardIds == null || deck.rideDeckCardIds.Count != 4)
        {
            errorMessage = $"라이드 덱은 정확히 4장이어야 합니다. (현재: {deck.rideDeckCardIds?.Count ?? 0}장)";
            return false;
        }

        // 3. 라이드 덱 그레이드 구성 검사 (G0, G1, G2, G3 각 1장)
        if (!ValidateRideDeckGrades(deck.rideDeckCardIds, out errorMessage))
        {
            return false;
        }

        // 4. 메인 덱 + 라이드 덱 통합 매수 제한 (동명 카드 및 maxCount)
        if (!ValidateCardMaxCounts(deck, out errorMessage))
        {
            return false;
        }

        // 5. 트리거 매수 검사 (메인 덱 기준 정확히 16장)
        int triggerCount = deck.mainDeckCardIds
            .Count(id => CardDatabase.Get(id)?.triggerType != TriggerType.None);

        if (triggerCount != 16)
        {
            errorMessage = $"트리거 카드는 정확히 16장이어야 합니다. (현재: {triggerCount}장)";
            return false;
        }

        // 6. 오버트리거 매수 검사 (메인/라이드 덱 통틀어 최대 1장)
        int overTriggerCount = deck.mainDeckCardIds.Concat(deck.rideDeckCardIds)
            .Count(id => CardDatabase.Get(id)?.triggerType == TriggerType.Over);

        if (overTriggerCount > 1)
        {
            errorMessage = $"오버트리거 카드는 덱 전체에 단 1장만 투입 가능합니다. (현재: {overTriggerCount}장)";
            return false;
        }

        return true;
    }

    /// <summary>
    /// 라이드 덱 G0~G3 구성 단독 검증
    /// </summary>
    private static bool ValidateRideDeckGrades(List<int> rideDeckIds, out string errorMessage)
    {
        errorMessage = string.Empty;
        var grades = rideDeckIds.Select(id => CardDatabase.Get(id)?.grade ?? -1).ToList();

        for (int g = 0; g <= 3; g++)
        {
            if (grades.Count(grade => grade == g) != 1)
            {
                errorMessage = $"라이드 덱에는 Grade {g} 카드가 정확히 1장 포함되어야 합니다.";
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 메인 + 라이드 덱 통합 maxCount 제한 검사
    /// </summary>
    private static bool ValidateCardMaxCounts(DeckData deck, out string errorMessage)
    {
        errorMessage = string.Empty;
        var allCardIds = deck.mainDeckCardIds.Concat(deck.rideDeckCardIds);
        var cardGroups = allCardIds.GroupBy(id => id);

        foreach (var group in cardGroups)
        {
            int cardId = group.Key;
            int count = group.Count();
            var cardData = CardDatabase.Get(cardId);

            int allowedMaxCount = cardData?.maxCount ?? 4; // 기본 4장

            if (count > allowedMaxCount)
            {
                string cardName = cardData?.cardName ?? $"ID:{cardId}";
                errorMessage = $"'{cardName}' 카드는 최대 {allowedMaxCount}장까지 투입할 수 있습니다. (현재: {count}장)";
                return false;
            }
        }
        return true;
    }
}
using EnumData;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vanguard.Data.Databases;
using Vanguard.Data.DataModels;


public static class DeckValidator
{
    #region 카드추가시 검증
    public static bool CanAddCard(DeckData deck, int rawCardId, out string reason)
    {
        reason = string.Empty;

        // 1. 기본 Validation
        if (deck == null)
        {
            reason = "선택된 덱이 없습니다.";
            return false;
        }

        var cardData = CardDatabase.Get(rawCardId);
        if (cardData == null)
        {
            reason = "존재하지 않는 카드입니다.";
            return false;
        }

        // 2. 메인 덱 최대 수량 제한 (50장)
        if (deck.mainDeckCardIds.Count >= 50)
        {
            reason = "메인 덱에는 최대 50장의 카드만 넣을 수 있습니다.";
            return false;
        }

        // 3. 동일 카드(Base ID 기준) 매수 제한 검사
        int targetBaseId = CardIdUtil.GetBaseCardId(rawCardId);
        int mainCount = deck.mainDeckCardIds.Count(id => CardIdUtil.GetBaseCardId(id) == targetBaseId);
        int rideCount = deck.rideDeckCardIds?.Count(id => CardIdUtil.GetBaseCardId(id) == targetBaseId) ?? 0;

        int totalCount = mainCount + rideCount;
        int allowedMaxCount = cardData.maxCount;

        if (totalCount + 1 > allowedMaxCount)
        {
            string cardName = string.IsNullOrEmpty(cardData.cardName) ? $"ID:{rawCardId}" : cardData.cardName;
            reason = $"'{cardName}' 카드는 덱 전체에 최대 {allowedMaxCount}장까지 넣을 수 있습니다. (현재: {totalCount}장)";
            return false;
        }

        // 4.트리거 매수 사전 검사 
        var allCurrentIds = deck.mainDeckCardIds.Concat(deck.rideDeckCardIds ?? new List<int>());

        if (cardData.triggerType == TriggerType.Over)
        {
            int currentOverCount = allCurrentIds.Count(id => CardDatabase.Get(id)?.triggerType == TriggerType.Over);
            if (currentOverCount >= 1)
            {
                reason = "오버트리거 카드는 덱 전체에 단 1장만 투입할 수 있습니다.";
                return false;
            }
        }
        if (cardData.triggerType == TriggerType.Critical)
        {
            int currentCriticalCount = allCurrentIds.Count(id => CardDatabase.Get(id)?.triggerType == TriggerType.Critical);
            if (currentCriticalCount >= 8)
            {
                reason = "크리티컬트리거 카드는 덱 전체에 단 8장만 투입할 수 있습니다.";
                return false;
            }
        }
        if (cardData.triggerType == TriggerType.Draw)
        {
            int currentDrawCount = allCurrentIds.Count(id => CardDatabase.Get(id)?.triggerType == TriggerType.Draw);
            if (currentDrawCount >= 8)
            {
                reason = "드로우트리거 카드는 덱 전체에 단 8장만 투입할 수 있습니다.";
                return false;
            }
        }
        if (cardData.triggerType == TriggerType.Front)
        {
            int currentFrontCount = allCurrentIds.Count(id => CardDatabase.Get(id)?.triggerType == TriggerType.Front);
            if (currentFrontCount >= 8)
            {
                reason = "프론트트리거 카드는 덱 전체에 단 8장만 투입할 수 있습니다.";
                return false;
            }
        }
        if (cardData.triggerType == TriggerType.Heal)
        {
            int currentHealCount = allCurrentIds.Count(id => CardDatabase.Get(id)?.triggerType == TriggerType.Heal);
            if (currentHealCount >= 4)
            {
                reason = "힐 트리거 카드는 덱 전체에 최대 4장까지만 투입할 수 있습니다.";
                return false;
            }
        }

        if (cardData.triggerType != TriggerType.None)
        {
            int currentTotalTriggers = allCurrentIds.Count(id => CardDatabase.Get(id)?.triggerType != TriggerType.None);
            if (currentTotalTriggers >= 16)
            {
                reason = "트리거 카드는 덱 전체에 최대 16장까지만 투입할 수 있습니다.";
                return false;
            }
        }

        // 4.수호자 매수 사전 검사 
        if (cardData.isSentinel == true)
        { 
            int currentSentinel = allCurrentIds.Count(id => CardDatabase.Get(id)?.isSentinel == true );
            if (currentSentinel >= 4)
            {
                reason = "수호자 카드는 덱 전체에 최대 4장까지만 투입할 수 있습니다.";
                return false;
            }
        }

        return true;
    }
    #endregion
    #region 덱완성도 검증
    public static bool Validate(DeckData deck, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (deck.mainDeckCardIds == null || deck.mainDeckCardIds.Count != 50)
        {
            errorMessage = $"메인 덱은 정확히 50장이어야 합니다. (현재: {deck.mainDeckCardIds?.Count ?? 0}장)";
            return false;
        }

        if (deck.rideDeckCardIds == null || deck.rideDeckCardIds.Count != 4)
        {
            errorMessage = $"라이드 덱은 정확히 4장이어야 합니다. (현재: {deck.rideDeckCardIds?.Count ?? 0}장)";
            return false;
        }

        if (!ValidateRideDeckGrades(deck.rideDeckCardIds, out errorMessage))
        {
            return false;
        }

        if (!ValidateCardMaxCounts(deck, out errorMessage))
        {
            return false;
        }

        int triggerCount = deck.mainDeckCardIds.Concat(deck.rideDeckCardIds).Count(id => CardDatabase.Get(id)?.triggerType != TriggerType.None);

        if (triggerCount != 16)
        {
            errorMessage = $"트리거 카드는 정확히 16장이어야 합니다. (현재: {triggerCount}장)";
            return false;
        }

        int ciritcalTriggerCount = deck.mainDeckCardIds.Concat(deck.rideDeckCardIds).Count(id => CardDatabase.Get(id)?.triggerType == TriggerType.Critical);

        if (ciritcalTriggerCount > 8)
        {
            errorMessage = $"크리티컬 트리거 카드는 덱 전체에 단 1장만 투입 가능합니다. (현재: {ciritcalTriggerCount}장)";
            return false;
        }

        int frontTriggerCount = deck.mainDeckCardIds.Concat(deck.rideDeckCardIds).Count(id => CardDatabase.Get(id)?.triggerType == TriggerType.Front);

        if (frontTriggerCount > 8)
        {
            errorMessage = $"크리티컬 트리거 카드는 덱 전체에 단 1장만 투입 가능합니다. (현재: {frontTriggerCount}장)";
            return false;
        }

        int drawTriggerCount = deck.mainDeckCardIds.Concat(deck.rideDeckCardIds).Count(id => CardDatabase.Get(id)?.triggerType == TriggerType.Draw);

        if (drawTriggerCount > 8)
        {
            errorMessage = $"크리티컬 트리거 카드는 덱 전체에 단 1장만 투입 가능합니다. (현재: {drawTriggerCount}장)";
            return false;
        }

        int healTriggerCount = deck.mainDeckCardIds.Concat(deck.rideDeckCardIds).Count(id => CardDatabase.Get(id)?.triggerType == TriggerType.Heal);

        if (healTriggerCount > 4)
        {
             errorMessage = $"힐 트리거 카드는 4장까지만 투입 가능합니다. (현재: {healTriggerCount}장)";
            return false;
        }

        int overTriggerCount = deck.mainDeckCardIds.Concat(deck.rideDeckCardIds).Count(id => CardDatabase.Get(id)?.triggerType == TriggerType.Over);

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
    #endregion
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Vanguard.Data.Databases;
using Vanguard.Data.DataModels;

public class DeckManager : MonoBehaviour, IPointerClickHandler
{
    public static DeckManager Instance { get; private set; }
    public DeckData currentSelectedDeck;

    public const int MAX_DECK_COUNT = 30;         // 최대 덱 저장 개수
    public const int MAX_CARDS_PER_DECK = 50;     // 메인 덱 구성 카드 수 (50장)
    public const int MAX_RIDE_DECK_COUNT = 4;     // 라이드 덱 구성 카드 수 (4장)
    public const int MAX_SAME_CARD_COUNT = 4;    // 동일 카드 최대 매수 제한

    public event EventHandler OnspacePressed;

    [Header("현재 로드된 덱 목록")]
    public List<DeckData> deckList = new List<DeckData>();

    private string saveFilePath;

    public event Action<DeckData> OnDeckUpdated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, "DeckData.json");
            LoadDecks();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectDeck(DeckData deck)
    {
        currentSelectedDeck = deck;
        OnDeckUpdated?.Invoke(currentSelectedDeck);
    }

    public void SelectDeckById(string deckId)
    {
        currentSelectedDeck = deckList.Find(d => d.deckId == deckId);
        if (currentSelectedDeck != null)
        {
            OnDeckUpdated?.Invoke(currentSelectedDeck);
        }
    }

    #region 덱 C.R.U.D 로직

    // 1. 신규 덱 생성
    public bool CreateNewDeck(string deckName, out DeckData newDeck)
    {
        newDeck = null;

        if (deckList.Count >= MAX_DECK_COUNT)
        {
            Debug.LogWarning($"[DeckManager] 덱은 최대 {MAX_DECK_COUNT}개까지만 저장할 수 있습니다.");
            return false;
        }

        newDeck = new DeckData(deckName);
        deckList.Add(newDeck);
        SaveDecks();
        return true;
    }

    // 2. 덱 삭제
    public bool DeleteDeck(string deckId)
    {
        int index = deckList.FindIndex(d => d.deckId == deckId);
        if (index != -1)
        {
            deckList.RemoveAt(index);
            SaveDecks();
            return true;
        }
        return false;
    }

    // 3. 덱에 카드 추가 검증 (메인 덱 + 라이드 덱 합산 수량 검사)
    public bool CanAddCardToDeck(DeckData deck, int rawCardId, out string failureReason)
    {
        failureReason = "";

        if (deck == null)
        {
            failureReason = "선택된 덱이 없습니다.";
            return false;
        }

        // 메인 덱 최대 수량 체크
        if (deck.mainDeckCardIds.Count >= MAX_CARDS_PER_DECK)
        {
            failureReason = $"메인 덱에는 최대 {MAX_CARDS_PER_DECK}장의 카드만 넣을 수 있습니다.";
            return false;
        }

        // 메인 덱 + 라이드 덱 합산 동명 카드(Base ID 기준) 수량 체크
        int targetBaseId = CardIdUtil.GetBaseCardId(rawCardId);

        int mainCount = deck.mainDeckCardIds.Count(id => CardIdUtil.GetBaseCardId(id) == targetBaseId);
        int rideCount = deck.rideDeckCardIds != null ? deck.rideDeckCardIds.Count(id => CardIdUtil.GetBaseCardId(id) == targetBaseId) : 0;

        int totalSameCardCount = mainCount + rideCount;

        if (totalSameCardCount >= MAX_SAME_CARD_COUNT)
        {
            failureReason = $"동일한 카드는 메인/라이드 덱을 합쳐 최대 {MAX_SAME_CARD_COUNT}장까지 넣을 수 있습니다.";
            return false;
        }

        return true;
    }

    // 4. 메인 덱에 카드 추가
    public bool AddCardToCurrentDeck(int rawCardId)
    {
        if (currentSelectedDeck == null) return false;

        if (!CanAddCardToDeck(currentSelectedDeck, rawCardId, out string failureReason))
        {
            Debug.LogWarning($"[DeckManager] 카드 추가 실패: {failureReason}");
            return false;
        }

        currentSelectedDeck.mainDeckCardIds.Add(rawCardId);
        SaveDecks();
        OnDeckUpdated?.Invoke(currentSelectedDeck);
        return true;
    }

    // 5. 덱에서 카드 제거 (메인 덱 및 라이드 덱 처리)
    public bool RemoveCardFromDeck(DeckData deck, int rawCardId)
    {
        if (deck == null) return false;

        bool removedFromMain = deck.mainDeckCardIds.Remove(rawCardId);
        bool removedFromRide = deck.rideDeckCardIds != null && deck.rideDeckCardIds.Remove(rawCardId);

        if (removedFromMain || removedFromRide)
        {
            SaveDecks();
            OnDeckUpdated?.Invoke(currentSelectedDeck);
            return true;
        }
        return false;
    }

    // 6. 메인 덱에 있는 카드를 라이드 덱으로 지정 / 해제 토글
    public bool ToggleRideDeckCard(int rawCardId, out string failureReason)
    {
        failureReason = string.Empty;

        if (currentSelectedDeck == null)
        {
            failureReason = "선택된 덱이 없습니다.";
            return false;
        }

        if (currentSelectedDeck.rideDeckCardIds == null)
        {
            currentSelectedDeck.rideDeckCardIds = new List<int>();
        }

        // 이미 라이드 덱에 지정되어 있다면 -> 지정 해제
        if (currentSelectedDeck.rideDeckCardIds.Contains(rawCardId))
        {
            currentSelectedDeck.rideDeckCardIds.Remove(rawCardId);
            SaveDecks();
            OnDeckUpdated?.Invoke(currentSelectedDeck);
            return true;
        }

        // 새로 라이드 덱으로 지정하려는 경우
        var cardData = CardDatabase.Get(rawCardId);
        if (cardData == null)
        {
            failureReason = "존재하지 않는 카드입니다.";
            return false;
        }

        // 동일 Grade 카드가 라이드 덱에 있다면 기존 카드는 해제 (교체)
        int sameGradeCardId = currentSelectedDeck.rideDeckCardIds
            .FirstOrDefault(id => CardDatabase.Get(id)?.grade == cardData.grade);

        if (sameGradeCardId != 0)
        {
            currentSelectedDeck.rideDeckCardIds.Remove(sameGradeCardId);
        }

        currentSelectedDeck.rideDeckCardIds.Add(rawCardId);
        SaveDecks();
        OnDeckUpdated?.Invoke(currentSelectedDeck);
        return true;
    }

    public bool RenameDeck(string deckId, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName)) return false;

        DeckData targetDeck = deckList.Find(d => d.deckId == deckId);
        if (targetDeck != null)
        {
            targetDeck.deckName = newName.Trim();
            SaveDecks();
            return true;
        }

        return false;
    }

    #endregion

    #region 저장 및 불러오기

    public void SaveDecks()
    {
        string json = JsonUtility.ToJson(new DeckListWrapper { items = deckList }, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"<color=green>[DeckManager]</color> 덱 {deckList.Count}개 저장 완료.");
    }

    public void LoadDecks()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            var wrapper = JsonUtility.FromJson<DeckListWrapper>(json);
            deckList = wrapper != null && wrapper.items != null ? wrapper.items : new List<DeckData>();
        }
        else
        {
            deckList = new List<DeckData>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 필요 시 UI 이벤트 수신용 구현
    }

    #endregion
}

[Serializable]
public class DeckListWrapper
{
    public List<DeckData> items;
}
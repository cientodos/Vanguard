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
    #region Header
    public static DeckManager Instance { get; private set; }
    public DeckData currentSelectedDeck;

    public const int MAX_DECK_COUNT = 30;         // 최대 덱 저장 개수
    public const int MAX_CARDS_PER_DECK = 50;     // 메인 덱 구성 카드 수 (50장)
    public const int MAX_RIDE_DECK_COUNT = 4;     // 라이드 덱 구성 카드 수 (4장)
    public const int MAX_SAME_CARD_COUNT = 4;    // 동일 카드 최대 매수 제한
    public List<int> mainDeckCardIds = new List<int>();  // 메인 덱
    public List<int> extraDeckCardIds = new List<int>(); // 엑스트라 덱
    public List<int> rideDeckCardIds = new List<int>();  // 라이드 덱
   

    [Header("현재 로드된 덱 목록")]
    public List<DeckData> deckList = new List<DeckData>();
    public static event Action<string> OnDeckErrorOccurred;
    public event Action<DeckData> OnDeckUpdated;
    public event EventHandler OnspacePressed;
    private string saveFilePath;
    #endregion


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

    #region 덱 C.R.U.D 로직

    // 1. 신규 덱 생성
    public bool CreateNewDeck(string deckName, out DeckData newDeck)
    {
        newDeck = null;

        if (deckList.Count >= MAX_DECK_COUNT)
        {
            OnDeckErrorOccurred?.Invoke("덱은최대 30개까지 보유가능합니다");
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

  
    public bool AddCardToCurrentDeck(int rawCardId)
    {
        if (currentSelectedDeck == null) return false;

        if (!DeckValidator.CanAddCard(currentSelectedDeck, rawCardId, out string errorMessage))
        {
            // 실패 시 Manager가 에러 이벤트 발송
            OnDeckErrorOccurred?.Invoke(errorMessage);
            return false;
        }

        currentSelectedDeck.mainDeckCardIds.Add(rawCardId);
        SaveDecks();
        OnDeckUpdated?.Invoke(currentSelectedDeck);
        return true;
    }

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


    public bool ToggleRideDeckCard(int rawCardId)
    {
        if (currentSelectedDeck == null)
        {
            OnDeckErrorOccurred?.Invoke("선택된 덱이 없습니다.");
            return false;
        }

      
        if (currentSelectedDeck.rideDeckCardIds.Contains(rawCardId))
        {
            currentSelectedDeck.rideDeckCardIds.Remove(rawCardId);
            currentSelectedDeck.mainDeckCardIds.Add(rawCardId); 

            SaveDecks();
            OnDeckUpdated?.Invoke(currentSelectedDeck);
            return true;
        }

        // 2. 메인 덱에서 가져와 라이드 덱으로 이동하려는 경우
        var cardData = CardDatabase.Get(rawCardId);
        if (cardData == null) return false;

        // 동일 Grade 카드가 이미 라이드 덱에 있다면 기존 카드를 해제하여 메인 덱으로 복귀 (교체)
        int sameGradeCardId = currentSelectedDeck.rideDeckCardIds
            .FirstOrDefault(id => CardDatabase.Get(id)?.grade == cardData.grade);

        if (sameGradeCardId != 0)
        {
            currentSelectedDeck.rideDeckCardIds.Remove(sameGradeCardId);
            currentSelectedDeck.mainDeckCardIds.Add(sameGradeCardId); 
        }

        // 메인 덱에서 제거 후 라이드 덱에 추가
        if (currentSelectedDeck.mainDeckCardIds.Remove(rawCardId))
        {
            currentSelectedDeck.rideDeckCardIds.Add(rawCardId);

            SaveDecks();
            OnDeckUpdated?.Invoke(currentSelectedDeck);
            return true;
        }

        return false;
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
        if (deckList.Count > 0)
        {
            currentSelectedDeck = deckList[0];
            Debug.Log($"[DeckManager]</color> 기본 덱 지정 완료: {currentSelectedDeck.deckName}");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    #endregion
}

[Serializable]
public class DeckListWrapper
{
    public List<DeckData> items;
}
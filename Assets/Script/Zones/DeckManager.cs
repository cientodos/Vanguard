using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance { get; private set; }

    [Header("[ 덱 데이터 ]")]
    // 덱은 가변적인 카드 목록이므로 List를 사용합니다.
    [SerializeField] private List<Card> deckList = new List<Card>();
    [SerializeField] public List<Card> ridedeckList = new List<Card>();

    // 남은 카드의 수
    public int CardCount => deckList.Count;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// 덱 셔플 (Fisher-Yates 알고리즘)
    /// </summary>
    public void Shuffle()
    {
        for (int i = 0; i < deckList.Count; i++)
        {
            Card temp = deckList[i];
            int randomIndex = Random.Range(i, deckList.Count);
            deckList[i] = deckList[randomIndex];
            deckList[randomIndex] = temp;
        }
        Debug.Log("덱을 셔플했습니다.");
    }

    /// <summary>
    /// 덱 맨 위에서 카드 1장 드로우
    /// </summary>
    public Card DrawCard()
    {
        if (deckList.Count <= 0)
        {
            Debug.LogWarning("덱에 남은 카드가 없습니다!");
            return null;
        }

        // 맨 위(0번 인덱스) 카드를 가져온 후 덱 리스트에서 제거
        Card drawnCard = deckList[0];
        deckList.RemoveAt(0);

        return drawnCard;
    }

    /// <summary>
    /// 특정 카드를 덱 맨 아래에 추가
    /// </summary>
    public void AddCardToBottom(Card card)
    {
        deckList.Add(card);
    }

    /// <summary>
    /// 특정 카드를 덱 맨 위에 추가
    /// </summary>
    public void AddCardToTop(Card card)
    {
        deckList.Insert(0, card);
    }
}
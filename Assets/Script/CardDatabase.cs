using System.Collections.Generic;
using UnityEngine;

public static class CardDatabase
{
    private static CardReader _cardReader;
    private static readonly Dictionary<int, CardData> _cardDict = new();
    private static bool _isInitialized = false;

    public static void Initialize()
    {
        if (_isInitialized) return;

        // Resources 폴더에서 CardReader.asset 로드
        _cardReader = Resources.Load<CardReader>("CardReader");

        if (_cardReader == null)
        {
            Debug.LogError("[CardDatabase] Resources 폴더에서 CardReader.asset을 찾을 수 없습니다!");
            return;
        }

        // 빠른 조회를 위해 딕셔너리에 매핑
        _cardDict.Clear();
        foreach (var data in _cardReader.DataList) 
        {
            if (data != null && !_cardDict.ContainsKey(data.id))
            {
                _cardDict.Add(data.id, data);
            }
        }

        _isInitialized = true;
        Debug.Log($"[CardDatabase] 총 {_cardDict.Count}장의 카드가 성공적으로 로드되었습니다.");
    }

    public static CardData Get(int id)
    {
        if (!_isInitialized) Initialize();

        if (_cardDict.TryGetValue(id, out var card))
        {
            return card;
        }

        Debug.LogError($"[CardDatabase] ID {id}에 해당하는 카드가 없습니다.");
        return null;
    }
}
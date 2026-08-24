using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeckData
{
    public string deckName = "기본덱"; // 이후에는 유저 입력값에 따라 덱이름 정하기 가능
    public List<int> rideDeckCardIds = new();
    public List<int> mainDeckCardIds = new();
}

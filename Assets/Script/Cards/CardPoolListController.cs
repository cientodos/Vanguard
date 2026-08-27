using System.Collections.Generic;
using UnityEngine;
using PolyAndCode.UI;

public class CardPoolListController : MonoBehaviour, IRecyclableScrollRectDataSource
{
    [SerializeField] private RecyclableScrollRect recyclableScrollRect;
    [SerializeField] private CardReader cardReader;
    // CardReader의 dataList 원본 데이터 연결
    private List<CardData> cardDataList = new();

    public void Awake()
    {
        recyclableScrollRect.DataSource = this;
    }
    private void Start()
    {
        InitCardList();
    }

    public void InitCardList()
    {
        if (cardReader != null)
        {
            cardDataList = cardReader.DataList;

            Debug.Log($"[CardPoolListController] 로드된 카드 개수: {cardDataList.Count}");
            recyclableScrollRect.Initialize(this);
            recyclableScrollRect.ReloadData();
        }
        else
        {
            Debug.Log("[CardPoolListController] cardReader 에셋이 할당되지않음");
        }
    }

    public int GetItemCount() => cardDataList.Count;
    
    public void SetCell(ICell cell, int index)
    {
        var itemCell = cell as CardItemCell;
        if (itemCell != null && index < cardDataList.Count)
        {
            itemCell.ConfigureCell(cardDataList[index]);
        }
    }
}
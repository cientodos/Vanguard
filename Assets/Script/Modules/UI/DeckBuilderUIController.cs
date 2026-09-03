using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DeckBuilderUIController : MonoBehaviour
{
    [Header("UI References - Info & Statas")]
    [SerializeField] private Text deckCountText;
    [SerializeField] private Text rideDeckCountText;
    [SerializeField] private Text MessageText;

    [Header("UI References - Button")]
    [SerializeField] public Button saveButton;
    [SerializeField] public Button openCardList;

    [Header("UI References - Canvas")]
    [SerializeField] private GameObject cardPanel;
    [SerializeField] private GameObject deckPanel;


    public void OpenCardPanel()
    { 
        cardPanel.SetActive(true);
        deckPanel.SetActive(false);
    }
    public void CloseCardPanel()
    {
        cardPanel.SetActive(false);
        deckPanel.SetActive(true);
    }





}

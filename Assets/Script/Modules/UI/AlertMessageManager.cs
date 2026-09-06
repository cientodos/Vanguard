using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vanguard.CardSystem.UI;

public class AlertMessageManager : MonoBehaviour
{
    #region Header
    [Header("UI ÄÄÆ÷³ÍÆ®")]
    [SerializeField] private Text alertText;
    [SerializeField] private GameObject alertPanel;
    [SerializeField] private Image alertPanelAlpha;
    #endregion


    private void OnEnable()
    {
        DeckManager.OnDeckErrorOccurred += ShowAlert;
        CardContextMenu.OnMenuErrorOccurred += ShowAlert; 
    }

    private void OnDisable()
    {
        DeckManager.OnDeckErrorOccurred -= ShowAlert;
        CardContextMenu.OnMenuErrorOccurred -= ShowAlert;
    }

    public void ShowAlert(string message)
    {
        alertText.text = message;
        Color color = alertPanelAlpha.color;
        color.a = 0.5f;
        alertPanelAlpha.color = color;
        alertPanel.SetActive(true);
        
      
        CancelInvoke(nameof(HideAlert));
        Invoke(nameof(HideAlert), 2.0f);
    }

    private void HideAlert()
    {
        alertPanel.SetActive(false);
    }
}

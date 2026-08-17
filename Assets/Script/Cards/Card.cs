using UnityEngine;

public class Card : MonoBehaviour
{
    public enum CardState
    {
        stand,
        rest,
        bind,
        guard,
    }
    [SerializeField] private CardState currentState = CardState.stand;

    public CardState CurrentState => currentState;

    [SerializeField]
    public string cardName;
    public string effectText;
    public string tribe;
    public string flavorText;
    public int grade;
    public int power;
    public int shiled;
    public int crit;
    public string countary;
    public bool trigger;


    public void SetState(CardState newstate)
    {
        if(currentState == newstate) return;  

        currentState = newstate;
        OnStateChanged(currentState);
    }
    private void OnStateChanged(CardState state)
    { 
        
    }


}

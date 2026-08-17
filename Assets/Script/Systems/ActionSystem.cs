using UnityEngine;

public class ActionSystem : MonoBehaviour
{
    public static ActionSystem Instance { get; private set; }

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {

            Destroy(gameObject);
        }
    }


    public Card card;
    public void Stand(int id)
    {
        card.SetState(Card.CardState.stand);
    }

    public void Rest()
    {

    }
    public void SystemDraw() 
    {
        
    }
    public void Ride()
    {
        if (DeckManager.Instance?.ridedeckList != null) 
        {
            //HandManager.Instance?.handList.RemoveAt();
        }
    }

}

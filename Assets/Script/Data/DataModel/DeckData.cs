using System;
using System.Collections.Generic;

namespace Vanguard.Data.DataModels
{
    [System.Serializable]
    public class DeckData
    {
        public string deckId;
        public string deckName;
        public List<int> mainDeckCardIds = new List<int>();
        public List<int> rideDeckCardIds = new List<int>();

        public DeckData(string name)
        {
            deckId = Guid.NewGuid().ToString();
            deckName = string.IsNullOrWhiteSpace(name) ? "»õ µ¦" : name;
            mainDeckCardIds = new List<int>();
            rideDeckCardIds = new List<int>();
        }
        [Serializable]
        public class DeckListWrapper
        {
            public List<DeckData> items;
        }
    }

}
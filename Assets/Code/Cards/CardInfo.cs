using UnityEngine;

[CreateAssetMenu(fileName = "New Card Info", menuName = "Create Card Info")]
public class CardInfo : ScriptableObject
{
    [System.Serializable]
    public struct CardImages
    {
        public CardRank cardRank;
        public Sprite sprite;
    }

    public enum CardSuit
    {
        Hearts,
        Diamonds,
        Spades,
        Clubs
    }

    public enum CardRank
    {
        Ace,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }

    [SerializeField] private CardSuit cardSuits;
    [SerializeField] private CardImages[] cardImages;
    public Sprite cardBack;

    public Sprite GetCardImage(CardRank rank)
    {
        for (int i = 0; i < cardImages.Length; i++)
        {
            if (cardImages[i].cardRank == rank)
            {
                return cardImages[i].sprite;
            }
        }

        return null;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNO.GameObjects
{
    public class CardDeck
    {
        public List<Card> Cards { get; set; }

        public CardDeck()
        {
            Cards = new List<Card>();

            foreach (CardColor color in Enum.GetValues(typeof(CardColor)))
            {
                if (color != CardColor.Wild)
                {
                    foreach (CardValue val in Enum.GetValues(typeof(CardValue)))
                    {
                        switch (val)
                        {
                            case CardValue.One:
                            case CardValue.Two:
                            case CardValue.Three:
                            case CardValue.Four:
                            case CardValue.Five:
                            case CardValue.Six:
                            case CardValue.Seven:
                            case CardValue.Eight:
                            case CardValue.Nine:

                                Cards.Add(new Card()
                                {
                                    Color = color,
                                    Value = val,
                                    Score = (int)val
                                });
                                Cards.Add(new Card()
                                {
                                    Color = color,
                                    Value = val,
                                    Score = (int)val
                                });
                                break;
                            case CardValue.Skip:
                            case CardValue.Reverse:
                            case CardValue.DrawTwo:
                                //Two copies of each
                                Cards.Add(new Card()
                                {
                                    Color = color,
                                    Value = val,
                                    Score = 20
                                });
                                Cards.Add(new Card()
                                {
                                    Color = color,
                                    Value = val,
                                    Score = 20
                                });
                                break;

                            case CardValue.Zero:
                                //Zero: One copy per color
                                Cards.Add(new Card()
                                {
                                    Color = color,
                                    Value = val,
                                    Score = 0
                                });
                                break;


                        }
                    }
                }
                else //Wild card
                {
                    for(int i = 1; i <= 4; i++)
                    {
                        Cards.Add(new Card()
                        {
                            Color = color,
                            Value = CardValue.Wild,
                            Score = 50
                        });
                    }
                    //DrawFour
                    for(int i = 1; i <= 4; i++)
                    {
                        Cards.Add(new Card()
                        {
                            Color = color,
                            Value = CardValue.DrawFour,
                            Score = 50
                        });
                    }
                }

            }
        }
        //implementing shuffle method
        public void Shuffle()
        {
            Random rand = new Random();
            List<Card> cards = Cards;

            for (int n = cards.Count - 1; n > 0; --n)
            {
                int j = rand.Next(n + 1);
                Card temp = cards[n];
                cards[n] = cards[j];
                cards[j] = temp;
            }
        }
        public List<Card> Draw(int count)
        {
            var drawnCards = Cards.Take(count).ToList();

            Cards.RemoveAll(x => drawnCards.Contains(x));

            return drawnCards;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNO.GameObjects
{
    public class Player
    {
        public List<Card> Hand { get; set; }

        public int Position { get; set; }

        public Player()
        {
            Hand = new List<Card>();
        }
        public PlayerTurn PlayTurn(PlayerTurn previousTurn, CardDeck drawPile)
        {
            PlayerTurn turn = new PlayerTurn();
            if (previousTurn.Result == TurnResult.Skip
                || previousTurn.Result == TurnResult.DrawTwo
                || previousTurn.Result == TurnResult.WildCardDrawFour)
            {
                return ProcessAttack(previousTurn.Card, drawPile);
            }
            else if ((previousTurn.Result == TurnResult.WildCard
                        || previousTurn.Result == TurnResult.DrawTwoByOpponent
                        || previousTurn.Result == TurnResult.ForceDrawNoMatch)
                        && HasMatch(previousTurn.DeclaredColor))
            {
                turn = PlayMatchingCard(previousTurn.DeclaredColor);
            }
            else if (HasMatch(previousTurn.Card))
            {
                turn = PlayMatchingCard(previousTurn.Card);
            }
            else //See if drawn card is playable
            {
                turn = DrawCard(previousTurn, drawPile);
            }

            DisplayTurn(turn);
            return turn;
        }

        private PlayerTurn DrawCard(PlayerTurn previousTurn, CardDeck drawPile)
        {
            PlayerTurn turn = new PlayerTurn();
            var drawnCard = drawPile.Draw(1);
            Hand.AddRange(drawnCard);

            if (HasMatch(previousTurn.Card))
            {
                turn = PlayMatchingCard(previousTurn.Card);
                turn.Result = TurnResult.ForceDrawNoMatch;
            }
            else
            {
                turn.Result = TurnResult.ForceDrawNoMatch;
                turn.Card = previousTurn.Card;
            }

            return turn;
        }

        private void DisplayTurn(PlayerTurn currentTurn)
        {
            if (currentTurn.Result == TurnResult.ForceDrawNoMatch)
            {
                Console.WriteLine("Player " + Position.ToString() + " is forced to draw.");
            }
            if (currentTurn.Result == TurnResult.ForceDrawNoMatch)
            {
                Console.WriteLine("Player " + Position.ToString() + " is forced to draw AND can play the drawn card!");
            }

            if (currentTurn.Result == TurnResult.PlayedCard
                || currentTurn.Result == TurnResult.Skip
                || currentTurn.Result == TurnResult.DrawTwo
                || currentTurn.Result == TurnResult.WildCard
                || currentTurn.Result == TurnResult.WildCardDrawFour
                || currentTurn.Result == TurnResult.Reversed
                || currentTurn.Result == TurnResult.ForceDrawNoMatch)
            {
                Console.WriteLine("Player " + Position.ToString() + " plays a " + currentTurn.Card.DisplayValue + " card.");
                if (currentTurn.Card.Color == CardColor.Wild)
                {
                    Console.WriteLine("Player " + Position.ToString() + " declares " + currentTurn.DeclaredColor.ToString() + " as the new color.");
                }
                if (currentTurn.Result == TurnResult.Reversed)
                {
                    Console.WriteLine("Turn order reversed!");
                }
            }

            if (Hand.Count == 1)
            {
                Console.WriteLine("Player " + Position.ToString() + " shouts Uno!");
            }
        }

        private PlayerTurn ProcessAttack(Card currentDiscard, CardDeck drawPile)
        {
            PlayerTurn turn = new PlayerTurn();
            turn.Result = TurnResult.DrawTwoByOpponent;
            turn.Card = currentDiscard;
            turn.DeclaredColor = currentDiscard.Color;
            if (currentDiscard.Value == CardValue.Skip)
            {
                Console.WriteLine("Player " + Position.ToString() + " was skipped!");
                return turn;
            }
            else if (currentDiscard.Value == CardValue.DrawTwo)
            {
                Console.WriteLine("Player " + Position.ToString() + " must draw two cards!");
                Hand.AddRange(drawPile.Draw(2));
            }
            else if (currentDiscard.Value == CardValue.DrawFour)
            {
                Console.WriteLine("Player " + Position.ToString() + " must draw four cards!");
                Hand.AddRange(drawPile.Draw(4));
            }

            return turn;
        }

        private bool HasMatch(Card card)
        {
            return Hand.Any(x => x.Color == card.Color || x.Value == card.Value || x.Color == CardColor.Wild);
        }

        private bool HasMatch(CardColor color)
        {
            return Hand.Any(x => x.Color == color || x.Color == CardColor.Wild);
        }

        private PlayerTurn PlayMatchingCard(CardColor color)
        {
            var turn = new PlayerTurn();
            turn.Result = TurnResult.PlayedCard;
            var matching = Hand.Where(x => x.Color == color || x.Color == CardColor.Wild).ToList();

            //We cannot play the wild draw four card unless there are no other matches.
            if (matching.All(x => x.Value == CardValue.DrawFour))
            {
                turn.Card = matching.First();
                turn.DeclaredColor = SelectDominantColor();
                turn.Result = TurnResult.WildCard;
                Hand.Remove(matching.First());

                return turn;
            }

           
            if (matching.Any(x => x.Value == CardValue.DrawTwo))
            {
                turn.Card = matching.First(x => x.Value == CardValue.DrawTwo);
                turn.Result = TurnResult.DrawTwo;
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(turn.Card);

                return turn;
            }

            if (matching.Any(x => x.Value == CardValue.Skip))
            {
                turn.Card = matching.First(x => x.Value == CardValue.Skip);
                turn.Result = TurnResult.Skip;
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(turn.Card);

                return turn;
            }

            if (matching.Any(x => x.Value == CardValue.Reverse))
            {
                turn.Card = matching.First(x => x.Value == CardValue.Reverse);
                turn.Result = TurnResult.Reversed;
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(turn.Card);

                return turn;
            }

            var matchOnColor = matching.Where(x => x.Color == color);
            if (matchOnColor.Any())
            {
                turn.Card = matchOnColor.First();
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(matchOnColor.First());

                return turn;
            }

            if (matching.Any(x => x.Value == CardValue.Wild))
            {
                turn.Card = matching.First(x => x.Value == CardValue.Wild);
                turn.DeclaredColor = SelectDominantColor();
                turn.Result = TurnResult.WildCard;
                Hand.Remove(turn.Card);

                return turn;
            }

            
            turn.Result = TurnResult.ForceDrawNoMatch;
            return turn;
        }

        private PlayerTurn PlayMatchingCard(Card currentDiscard)
        {
            var turn = new PlayerTurn();
            turn.Result = TurnResult.PlayedCard;
            var matching = Hand.Where(x => x.Color == currentDiscard.Color || x.Value == currentDiscard.Value || x.Color == CardColor.Wild).ToList();

            //unless there are no other matches, we cannot play wild card
            if (matching.All(x => x.Value == CardValue.DrawFour))
            {
                turn.Card = matching.First();
                turn.DeclaredColor = SelectDominantColor();
                turn.Result = TurnResult.WildCard;
                Hand.Remove(matching.First());

                return turn;
            }

           
            if (matching.Any(x => x.Value == CardValue.DrawTwo))
            {
                turn.Card = matching.First(x => x.Value == CardValue.DrawTwo);
                turn.Result = TurnResult.DrawTwo;
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(turn.Card);

                return turn;
            }

            if (matching.Any(x => x.Value == CardValue.Skip))
            {
                turn.Card = matching.First(x => x.Value == CardValue.Skip);
                turn.Result = TurnResult.Skip;
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(turn.Card);

                return turn;
            }

            if (matching.Any(x => x.Value == CardValue.Reverse))
            {
                turn.Card = matching.First(x => x.Value == CardValue.Reverse);
                turn.Result = TurnResult.Reversed;
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(turn.Card);

                return turn;
            }

            
            var matchOnColor = matching.Where(x => x.Color == currentDiscard.Color);
            var matchOnValue = matching.Where(x => x.Value == currentDiscard.Value);
            if (matchOnColor.Any() && matchOnValue.Any())
            {
                var correspondingColor = Hand.Where(x => x.Color == matchOnColor.First().Color);
                var correspondingValue = Hand.Where(x => x.Value == matchOnValue.First().Value);
                if (correspondingColor.Count() >= correspondingValue.Count())
                {
                    turn.Card = matchOnColor.First();
                    turn.DeclaredColor = turn.Card.Color;
                    Hand.Remove(matchOnColor.First());

                    return turn;
                }
                else //Matches depend on the value of the card
                {
                    turn.Card = matchOnValue.First();
                    turn.DeclaredColor = turn.Card.Color;
                    Hand.Remove(matchOnValue.First());

                    return turn;
                }
                
            }
            else if (matchOnColor.Any())
            {
                turn.Card = matchOnColor.First();
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(matchOnColor.First());

                return turn;
            }
            else if (matchOnValue.Any())
            {
                turn.Card = matchOnValue.First();
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(matchOnValue.First());

                return turn;
            }

            //If a wild becomes our last card, player wins
            if (matching.Any(x => x.Value == CardValue.Wild))
            {
                turn.Card = matching.First(x => x.Value == CardValue.Wild);
                turn.DeclaredColor = SelectDominantColor();
                turn.Result = TurnResult.WildCard;
                Hand.Remove(turn.Card);

                return turn;
            }

           
            turn.Result = TurnResult.ForceDrawNoMatch;
            return turn;
        }

        private CardColor SelectDominantColor()
        {
            if (!Hand.Any())
            {
                return CardColor.Wild;
            }
            var colors = Hand.GroupBy(x => x.Color).OrderByDescending(x => x.Count());
            return colors.First().First().Color;
        }

        private void SortHand()
        {
            this.Hand = this.Hand.OrderBy(x => x.Color).ThenBy(x => x.Value).ToList();
        }

        public void ShowHand()
        {
            SortHand();
            Console.WriteLine("Player " + Position + "'s Hand: ");
            foreach (var card in Hand)
            {
                Console.Write(Enum.GetName(typeof(CardColor), card.Color) + " " + Enum.GetName(typeof(CardValue), card.Value) + "  ");
            }
            Console.WriteLine("");
        }
    }
}

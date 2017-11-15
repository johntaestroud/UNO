using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UNO.GameObjects;

namespace UNO
{
    class Program
    {
        static void Main(string[] args)
        {
            GameManager manager = new GameManager(6);

            manager.PlayGame();

            Console.ReadKey();
        }
    }
    //public enum CardColor
    // {
    //     Red,
    //     Blue,
    //     Green,
    //     Yellow,
    //     Wild
    // }

    // public enum CardValue
    // {
    //     Zero,
    //     One,
    //     Two,
    //     Three,
    //     Four,
    //     Five,
    //     Six,
    //     Seven,
    //     Eight,
    //     Nine,
    //     DrawTwo,
    //     DrawFour,
    //     Wild,
    //     Skip,
    //     Reverse
    // }

    // public enum TurnResult
    // {
    //     StartGame,
    //     PlayedCard,
    //     Skip,
    //     DrawTwo,
    //     DrawTwoByOpponent,//Forced to draw by other player
    //     ForceDrawNoMatch,//Forced to draw due to no cards matching
    //     ForceDrawNoMatchCardDrawn, //Forced to draw but received matching card
    //     WildCard,
    //     WildCardDrawFour,
    //     Reversed



    // }


}

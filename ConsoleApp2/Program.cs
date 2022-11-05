using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Sockets;
using System.Xml;

namespace Program
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var playerOne = new GameAccount("Bender");
            var playerTwo = new VipAccount("Stepan");
            var playerThree = new CringeAccount("Gordon");
            var g1 = Matchmaking.ReturningClass.MatchmakingG(playerOne, playerTwo, 20);
            var g2 = Matchmaking.ReturningClass.CasualG(playerOne, playerThree);
            var g3 = Matchmaking.ReturningClass.MatchmakingG(playerThree, playerTwo, 10);
            g3.Process();
            g1.Process();
            g2.Process();
            g1.Process();
            g2.Process();
            g3.Process();
            Console.WriteLine(playerOne.GetStats());
            Console.WriteLine(playerTwo.GetStats());
            Console.WriteLine(playerThree.GetStats());
          
        }

        public class GameAccount
        {
            public string UserName { get; }
            public int GamesCount { get; }



            public int GameId
            {
                get
                {
                    int gameId = 1;
                    foreach (var t in allOperations)
                    {
                        gameId += t.GameId;
                    }

                    return gameId;
                }

            }

            public int CurrentRating
            {
                get
                {
                    int currentRating = 1000;
                    foreach (var t in allOperations)
                    {
                        currentRating += t.Rating;
                    }

                    return currentRating;
                }
            }




            public virtual void WinGame(string opponentName,Game game)
            {
                var winGame = new Operation(game.Rating, "Win", opponentName, 1);
                allOperations.Add(winGame);
            }

            public void LoseGame(string opponentName, Game game)
            {
                if (CurrentRating - game.Rating < 1)
                {
                    throw new InvalidOperationException("Game rating cant be less than 1");
                }

                var loseGame = new Operation(-game.Rating, "Lose", opponentName, 1);
                allOperations.Add(loseGame);
            }

            public string GetStats()
            {
                var rep = new System.Text.StringBuilder();
                int gameId = 0;
                int currentRating = 1000;
                rep.AppendLine("|Player|\t|CurrentRating|\t\t|Status|\t|OpponentName|\t|GameRating|\t|GameId|");
                foreach (var t in allOperations)
                {
                    currentRating += t.Rating;
                    gameId += t.GameId;
                    rep.AppendLine(
                        $"|{UserName}|\t|{currentRating}|\t\t\t|{t.Status}|\t\t|{t.OpponentName}|\t|{t.Rating}|\t\t|{gameId}|");
                }

                return rep.ToString();
            }

            public GameAccount(string name)
            {
                UserName = name;
                GamesCount = 0;

            }

            public List<Operation> allOperations = new List<Operation>();
        }

        public class VipAccount : GameAccount
        {
            public VipAccount(string name) : base(name)
            {
            }

            public override void WinGame(string opponentName, Game game)
            {

                var winGame = new Operation(game.Rating * 2, "Win", opponentName, 1);
                allOperations.Add(winGame);
            }
        }

        public class CringeAccount : GameAccount
        {
            public CringeAccount(string name) : base(name)
            {

            }

            public override void WinGame(string opponentName, Game game)
            {
                for (int i = 0; i < allOperations.Count - 1; i++)
                {
                    if (allOperations[i].Status == "Win" && allOperations[i + 1].Status == "Win")
                    {
                        game.Rating = game.Rating * 2;
                    }
                }

                var winGame = new Operation(game.Rating, "Win", opponentName, 1);
                allOperations.Add(winGame);
            }
        }

        public class Operation
        {
            public int Rating { get; }
            public string Status { get; }
            public string OpponentName { get; }
            public int GameId { get; }

            public Operation(int rating, string status, string opponentName, int gameIndex)
            {
                Rating = rating;
                Status = status;
                OpponentName = opponentName;
                GameId = gameIndex;
            }
        }

        public abstract class Game
        {
            public readonly GameAccount PlayerOne;
            public readonly GameAccount PlayerTwo;
            public int Rating { get; set; }

            protected Game(GameAccount p1, GameAccount p2, int rating)
            {
                PlayerOne = p1;
                PlayerTwo = p2;
                Rating = rating;
            }

            protected Game(GameAccount p1, GameAccount p2)
            {
                PlayerOne = p1;
                PlayerTwo = p2;
                Rating = 0;
            }


            public abstract void Process();


        }

        public class Matchmaking : Game
        {
            public Matchmaking(GameAccount p1, GameAccount p2, int rating) : base(p1, p2, rating)
            {
                if (rating <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(rating), "incorrect rating");
                }

            }

            public override void Process()
            {
                var r = new Random();
                int power1 = r.Next(0, 999);
                int power2 = r.Next(0, 999);
                Console.WriteLine("\nMatchmaking for " + Rating + "pts");
                if (power1 > power2)
                {
                    PlayerOne.WinGame(PlayerTwo.UserName, this);
                    PlayerTwo.LoseGame(PlayerOne.UserName, this);
                    Console.WriteLine(PlayerOne.UserName + " " + power1 + " power " + " wins " + PlayerTwo.UserName +
                                      " " + power2 + " power ");
                }
                else
                {
                    Console.WriteLine(PlayerTwo.UserName + " " + power2 + " power " + " wins " + PlayerOne.UserName +
                                      " " + power1 + " power ");
                    PlayerTwo.WinGame(PlayerOne.UserName, this);
                    PlayerOne.LoseGame(PlayerTwo.UserName, this);
                }
            }

            public class Casual : Game
            {
                public Casual(GameAccount p1, GameAccount p2) : base(p1, p2)
                {

                }

                public override void Process()
                {
                    var r = new Random();
                    int power1 = r.Next(0, 999);
                    int power2 = r.Next(0, 999);
                    Console.WriteLine("\nCasual Game");
                    if (power1 > power2)
                    {
                        PlayerOne.WinGame(PlayerTwo.UserName, this);
                        PlayerTwo.LoseGame(PlayerOne.UserName, this);
                        Console.WriteLine(PlayerOne.UserName + " " + power1 + " power " + " wins " +
                                          PlayerTwo.UserName + " " + power2 + " power ");
                    }
                    else
                    {
                        Console.WriteLine(PlayerTwo.UserName + " " + power2 + " power " + " wins " +
                                          PlayerOne.UserName + " " + power1 + " power ");
                        PlayerTwo.WinGame(PlayerOne.UserName, this);
                        PlayerOne.LoseGame(PlayerTwo.UserName, this);
                    }
                }

            }
            

            public class ReturningClass
            {
                public static Game MatchmakingG(GameAccount playerOne, GameAccount playerTwo, int rating)
                {
                    return new Matchmaking(playerOne, playerTwo, rating);
                }
                public static Game CasualG(GameAccount playerOne, GameAccount playerTwo)
                {
                    return new Casual(playerOne, playerTwo);
                }
            }
            
            
        }
    }
}


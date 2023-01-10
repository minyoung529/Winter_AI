using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforcement_Learning
{
    public enum GamePlayer
    {
        DynamicProgramming,
        Human,
        None
    }

    internal class GameManager
    {
        public GamePlayer BlackPlayer;
        public GamePlayer WhitePlayer;

        public void PlayGame()
        {
            while (true)
            {
                BlackPlayer = GetBlackPlayer();
                if (BlackPlayer == GamePlayer.None) return;

                WhitePlayer = GetWhitePlayer();
                if (BlackPlayer == GamePlayer.None) return;

                ManageGame();
            }
        }

        private GamePlayer GetWhitePlayer()
        {
            return PlayerSelection("X 플레이어를 선택해주세요");
        }

        private GamePlayer GetBlackPlayer()
        {
            return PlayerSelection("O 플레이어를 선택해주세요");
        }

        private GamePlayer PlayerSelection(string menuLabel)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(menuLabel);
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("1) 동적 프로그래밍");   // dp를 이용한 AI를 플레이어로 쓰겠다는 것
                Console.WriteLine("2) 사람");
                Console.WriteLine("3) 게임 종료");
                Console.WriteLine("선택 (1~3)");

                switch (Console.ReadLine())
                {
                    case "1":
                        if (Program.DPManager.StateValueFunction.Count > 0)
                        {
                            return GamePlayer.DynamicProgramming;
                        }
                        else
                        {
                            Console.Write("동적프로그래밍을 수행하세요.");
                            Console.WriteLine(Environment.NewLine);
                            Console.Write("아무 키나 누르세요");
                            Console.ReadLine();
                        }
                        break;

                    case "2":
                        return GamePlayer.Human;
                    case "3":
                        Console.Write("메인 메뉴로 돌아갑니다.");
                        Console.WriteLine(Environment.NewLine);
                        Console.Write("아무 키나 누르세요.");
                        Console.ReadLine();
                        return GamePlayer.None;
                    default:
                        break;
                }
            }
        }

        public void ManageGame()
        {
            GameState gameState = new GameState();
            int gameTurnCount = 0;
            int gameMove = 0;
            bool isGameFinished = gameState.isFinalState();

            while (!isGameFinished)
            {
                gameState.DisplayBoard(gameTurnCount, gameMove, BlackPlayer, WhitePlayer);

                isGameFinished = gameState.isFinalState();
                Console.WriteLine(Environment.NewLine);

                // TODO: 밖으로 빼내기
                if (isGameFinished)
                {
                    Console.Write("게임이 끝났습니다. 아무 키나 눌러주세요.");
                    Console.ReadLine();
                }
                else
                {
                    GamePlayer playerForNextTurn = GetGamePlayer(gameState.NextTurn);

                    if(playerForNextTurn == GamePlayer.Human)
                    {
                        gameMove = GetHumanGameMove(gameState);
                    }
                    else
                    {
                        Console.Write("아무 키나 눌러주세요");
                        Console.ReadLine();
                        gameMove = Program.DPManager.GetNextMove(gameState.BoardStateKey);
                    }

                    gameState.MakeMove(gameMove);
                    gameTurnCount++;
                }
            }
        }

        public int GetHumanGameMove(GameState gameState)
        {
            // TODO: dowhile로 바꾸기
            Console.Write("다음 행동을 입력하세요 (1~9): ");
            string sMove = Console.ReadLine();

            while(true)
            {
                try
                {
                    int iMove = Int32.Parse(sMove);

                    if (iMove >= 1 && iMove <= 9 && gameState.IsValidMove(iMove))
                        return iMove;
                    else
                    {
                        Console.Write("-.-!!!!!!! (1~9까지 입력하라고 했지!!!!!!!!) : ");
                        sMove = Console.ReadLine();
                    }
                }
                catch
                {
                    Console.Write("-.-!!!!!!! (1~9까지 입력하라고 했지!!!!!!!!) : ");
                    sMove = Console.ReadLine();
                }
            }
        }

        private GamePlayer GetGamePlayer(int turn)
        {
            if (turn == 1) return BlackPlayer;
            else return WhitePlayer;
        }
    }
}

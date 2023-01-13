using System;
using System.Collections.Generic;

namespace Reinforcement_Learning
{
    class Program
    {
        public static DynamicProgrammingManager DPManager;
        public static SarsaManager SarsaManager;
        public static QLearningManager QLearningManager;
        public static GameManager gameManager;

        static void Main(string[] args)
        {
            DPManager = new DynamicProgrammingManager();
            SarsaManager = new SarsaManager();
            QLearningManager = new QLearningManager();
            gameManager = new GameManager();

            bool showMenu = true;

            while (showMenu)
            {
                showMenu = MainMenu();
            }
        }

        private static bool MainMenu()
        {
            Console.Clear();
            Console.WriteLine("메뉴 번호를 선택해주세요.");
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("1) 동적프로그래밍 실행");
            Console.WriteLine("2) SARSA");
            Console.WriteLine("3) Q-Learning");
            Console.WriteLine("4) 게임하기");
            Console.WriteLine("5) 나가기");
            Console.WriteLine(Environment.NewLine);
            Console.Write("동작 선택 : ");

            switch (Console.ReadLine())
            {
                case "1":
                    DPManager.UpdateByDynamicProgramming();
                    return true;
                case "2":
                    SarsaManager.UpdateBySarsa();
                    return true;
                case "3":
                    QLearningManager.UpdateByQLearning();
                    return true;
                case "4":
                    gameManager.PlayGame();
                    return true;
                case "5":
                    return false;
                default:
                    return true;
            }
        }
    }
}



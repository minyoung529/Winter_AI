﻿using System;

namespace Mini_Othello
{
	public enum GamePlayer
	{
		DynamicProgramming,
		QLearning,
		Human,
		None
	}

	public class GameManager
	{
		public GamePlayer BlackPlayer;
		public GamePlayer WhitePlayer;

		public void PlayGame()
		{ 
			while (true)
			{ 
				BlackPlayer = GetBlackPlayer();
				if (BlackPlayer == GamePlayer.None)
					return;
				 
				WhitePlayer = GetWhitePlayer();
				if (WhitePlayer == GamePlayer.None)
					return;
				 
				ManageGame();
			}
		}

		public void ManageGame()
		{ 
			GameState gameState = new GameState();
			int gameTurnCount = 0;
			int gameMove = 0;
			bool isGameFinished = gameState.IsFinalState();

			while (!isGameFinished)  
			{ 
				gameState.DisplayBoard(gameTurnCount, gameMove, BlackPlayer, WhitePlayer);

				isGameFinished = gameState.IsFinalState();

				Console.WriteLine(Environment.NewLine);

				if (isGameFinished)
				{ 
					Console.Write("게임이 끝났습니다. 아무 키나 누르세요:");
					Console.ReadLine();
				}
				else
				{ 

					GamePlayer playerforNextTurn = GetGamePlayer(gameState.NextTurn);
					if (playerforNextTurn == GamePlayer.Human)
					{
						gameMove = GetHumanGameMove(gameState);
					}
					else
					{ 
						Console.Write("아무 키나 누르세요:");
						Console.ReadLine();

						if (playerforNextTurn == GamePlayer.DynamicProgramming)
							gameMove = MainProgram.ValueFunctionManager.GetNextMove(gameState.BoardStateKey);
						else if (playerforNextTurn == GamePlayer.QLearning)
							gameMove = MainProgram.QLearningValueFunctionManager.GetNextMove(gameState.BoardStateKey);
					}
					 
					gameState.MakeMove(gameMove);
					gameTurnCount++;
				}
			}
		}

		public int GetHumanGameMove(GameState gameState)
		{
			if(gameState.CountValidMoves() == 0)
			{
				Console.Write("둘 곳에 없어서 패스합니다. 아무 키나 입력하세요");
				Console.ReadLine();
				return 0;
			} 

			Console.Write("다음 행동을 입력하세요 (1-16):");
			string humanMove = Console.ReadLine();

			while (true)
			{
				try
				{
					int gameMove = Int32.Parse(humanMove);

					if (gameMove >= 1 && gameMove <= 16 && gameState.IsValidMove(gameMove))
						return gameMove;
					else
					{
						Console.Write("잘못된 행동입니다. 다음 행동을 입력하세요 (1-16):");
						humanMove = Console.ReadLine();
					}
				}
				catch
				{
					Console.Write("잘못된 행동입니다. 다음 행동을 입력하세요 (1-16):");
					humanMove = Console.ReadLine();
				}
			}
		}

		public GamePlayer GetGamePlayer(int Turn)
		{
			if (Turn == 1)
				return BlackPlayer;
			else
				return WhitePlayer;
		}

		public GamePlayer GetBlackPlayer()
		{ 
			return PlayerSelection("X 플레이어를 선택해 주세요.");
		}

		public GamePlayer GetWhitePlayer()
		{ 
			return PlayerSelection("O 플레이어를 선택해 주세요.");
		}

		public GamePlayer PlayerSelection(string menuLabel)
		{ 

			while (true)
			{
				Console.Clear();
				Console.WriteLine(menuLabel);
				Console.WriteLine(Environment.NewLine);
				Console.WriteLine("1) 동적프로그래밍");
				Console.WriteLine("2) Q-러닝");
				Console.WriteLine("3) 사람");
				Console.WriteLine("4) 게임 종료");
				Console.Write("선택 (1-4):");

				switch (Console.ReadLine())
				{
					case "1":
						if (MainProgram.ValueFunctionManager.StateValueFunction.Count > 0)
						{
							return GamePlayer.DynamicProgramming;
						}
						else
						{
							Console.Write("상태 가치 함수가 정의되어 있지 않습니다. 동적 프로그래밍을 수행하거나, 가치 함수를 읽어오세요.");
							Console.WriteLine(Environment.NewLine);
							Console.Write("아무 키나 누르세요:");
							Console.ReadLine();
						}
						break;
					case "2":
						if (MainProgram.QLearningValueFunctionManager.ActionValueFunction.Count > 0)
						{
							return GamePlayer.QLearning;
						}
						else
						{
							Console.Write("상태 가치 함수가 정의되어 있지 않습니다. Q-러닝을 수행하거나, 가치 함수를 읽어오세요.");
							Console.WriteLine(Environment.NewLine);
							Console.Write("아무 키나 누르세요:");
							Console.ReadLine();
						}
						break;
					case "3":
						Console.Write("사람을 선택하셨습니다..");
						Console.WriteLine(Environment.NewLine);
						Console.Write("아무 키나 누르세요:");
						Console.ReadLine();
						return GamePlayer.Human;
					case "4":
						Console.Write("메인 메뉴로 돌아갑니다..");
						Console.WriteLine(Environment.NewLine);
						Console.Write("아무 키나 누르세요:");
						Console.ReadLine();
						return GamePlayer.None;
					default:
						break;
				}
			}
		}
	}
}

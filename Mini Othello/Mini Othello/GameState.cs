using System;
using System.Collections.Generic;
using System.Linq;

namespace Mini_Othello
{
    public class GameParameters
    {
        public static int ActionMinIndex = 1;
        public static int ActionMaxIndex = 16;
        public static int BoardRowCount = 4;
        public static int BoardColCount = 4;
        public static int BoardMaxIndex = 15;
    }

    public class GameState
    {
        public int[,] BoardState;
        public int NextTurn;
        public int BoardStateKey;
        public int GameWinner;
        public int NumOfBlack;
        public int NumOfWhite;

        public GameState()
        {
            // 초기상태
            BoardState = new int[,] { { 0, 0, 0, 0 }, { 0, 1, 2, 0 }, { 0, 2, 1, 0 }, { 0, 0, 0, 0 } };
            NextTurn = 1;   // 흑돌 차례

            int boardStateKey = 0;
            for (int i = 0; i <= GameParameters.BoardMaxIndex; i++) // 3진수
            {
                boardStateKey = boardStateKey * 3;
                boardStateKey = boardStateKey + BoardState[GetRowFromIndex(i), GetColFromIndex(i)];
            }

            BoardStateKey = boardStateKey * 3 + NextTurn; // 3진수 + 다음 턴
            GameWinner = 0;
            NumOfBlack = 2;
            NumOfWhite = 2;
        }

        // 주어진 게임 상태 키로부터 만들어지는 게임
        public GameState(int boardStateKey)
        {
            BoardState = new int[4, 4];
            BoardStateKey = boardStateKey;
            NextTurn = boardStateKey % 3;
            GameWinner = 0;

            // 마지막은 턴이므로 /3
            PopulateBoard(boardStateKey / 3);
        }

        // 10진수 => 3진수
        public void PopulateBoard(int boardState)
        {
            int boardValueProcessing = boardState;
            NumOfBlack = 0;
            NumOfWhite = 0;

            for (int i = GameParameters.BoardMaxIndex; i >= 0; i--)
            {
                int boardValue = boardValueProcessing % 3;
                boardValueProcessing = boardValueProcessing / 3;

                BoardState[GetRowFromIndex(i), GetColFromIndex(i)] = boardValue;

                if (boardValue == 1)
                    NumOfBlack++;
                if (boardValue == 2)
                    NumOfWhite++;
            }
        }

        // 게임이 끝났는지 아닌지
        public bool IsFinalState()
        {
            GameWinner = 0;
            NumOfWhite = 0;
            NumOfBlack = 0;

            for (int i = 0; i < GameParameters.BoardRowCount; i++)
            {
                for (int j = 0; j < GameParameters.BoardColCount; j++)
                {
                    switch (BoardState[i, j])
                    {
                        case 1:
                            NumOfBlack++;
                            break;
                        case 2:
                            NumOfWhite++;
                            break;
                        default:
                            break;
                    }
                }
            }

            if (CountValidMoves(1) == 0 && CountValidMoves(2) == 0)
            {
                if (NumOfBlack > NumOfWhite)
                {
                    GameWinner = 1;
                }
                else if (NumOfBlack < NumOfWhite)
                {
                    GameWinner = 2;
                }
                return true;
            }

            return false;
        }

        // 현재 상태에 대한 보상값
        public float GetReward()
        {
            if (IsFinalState())
            {
                if (GameWinner == 1)
                    return 100.0f;
                else if (GameWinner == 2)
                    return -100.0f;
            }

            return 0.0f;
        }

        public int CountValidMoves()
        {
            return CountValidMoves(NextTurn);
        }

        private int CountValidMoves(int turn)
        {
            int count = 0;
            for (int i = GameParameters.ActionMinIndex; i <= GameParameters.ActionMaxIndex; i++)
            {
                if (IsValidMove(i, turn))
                    count++;
            }
            return count;
        }

        public bool IsValidMove(int move)
        {
            return IsValidMove(move, NextTurn);
        }

        /// <summary>
        /// 상태에 대한 행동을 적용할 수 있는지 판단
        /// </summary>
        public bool IsValidMove(int move, int turn)
        {
            int index = move - 1;
            int row = GetRowFromIndex(index);
            int col = GetColFromIndex(index);

            // 8방향 검색
            if (BoardState[row, col] == 0)
            {
                for (int incRow = -1; incRow <= 1; incRow++)
                {
                    for (int incCol = -1; incCol <= 1; incCol++)
                    {
                        if (incRow != 0 || incCol != 0)
                        {
                            // 8방향 검색, 상대방 돌을 바꿀 수 있는지 검색 
                            if (IsMoveValidInDirection(row, col, incRow, incCol, turn))
                                return true;
                        }
                    }
                }

            }

            return false;
        }

        // (targetRow, targetCol)에서 (incRow, incCol)의 방향으로
        // 상대방 돌을 바꿀 수 있는지 판단
        public bool IsMoveValidInDirection(int targetRow, int targetCol, int incRow, int incCol, int turn)
        {
            int testingRow;
            int testingCol;
            int enemyStoneCount = 0;
            int increment = 1;

            while (true)
            {
                testingRow = targetRow + incRow * increment;
                testingCol = targetCol + incCol * increment;

                // 보드 외곽을 나갔는지
                if (testingRow < 0 || testingRow >= GameParameters.BoardRowCount)
                    return false;
                if (testingCol < 0 || testingCol >= GameParameters.BoardColCount)
                    return false;

                int testingPlaceStone = BoardState[testingRow, testingCol];

                // 진행 중에 빈칸이 있으면 X
                if (testingPlaceStone == 0)
                    return false;

                // 상대방 돌을 만나면 개수 기록
                if (testingPlaceStone != turn)
                    enemyStoneCount++;

                // 내 돌을 만났을 때
                if (testingPlaceStone == turn)
                {
                    // 바꿀 수 있는 돌이 없다면 X
                    if (enemyStoneCount == 0)
                        return false;
                    else
                        return true;
                }
                increment++;
            }
        }

        // (targetRow, targetCol)에서 (incRow, incCol)로 진행하며
        // 우리편 돌 만날 때까지 상대편 돌 바꾸기
        public void MakeMoveInDirection(int targetRow, int targetCol, int incRow, int incCol, int turn)
        {
            int testingRow, testingCol;
            int increment = 1;

            while (true)
            {
                testingRow = targetRow + incRow * increment;
                testingCol = targetCol + incCol * increment;

                int testingPlaceStone = BoardState[testingRow, testingCol];

                if (testingPlaceStone != turn)
                {
                    // 바꾸기
                    BoardState[testingRow, testingCol] = turn;

                    if (turn == 1)
                    {
                        NumOfBlack++;
                        NumOfWhite--;
                    }
                    else if (turn == 2)
                    {
                        NumOfBlack--;
                        NumOfWhite++;
                    }
                }
                if (testingPlaceStone == turn)
                    return;
                increment++;
            }
        }

        // 행동을 취한 다음의 상태를 반환
        public GameState GetNextState(int move)
        {
            GameState nextState = new GameState(BoardStateKey);
            nextState.MakeMove(move);
            return nextState;
        }

        public void MakeMove(int move)
        {
            int boardStateKey = 0;

            if (move != 0)  // Non Pass
            {
                int index = move - 1;           // 행동값
                int row = GetRowFromIndex(index);
                int col = GetColFromIndex(index);

                // 돌을 놓는다
                BoardState[row, col] = NextTurn;

                if (NextTurn == 1)
                    NumOfBlack++;
                else if (NextTurn == 2)
                    NumOfWhite++;

                for (int incRow = -1; incRow <= 1; incRow++)
                {
                    for (int incCol = -1; incCol <= 1; incCol++)
                    {
                        if (incRow != 0 || incCol != 0)
                        {
                            // 바꿀 수 있다면 바꾸기
                            if (IsMoveValidInDirection(row, col, incRow, incCol, NextTurn))
                            {
                                MakeMoveInDirection(row, col, incRow, incCol, NextTurn);
                            }
                        }
                    }
                }

                for (int i = 0; i <= GameParameters.BoardMaxIndex; i++)
                {
                    boardStateKey = boardStateKey * 3;
                    boardStateKey = boardStateKey + BoardState[GetRowFromIndex(i), GetColFromIndex(i)];
                }
            }
            else // Pass
            {
                boardStateKey = BoardStateKey / 3;
            }

            if (NextTurn == 1)
                NextTurn = 2;
            else if (NextTurn == 2)
                NextTurn = 1;

            BoardStateKey = boardStateKey * 3 + NextTurn;
        }

        public void DisplayBoard(int turnCount, int lastMove, GamePlayer blackPlayer, GamePlayer whitePlayer)
        {
            // 화면에 현재 게임 상태를 출력하는 함수. 게임 진행 과정에서 사용됨
            Console.Clear();

            Console.WriteLine($"X: {blackPlayer}, O: {whitePlayer}");
            Console.WriteLine();
            Console.WriteLine($"게임 턴: {turnCount}, {GetTurnMark()} 차례입니다.");
            Console.Write($"Black:{NumOfBlack}, White:{NumOfWhite}, ");

            if (turnCount != 0)
            {
                if (lastMove != 0)
                {
                    Console.WriteLine($" 지난 행동, Row: {GetRowFromIndex(lastMove - 1)}, Column: {GetColFromIndex(lastMove - 1)}");
                }
                else
                {
                    Console.WriteLine($" 지난 행동, Pass");
                }
            }
            else
            {
                Console.WriteLine();
            }
            Console.WriteLine();

            Console.WriteLine("    +-------+-------+-------+-------+");
            Console.WriteLine("    |       |       |       |       |");
            Console.WriteLine($"    |   {GetGameBoardValue(0, 0)}   |   {GetGameBoardValue(0, 1)}   |   {GetGameBoardValue(0, 2)}   |   {GetGameBoardValue(0, 3)}   |");
            Console.WriteLine("    |       |       |       |       |");
            Console.WriteLine("    +-------+-------+-------+-------+");
            Console.WriteLine("    |       |       |       |       |");
            Console.WriteLine($"    |   {GetGameBoardValue(1, 0)}   |   {GetGameBoardValue(1, 1)}   |   {GetGameBoardValue(1, 2)}   |   {GetGameBoardValue(1, 3)}   |");
            Console.WriteLine("    |       |       |       |       |");
            Console.WriteLine("    +-------+-------+-------+-------+");
            Console.WriteLine("    |       |       |       |       |");
            Console.WriteLine($"    |   {GetGameBoardValue(2, 0)}   |   {GetGameBoardValue(2, 1)}   |   {GetGameBoardValue(2, 2)}   |   {GetGameBoardValue(2, 3)}   |");
            Console.WriteLine("    |       |       |       |       |");
            Console.WriteLine("    +-------+-------+-------+-------+");
            Console.WriteLine("    |       |       |       |       |");
            Console.WriteLine($"    |   {GetGameBoardValue(3, 0)}   |   {GetGameBoardValue(3, 1)}   |   {GetGameBoardValue(3, 2)}   |   {GetGameBoardValue(3, 3)}   |");
            Console.WriteLine("    |       |       |       |       |");
            Console.WriteLine("    +-------+-------+-------+-------+");


            IsFinalState();
            Console.WriteLine(Environment.NewLine);
            if (IsFinalState())
            {
                switch (GameWinner)
                {
                    case 1:
                        Console.WriteLine("X 가 이겼습니다!!");
                        break;
                    case 2:
                        Console.WriteLine("O 가 이겼습니다!!");
                        break;
                    default:
                        Console.WriteLine("게임이 비겼습니다!!");
                        break;
                }
            }
            else
            {
                Console.WriteLine("게임 진행중입니다!!");
            }
        }

        private string GetTurnMark()
        {
            return NextTurn == 1 ? "X" : "O";
        }

        private string GetGameBoardValue(int row, int col)
        {
            switch (BoardState[row, col])
            {
                case 1:
                    return "X";
                case 2:
                    return "O";
                default:
                    return "+";
            }
        }

        private int GetRowFromIndex(int index)
        {
            return index / GameParameters.BoardRowCount;
        }

        private int GetColFromIndex(int index)
        {
            return index % GameParameters.BoardColCount;
        }
    }
}

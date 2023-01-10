//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Cryptography.X509Certificates;
//using System.Text;
//using System.Threading.Tasks;

//namespace Reinforcement_Learning
//{
//    /// <summary>
//    /// 연결된 위치
//    /// </summary>
//    public class ConnectedPosition
//    {
//        public int row;
//        public int col;
//        public ConnectedPosition(int r, int c)
//        {
//            row = r;
//            col = c;
//        }
//    }

//    public class GameParameters
//    {
//        public static int StateCount = 19650;   // 222211110 10진수
//        public static int ActionMinIndex = 1;
//        public static int ActionMaxIndex = 9;
//    }

//    class GameState
//    {
//        public int[,] BoardState;
//        public int NextTurn;
//        public int BoardStateKey;
//        public int NumberOfBlacks;
//        public int NumberOfWhites;
//        public int GameWinner;

//        public GameState()
//        {
//            BoardState = new int[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
//            NextTurn = 1;
//            BoardStateKey = 1;
//            NumberOfBlacks = 0;
//            NumberOfWhites = 0;
//            GameWinner = 0;
//        }

//        public GameState(int boardStateKey)
//        {
//            BoardState = new int[3, 3];
//            BoardStateKey = boardStateKey;
//            NextTurn = boardStateKey % 3; // 0, 1, 2
//            GameWinner = 0;

//            PopulateBoard(boardStateKey / 3);
//        }

//        public void PopulateBoard(int boardState)
//        {
//            // 주어진 보드 상태 값을 3진수로 변환시키면서
//            // 보드 상태 생성

//            int boardValueProcessing = boardState;
//            NumberOfBlacks = 0;
//            NumberOfWhites = 0;

//            for (int i = 8; i >= 0; i--)
//            {
//                int boardValue = boardValueProcessing % 3;
//                boardValueProcessing /= 3;

//                BoardState[i / 3, i % 3] = boardValue;

//                if (boardValue == 1) NumberOfBlacks++;
//                if (boardValue == 2) NumberOfWhites++;
//            }
//        }

//        public bool IsValidFirstStage()
//        {
//            if (NumberOfBlacks > 4) return false;
//            if (NumberOfWhites > 3) return false;

//            if (NumberOfWhites == NumberOfBlacks || NumberOfBlacks == NumberOfWhites + 1) return true;
//            return false;
//        }

//        public bool IsValidSecondStage()
//        {
//            if (NumberOfBlacks == 4 && NumberOfWhites == 4)
//                return true;

//            return false;
//        }

//        public int GetFirstStageTurn()
//        {
//            if (NumberOfBlacks == NumberOfWhites)
//                return 1;
//            if (NumberOfBlacks == NumberOfWhites + 1)
//                return 2;

//            return 0;
//        }

//        public bool IsFinalState()
//        {
//            GameWinner = 0;

//            for (int i = 0; i < 3; i++)
//            {
//                // 세로
//                if (BoardState[i, 0] == BoardState[i, 1] && BoardState[i, 1] == BoardState[i, 2])
//                {
//                    if (BoardState[i, 0] != 0)
//                    {
//                        GameWinner = BoardState[i, 0];
//                        return true;
//                    }
//                }

//                // 가로
//                if (BoardState[0, i] == BoardState[1, i] && BoardState[1, i] == BoardState[2, i])
//                {
//                    if (BoardState[0, i] != 0)
//                    {
//                        GameWinner = BoardState[0, i];
//                        return true;
//                    }
//                }
//            }

//            if (BoardState[0, 0] == BoardState[1, 1] && BoardState[1, 1] == BoardState[2, 2])
//            {
//                if (BoardState[0, 0] != 0)
//                    return true;
//            }

//            if (BoardState[0, 2] == BoardState[1, 1] && BoardState[1, 1] == BoardState[2, 0])
//            {
//                if (BoardState[0, 2] != 0)
//                    return true;
//            }

//            return false;
//        }

//        public bool IsValidMove(int move)
//        {
//            int row = (move - 1) / 3;
//            int col = (move - 1) % 3;

//            // 1단계이면
//            if (IsValidFirstStage())
//            {
//                if (BoardState[row, col] == 0) return true;
//            }
//            else if (IsValidSecondStage())
//            {
//                // 상대 것이 아니다
//                if (BoardState[row, col] != NextTurn)
//                {
//                    return false;
//                }

//                IEnumerable<ConnectedPosition> ConnectedPositions = GetConnectedPositions(row, col);
//                IEnumerable<ConnectedPosition> ConnectedEmptSports = ConnectedPositions.Where(e => BoardState[e.row, e.col] == 0);

//                if (ConnectedEmptSports.Count() > 0)
//                {
//                    IEnumerable<ConnectedPosition> validPositions = ConnectedPositions.Where
//                        (
//                        e => BoardState[e.row, e.col] != 0 && BoardState[e.row, e.col] != NextTurn
//                        );

//                    if (validPositions.Count() > 0)
//                    {
//                        return true;
//                    }
//                }
//            }
//            return false;
//        }

//        private IEnumerable<ConnectedPosition> GetConnectedPositions(int row, int col)
//        {
//            // 접해있는 아이들 구하기
//            List<ConnectedPosition> connectedPositionList = new List<ConnectedPosition>();

//            int[] dx = new int[8] { 0, -1, -1, -1, 0, 1, 1, 1 };
//            int[] dy = new int[8] { 1, 1, 0, -1, -1, -1, 0, 1 };

//            for (int i = 0; i < 8; i++)
//            {
//                int ny = row + dy[i];
//                int nx = col + dx[i];

//                if (ny < 0 || ny > 2 || nx < 0 || nx > 2) continue;

//                connectedPositionList.Add(new ConnectedPosition(ny, nx));
//            }

//            return connectedPositionList;
//        }

//        public GameState GetNextStage(int move)
//        {
//            GameState nextState = new GameState(BoardStateKey);
//            nextState.MakeMove(move);
//            return nextState;
//        }

//        public void MakeMove(int move)
//        {
//            int row = (move - 1) / 3;
//            int col = (move - 1) % 3;

//            if (IsValidFirstStage())
//            {
//                BoardState[row, col] = NextTurn;

//                if (NextTurn == 1) NumberOfBlacks++;
//                else if (NextTurn == 2) NumberOfWhites++;
//            }
//            else if (IsValidSecondStage())
//            {
//                int emptyRow = 0, emptyCol = 0;

//                for (int i = 0; i < 3; i++)
//                {
//                    for (int j = 0; j < 3; j++)
//                    {
//                        if (BoardState[i, j]==0)
//                        {
//                            emptyRow = i;
//                            emptyCol = j;
//                            break;
//                        }
//                    }
//                }

//                BoardState[row, col] = 0;
//                BoardState[emptyRow, emptyCol] = NextTurn;
//            }

//            if (NextTurn == 1) NextTurn = 2;
//            else if (NextTurn == 2) NextTurn = 1;

//            int key = 0;

//            // 보드판의 상황을 갱신
//            for(int i = 0; i < 9; i++)
//            {
//                key += 3;
//                key += BoardState[i / 3, i % 3];
//            }

//            BoardStateKey = key * 3 + NextTurn; // 10진수로 바꿔준다
//        }

//        public float GetReward()
//        {
//            // 종료가 됐다면 (누가 이기거나 비겼다면)
//            if(IsFinalState())
//            {
//                if (GameWinner == 1)
//                    return 100f;
//                else if (GameWinner == 2)
//                    return -100f;
//            }

//            return 0f;
//        }
//    }
//}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforcement_Learning
{
    public class GameParameters
    {
        public static int StateCount = 19560;
        public static int ActionMinIndex = 1;
        public static int ActionMaxIndex = 9;
    }

    public class ConnectedPosition
    {
        public int row;
        public int col;
        public ConnectedPosition(int r, int c)
        {
            row = r;
            col = c;
        }
    }

    public class GameState
    {
        public int[,] BoardState;
        public int NextTurn;
        public int BoardStateKey;
        public int NumberOfBlacks;
        public int NumberOfWhites;
        public int GameWinner;

        public GameState()
        {
            BoardState = new int[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
            NextTurn = 1;
            BoardStateKey = 1;
            NumberOfBlacks = 0;
            NumberOfWhites = 0;
            GameWinner = 0;
        }


        public GameState(int boardStateKey)
        {
            BoardState = new int[3, 3];
            BoardStateKey = boardStateKey;
            NextTurn = boardStateKey % 3;
            GameWinner = 0;
            PopulateBoard(boardStateKey / 3);
        }

        public void PopulateBoard(int boardState)
        {

            int boardValueProcessing = boardState;
            NumberOfBlacks = 0;
            NumberOfWhites = 0;

            for (int i = 8; i >= 0; i--)
            {
                int boardValue = boardValueProcessing % 3;
                boardValueProcessing = boardValueProcessing / 3;

                BoardState[i / 3, i % 3] = boardValue;

                if (boardValue == 1)
                    NumberOfBlacks++;

                if (boardValue == 2)
                    NumberOfWhites++;
            }
        }


        public bool IsValidSecondStage()
        {
            if (NumberOfBlacks == 4 && NumberOfWhites == 4)
                return true;
            return false;
        }

        public bool IsValidFirstStage()
        {
            if (NumberOfBlacks > 4)
                return false;
            if (NumberOfWhites > 3)
                return false;

            if (NumberOfBlacks == NumberOfWhites || NumberOfBlacks == NumberOfWhites + 1)
                return true;

            return false;
        }

        public int GetFirstStageTurn()
        {
            if (NumberOfBlacks == NumberOfWhites)
                return 1;
            if (NumberOfBlacks == NumberOfWhites + 1)
                return 2;

            return 0;
        }


        public bool isFinalState()
        {
            GameWinner = 0;

            if (BoardState[0, 0] == BoardState[0, 1] && BoardState[0, 1] == BoardState[0, 2])
            {
                if (BoardState[0, 0] != 0)
                {
                    GameWinner = BoardState[0, 0];
                    return true;
                }
            }
            if (BoardState[1, 0] == BoardState[1, 1] && BoardState[1, 1] == BoardState[1, 2])
            {
                if (BoardState[1, 0] != 0)
                {
                    GameWinner = BoardState[1, 0];
                    return true;
                }
            }
            if (BoardState[2, 0] == BoardState[2, 1] && BoardState[2, 1] == BoardState[2, 2])
            {
                if (BoardState[2, 0] != 0)
                {
                    GameWinner = BoardState[2, 0];
                    return true;
                }
            }
            if (BoardState[0, 0] == BoardState[1, 0] && BoardState[1, 0] == BoardState[2, 0])
            {
                if (BoardState[0, 0] != 0)
                {
                    GameWinner = BoardState[0, 0];
                    return true;
                }
            }
            if (BoardState[0, 1] == BoardState[1, 1] && BoardState[1, 1] == BoardState[2, 1])
            {
                if (BoardState[0, 1] != 0)
                {
                    GameWinner = BoardState[0, 1];
                    return true;
                }
            }
            if (BoardState[0, 2] == BoardState[1, 2] && BoardState[1, 2] == BoardState[2, 2])
            {
                if (BoardState[0, 2] != 0)
                {
                    GameWinner = BoardState[0, 2];
                    return true;
                }
            }
            if (BoardState[0, 0] == BoardState[1, 1] && BoardState[1, 1] == BoardState[2, 2])
            {
                if (BoardState[0, 0] != 0)
                {
                    GameWinner = BoardState[0, 0];
                    return true;
                }
            }
            if (BoardState[0, 2] == BoardState[1, 1] && BoardState[1, 1] == BoardState[2, 0])
            {
                if (BoardState[0, 2] != 0)
                {
                    GameWinner = BoardState[0, 2];
                    return true;
                }
            }

            return false;
        }

        public bool IsValidMove(int move)
        {


            int row = (move - 1) / 3;
            int col = (move - 1) % 3;

            if (IsValidFirstStage())
            {
                if (BoardState[row, col] == 0)
                    return true;
            }
            else if (IsValidSecondStage())
            {
                if (BoardState[row, col] != NextTurn)
                    return false;

                IEnumerable<ConnectedPosition> ConnectedPositions = GetConnectedPosition(row, col);
                IEnumerable<ConnectedPosition> ConnectedEmptySpots = ConnectedPositions.Where(e => BoardState[e.row, e.col] == 0);

                if (ConnectedEmptySpots.Count() > 0)
                {
                    IEnumerable<ConnectedPosition> ConnectedOpponents = ConnectedPositions.Where(e => BoardState[e.row, e.col] != 0 && BoardState[e.row, e.col] != NextTurn);

                    if (ConnectedOpponents.Count() > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        private IEnumerable<ConnectedPosition> GetConnectedPosition(int row, int col)
        {
            // 접해있는 아이들 구하기
            List<ConnectedPosition> connectedPositionList = new List<ConnectedPosition>();

            int[] dx = new int[8] { 0, -1, -1, -1, 0, 1, 1, 1 };
            int[] dy = new int[8] { 1, 1, 0, -1, -1, -1, 0, 1 };

            for (int i = 0; i < 8; i++)
            {
                int ny = row + dy[i];
                int nx = col + dx[i];

                if (ny < 0 || ny > 2 || nx < 0 || nx > 2) continue;

                connectedPositionList.Add(new ConnectedPosition(ny, nx));
            }

            return connectedPositionList;
        }


        public GameState GetNextState(int move)
        {
            GameState nextState = new GameState(BoardStateKey);
            nextState.MakeMove(move);
            return nextState;
        }

        public void MakeMove(int move)
        {

            int row = (move - 1) / 3;
            int col = (move - 1) % 3;

            if (IsValidFirstStage())
            {

                BoardState[row, col] = NextTurn;

                if (NextTurn == 1)
                    NumberOfBlacks++;
                else if (NextTurn == 2)
                    NumberOfWhites++;
            }
            else if (IsValidSecondStage())
            {
                int emptyRow = 0;
                int emptyCol = 0;

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (BoardState[i, j] == 0)
                        {
                            emptyRow = i;
                            emptyCol = j;
                            break;
                        }
                    }
                }
                BoardState[row, col] = 0;
                BoardState[emptyRow, emptyCol] = NextTurn;
            }

            if (NextTurn == 1)
                NextTurn = 2;
            else if (NextTurn == 2)
                NextTurn = 1;

            int boardStateKey = 0;

            for (int i = 0; i < 9; i++)
            {
                boardStateKey = boardStateKey * 3;
                boardStateKey = boardStateKey + BoardState[i / 3, i % 3];
            }

            BoardStateKey = boardStateKey * 3 + NextTurn;
        }

        public float GetReward()
        {

            if (isFinalState())
            {
                if (GameWinner == 1)
                    return 100.0f;
                else if (GameWinner == 2)
                    return -100.0f;
            }

            return 0.0f;
        }


    }
}

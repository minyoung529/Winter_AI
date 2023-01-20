using System;
using System.Collections.Generic;
using System.Linq;

namespace Mini_Othello
{
    public class Utilities
    {
        public static Random random = new Random();
        public static Dictionary<int, Dictionary<int, float>> CreateActionValueFunction()
        {
            // SARSA, Q 러닝에서 사용되는 행동 가치 함수를 초기화하는 함수

            Dictionary<int, Dictionary<int, float>> actionValueFunction = new Dictionary<int, Dictionary<int, float>>();

            GameState gameState = new GameState(); // 초기 게임 상태 생성
            List<int> boardStateKeyList = new List<int>(); // 게임 상태 후보 리스트 선언
            boardStateKeyList.Add(gameState.BoardStateKey); // 초기 상태 게임 상태키를 후보 리스트에 추가

            while (true) // 루프 시작
            {
                List<int> mergedChildStateList = new List<int>();
                foreach (int gameBoardKey in boardStateKeyList) // 게임 상태 후보 리스트에 있는 상태들에 대해
                {
                    if (!actionValueFunction.ContainsKey(gameBoardKey)) // 가치 함수에 아직 포함되지 않았으면
                    {
                        Dictionary<int, float> actionValues = new Dictionary<int, float>();
                        GameState processingGameState = new GameState(gameBoardKey); // 게임 상태 생성

                        if (!processingGameState.IsFinalState()) // 게임 종료 상태가 아니면
                        {
                            List<int> childStateList = new List<int>();
                            for (int i = GameParameters.ActionMinIndex; i <= GameParameters.ActionMaxIndex; i++)  // 모든 행동에 대해 루프를 수행
                            {
                                if (processingGameState.IsValidMove(i)) // 이 행동이 올바른 행동이면
                                {
                                    GameState nextState = processingGameState.GetNextState(i); // 행동을 통해 전이해 간 다음 상태 구성
                                    childStateList.Add(nextState.BoardStateKey); // 자식 상태 임시 후보 리스트에 추가
                                    actionValues.Add(i, 0.0f); // 가치 함수값을 0으로 초기화 한후 dictionary에 추가
                                }
                            }
                            if (childStateList.Count == 0) // 만일 후보 리스트에 포함된 상태가 없으면
                            {
                                GameState nextState = processingGameState.GetNextState(0); // Pass 로 턴만 바뀐 상태 생성
                                childStateList.Add(nextState.BoardStateKey); // 임시 후보 리스트에 추가
                                actionValues.Add(0, 0.0f); // Pass 행동
                            }

                            // 임시 후보 리스트의 상태 중 자식 상태 리스트에 포함되어 있지 않은 상태를 자식 상태 리스트에 추가
                            mergedChildStateList.AddRange(childStateList.Where(e => !mergedChildStateList.Contains(e)));
                        }
                        actionValueFunction.Add(gameBoardKey, actionValues); // 가치 함수에 추가
                    }
                }
                if (mergedChildStateList.Count == 0) // 자식 상태 리스트에 상태가 없으면
                    break; // 루프 종료
                else
                    boardStateKeyList = new List<int>(mergedChildStateList); // 게임 상태 후보 리스트를 자식 상태로 치환하고 루프 지속
            }

            return actionValueFunction;
        }

        public static int GetEpsilonGreedyAction(int turn, Dictionary<int, float> actionValues)
        {
            float greedyActionValue = 0.0f;
            float epsilon = 10;

            if (actionValues.Count == 0)
                return 0;

            if (turn == 1)
                greedyActionValue = actionValues.Select(e => e.Value).Max();

            else if (turn == 2)
                greedyActionValue = actionValues.Select(e => e.Value).Min();

            int exploitRandom = random.Next(0, 100);
            IEnumerable<int> actionCandidates = null;

            // 10% 확률로 그리디 값이 아닌 것들 또한 선택
            if (exploitRandom < epsilon)
            {
                actionCandidates = actionValues.Where(e => e.Value != greedyActionValue).Select(e => e.Key);
            }

            // null이거나 그리디 최적해 외 카운트가 0이라면
            if (actionCandidates == null || actionCandidates.Count() == 0)
                actionCandidates = actionValues.Where(e => e.Value == greedyActionValue).Select(e => e.Key);

            return actionCandidates.ElementAt(random.Next(0, actionCandidates.Count()));
        }

        public static int GetGreedyAction(int turn, Dictionary<int, float> actionValues)
        {
            IEnumerable<int> actionCandidates = GetGreedyActionCandidate(turn, actionValues);

            if (actionCandidates.Count() == 0)
                return 0;

            return actionCandidates.ElementAt(random.Next(0, actionCandidates.Count()));
        }

        public static IEnumerable<int> GetGreedyActionCandidate(int turn, Dictionary<int, float> actionValues)
        {
            float greedyActionValue = 0.0f;

            if (turn == 1)
            {
                greedyActionValue = actionValues.Select(e => e.Value).Max();
            }
            else if (turn == 2)
            {
                greedyActionValue = actionValues.Select(e => e.Value).Min();
            }

            return actionValues.Where(e => e.Value == greedyActionValue).Select(e => e.Key);
        }

        public static float GetGreedyActionValue(int turn, Dictionary<int, float> actionValues)
        {
            if (actionValues.Count == 0)
                return 0.0f;

            if (turn == 1)
            {
                return actionValues.Select(e => e.Value).Max();
            }
            else if (turn == 2)
            {
                return actionValues.Select(e => e.Value).Min();
            }

            return 0.0f;
        }

        // 진행도
        public static float EvaluateValueFunction()
        {
            if (MainProgram.ValueFunctionManager.StateValueFunction.Count == 0)
                return 0.0f;

            int totalStateCount = 0;    // 전체 상태 카운트
            int matchingStateCount = 0; // 

            // Q함수 - 상태 + 행동에 대한 가치
            // 그냥(int, float) - 상태에 대한 가치
            foreach (KeyValuePair<int, Dictionary<int, float>> valueFunctionEntry in MainProgram.QLearningValueFunctionManager.ActionValueFunction)
            {
                GameState gameState = new GameState(valueFunctionEntry.Key);
                if (!gameState.IsFinalState() && gameState.CountValidMoves() > 0)
                {
                    if (CompareActionCandidate(valueFunctionEntry.Key))
                    {
                        matchingStateCount++;
                    }
                    totalStateCount++;
                }
            }
            return ((float)matchingStateCount) / ((float)totalStateCount) * 100.0f;
        }

        public static bool CompareActionCandidate(int boardStateKey)
        {
            IEnumerable<int> DPActionCandidate = MainProgram.ValueFunctionManager.GetNextMoveCandidate(boardStateKey);
            IEnumerable<int> QActionCandidate = MainProgram.QLearningValueFunctionManager.GetNextMoveCandidate(boardStateKey);

            if (QActionCandidate.Count() == 0 && DPActionCandidate.Count() > 0)
                return false;

            IEnumerable<int> UnmatchedActionList = QActionCandidate.Where(e => !DPActionCandidate.Contains(e));

            if (UnmatchedActionList.Count() == 0)
                return true;

            return false;
        }
    }
}

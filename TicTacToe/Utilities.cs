using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforcement_Learning
{
    public static class Utilities
    {
        public static Dictionary<int, Dictionary<int, float>> CreateActionValueFunction()
        {
            // SARSA, Q 러닝에서 사용되는 행동 가치 함수를 초기화하는 함수

            Dictionary<int, Dictionary<int, float>> actionValueFunction
                                            = new Dictionary<int, Dictionary<int, float>>();

            for (int i = 0; i <= GameParameters.StateCount; i++)
            {
                GameState state = new GameState();
                state.PopulateBoard(i);

                if (state.IsValidSecondStage()) // 올바른 2단계 게임 보드인 경우
                {
                    // 흑돌 차례인 상태에 대한 가치 함수 엔트리 생성
                    actionValueFunction.Add(i * 3 + 1, GetActionValueDictionary(i * 3 + 1));
                    // 백돌 차례인 상태에 대한 가치 함수 엔트리 생성
                    actionValueFunction.Add(i * 3 + 2, GetActionValueDictionary(i * 3 + 2));

                }
                else if (state.IsValidFirstStage())
                {
                    // 올바른 2단계 게임 보드인 경우
                    int nextTurn = state.GetFirstStageTurn();
                    // 현재 둘 차례인 돌을 반영한 상태에 대한 가치 함수 엔트리 생성
                    actionValueFunction.Add(i * 3 + nextTurn, GetActionValueDictionary(i * 3 + nextTurn));
                }
            }

            return actionValueFunction;
        }

        public static Dictionary<int, float> GetActionValueDictionary(int gameStateKey)
        {
            GameState gameState = new GameState(gameStateKey);
            Dictionary<int, float> actionValues = new Dictionary<int, float>();

            // 1부터 9까지의 모든 행동에 대해
            for (int i = GameParameters.ActionMinIndex; i <= GameParameters.ActionMaxIndex; i++)
            {
                if (gameState.IsValidMove(i)) // 올바른 행동인 경우
                {
                    actionValues.Add(i, 0.0f); // 가치 함수값을 0으로 초기화 한후 dictionary에 추가
                }
            }
            return actionValues;
        }


        public static Random random = new Random();

        public static int GetEpsilonGreedyAction(int turn, Dictionary<int, float> actionValues)
        {
            // Epsilon 탐욕 정책으로 행동을 선택하는 함수
            float greedyActionValue = 0.0f;
            float epsilon = 10;

            if (actionValues.Count == 0)
                return 0;

            if (turn == 1) // 흑돌 차례인 경우 가치 함수 최대값 선택
            {
                greedyActionValue = actionValues.Select(e => e.Value).Max();
            }
            else if (turn == 2) // 백돌 차례인 경우 가치 함수 최소값 선택
            {
                greedyActionValue = actionValues.Select(e => e.Value).Min();
            }

            int exploitRandom = random.Next(0, 100); // 랜덤값 발생
            IEnumerable<int> actionCandidates;

            if (exploitRandom < epsilon) // 탐험을 하는 경우
            {
                // 선택되지 않은 가치 함수값을 가지는 행동들을 선택
                actionCandidates = actionValues.Where(e => e.Value != greedyActionValue).Select(e => e.Key);
                if (actionCandidates.Count() == 0) // 만일 선택된 행동이 없으면 (가치함수값이 모두 똑같은 경우), 전체 행동 고려
                    actionCandidates = actionValues.Where(e => e.Value == greedyActionValue).Select(e => e.Key);
            }
            else // 탐험하지 않는 경우
            {
                // 선택된 가치 함수값을 가지는 행동들을 선택
                actionCandidates = actionValues.Where(e => e.Value == greedyActionValue).Select(e => e.Key);
            }

            // 선택된 행동들 중 하나를 랜덤하게 선택해서 반환
            return actionCandidates.ElementAt(random.Next(0, actionCandidates.Count()));
        }

        public static int GetGreedyAction(int turn, Dictionary<int, float> actionValues)
        {
            IEnumerable<int> actionCandidates = GetGreedyActionCandidate(turn, actionValues);

            if (actionCandidates.Count() == 0) return 0;

            return actionCandidates.ElementAt(random.Next(0, actionCandidates.Count()));
        }

        public static IEnumerable<int> GetGreedyActionCandidate(int turn, Dictionary<int, float> actionValues)
        {
            float greedyActionValue = 0.0f;
            if (actionValues.Count == 0)
            {
                return new List<int>();
            }

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
            {
                return 0.0f;
            }

            if (turn == 1)
            {
                // value의 최댓값
                return actionValues.Select(e => e.Value).Max();
            }
            else if (turn == 2)
            {
                return actionValues.Select(e => e.Value).Min();
            }

            return 0.0f;
        }
    }
}

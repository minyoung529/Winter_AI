using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforcement_Learning
{
    public class SarsaManager
    {
        public Dictionary<int, Dictionary<int, float>> ActionValueFunction;
        public float DiscountFactor = 0.9f;
        public float UpdateStep = 0.01f;

        public SarsaManager()
        {
            // 상태 가치 함수
            ActionValueFunction = new Dictionary<int, Dictionary<int, float>>();
        }

        public void UpdateBySarsa()
        {
            InitializeValueFunction();
            ApplySarsa();
        }

        public void InitializeValueFunction()
        {
            Console.Clear();
            Console.WriteLine("SARSA 시작");
            Console.WriteLine("가치 함수 초기화");

            ActionValueFunction.Clear();

            ActionValueFunction = Utilities.CreateActionValueFunction();

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("가치 함수 초기화 완료");

            Console.WriteLine(Environment.NewLine);
            Console.Write("아무 키나 누르세요:");
            Console.ReadLine();
        }

        // 다음 행동을 내주는 곳
        public int GetNextMove(int boardStateKey)
        {
            GameState gameState = new GameState(boardStateKey);
            return Utilities.GetEpsilonGreedyAction(gameState.NextTurn, ActionValueFunction[boardStateKey]);
        }

        public void ApplySarsa()
        {
            Console.Clear();
            Console.WriteLine("가치 함수 업데이트 시작");
            Console.WriteLine(Environment.NewLine);

            int episodeCount = 0;
            bool keepUpdating = true;

            while (keepUpdating)
            {
                GameState firstState = new GameState(); // 초기 게임 상태 생성
                bool episodeFinished = false; // 게임 종료 여부

                while (!episodeFinished)
                {
                    int firstAction // Epsilon 탐욕 정책으로 첫번째 행동 선택
                        = Utilities.GetEpsilonGreedyAction(
                                                            firstState.NextTurn,
                                                            ActionValueFunction[firstState.BoardStateKey]);

                    // 선택된 행동을 통해 전이해 간 두번째 상태 생성
                    GameState secondState = firstState.GetNextState(firstAction);

                    int secondAction // Epsilon 탐욕 정책으로 두번째 행동 선택
                        = Utilities.GetEpsilonGreedyAction(
                                                            secondState.NextTurn,
                                                            ActionValueFunction[secondState.BoardStateKey]);

                    // 두번째 상태에 대한 보상 계산
                    float reward = secondState.GetReward();

                    // 첫번째 상태, 행동에 대한 가치 함수값
                    float firstStateActionValue = ActionValueFunction[firstState.BoardStateKey][firstAction];

                    // 두번째 상태, 행동에 대한 가치 함수값
                    float secondStateActionValue = 0.0f;
                    if (secondAction != 0)
                        secondStateActionValue = ActionValueFunction[secondState.BoardStateKey][secondAction];

                    /// 보상!
                    // 가치 함수 업데이트
                    float _reward = (reward + DiscountFactor * secondStateActionValue - firstStateActionValue);
                    float updatedActionValue
                        = firstStateActionValue + UpdateStep * _reward;
                    ActionValueFunction[firstState.BoardStateKey][firstAction] = updatedActionValue;

                    // 에피소드가 종료된 경우
                    if (secondState.isFinalState() || ActionValueFunction[secondState.BoardStateKey].Count == 0)
                    {
                        episodeFinished = true;
                        episodeCount++;
                    }
                    else // 에피소드가 계속 진행되는 경우. 두번째 상태를 첫번째 상태로 재설정
                    {
                        firstState = secondState;
                    }
                }

                if (episodeCount % 10000 == 0) // 에피소드 10000개 수행할 때 마다 상태 출력
                {
                    Console.WriteLine($"에피소드를 {episodeCount}개 처리했습니다.");
                }

                if (episodeCount > 1000000) // 에피소드 100만개 처리 후 종료
                {
                    keepUpdating = false;
                }

            }

            Console.WriteLine(Environment.NewLine);
            Console.Write("SARSA를 종료합니다. 아무 키나 누르세요:");
            Console.ReadLine();
        }
    }
}

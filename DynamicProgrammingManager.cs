//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Reinforcement_Learning
//{
//    internal class DynamicProgrammingManager
//    {
//        public Dictionary<int, float> StateValueFunction;   // 가치 함수가 있는 딕셔너리

//        // 감가율
//        public float DiscountFactor = 0.9f;

//        int num00 = 0;
//        int num10 = 0;
//        int num11 = 0;
//        int num21 = 0;
//        int num22 = 0;
//        int num32 = 0;
//        int num33 = 0;
//        int num43 = 0;
//        int num44 = 0;

//        public DynamicProgrammingManager()
//        {
//            StateValueFunction = new Dictionary<int, float>();
//        }

//        public void UpdateByDynamicProgramming()
//        {
//            // 가치함수에 대한 초기화
//            InitializeValueFunction();

//            // 동적프로그래밍 사용
//            ApplyDynamicProgram();
//        }

//        public void InitializeValueFunction()
//        {
//            Console.Clear();
//            Console.WriteLine("동적 프로그래밍 시작");
//            Console.WriteLine("가치함수 초기화");

//            StateCountReset();
//            StateValueFunction.Clear();

//            for (int i = 0; i < GameParameters.StateCount; i++)
//            {
//                GameState state = new GameState();
//                state.PopulateBoard(i);

//                if (state.IsValidSecondStage())  // 2단계
//                {
//                    // i => 경우의 수
//                    // i*3+1 => 칸마다 해당되는 애가 3가지
//                    StateValueFunction.Add(i * 3 + 1, 0.0f);    // 검은 돌 차례 때 상태를 가치 함수로 저장
//                    StateValueFunction.Add(i * 3 + 2, 0.0f);    // 하얀 돌 차례 때 상태를 가치 함수로 저장

//                    // 왜 하얀 돌은 2일까?
//                    // 처음에 하얀 돌을 2로 정했기 때문에

//                    if (state.NumberOfBlacks == 4 && state.NumberOfWhites == 4)
//                    {
//                        num44++;
//                    }
//                }
//                else if (state.IsValidFirstStage())  // 1단계
//                {
//                    StateValueFunction.Add(i * 3 + state.GetFirstStageTurn(), 0.0f);

//                    if (state.NumberOfBlacks == 0 && state.NumberOfWhites == 0) num00++;
//                    if (state.NumberOfBlacks == 1 && state.NumberOfWhites == 0) num10++;
//                    if (state.NumberOfBlacks == 1 && state.NumberOfWhites == 1) num11++;
//                    if (state.NumberOfBlacks == 2 && state.NumberOfWhites == 1) num21++;
//                    if (state.NumberOfBlacks == 2 && state.NumberOfWhites == 2) num22++;
//                    if (state.NumberOfBlacks == 3 && state.NumberOfWhites == 2) num32++;
//                    if (state.NumberOfBlacks == 3 && state.NumberOfWhites == 3) num33++;
//                    if (state.NumberOfBlacks == 4 && state.NumberOfWhites == 3) num43++;
//                }
//            }

//            Console.WriteLine(Environment.NewLine);
//            Console.WriteLine("가치 함수 초기화 완료");
//            Console.WriteLine($"Black 0, White 0 : {num00}");
//            Console.WriteLine($"Black 1, White 0 : {num10}");
//            Console.WriteLine($"Black 1, White 1 : {num11}");
//            Console.WriteLine($"Black 2, White 1 : {num21}");
//            Console.WriteLine($"Black 2, White 2 : {num22}");
//            Console.WriteLine($"Black 3, White 2 : {num32}");
//            Console.WriteLine($"Black 3, White 3 : {num33}");
//            Console.WriteLine($"Black 4, White 3 : {num43}");
//            Console.WriteLine($"Black 4, White 4 : {num44}");
//            Console.WriteLine(Environment.NewLine);

//            Console.WriteLine("아무 키나 누르세요");
//            Console.ReadLine();
//        }

//        // 학습
//        public void ApplyDynamicProgram()
//        {
//            Console.Clear();
//            Console.WriteLine("동적 프로그래밍 학습");
//            Console.WriteLine(Environment.NewLine);

//            int loopCount = 0; // 반복 학습, 얼마만에 알았는지
//            bool terminateLoop = false;

//            // 목표에 도달했을 때 멈춘다
//            // 레벨링을 만들 수 있다!

//            while (!terminateLoop)
//            {
//                Dictionary<int, float> nextStateValueFunction = new Dictionary<int, float>();
//                float valueFunctionUpdateAmount = 0.0f;

//                foreach (KeyValuePair<int, float> valueFunctionEntry in StateValueFunction)
//                {
//                    // 학습을 통해 얻은 값 (책에 있는 내용)
//                    float updatedValue = UpdateValueFunction(valueFunctionEntry.Key);
//                    // 종료를 위해서 만들어놓음 (오차?)
//                    // valueFunctionEntry.Value => 기댓값
//                    float updateAmount = Math.Abs(valueFunctionEntry.Value - updatedValue);
//                    nextStateValueFunction[valueFunctionEntry.Key] = updatedValue;

//                    if (updateAmount > valueFunctionUpdateAmount)
//                    {
//                        valueFunctionUpdateAmount = updateAmount;
//                    }

//                    //valueFunctionUpdateAmount = Math.Max(updateAmount, valueFunctionUpdateAmount);
//                }

//                StateValueFunction = new Dictionary<int, float>(nextStateValueFunction);
//                loopCount++;

//                Console.WriteLine($"동적 프로그래밍 {loopCount}회 수행, 업데이트 오차 {valueFunctionUpdateAmount}");

//                if (valueFunctionUpdateAmount < 0.01f) terminateLoop = true;
//            }

//            Console.WriteLine(Environment.NewLine);
//            Console.Write("아무 키나 누르세요");
//            Console.ReadLine();
//        }

//        /// <summary>
//        /// 행동!
//        /// </summary>
//        private float UpdateValueFunction(int key)
//        {
//            GameState gameState = new GameState(key);
//            if (gameState.isFinalState()) return 0.0f;

//            // 행동 기댓값
//            List<float> actionExpectations = new List<float>();

//            for (int i = GameParameters.ActionMinIndex; i <= GameParameters.ActionMaxIndex; i++)
//            {
//                // 행동
//                if (gameState.IsValidMove(i))
//                {
//                    // 행동에서 바뀐 걳으로부터의 보상
//                    GameState nextState = gameState.GetNextState(i);
//                    float reward = nextState.GetReward();

//                    // 정책
//                    float actionExpect = reward + DiscountFactor * StateValueFunction[nextState.BoardStateKey];
//                    actionExpectations.Add(actionExpect);
//                }
//            }

//            if (actionExpectations.Count > 0)
//            {
//                if (gameState.NextTurn == 1)
//                {
//                    return actionExpectations.Max(); // 착한 정책
//                }
//                else if (gameState.NextTurn == 2)
//                {
//                    return actionExpectations.Min();
//                }
//            }

//            return 0.0f;
//        }

//        public void StateCountReset()
//        {
//            num00 = 0;
//            num10 = 0;
//            num11 = 0;
//            num21 = 0;
//            num22 = 0;
//            num32 = 0;
//            num33 = 0;
//            num43 = 0;
//            num44 = 0;
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
    public class DynamicProgrammingManager
    {
        public Dictionary<int, float> StateValueFunction;
        public float DiscountFactor = 0.9f;

        int num00 = 0;
        int num10 = 0;
        int num11 = 0;
        int num21 = 0;
        int num22 = 0;
        int num32 = 0;
        int num33 = 0;
        int num43 = 0;
        int num44 = 0;

        public DynamicProgrammingManager()
        {
            StateValueFunction = new Dictionary<int, float>();
        }

        public void UpdateByDynamicProgramming()
        {
            InitializeValueFunction();
            ApplyDynamicProgramming();
        }

        public void InitializeValueFunction()
        {
            Console.Clear();
            Console.WriteLine("동적 프로그래밍 시작");
            Console.WriteLine("가치 함수 초기화");

            StateCountReset();
            StateValueFunction.Clear();

            for (int i = 0; i <= GameParameters.StateCount; i++)
            {
                GameState state = new GameState();
                state.PopulateBoard(i);

                if (state.IsValidSecondStage())
                {
                    StateValueFunction.Add(i * 3 + 1, 0.0f);
                    StateValueFunction.Add(i * 3 + 2, 0.0f);

                    if (state.NumberOfBlacks == 4 && state.NumberOfWhites == 4)
                        num44++;
                }
                else if (state.IsValidFirstStage())
                {
                    StateValueFunction.Add(i * 3 + state.GetFirstStageTurn(), 0.0f);

                    if (state.NumberOfBlacks == 0 && state.NumberOfWhites == 0)
                        num00++;
                    if (state.NumberOfBlacks == 1 && state.NumberOfWhites == 0)
                        num10++;
                    if (state.NumberOfBlacks == 1 && state.NumberOfWhites == 1)
                        num11++;
                    if (state.NumberOfBlacks == 2 && state.NumberOfWhites == 1)
                        num21++;
                    if (state.NumberOfBlacks == 2 && state.NumberOfWhites == 2)
                        num22++;
                    if (state.NumberOfBlacks == 3 && state.NumberOfWhites == 2)
                        num32++;
                    if (state.NumberOfBlacks == 3 && state.NumberOfWhites == 3)
                        num33++;
                    if (state.NumberOfBlacks == 4 && state.NumberOfWhites == 3)
                        num43++;

                }
            }

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("가치 함수 초기화 완료");
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"Black 0, White 0: {num00}");
            Console.WriteLine($"Black 1, White 0: {num10}");
            Console.WriteLine($"Black 1, White 1: {num11}");
            Console.WriteLine($"Black 2, White 1: {num21}");
            Console.WriteLine($"Black 2, White 2: {num22}");
            Console.WriteLine($"Black 3, White 2: {num32}");
            Console.WriteLine($"Black 3, White 3: {num33}");
            Console.WriteLine($"Black 4, White 3: {num43}");
            Console.WriteLine($"Black 4, White 4: {num44}");
            Console.WriteLine(Environment.NewLine);
            Console.Write("아무 키나 누르세요:");
            Console.ReadLine();
        }


        public void ApplyDynamicProgramming()
        {
            Console.Clear();
            Console.WriteLine("동적 프로그래밍 학습");
            Console.WriteLine(Environment.NewLine);

            int loopCount = 0;
            bool terminateLoop = false;

            while (!terminateLoop)
            {
                Dictionary<int, float> nextStateValueFunction = new Dictionary<int, float>();

                float valueFunctionUpdateAmount = 0.0f;

                foreach (KeyValuePair<int, float> valueFunctionEntry in StateValueFunction)
                {
                    float updatedValue = UpdateValueFunction(valueFunctionEntry.Key);
                    float updateAmount = Math.Abs(valueFunctionEntry.Value - updatedValue);
                    nextStateValueFunction[valueFunctionEntry.Key] = updatedValue;
                    if (updateAmount > valueFunctionUpdateAmount)
                    {
                        valueFunctionUpdateAmount = updateAmount;
                    }
                }

                StateValueFunction = new Dictionary<int, float>(nextStateValueFunction);
                loopCount++;

                Console.WriteLine($"동적 프로그래밍 {loopCount}회 수행, 업데이트 오차{valueFunctionUpdateAmount}");

                if (valueFunctionUpdateAmount < 0.01f) terminateLoop = true;

            }

            Console.WriteLine(Environment.NewLine);
            Console.Write("아무 키나 누르세요");
            Console.ReadLine();

        }

        public float UpdateValueFunction(int gameStateKey)
        {
            GameState gameState = new GameState(gameStateKey);
            if (gameState.isFinalState()) return 0.0f;

            List<float> actionExpecatationList = new List<float>();

            for (int i = GameParameters.ActionMinIndex; i <= GameParameters.ActionMaxIndex; i++)
            {
                if (gameState.IsValidMove(i))
                {
                    GameState nextState = gameState.GetNextState(i);
                    float reward = nextState.GetReward();

                    float actionExpectation = reward + DiscountFactor * StateValueFunction[nextState.BoardStateKey];

                    actionExpecatationList.Add(actionExpectation);
                }
            }

            if (actionExpecatationList.Count > 0)
            {
                if (gameState.NextTurn == 1) return actionExpecatationList.Max();
                else if (gameState.NextTurn == 2) return actionExpecatationList.Min();
            }

            return 0.0f;

        }

        public int GetNextMove(int boardStateKey)
        {
            IEnumerable<int> actionCandidates = GetNextMoveCandidate(boardStateKey);

            if (actionCandidates.Count() == 0)
                return 0;

            return actionCandidates.ElementAt(new Random().Next(0, actionCandidates.Count()));
        }

        public IEnumerable<int> GetNextMoveCandidate(int boardStateKey)
        {
            float selectedExpection = 0.0f;
            GameState gameState = new GameState(boardStateKey);
            Dictionary<int, float> actionCandidateDictionary = new Dictionary<int, float>();

            for (int i = GameParameters.ActionMinIndex; i <= GameParameters.ActionMaxIndex; i++)
            {
                if (gameState.IsValidMove(i))
                {
                    GameState nextState = gameState.GetNextState(i);
                    float reward = nextState.GetReward();

                    float actionExpectation = reward + DiscountFactor * StateValueFunction[nextState.BoardStateKey];

                    actionCandidateDictionary.Add(i, actionExpectation);
                }
            }

            if(actionCandidateDictionary.Count == 0)
                return new List<int>();

            if(gameState.NextTurn == 1)
            {
                selectedExpection = actionCandidateDictionary.Select(e => e.Value).Max();
            }
            else if (gameState.NextTurn == 2)
            {
                selectedExpection = actionCandidateDictionary.Select(e => e.Value).Min();
            }

            return actionCandidateDictionary.Where(e => e.Value == selectedExpection).Select(e => e.Key);
        }

        private void StateCountReset()
        {
            num00 = 0;
            num10 = 0;
            num11 = 0;
            num21 = 0;
            num22 = 0;
            num32 = 0;
            num33 = 0;
            num43 = 0;
            num44 = 0;
        }

    }
}

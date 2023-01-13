using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Mini_Othello
{
    public class DynamicProgrammingManager
    {
        public Dictionary<int, float> StateValueFunction { get; set; }
        public float DiscountFactor = 0.9f;
        public string stateValueFunctionFilePath = "StateValueFunction.json";

        public DynamicProgrammingManager()
        {
            StateValueFunction = new Dictionary<int, float>();
        }

        public void UpdateByDynamicProgramming()
        {
            InitializeValueFunction();

            ApplyDynamicProgramming();
        }

        // 가치함수 초기화
        public void InitializeValueFunction()
        {
            Console.Clear();
            Console.WriteLine("동적 프로그래밍 시작");
            Console.WriteLine("가치 함수 초기화");

            StateValueFunction.Clear();

            // 초기 상태
            GameState gameState = new GameState();
            // 게임 상태 후보 리스트
            List<int> boardStateKeyList = new List<int>() { gameState.BoardStateKey };

            // 모든 게임 상태에 대해서 반복
            while (true)
            {
                List<int> mergedChildStateList = new List<int>();

                foreach (int gameBoardKey in boardStateKeyList)
                {
                    // 가치함수에 상태가 포함되어있지 않다면 넣어준다
                    if (!StateValueFunction.ContainsKey(gameBoardKey))
                    {
                        StateValueFunction.Add(gameBoardKey, 0.0f);
                        // 상태 생성
                        GameState processingGameState = new GameState(gameBoardKey);

                        // 끝나지 않는다면
                        if (!processingGameState.IsFinalState())
                        {
                            List<int> childStateList = new List<int>();

                            // 행동을 모두 실행
                            for (int i = GameParameters.ActionMinIndex; i <= GameParameters.ActionMaxIndex; i++)
                            {
                                // 행동할 수 있다면
                                if (processingGameState.IsValidMove(i))
                                {
                                    GameState nextState = processingGameState.GetNextState(i);  // 행동을 통해 다음 상태 만들기
                                    childStateList.Add(nextState.BoardStateKey);                // 후보리스트에 넣어줌
                                }
                            }

                            // 아무 행동도 할 수 없었다면
                            if (childStateList.Count == 0)
                            {
                                GameState nextState = processingGameState.GetNextState(0);  // 패스
                                childStateList.Add(nextState.BoardStateKey);
                            }

                            // 임시로 만든 후보 리스트의 상태 중에
                            // 자식 상태 리스트에 포함되어있지 않은 상태를 자식 상태에 추가
                            mergedChildStateList.AddRange(childStateList.Where(e => !mergedChildStateList.Contains(e)));
                        }
                    }
                }
                if (mergedChildStateList.Count == 0)
                    break;
                else
                    boardStateKeyList = new List<int>(mergedChildStateList);
            }

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"가치 함수 초기화 완료, 상태 {StateValueFunction.Count} 개");
            Console.WriteLine(Environment.NewLine);
            Console.Write("아무 키나 누르세요:");
            Console.ReadLine();
        }

        // DP 적용 (학습)
        public void ApplyDynamicProgramming()
        {
            Console.Clear();
            Console.WriteLine("동적 프로그래밍 적용");
            Console.WriteLine(Environment.NewLine);

            int loopCount = 0;
            bool terminateLoop = false;

            while (!terminateLoop)
            {
                // 업데이트가 되는 가치 함수값을 임시로 저장
                Dictionary<int, float> nextStateValueFunction = new Dictionary<int, float>();
                float valueFunctionUpdateAmount = 0.0f;

                foreach (KeyValuePair<int, float> valueFunctionEntry in StateValueFunction)
                {
                    float updatedValue = UpdateValueFunction(valueFunctionEntry.Key);           // 가치 함수 업데이트 계산
                    float updatedAmount = Math.Abs(valueFunctionEntry.Value - updatedValue);    // 오차
                    nextStateValueFunction[valueFunctionEntry.Key] = updatedValue;              // 갱신

                    // 그리디 정책에 대한 값 추출
                    valueFunctionUpdateAmount = Math.Max(valueFunctionUpdateAmount, updatedAmount);
                }

                StateValueFunction = new Dictionary<int, float>(nextStateValueFunction);

                Console.WriteLine($"동적 프로그래밍 {loopCount++}회 수행, 업데이트 오차 {valueFunctionUpdateAmount}");

                // 오차가 0.01 이하이면 종료
                if (valueFunctionUpdateAmount < 0.01f)
                    terminateLoop = true;
            }

            Console.WriteLine(Environment.NewLine);
            Console.Write("아무 키나 누르세요:");
            Console.ReadLine();
        }

        // 다음 행동의 기댓값 중 최적의 값
        public float UpdateValueFunction(int gameStateKey)
        {
            GameState gameState = new GameState(gameStateKey);

            if (gameState.IsFinalState())
                return 0.0f;

            List<float> actionExpectationList = new List<float>();

            for (int i = GameParameters.ActionMinIndex; i <= GameParameters.ActionMaxIndex; i++)
            {
                // 움직일 수 있다면 다음 스테이지로
                if (gameState.IsValidMove(i))
                {
                    GameState nextState = gameState.GetNextState(i);
                    float reward = nextState.GetReward();
                    float actionExpectation = reward + DiscountFactor * StateValueFunction[nextState.BoardStateKey];

                    actionExpectationList.Add(actionExpectation);
                }
            }

            // 패스
            if (actionExpectationList.Count == 0)
            {
                GameState nextState = gameState.GetNextState(0);
                float reward = nextState.GetReward();

                float actionExpectation = reward + DiscountFactor * StateValueFunction[nextState.BoardStateKey];

                actionExpectationList.Add(actionExpectation);
            }

            if (gameState.NextTurn == 1)
            {
                return actionExpectationList.Max();
            }
            else if (gameState.NextTurn == 2)
            {
                return actionExpectationList.Min();
            }
            return 0.0f;
        }

        // 학습한 값을 토대로 다음 행동 반환
        public int GetNextMove(int boardStateKey)
        {
            IEnumerable<int> actionCandidates = GetNextMoveCandidate(boardStateKey);

            if (actionCandidates.Count() == 0)
                return 0;

            return actionCandidates.ElementAt(Utilities.random.Next(0, actionCandidates.Count()));
        }

        // 다음 행동 후보를 반환
        public IEnumerable<int> GetNextMoveCandidate(int boardStateKey)
        {
            float selectedExpectation = 0.0f;

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

            if (gameState.NextTurn == 1)
            {
                selectedExpectation = actionCandidateDictionary.Select(e => e.Value).Max();
            }
            else if (gameState.NextTurn == 2)
            {
                selectedExpectation = actionCandidateDictionary.Select(e => e.Value).Min();
            }

            return actionCandidateDictionary.Where(e => e.Value == selectedExpectation).Select(e => e.Key);
        }

        public void SaveStateValueFunction()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;

            string stateValueFunctionInJson = JsonConvert.SerializeObject(StateValueFunction, settings);

            File.WriteAllText(stateValueFunctionFilePath, stateValueFunctionInJson);

            Console.Clear();
            Console.WriteLine($"가치 함수가 파일 {stateValueFunctionFilePath}에 저장되었습니다.");
            Console.WriteLine(Environment.NewLine);
            Console.Write("아무 키나 누르세요:");
            Console.ReadLine();
        }

        public void LoadStateValueFunction()
        {
            if (File.Exists(stateValueFunctionFilePath))
            {
                string stateValueFunctionInJson = File.ReadAllText(stateValueFunctionFilePath);

                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Formatting = Formatting.Indented;

                StateValueFunction = JsonConvert.DeserializeObject<Dictionary<int, float>>(stateValueFunctionInJson, settings);

                Console.Clear();
                Console.WriteLine($"가치 함수가 파일 {stateValueFunctionFilePath}에서 로드되었습니다.");
            }
            else
            {
                Console.Clear();
                Console.WriteLine($"가치 함수 파일 {stateValueFunctionFilePath}이 존재하지 않습니다.");
            }
            Console.WriteLine(Environment.NewLine);
            Console.Write("아무 키나 누르세요:");
            Console.ReadLine();
        }
    }
}

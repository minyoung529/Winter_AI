using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforcement_Learning
{
    public class Define
    {
        public const int GRID = 3;
        public const int SPACE = GRID*GRID;
    }

    public enum GamePlayer
    {
        DynamicProgramming,
        SARSA,
        QLearning,
        Human,
        None
    }

    public enum PlayerType
    {
        None,
        Black,
        White
    }
}

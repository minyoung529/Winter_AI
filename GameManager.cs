using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforcement_Learning
{
    public enum GamePlayer
    {
        DynamicProgramming,
        Human,
        None
    }

    internal class GameManager
    {
        public GamePlayer BlackPlayer;
        public GamePlayer WhitePlayer;

        public void PlayGame()
        {
            while(true)
            {
                BlackPlayer = GetBlackPlayer();
                if (BlackPlayer == GamePlayer.None) return;

                WhitePlayer = GetWhitePlayer();
                if (BlackPlayer == GamePlayer.None) return;
            }
        }

        private GamePlayer GetWhitePlayer()
        {
            return PlayerSelection("X 플레이어를 선택해주세요");
        }

        private GamePlayer GetBlackPlayer()
        {
            return PlayerSelection("O 플레이어를 선택해주세요");
        }

        private GamePlayer PlayerSelection(string menuLabel)
        {
            while(true)
            {
                Console.
            }
        }
    }
}

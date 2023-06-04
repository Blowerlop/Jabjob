using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    [CreateAssetMenu(menuName = "Scriptable Objects /GameModes/FreeForAll")]
    public class GameMode_FreeForAll : GameMode
    {
        public int killToWin = 30;


        public override void Start()
        {
            GameEvent.onPlayerGetAKillEvent.Subscribe(WinCondition, true);
        }

        public override void OnDestroy()
        {
            GameEvent.onPlayerGetAKillEvent.Unsubscribe(WinCondition);
        }


        private void WinCondition(ulong player, int killNumber)
        {
            if (killNumber >= killToWin)
            {
                Debug.Log($"The winner is : {GameManager.instance.GetPlayer(player).name}");
                GameEvent.onGameFinishedEvent.Invoke(this, true);
            }
        }
    }
}

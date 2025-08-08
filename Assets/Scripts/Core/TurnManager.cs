using UnityEngine;
using System;

namespace Robotech.TBS.Core
{
    // Simple turn manager for single-player skirmish: Player -> AI -> repeat
    public class TurnManager : MonoBehaviour
    {
        public enum TurnPhase { Player, AI }
        public TurnPhase CurrentPhase { get; private set; } = TurnPhase.Player;
        public int TurnNumber { get; private set; } = 1;

        public static event Action<int> OnTurnStarted;       // turn number
        public static event Action<int> OnTurnEnded;         // turn number
        public static event Action<TurnPhase> OnPhaseChanged; // phase

        void Start()
        {
            OnTurnStarted?.Invoke(TurnNumber);
            OnPhaseChanged?.Invoke(CurrentPhase);
        }

        public void EndPhase()
        {
            if (CurrentPhase == TurnPhase.Player)
            {
                CurrentPhase = TurnPhase.AI;
                OnPhaseChanged?.Invoke(CurrentPhase);
                // For now, immediately end AI phase (stub AI)
                EndPhase();
                return;
            }

            // End of AI phase -> advance turn
            EndTurn();
        }

        public void EndTurn()
        {
            OnTurnEnded?.Invoke(TurnNumber);
            TurnNumber++;
            CurrentPhase = TurnPhase.Player;
            OnTurnStarted?.Invoke(TurnNumber);
            OnPhaseChanged?.Invoke(CurrentPhase);
        }
    }
}

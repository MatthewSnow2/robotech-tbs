using UnityEngine;
using System;
using System.Collections;

namespace Robotech.TBS.Core
{
    /// <summary>
    /// Manages turn progression and phase transitions for single-player skirmish gameplay.
    /// Handles the Player -> AI -> Next Turn cycle.
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        /// <summary>
        /// Represents the current phase of gameplay.
        /// </summary>
        public enum TurnPhase
        {
            /// <summary>Player's turn to act.</summary>
            Player,
            /// <summary>AI's turn to act.</summary>
            AI
        }

        /// <summary>
        /// Gets the current phase of the turn (Player or AI).
        /// </summary>
        public TurnPhase CurrentPhase { get; private set; } = TurnPhase.Player;

        /// <summary>
        /// Gets the current turn number (1-based indexing).
        /// </summary>
        public int TurnNumber { get; private set; } = 1;

        /// <summary>
        /// Event triggered when a new turn begins.
        /// Passes the new turn number as parameter.
        /// </summary>
        public static event Action<int> OnTurnStarted;

        /// <summary>
        /// Event triggered when a turn ends (after both Player and AI phases complete).
        /// Passes the ending turn number as parameter.
        /// </summary>
        public static event Action<int> OnTurnEnded;

        /// <summary>
        /// Event triggered when the phase changes (Player <-> AI).
        /// Passes the new phase as parameter.
        /// </summary>
        public static event Action<TurnPhase> OnPhaseChanged;

        /// <summary>
        /// AI thinking delay in seconds. Adjust for desired AI response time.
        /// </summary>
        [SerializeField]
        private float aiThinkingDelay = 0.5f;

        void Start()
        {
            // Initialize the first turn
            OnTurnStarted?.Invoke(TurnNumber);
            OnPhaseChanged?.Invoke(CurrentPhase);
        }

        /// <summary>
        /// Ends the current phase and transitions to the next phase or turn.
        /// If in Player phase, transitions to AI phase.
        /// If in AI phase, advances to the next turn.
        /// </summary>
        public void EndPhase()
        {
            if (CurrentPhase == TurnPhase.Player)
            {
                // Transition from Player to AI phase
                StartCoroutine(ProcessAIPhase());
            }
            else
            {
                // End of AI phase -> advance to next turn
                EndTurn();
            }
        }

        /// <summary>
        /// Processes the AI phase using a coroutine to avoid recursion.
        /// Allows for delayed AI actions and smooth UI transitions.
        /// </summary>
        /// <returns>IEnumerator for coroutine execution.</returns>
        private IEnumerator ProcessAIPhase()
        {
            // Transition to AI phase
            CurrentPhase = TurnPhase.AI;
            OnPhaseChanged?.Invoke(CurrentPhase);

            // Allow UI to update before AI acts
            yield return null;

            // Simulate AI "thinking" time
            // TODO: Replace with actual AI logic when implemented
            yield return new WaitForSeconds(aiThinkingDelay);

            // AI actions would go here
            // For now, immediately end AI phase (stub AI)

            // End the AI phase (non-recursively)
            EndPhase();
        }

        /// <summary>
        /// Ends the current turn and begins the next turn.
        /// Increments turn number and resets phase to Player.
        /// </summary>
        private void EndTurn()
        {
            OnTurnEnded?.Invoke(TurnNumber);
            TurnNumber++;
            CurrentPhase = TurnPhase.Player;
            OnTurnStarted?.Invoke(TurnNumber);
            OnPhaseChanged?.Invoke(CurrentPhase);
        }

        /// <summary>
        /// Resets the turn manager to initial state.
        /// Useful for restarting matches or scenarios.
        /// </summary>
        public void ResetTurnManager()
        {
            TurnNumber = 1;
            CurrentPhase = TurnPhase.Player;
            OnTurnStarted?.Invoke(TurnNumber);
            OnPhaseChanged?.Invoke(CurrentPhase);
        }
    }
}

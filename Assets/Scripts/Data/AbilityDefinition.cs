using UnityEngine;

namespace Robotech.TBS.Data
{
    /// <summary>
    /// Placeholder for ability definitions.
    /// To be fully implemented in a future phase.
    /// </summary>
    [CreateAssetMenu(fileName = "AbilityDefinition", menuName = "Robotech/Data/Ability Definition", order = 11)]
    public class AbilityDefinition : ScriptableObject
    {
        /// <summary>
        /// Unique identifier for this ability.
        /// </summary>
        public string abilityId;

        /// <summary>
        /// Display name shown to the player.
        /// </summary>
        public string displayName;

        /// <summary>
        /// Description of what this ability does.
        /// </summary>
        [TextArea]
        public string description;
    }
}

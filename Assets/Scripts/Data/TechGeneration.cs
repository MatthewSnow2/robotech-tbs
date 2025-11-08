namespace Robotech.TBS.Data
{
    /// <summary>
    /// Represents the generation or era of a technology in the tech tree.
    /// Technologies are grouped into generations that roughly correspond to
    /// the progression of the Robotech timeline and game eras.
    /// </summary>
    public enum TechGeneration
    {
        /// <summary>
        /// Generation 0: Starting technologies available at game start.
        /// Represents pre-war baseline technology.
        /// </summary>
        Gen0,

        /// <summary>
        /// Generation 1: Early-war technologies.
        /// Initial research options and basic military advances.
        /// </summary>
        Gen1,

        /// <summary>
        /// Generation 2: Mid-war technologies.
        /// Advanced military systems and improved infrastructure.
        /// </summary>
        Gen2,

        /// <summary>
        /// Generation 3: Late-war technologies.
        /// Sophisticated weapons and defensive systems.
        /// </summary>
        Gen3,

        /// <summary>
        /// Generation 4: Post-war reconstruction technologies.
        /// Advanced protoculture applications and specialized systems.
        /// </summary>
        Gen4,

        /// <summary>
        /// Generation 5: Endgame technologies.
        /// Ultimate technological achievements and victory conditions.
        /// </summary>
        Gen5
    }
}

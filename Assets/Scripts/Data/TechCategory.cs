namespace Robotech.TBS.Data
{
    /// <summary>
    /// Represents the category or domain of a technology.
    /// Technologies are classified by their primary area of application,
    /// which helps organize the tech tree and provides thematic coherence.
    /// </summary>
    public enum TechCategory
    {
        /// <summary>
        /// Power and energy systems.
        /// Includes protoculture refinement, reactor efficiency, and energy infrastructure.
        /// </summary>
        Power,

        /// <summary>
        /// Mecha and transformable combat systems.
        /// Covers Veritech fighters, Battloids, and other transformable units.
        /// </summary>
        Mecha,

        /// <summary>
        /// Weapons and offensive systems.
        /// Includes beam weapons, missiles, and advanced armaments.
        /// </summary>
        Weapons,

        /// <summary>
        /// Defense and armor technologies.
        /// Covers shielding, armor plating, point defense, and ECM systems.
        /// </summary>
        Defense,

        /// <summary>
        /// Aerospace and space-based technologies.
        /// Includes spacecraft, orbital systems, and space-to-ground capabilities.
        /// </summary>
        Aerospace,

        /// <summary>
        /// Special technologies and unique systems.
        /// Covers protoculture research, special abilities, and unique game mechanics.
        /// </summary>
        Special
    }
}

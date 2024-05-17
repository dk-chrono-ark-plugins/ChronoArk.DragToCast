namespace DragToCast.Api;

internal interface ICastable
{
    enum CastingType
    {
        /// <summary>
        /// <see cref="global::SkillButton"/>
        /// </summary>
        SkillButton,

        /// <summary>
        /// <see cref="global::BasicSkill"/>
        /// </summary>
        BasicSkill,
    }

    /// <summary>
    /// The skill implementation type
    /// </summary>
    CastingType CastType { get; }

    /// <summary>
    /// Skill data
    /// </summary>
    Skill SkillData { get; }

    /// <summary>
    /// Cast this skill into the void
    /// </summary>
    void Cast();
}

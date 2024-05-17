namespace DragToCast.Api;

internal interface IConfigurable
{
    /// <summary>
    /// The unique ID of this configuration
    /// </summary>
    string Id { get; }

    /// <summary>
    /// The name entry in config menu
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The explanatory info for this entry
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Whether this config is mandatory
    /// </summary>
    bool Mandatory { get; }
}

namespace DragToCast.Api;

internal interface IPatch : IConfigurable
{
    /// <summary>
    /// The patch itself
    /// </summary>
    void Commit();
}

namespace DragToCast.Api;

internal interface ITarget
{
    void Accept(ICastable castable);

    bool IsValidTargetOf(ICastable castable);
}

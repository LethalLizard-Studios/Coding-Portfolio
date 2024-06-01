public struct ProjectedInt32Value
{
    private int _projected;

    public ProjectedInt32Value(int value)
    {
        _projected = value - 111;
    }

    public readonly int GetValue()
    {
        return _projected + 111;
    }

    public void SetValue(int value)
    {
        _projected = value - 111;
    }
}

public class ItemBase
{
    public int x
    {
        get;
        set;
    }

    public int y
    {
        get;
        set;
    }

    public ItemBase() { }

    public void SetPosition(int newX, int newY)
    {
        x = newX;
        y = newY;
    }

    public ItemBase DeepCopy()
    {
        ItemBase copy = new ItemBase();
        copy.x = this.x;
        copy.y = this.y;
        return copy;
    }
}

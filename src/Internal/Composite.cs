namespace DiComposite.Internal;

public class  Composite<T> : IComposite<T>
{
    public Composite(T instance)
    {
        Value = instance;
    }
    public T Value { get; }
}

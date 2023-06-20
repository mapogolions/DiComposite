namespace DiComposite.Internal;

public class  Composite<T> : IComposite<T>
{
    public Composite(T instance)
    {
        Instance = instance;
    }
    public T Instance { get; }
}

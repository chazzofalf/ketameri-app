namespace Tokenizer;

public class Tokenizer
{
    private ForwardTokenizer? _ForwardTokenizer = null;
    private ReverseTokenizer? _ReverseTokenizer = null;
    private ForwardTokenizer ForwardTokenizer => _ForwardTokenizer = _ForwardTokenizer = new ForwardTokenizer();
    private ReverseTokenizer ReverseTokenizer => _ReverseTokenizer = _ReverseTokenizer = new ReverseTokenizer();
    public void Put<T>(T value)
    {
        if (value is char vc)
        {
            ForwardTokenizer.Put(vc);
        }
        else if (value is Graphic.Graphic token)
        {
            ReverseTokenizer.Put(token);
        }
    }
    public T? Finish<T>()
    {
        if (typeof(T) == typeof(Graphic.Graphic[]))
        {
            return (T)(object)ForwardTokenizer.Finish();
        }
        else if (typeof(T) == typeof(string))
        {
            return (T)(object)ReverseTokenizer.Finish();
        }
        return default(T);
    }
    
}

using Bible;

public interface ISessionAdder
{
    void AddSession(BookNames book, int chapter, int verse);
    public static event Action<BookNames, int, int>? OnSessionCompleted;
}
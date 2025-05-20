using Bible;

public interface ISessionAdder
{
    void AddSession(BibleBooks book, int chapter, int verse);
    public static event Action<BibleBooks, int, int>? OnSessionCompleted;
}
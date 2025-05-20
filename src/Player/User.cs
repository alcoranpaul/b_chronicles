using Bible;

namespace Player;

public class User
{
    private readonly BookComponent bookComponent;
    public bool HasBooks => bookComponent.HasBooks;
    public User()
    {
        bookComponent = new();
    }

    public void AddBook(BibleBooks book)
    {
        bookComponent.AddBook(book);
    }

    public void End()
    {
        bookComponent.End();
    }


}
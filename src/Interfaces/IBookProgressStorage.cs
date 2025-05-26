namespace Player.BibleBook;

public interface IBookProgressStorage
{
    void BeginSession(Bible.BookNames book, int chapter, int verse);
    void SaveProgress(Bible.BookNames book, int chapter, int verse,
        (Bible.BookNames nextBook, int nextChapter, int nextVerse) next);
    ProgressData? LoadProgress();
    void Flush();
    void UpdateBookProgress(Bible.BookNames book, BookJsonProgressStorage.BookProgressState state);
}

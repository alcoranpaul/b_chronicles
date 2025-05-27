using System.Text.Json;


namespace Player.BibleBook;

/// <summary>
/// Manages the collection of books that the user has read,
/// including loading from and saving to a JSON save file.
/// </summary>
using System;
using Bible;

public sealed class BookTracker : IDisposable
{
    private readonly IBookProgressStorage _storage;
    private readonly BookMetadata _metadata;
    public Book? CurrentBook { get; private set; }

    public BookTracker()
    {
        _storage = new BookJsonProgressStorage();
        _metadata = new BookMetadata();
    }

    public void StartReading(BookNames book, int chapter, int verse)
    {
        if (!_metadata.IsValidPassage(book, chapter, verse))
            throw new ArgumentException($"Invalid scripture: {book} {chapter}:{verse}");

        CurrentBook = new Book(book);
        _storage.BeginSession(book, chapter, verse);
        _storage.UpdateBookProgress(book, BookJsonProgressStorage.BookProgressState.InProgress);
    }

    public void FinishReading(BookNames book, int chapter, int verse)
    {
        if (CurrentBook?.Name != book) return;

        var next = CalculateNextPassage(book, chapter, verse);
        LogDebug($"Saving finished [{book} {chapter}:{verse}]");
        LogDebug($"Next passage is [{next.book} {next.chapter}:{next.verse}]");

        if (next.book != book)
        {
            LogDebug($"Moving on to the book of {next.book} <= {book}");
            _storage.UpdateBookProgress(book, BookJsonProgressStorage.BookProgressState.Completed);
        }

        _storage.SaveProgress(book, chapter, verse, next);
        CurrentBook = null;
    }

    public (BookNames book, int chapter, int verse) GetNextReading()
    {
        ProgressData? progress = _storage.LoadProgress();
        LogDebug($"Request confirmed: Next Reading is {progress}");

        if (progress == null)
            return (BookNames.Genesis, 1, 1);
        else
        {
            ProgressData progressValue = progress.Value;
            if (progressValue.LastRead == null)
                return (progressValue.CurrentBook, progressValue.CurrentChapter, progressValue.CurrentVerse);
            else
                return (progressValue.NextBook, progressValue.NextChapter, progressValue.NextVerse);
        }
    }

    private (BookNames book, int chapter, int verse) CalculateNextPassage(
        BookNames book, int chapter, int verse)
    {
        // Verse → Chapter → Book progression
        if (verse < _metadata.GetVerseCount(book, chapter))
            return (book, chapter, verse + 1);

        if (chapter < _metadata.GetChapterCount(book))
            return (book, chapter + 1, 1);

        if (!_metadata.GetNextBookName(book, out BookNames nextBook))
        {
            LogError($"Next Book is Null"); // TODO: Handle case when Revelation is finished
        }

        return (nextBook, 1, 1);
    }

    public void Dispose() => _storage.Flush();
}
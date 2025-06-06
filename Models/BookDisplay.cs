namespace LibraryApp.Models;

public class BookDisplay
{
    public int BookID { get; set; }
    public string Title { get; set; }
    public string AuthorName { get; set; }
    public int PublishedYear { get; set; }
    public bool Available { get; set; }
}
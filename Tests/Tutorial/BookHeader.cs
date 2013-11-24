namespace Nejdb.Tests
{
    public struct BookHeader
    {
        public BookHeader(string title, int published) : this()
        {
            Title = title;
            Published = published;
        }

        public string Title { get; set; }

        public int Published { get; set; }
    }
}
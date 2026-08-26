namespace ExamSystem.DTOs
{
    public class Paginated<T>
    {
        public List<T> Items { get; set; }
        public List<T> Data
        {
            get => Items;
            set => Items = value;
        }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}

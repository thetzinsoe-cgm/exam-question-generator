namespace ExamSystem.DTOs.Common
{
    public class BaseFilterDto
    {
        private int _pageNumber = 1;
        private int _pageSize = 10;

        public int page_number
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        public int page_size
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? 10 : value;
        }
    }
}

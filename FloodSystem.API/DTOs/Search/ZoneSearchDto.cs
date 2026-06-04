namespace FloodSystem.API.DTOs.Search
{
    public class ZoneSearchDto
    {
        public string? Name { get; set; }
        public bool? IsActive { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } = "CreatedAt";
        public bool SortDescending { get; set; } = true;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
namespace RestApiDemo.WebApi.Pagination
{
    public class PagingBaseDto
    {
        public int? Offset { get; set; }
        public int? Limit { get; set; }
        public string Sort { get; set; }
    }
}
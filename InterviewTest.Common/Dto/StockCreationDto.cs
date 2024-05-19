namespace InterviewTest.Common.Dto
{
    public class StockCreationDto
    {
        public TransactionType TransactionType { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public IEnumerable<StockDetailCreationDto> StockDetailListDtos { get; set; } = Enumerable.Empty<StockDetailCreationDto>();
    }
}

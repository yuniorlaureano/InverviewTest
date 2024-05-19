namespace InterviewTest.Common.Dto
{
    public class StockUpdateDto
    {
        public long Id { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

        public IEnumerable<StockDetailCreationDto> StockDetailListDtos { get; set; } = Enumerable.Empty<StockDetailCreationDto>();
    }
}

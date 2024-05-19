namespace InterviewTest.Common.Dto
{
    public class StockListDto
    {
        public long Id { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public IEnumerable<StockDetailListDto> StockDetailListDtos { get; set; }
    }
}

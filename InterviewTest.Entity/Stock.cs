using InterviewTest.Common;

namespace InterviewTest.Entity
{
    public class Stock
    {
        public long Id { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}

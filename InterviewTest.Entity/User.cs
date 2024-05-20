namespace InterviewTest.Entity
{
    public class User
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public byte Age { get; set; }
        public DateTime Date { get; set; }
        public long CountryId { get; set; }
        public long ProvinceId { get; set; }
        public long CityId { get; set; }
    }
}

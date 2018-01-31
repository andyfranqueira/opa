namespace OPA.BusinessObjects
{
    public class PersonAddress
    {
        public int Id { get; set; }
        public Person Person { get; set; }
        public Address Address { get; set; }
    }
}

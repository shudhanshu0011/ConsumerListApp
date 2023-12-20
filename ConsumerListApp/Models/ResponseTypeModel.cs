namespace ConsumerListApp.Models
{
    public class ResponseTypeModel
    {
        public List<ApiResModel> Result { get; set; }

        public string Message { get; set; }

        public string Status { get; set; }
    }
}

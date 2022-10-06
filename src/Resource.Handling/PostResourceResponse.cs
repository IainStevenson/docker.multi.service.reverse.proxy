namespace Resource.Handling
{
    public class PostResourceResponse
    {
        /// <summary>
        /// Determines the resulting action response from the calling controller. The default value is 400 = BadRequest.
        /// </summary>
        public int StatusCode { get; set; } = 400;

        /// <summary>
        /// The desired payload model
        /// </summary>
        public Data.Model.Response.Resource Model { get; set; }  = new() { };
        public IEnumerable<string> RequestValidationErrors { get; set; }
    }

}
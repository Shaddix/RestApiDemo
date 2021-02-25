namespace RestApiDemo.WebApi.PatchRequests
{
    public interface IPatchRequest
    {
        /// <summary>
        /// Returns true if property was present in http request; false otherwise 
        /// </summary>
        bool IsFieldPresent(string propertyName);

        void SetHasProperty(string propertyName);
    }
}
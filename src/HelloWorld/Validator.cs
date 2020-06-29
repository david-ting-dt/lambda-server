namespace HelloWorld
{
    public static class Validator
    {
        public static bool ValidateRequest(string requestBody)
        {
            return requestBody.Length > 0 && requestBody.Length < 30;
        }
    }
}
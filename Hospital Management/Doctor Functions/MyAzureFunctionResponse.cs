namespace Hospital_Management.Doctor_Functions
{
    internal class MyAzureFunctionResponse
    {
        public MyAzureFunctionResponse()
        {
        }

        public string Message { get; internal set; }
        public int ErrorCode { get; internal set; }
    }
}
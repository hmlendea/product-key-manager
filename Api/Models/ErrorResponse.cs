using System;

namespace ProductKeyManager.Api.Models
{
    public sealed class ErrorResponse : Response
    {
        public override bool IsSuccess => false;

        public string Message { get; }

        public ErrorResponse(string message)
        {
            Message = message;
        }

        public ErrorResponse(Exception exception)
        {
            Message = exception.Message;
        }
    }
}

using FluentValidation.Results;

namespace __NAMESPACE__.Dto.Base
{
    public class ResponseDto
    {
        public bool IsValid => Messages.All(x => x.MessageType != ApplicationMessageType.Error);
        public IEnumerable<ApplicationMessageDto> Messages { get; set; }

        public ResponseDto()
            => Messages = new List<ApplicationMessageDto>();

        public ResponseDto(IEnumerable<ApplicationMessageDto> messages)
            => Messages = messages ?? throw new ArgumentNullException(nameof(messages));

        public ResponseDto(string message) : this()
            => AddOkResult(message);

        public ResponseDto(Exception exception) : this()
            => AddErrorResult(exception);

        public void AddOkResult(string message)
            => AddResult(ApplicationMessageType.Ok, message);

        public void AddInfoResult(string message)
            => AddResult(ApplicationMessageType.Info, message);

        public void AddWarningResult(string message)
            => AddResult(ApplicationMessageType.Warning, message);

        public void AddErrorResult(string message)
            => AddResult(ApplicationMessageType.Error, message);

        public void AddErrorResult(Exception exception)
            => AddResult(ApplicationMessageType.Error, GetExceptionMessage(exception));

        public void AddErrorResult(string message, Exception exception)
            => AddResult(ApplicationMessageType.Error, string.Concat(message, Environment.NewLine, GetExceptionMessage(exception)));

        public void AddErrorResult(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
                AddResult(ApplicationMessageType.Error, error.ErrorMessage);
        }

        private static string GetExceptionMessage(Exception exception)
            => string.Concat(exception.Message, Environment.NewLine, exception.StackTrace);

        public void AddResult(ApplicationMessageType apiResultType, string message)
        {
            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));

            var messages = Messages.ToList();

            messages.Add(new ApplicationMessageDto { MessageType = apiResultType, Message = message });

            Messages = messages;
        }

        public void AttachResults(ResponseDto response)
        {
            if (response == null) return;

            var messages = Messages.ToList();

            response.Messages.ToList().ForEach(result => messages.Add(result));

            Messages = messages;
        }

        public void AttachResultsWithType(ApplicationMessageType apiResultType, ResponseDto response)
        {
            var messages = Messages.ToList();

            response.Messages.ToList().ForEach(result => messages.Add(new ApplicationMessageDto { MessageType = apiResultType, Message = result.Message }));

            Messages = messages;
        }

        public string GetFormattedApiResponse()
        {
            var response = string.Empty;

            response = string.Concat(response, "Is Valid: ", IsValid);

            if (Messages.Any())
            {
                response = string.Concat(response, Environment.NewLine, "Messages: ", Environment.NewLine);
                response = string.Concat(response, Messages.Select(x => string.Concat("- ", Enum.GetName(typeof(ApplicationMessageType), x.MessageType), ": ", x.Message)));
            }

            return response;
        }
    }

    public class ResponseDto<T> : ResponseDto
    {
        public T? Data { get; set; }

        public ResponseDto() : base()
        {

        }

        public ResponseDto(T data) : base()
            => Data = data;

        public ResponseDto(T data, IEnumerable<ApplicationMessageDto> messages) : base(messages)
            => Data = data;

        public ResponseDto(T data, string message) : base(message)
            => Data = data;

        public ResponseDto(Exception ex) : base(ex)
        {

        }

        public void UpdateData(T data)
            => Data = data;
    }
}

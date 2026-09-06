using System.Net;
using __NAMESPACE__.Dto.Base;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace __NAMESPACE__.Apis.Exception
{
    public static class ExceptionApplicationBuilderExtensions
    {
        public static void UseCustomExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (contextFeature == null)
                        return;

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.ContentType = "application/json";

                    var errorResponse = new ResponseDto<ErrorResponseDto>(new ErrorResponseDto()
                    {
                        Identifier = Guid.NewGuid(),
                        StatusCode = context.Response.StatusCode,
                        Title = "Internal Server Error",
                        Message = contextFeature.Error.InnerException?.Message ?? contextFeature.Error.Message,
                        StackTrace = contextFeature.Error.InnerException?.StackTrace ?? contextFeature.Error.StackTrace
                    }, new List<ApplicationMessageDto>())
                    {
                        Messages = new List<ApplicationMessageDto>() {
                            new() { MessageType = ApplicationMessageType.Error, Message = contextFeature.Error.Message }
                        }
                    };

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }));
                });
            });
        }
    }
}

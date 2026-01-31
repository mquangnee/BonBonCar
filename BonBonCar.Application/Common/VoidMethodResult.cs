using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Refit;

namespace BonBonCar.Application.Common
{
    public class VoidMethodResult
    {
        private readonly List<ErrorResult> _errorMessages = new List<ErrorResult>();

        public IReadOnlyCollection<ErrorResult> ErrorMessages => _errorMessages;

        public bool IsOK => _errorMessages.Count == 0;

        public int? StatusCode { get; set; }

        private static IList<Error> GetErrors(string? fieldName = null, params object[]? errorValues)
        {
            return new List<Error>
        {
            new Error(fieldName, errorValues)
        };
        }

        private static IList<Error> GetErrors(params object[]? errorValues)
        {
            return new List<Error>
        {
            new Error(errorValues)
        };
        }

        public void AddError(ErrorResult? errorResult)
        {
            if (errorResult != null)
            {
                _errorMessages.Add(errorResult);
            }
        }

        public void AddError(IReadOnlyCollection<ErrorResult>? errorResults)
        {
            if (errorResults == null)
            {
                return;
            }

            foreach (ErrorResult errorResult in errorResults)
            {
                _errorMessages.Add(errorResult);
            }
        }

        public void AddError(string? errorCode, params Error[]? errors)
        {
            if (errors != null)
            {
                _errorMessages.Add(new ErrorResult
                {
                    ErrorCode = errorCode,
                    Errors = errors
                });
            }
        }

        public void AddError(ApiException? exception)
        {
            if (exception == null || exception.Content == null)
            {
                return;
            }

            JObject jObject = JObject.Parse(exception.Content);
            JToken jToken = jObject["statusCode"];
            JToken jToken2 = jObject["errorMessages"];
            if (jToken2 != null && jToken != null)
            {
                IList<ErrorResult> errorResults = jToken2.ToObject<IList<ErrorResult>>();
                if (int.TryParse(jToken.ToString(), out var result))
                {
                    AddError(result, errorResults);
                }
            }
        }

        public void AddErrorServer()
        {
            AddError(500, "ServerError");
        }

        public void AddError(string? errorCode)
        {
            _errorMessages.Add(new ErrorResult
            {
                ErrorCode = errorCode
            });
        }

        public void AddError(string? errorCode, params object[]? errorValues)
        {
            _errorMessages.Add(new ErrorResult
            {
                ErrorCode = errorCode,
                Errors = GetErrors(errorValues)
            });
        }

        public void AddError(string? errorCode, string? fieldName = null, params object[]? errorValues)
        {
            _errorMessages.Add(new ErrorResult
            {
                ErrorCode = errorCode,
                Errors = GetErrors(fieldName, errorValues)
            });
        }

        public void AddError(int statusCode, string? errorCode, string? fieldName, params object[]? errorValues)
        {
            StatusCode = statusCode;
            AddError(errorCode, fieldName, errorValues);
        }

        public void AddError(int statusCode, string? errorCode, string? fieldName, object? errorValue)
        {
            StatusCode = statusCode;
            if (errorValue == null)
            {
                AddError(errorCode, fieldName);
                return;
            }

            AddError(errorCode, fieldName, errorValue);
        }

        public void AddError(int? statusCode, string? errorCode, params Error[]? errors)
        {
            StatusCode = statusCode;
            AddError(errorCode, errors);
        }

        public void AddError(int? statusCode, IList<ErrorResult>? errorResults)
        {
            StatusCode = statusCode;
            if (errorResults == null)
            {
                return;
            }

            foreach (ErrorResult errorResult in errorResults)
            {
                _errorMessages.Add(errorResult);
            }
        }

        public void AddError(int? statusCode, IReadOnlyCollection<ErrorResult>? errorResults)
        {
            StatusCode = statusCode;
            if (errorResults == null)
            {
                return;
            }

            foreach (ErrorResult errorResult in errorResults)
            {
                _errorMessages.Add(errorResult);
            }
        }

        public void AddErrorBadRequest(IReadOnlyCollection<ErrorResult>? errorResults)
        {
            AddError(400, errorResults);
        }

        public void AddErrorBadRequest(string? errorCode, string? fieldName = null, params object[]? errorValues)
        {
            AddError(400, errorCode, fieldName, errorValues);
        }

        public void AddErrorBadRequest(string? errorCode, string? fieldName, object? errorValue)
        {
            if (errorValue == null)
            {
                AddError(400, errorCode, fieldName);
            }
            else
            {
                AddError(400, errorCode, fieldName, errorValue);
            }
        }

        public void AddErrorBadRequest(string? errorCode, params Error[]? errors)
        {
            AddError(400, errorCode, errors);
        }

        public virtual IActionResult GetActionResult()
        {
            ObjectResult objectResult = new ObjectResult(this);
            if (!StatusCode.HasValue)
            {
                if (IsOK)
                {
                    StatusCode = 200;
                }
                else
                {
                    StatusCode = 500;
                }
            }

            objectResult.StatusCode = StatusCode;
            return objectResult;
        }
    }
}

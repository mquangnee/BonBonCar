using Microsoft.AspNetCore.Mvc;

namespace BonBonCar.Application.Common
{
    public class MethodResult<T> : VoidMethodResult
    {
        public T? Result { get; set; }

        public MethodResult()
        {
        }

        public MethodResult(T? result)
        {
            Result = result;
        }

        public void AddResultFromErrorList(IEnumerable<ErrorResult>? errorMessages)
        {
            if (errorMessages == null)
            {
                return;
            }

            foreach (ErrorResult errorMessage in errorMessages)
            {
                AddError(errorMessage);
            }
        }

        public override IActionResult GetActionResult()
        {
            ObjectResult objectResult = new ObjectResult(this);
            if (!base.StatusCode.HasValue)
            {
                if (base.IsOK)
                {
                    base.StatusCode = 200;
                }
                else
                {
                    base.StatusCode = 500;
                }
            }

            objectResult.StatusCode = base.StatusCode;
            return objectResult;
        }
    }
}

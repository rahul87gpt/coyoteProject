using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coyote.Console.App.WebAPI.Helper
{
    /// <summary>
    /// CustomModelStateError
    /// </summary>
    public class CustomModelStateError : HttpAPIResponseModel
    {
        /// <summary>
        /// CustomModelStateError
        /// </summary>
        /// <param name="context"></param>
        public CustomModelStateError(ActionContext context)
        {
            ConstructErrorMessages(context ?? new ActionContext());
        }

        private void ConstructErrorMessages(ActionContext context)
        {
            List<string> Errors = new List<string>();
            foreach (var keyModelStatePair in context.ModelState)
            {
                //var key = keyModelStatePair.Key;
                var errors = keyModelStatePair.Value.Errors;

                if (errors != null && errors.Count > 0)
                {
                    if (errors[0].ErrorMessage.ToString().Contains("required", StringComparison.OrdinalIgnoreCase))
                    {
                        var errorMessage = GetErrorMessage(errors[0]);
                        Message = errorMessage;
                        break;
                    }
                    if (keyModelStatePair.Value.ValidationState.ToString() == "Invalid")
                    {
                        if (keyModelStatePair.ToString().Contains("$", StringComparison.OrdinalIgnoreCase))
                        {
                            var keyValue = keyModelStatePair.Key.ToString().Replace("$.", " ", StringComparison.OrdinalIgnoreCase);
                            Message = $"Invalid {keyValue}.";
                            break;
                        }
                        else
                        {
                            var errorMessage = GetErrorMessage(errors[0]);
                            Message = errorMessage;
                            break;
                        }
                        //var keyValue = keyModelStatePair.ToString().Contains("$", StringComparison.OrdinalIgnoreCase) ? keyModelStatePair.Key.ToString().Replace("$.", " ", StringComparison.OrdinalIgnoreCase) :$" {keyModelStatePair.Key.ToString()}. ";

                        // Message = $"Invalid value of{keyValue}.";
                        // break;
                    }
                    else
                    {
                        var errorMessage = GetErrorMessage(errors[0]);
                        Message = errorMessage;
                        break;
                    }
                    //if (errors.Count == 1)
                    //{

                    // Errors.Add(key, new[] { errorMessage });
                    //Errors.Add(errorMessage

                    //break loop to show only first encountered error

                    //}
                    //else
                    //{
                    //    var errorMessages = new string[errors.Count];
                    //    for (var i = 0; i < errors.Count; i++)
                    //    {
                    //        errorMessages[i] = GetErrorMessage(errors[i]);
                    //    }

                    //    //Errors.Add(key, errorMessages);
                    //    Errors.AddRange(errorMessages);
                    //}
                }
            }

            //Message = string.Join('|', Errors);
        }

        private static string GetErrorMessage(ModelError error)
        {
            return string.IsNullOrEmpty(error.ErrorMessage) ? "The input was not valid." : error.ErrorMessage;
        }

    }
}

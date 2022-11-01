namespace Coyote.Console.ViewModels.Tests.MockRepository
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public static class MockViewModelRepository
    {
        public static IList<ValidationResult> ModelValidationCheck(object model)
        {
            var result = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            Validator.TryValidateObject(model, validationContext, result);
            if (model is IValidatableObject)
            {
                (model as IValidatableObject).Validate(validationContext);
            }

            return result;
        }
        public static IList<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, results, true);
            if (model is IValidatableObject)
            {
                (model as IValidatableObject).Validate(validationContext);
            }

            return results;
        }


    }

}

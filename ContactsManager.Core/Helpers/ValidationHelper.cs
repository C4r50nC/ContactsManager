﻿using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.Helpers
{
    public class ValidationHelper
    {
        internal static void ModelValidation(object obj)
        {
            ValidationContext validationContext = new(obj);
            List<ValidationResult> validationResults = [];
            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);
            if (!isValid)
            {
                throw new ArgumentException(validationResults.FirstOrDefault()?.ErrorMessage);
            }

        }
    }
}

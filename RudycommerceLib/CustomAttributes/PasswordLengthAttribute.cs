using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceLib.CustomAttributes
{
    public class PasswordLengthAttribute : ValidationAttribute
    {
        private readonly string _passwordRequiredPropertyName;
        private readonly string _encryptedPasswordPropertyName;
        private readonly string _saltPropertyName;
        private readonly int _minimumLength;
        private readonly int _maximumLength;

        public PasswordLengthAttribute(string passwordRequiredPropertyName, string encryptedPasswordPropertyName, string saltPropertyName, int minLength = 0, int maxLength = 255) : base("Oopsie PasswordLengthnAttribute")
        {
            _passwordRequiredPropertyName = passwordRequiredPropertyName;
            _encryptedPasswordPropertyName = encryptedPasswordPropertyName;
            _saltPropertyName = saltPropertyName;

            if (minLength > maxLength)
            {
                int temp = minLength;
                minLength = maxLength;
                maxLength = temp;
            }

            _minimumLength = minLength;
            _maximumLength = maxLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                bool pwdRequired = (bool)GetValue(validationContext, _passwordRequiredPropertyName);

                if (pwdRequired)
                {
                    string salt = (string)GetValue(validationContext, _saltPropertyName);
                    string encryptedPwd = (string)GetValue(validationContext, _encryptedPasswordPropertyName);

                    if (salt == null && encryptedPwd == null)
                    {
                        if (String.IsNullOrWhiteSpace((string)value))
                        {
                            return new ValidationResult(String.Format(this.ErrorMessageString, _minimumLength, _maximumLength));
                        }
                        else
                        {
                            int pwdLength = ((string)value).Length;
                            if (!Utilities.IntExtensions.TestRange(pwdLength, _minimumLength, _maximumLength))
                            {
                                return new ValidationResult(String.Format(this.ErrorMessageString, _minimumLength, _maximumLength));

                            }
                        }
                    }
                }

                return ValidationResult.Success;
            }
            catch (Exception)
            {
                return new ValidationResult("PasswordLengthAttribute validation error");
            }
        }

        private object GetValue(ValidationContext validationContext, string propertyName)
        {
            return validationContext.ObjectInstance.GetType().GetProperty(propertyName).GetValue(validationContext.ObjectInstance, null);
        }
    }
}

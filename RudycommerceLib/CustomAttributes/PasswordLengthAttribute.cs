using RudycommerceLib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceLib.CustomAttributes
{
    /// <summary>
    /// Validates whether a Password is between the minimum and maximum length.
    /// </summary>
    public class PasswordLengthAttribute : ValidationAttribute
    {
        /// <summary>
        /// The name of the boolean Property that determines if the Password is required
        /// </summary>
        private readonly string _passwordRequiredPropertyName;
        /// <summary>
        /// The name of the Encrypted Password Property
        /// </summary>
        private readonly string _encryptedPasswordPropertyName;
        /// <summary>
        /// The name of the Salt Property
        /// </summary>
        private readonly string _saltPropertyName;
        /// <summary>
        /// The wanted minimum length for the Password
        /// </summary>
        private readonly int _minimumLength;
        /// <summary>
        /// The wanted maximum length for the Password
        /// </summary>
        private readonly int _maximumLength;

        public PasswordLengthAttribute(string passwordRequiredPropertyName, string encryptedPasswordPropertyName, string saltPropertyName, int minLength = 0, int maxLength = 255) : base("Error PasswordLengthAttribute")
        {
            _passwordRequiredPropertyName = passwordRequiredPropertyName;
            _encryptedPasswordPropertyName = encryptedPasswordPropertyName;
            _saltPropertyName = saltPropertyName;

            //If for some reason someone entered a minimum length smaller than the max length, swap the 2 values
            if (minLength > maxLength)
            {
                IntExtensions.SwapTwoIntegers(minLength, maxLength, out minLength, out maxLength);
            }

            _minimumLength = minLength;
            _maximumLength = maxLength;
        }

        /// <summary>
        /// Determines whether the State of the Property in the model is valid.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                // Gets the value of the property that determines if the password is required.
                bool pwdRequired = (bool)GetValue(validationContext, _passwordRequiredPropertyName);

                // If the password is required
                if (pwdRequired)
                {
                    // Gets the salt and encrypted password
                    string salt = (string)GetValue(validationContext, _saltPropertyName);
                    string encryptedPwd = (string)GetValue(validationContext, _encryptedPasswordPropertyName);

                    // If there is no encrypted password and salt yet, that means the user has not been created yet, 
                    // Which means the unencrypted password's length must be validated
                    if (salt == null && encryptedPwd == null)
                    {
                        // If the Password is not filled in
                        if (String.IsNullOrWhiteSpace((string)value))
                        {
                            // Return the inserted error message (the error message is inserted where the attribute is called)
                            return new ValidationResult(String.Format(this.ErrorMessageString, _minimumLength, _maximumLength));
                        }
                        else
                        {
                            // If the password is filled in, validate whether the password length is between the allowed min and max
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

        /// <summary>
        /// Gets the value for the Property with the given Property name
        /// </summary>
        /// <param name="validationContext"></param>
        /// <param name="propertyName">The name of the property you want to know the value of</param>
        /// <returns></returns>
        private object GetValue(ValidationContext validationContext, string propertyName)
        {
            return validationContext.ObjectInstance.GetType().GetProperty(propertyName).GetValue(validationContext.ObjectInstance, null);
        }
    }
}

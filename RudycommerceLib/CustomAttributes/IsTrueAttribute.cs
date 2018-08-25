using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RudycommerceLib.CustomAttributes
{
    // https://stackoverflow.com/questions/4730183/mvc-model-require-true
    /// <summary>
    /// Validation attribute that checks if the bool is true.
    /// (Can be used for example to easily validate if a user has selected the 'I Agree to the Terms and Conditions' checkbox)
    /// </summary>
    public class IsTrueAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return false;
            if (!(value is bool)) throw new InvalidOperationException("NO-ML Can only be used on a boolean property.");

            return (bool)value;
        }
    }
}

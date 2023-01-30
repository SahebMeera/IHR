using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace ILT.IHR.UI.Shared
{
    public class CustomInputSelect<TValue> : InputSelect<TValue>
    {
        protected override bool TryParseValueFromString(string value, out TValue result,
            out string validationErrorMessage)
        {
            if (typeof(TValue) == typeof(int))
            {
                if (int.TryParse(value, out var resultInt))
                {
                    result = (TValue)(object)resultInt;
                    validationErrorMessage = null;
                    return true;
                }
                else
                {
                    result = default;
                    validationErrorMessage =
                        $"The selected value {value} is not a valid number.";
                    return false;
                }
            }
            else
            {
                if (typeof(TValue) == typeof(string))
                {
                    if(int.TryParse(value, out var resultInt))
                    {
                        result = (TValue)(object)resultInt.ToString();
                        validationErrorMessage = null;
                        return true;
                    }                    
                }
                else
                {
                    if (int.TryParse(value, out var resultInt))
                    {
                        result = (TValue)(object)resultInt;
                        validationErrorMessage = null;
                        return true;
                    }
                }
                return base.TryParseValueFromString(value, out result,
                    out validationErrorMessage);
            }
        }
    }
}

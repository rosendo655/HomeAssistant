using Jint;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace IoTService.Services
{
    public enum EvaluationResult
    {
        SUCCESS,
        NO_CONTAINS_VAL,
        INVALID,
        NOT_BOOLEAN,
    }
    public class JintService
    {
        public static EvaluationResult TestExpression(string expression)
        {
            try
            {
                if (!Regex.IsMatch(expression, ".*val.*"))
                {
                    return EvaluationResult.NO_CONTAINS_VAL;
                }

                var result = new Engine().SetValue("val", 0.0).Execute(expression).GetCompletionValue();

                if(!result.IsBoolean())
                {
                    return EvaluationResult.NOT_BOOLEAN;
                }

                return EvaluationResult.SUCCESS;
            }
            catch(Exception ex)
            {
                return EvaluationResult.INVALID;
            }
            
        }

        public static bool EvaluateExpression(string expression , float value)
        {
            try
            {
                var result = new Engine().SetValue("val", value).Execute(expression).GetCompletionValue();

                return result.IsBoolean() ? result.AsBoolean(): false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}

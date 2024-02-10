﻿using BettyLang.Core.AST;

namespace BettyLang.Core.Interpreter.IntrinsicFunctions
{
    public class ConvertToBooleanFunction : IIntrinsicFunction
    {
        public InterpreterValue Invoke(FunctionCall call, IExpressionVisitor visitor)
        {
            if (call.Arguments.Count != 1)
            {
                throw new ArgumentException("to_bool function requires exactly one argument.");
            }

            var argResult = call.Arguments[0].Accept(visitor);

            bool booleanValue;
            switch (argResult.Type)
            {
                case ValueType.Number:
                    // Any number other than 0 is true, 0 is false
                    booleanValue = argResult.AsNumber() != 0;
                    break;

                case ValueType.String:
                    // Consider non-empty strings as true, and optionally parse "true" and "false"
                    var str = argResult.AsString();
                    if (bool.TryParse(str, out bool parsedValue))
                    {
                        // Successfully parsed "true" or "false"
                        booleanValue = parsedValue;
                    }
                    else
                    {
                        // Any non-empty string is considered true, empty string false
                        booleanValue = !string.IsNullOrEmpty(str);
                    }
                    break;

                case ValueType.Boolean:
                    // Return the boolean value directly
                    return argResult;

                default:
                    throw new InvalidOperationException($"Conversion to boolean not supported for type {argResult.Type}.");
            }

            return InterpreterValue.FromBoolean(booleanValue);
        }
    }
}
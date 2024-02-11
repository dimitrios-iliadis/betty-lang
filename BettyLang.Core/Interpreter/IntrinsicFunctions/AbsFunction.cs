﻿using BettyLang.Core.AST;

namespace BettyLang.Core.Interpreter
{
    public static partial class IntrinsicFunctions
    {
        public static InterpreterValue AbsFunction(FunctionCall call, IExpressionVisitor visitor)
        {
            if (call.Arguments.Count != 1)
            {
                throw new ArgumentException($"{call.FunctionName} function requires exactly one numeric argument.");
            }

            var argValue = call.Arguments[0].Accept(visitor);
            if (argValue.Type != ValueType.Number)
            {
                throw new ArgumentException($"Argument for {call.FunctionName} must be a number.");
            }

            double result = Math.Abs(argValue.AsNumber());
            return InterpreterValue.FromNumber(result);
        }
    }
}
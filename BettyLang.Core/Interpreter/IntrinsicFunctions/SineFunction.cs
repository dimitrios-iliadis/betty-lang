﻿using BettyLang.Core.AST;

namespace BettyLang.Core.Interpreter.IntrinsicFunctions
{
    public class SineFunction : IIntrinsicFunction
    {
        public InterpreterValue Invoke(FunctionCall call, IExpressionVisitor visitor)
        {
            if (call.Arguments.Count != 1)
            {
                throw new ArgumentException("sin function requires exactly one numeric argument.");
            }

            var argValue = call.Arguments[0].Accept(visitor);
            if (argValue.Type != ValueType.Number)
            {
                throw new ArgumentException("Argument for sin must be a number.");
            }

            double result = Math.Sin(argValue.AsNumber());
            return InterpreterValue.FromNumber(result);
        }
    }
}
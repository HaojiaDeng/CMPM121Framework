using System;
using System.Collections.Generic;
using UnityEngine; // For Mathf.FloorToInt

public static class RPN
{
    public static int Evaluate(string expression, int wave, int baseValue)
    {
        if (string.IsNullOrEmpty(expression))
        {
            return baseValue; // Return base value if expression is empty or null
        }

        Stack<int> stack = new Stack<int>();
        string[] tokens = expression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string token in tokens)
        {
            if (int.TryParse(token, out int number))
            {
                stack.Push(number);
            }
            else if (token.Equals("wave", StringComparison.OrdinalIgnoreCase))
            {
                stack.Push(wave);
            }
            else if (token.Equals("base", StringComparison.OrdinalIgnoreCase))
            {
                stack.Push(baseValue);
            }
            else
            {
                if (stack.Count < 2) throw new ArgumentException($"Invalid RPN expression: '{expression}' - Operator '{token}' needs two operands, stack size is {stack.Count}.");
                int operand2 = stack.Pop();
                int operand1 = stack.Pop();

                switch (token)
                {
                    case "+":
                        stack.Push(operand1 + operand2);
                        break;
                    case "-":
                        stack.Push(operand1 - operand2);
                        break;
                    case "*":
                        stack.Push(operand1 * operand2);
                        break;
                    case "/":
                        if (operand2 == 0) throw new DivideByZeroException($"Invalid RPN expression: '{expression}' - Division by zero.");
                        stack.Push(Mathf.FloorToInt((float)operand1 / operand2)); // Use integer division
                        break;
                    case "%":
                         if (operand2 == 0) throw new DivideByZeroException($"Invalid RPN expression: '{expression}' - Modulo by zero.");
                         stack.Push(operand1 % operand2);
                         break;
                    default:
                        throw new ArgumentException($"Invalid RPN expression: '{expression}' - Unknown token '{token}'.");
                }
            }
        }

        if (stack.Count != 1) throw new ArgumentException($"Invalid RPN expression: '{expression}' - Expression did not resolve to a single value. Stack count: {stack.Count}.");
        return stack.Peek();
    }
}

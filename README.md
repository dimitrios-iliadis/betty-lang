# Betty

Betty is an interpreted, dynamic programming language designed for educational purposes, written in C#.

## Features

- **Simple Syntax:** Designed for readability and ease of use.
- **Standard Library:** Comes with a minimal standard library for tasks such as math and basic I/O operations.

## Getting Started

### Quick Start

Here is a simple program in Betty:

```C
// This program prompts the user to enter a number
// and then prints its factorial using recursion.

func fact(n)
{
    if (n == 1) return 1;
    return fact(n - 1) * n;
}

// The entry point for the program
func main()
{
    n = input("Enter a number: ");
    print("The factorial of " + str(n) + "is " + str(fact(n)));
}

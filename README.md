# Betty

Betty is an interpreted, dynamically-typed programming language designed for educational purposes, written in C#. At its core, Betty offers some of the adaptability of dynamic typing, allowing variables to be reassigned with different types seamlessly. Unlike traditional dynamically-typed languages, however, Betty introduces a rigorous type-checking system that eschews implicit type conversions, demanding explicit casting and operations to maintain type integrity. This design philosophy ensures that while developers can enjoy the flexibility in variable usage, they are also guided by a strong, type-safe environment that minimizes runtime errors and enhances code readability. Betty bridges the gap between the freedom of dynamic typing and the security of static type systems, making it an ideal choice for developers seeking to combine rapid development with code reliability.

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
    n = tonum(input("Enter a number: "));
    print("The factorial of", n, " is ", fact(n));
}

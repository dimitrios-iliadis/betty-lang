# Betty

Betty is a dynamically-typed, educational programming language built with C#. It merges the adaptability of dynamic typing with rigorous type safety, designed to facilitate both learning and efficient coding.

## Key Features

- **Dynamic Typing**: Easily change the type of data a variable holds, allowing for versatile programming approaches.
- **Rigorous Type Safety**: Initially emphasizing explicit casting and operations to maintain type integrity, Betty ensures code reliability and minimizes runtime errors.
- **User-Friendly Evolution**: Betty now incorporates more implicit operations and conventions, streamlining the development process while retaining its commitment to type safety.
- **Ideal for Learning**: With its balance between flexibility and safety, Betty provides a supportive environment for new and experienced developers aiming to deepen their understanding of programming concepts.

## Getting Started

To begin with Betty, clone this repository and follow the setup instructions provided in the documentation. Betty is designed for easy installation and quick start.

```bash
git clone https://github.com/yourusername/betty-lang.git
cd betty-lang
# Follow the setup instructions in the documentation
```

## Examples

Below are some examples to help you get acquainted with Betty's syntax and features:

### Example 1: Basic Function and Variable Increment

Demonstrates how to define a function and use post-increment and pre-increment operations.

```Python
func increment()
{
    count = 0;
    count = count++ + ++count; # Post-increment and pre-increment
    print(count); # Should output 2, demonstrating how increment operations work
}

func main()
{
    increment();
}
```

### Example 2: Implicit Type Conversion and Arithmetic Operations

Shows implicit type conversion from character to ASCII value and arithmetic operations.

```Python
func charArithmetic()
{
    charValue = 'a';
    charValue += 1; # Implicitly converts 'a' to its ASCII value, then increments
    print(charValue); # Outputs the number corresponding to 'b' in ASCII

    numberValue = 10;
    numberValue += '3'; # '3' is implicitly converted to its numeric ASCII value before addition
    print(numberValue); # Outputs the result of the addition
}

func main()
{
    charArithmetic();
}
```

### Example 3: Using Comments and Integer Division Shorthand ###

Illustrates the use of integer division shorthand and how to include comments.

```Python
func divisionExample()
{
    x = 10;
    y = 20;
    y //= 2; # Integer division shorthand
    print(y); # Outputs 10

    # This is a comment explaining that the next line prints the value of x
    print(x); # Outputs 10
}

func main()
{
    divisionExample();
}
```

### Example 4: Character Increment and Conversion ###

This example demonstrates character increment and explicit conversion to character type.

```Python
func charIncrement()
{
    x = 'a';
    x = tochar(++x); # Increment 'a' to 'b', then convert explicitly to char
    print(x); # Prints 'b'
}

func main()
{
    charIncrement();
}
```

### Example 5: Control Structures and Recursion

A simple demonstration of recursion and the if control structure to calculate a factorial.

```Python
func factorial(n)
{
    if (n == 1)
    {
        return 1;
    }
    else
    {
        return n * factorial(n - 1); # Recursive call
    }
}

func main()
{
    result = factorial(5);
    print(result); # Should print 120, the factorial of 5
}
```

### Example 6: Loop and Conditionals

Showcases loops (`while`, `for`) and conditional (`if-else`) statements.

```Python
func loopExamples()
{
    # While loop example
    i = 0;
    while (i < 5)
    {
        print(i);
        i++;
    }

    # For loop example
    for (j = 0; j < 5; j++)
    {
        print(j);
    }

    # If-else example
    if (i == 5)
    {
        print("Loop completed with while");
    }
    else
    {
        print("Unexpected value");
    }
}

func main()
{
    loopExamples();
}
```

We hope these examples help you get a good start with Betty. For more detailed documentation and advanced features, please refer to the official Betty programming language guide.

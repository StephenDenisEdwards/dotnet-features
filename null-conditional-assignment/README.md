# Null-Conditional Assignment

Investigation of the C# 14 null-conditional assignment feature (`?.` on the left-hand side of assignments).

This feature allows writing `obj?.Property = value;` instead of the traditional `if (obj != null) obj.Property = value;` pattern, including nested cases like `obj?.Inner?.Property = value;`.

## IL Comparison

IL comparison between the traditional null-check pattern and the new null-conditional assignment will be added.

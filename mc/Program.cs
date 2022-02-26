// See https://aka.ms/new-console-template for more information
while (true)
{
    Console.Write("> ");
    var line = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(line))
    {
        return;
    }

    var parser = new Parser(line);
    var expression = parser.Parse();

    var color = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.DarkGray;
    PrettyPrint(expression);
    Console.ForegroundColor = color;
}

static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
{
    // └── ├──  │

    var marker = isLast ? "└──" : "├──";
    Console.Write(indent);
    Console.Write(marker);
    Console.Write(node.Kind);

    if (node is SyntaxToken t && t.Value is not null)
    {
        Console.Write(" ");
        Console.Write(t.Value);
    }

    Console.WriteLine();

    indent += isLast ? "    " : "│   ";
    var lastChild = node.GetChildren().LastOrDefault();

    foreach (var child in node.GetChildren())
    {
        PrettyPrint(child, indent, child == lastChild);
    }
}

void TestLexer(string line)
{
    var lexer = new Lexer(line);
    while (true)
    {
        var token = lexer.NextToken();
        if (token.Kind == SyntaxKind.EndOfFileToken)
        {
            break;
        }
        Console.Write($"{token.Kind}: '{token.Text}'");
        if (token.Value is not null)
        {
            Console.Write($" {token.Value}");
        }
        Console.WriteLine();
    }
}
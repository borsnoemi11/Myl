using Minsk.CodeAnalysis;
using Minsk.CodeAnalysis.Syntax;

var showTree = false;
var variables = new Dictionary<VariableSymbol, object>();

while (true)
{
    Console.Write("> ");
    var line = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(line))
    {
        return;
    }

    if (line == "#showTree")
    {
        showTree = !showTree;
        Console.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees.");
        continue;
    }
    else if (line == "#clf")
    {
        Console.Clear();
        continue;
    }

    var syntaxTree = SyntaxTree.Parse(line);
    var compilation = new Compilation(syntaxTree);
    var result = compilation.Evaluate(variables);

    var diagnostics = result.Diagnostics;

    if (showTree)
    {
        RunWithColoredConsole(() => PrettyPrint(syntaxTree.Root), ConsoleColor.DarkGray);
    }

    if (!diagnostics.Any())
    {
        Console.WriteLine(result.Value);
    }
    else
    {
        foreach (var diagnostic in diagnostics)
        {
            Console.WriteLine();

            RunWithColoredConsole(() => { Console.WriteLine(diagnostic); }, ConsoleColor.DarkRed);
            
            var prefix = line.Substring(0, diagnostic.Span.Start);
            var error = line.Substring(diagnostic.Span.Start, diagnostic.Span.Length);
            var suffix = line.Substring(diagnostic.Span.End);

            Console.Write("    ");
            Console.Write(prefix);

            RunWithColoredConsole(() => { Console.Write(error); }, ConsoleColor.DarkRed);

            Console.Write(suffix);
            Console.WriteLine();
        }

        Console.WriteLine();
    }
}

void RunWithColoredConsole(Action action, ConsoleColor color)
{
    try
    {
        Console.ForegroundColor = color;
        action();
    }
    finally
    {
        Console.ResetColor();
    }   
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

    indent += isLast ? "   " : "│  ";
    var lastChild = node.GetChildren().LastOrDefault();

    foreach (var child in node.GetChildren())
    {
        PrettyPrint(child, indent, child == lastChild);
    }
}
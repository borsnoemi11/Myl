using System.Text;
using Minsk.CodeAnalysis;
using Minsk.CodeAnalysis.Syntax;
using Minsk.CodeAnalysis.Text;

var showTree = false;
var variables = new Dictionary<VariableSymbol, object>();
var textBuilder = new StringBuilder();

while (true)
{
    RunWithColoredConsole(() => 
        {
            if (textBuilder.Length == 0)
                Console.Write("» ");
            else
                Console.Write(". ");
        }, ConsoleColor.Green);
    
    var input = Console.ReadLine();
    var isBlank = string.IsNullOrWhiteSpace(input);

    if (textBuilder.Length == 0)
    {
        if (isBlank)
        {
            break;
        }
        else if (input == "#showTree")
        {
            showTree = !showTree;
            Console.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees.");
            continue;
        }
        else if (input == "#clf")
        {
            Console.Clear();
            continue;
        }
    }

    textBuilder.AppendLine(input);
    var text = textBuilder.ToString();

    var syntaxTree = SyntaxTree.Parse(text);
    if (!isBlank && syntaxTree.Diagnostics.Any())
        continue;

    var compilation = new Compilation(syntaxTree);
    var result = compilation.Evaluate(variables);

    var diagnostics = result.Diagnostics;

    if (showTree)
    {
        RunWithColoredConsole(() => syntaxTree.Root.WriteTo(Console.Out), ConsoleColor.DarkGray);
    }

    if (!diagnostics.Any())
    {
        RunWithColoredConsole(() => Console.WriteLine(result.Value), ConsoleColor.Magenta);
    }
    else
    {
        foreach (var diagnostic in diagnostics)
        {
            var lineIndex = syntaxTree.Text.GetLineIndex(diagnostic.Span.Start);
            var line = syntaxTree.Text.Lines[lineIndex];
            var lineNumber = lineIndex + 1;
            var characterIndexInLine = diagnostic.Span.Start - line.Start + 1;

            Console.WriteLine();

            RunWithColoredConsole(() => 
                { 
                    Console.Write($"({lineNumber}, {characterIndexInLine}): "); 
                    Console.WriteLine(diagnostic);
                }, ConsoleColor.DarkRed);
            
            var prefixSpan = TextSpan.FromBounds(line.Start, diagnostic.Span.Start);
            var suffixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.End);

            var prefix = syntaxTree.Text.ToString(prefixSpan);
            var error = syntaxTree.Text.ToString(diagnostic.Span);
            var suffix = syntaxTree.Text.ToString(suffixSpan);

            Console.Write("    ");
            Console.Write(prefix);

            RunWithColoredConsole(() => { Console.Write(error); }, ConsoleColor.DarkRed);

            Console.Write(suffix);
            Console.WriteLine();
        }

        Console.WriteLine();
    }

    textBuilder.Clear();
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
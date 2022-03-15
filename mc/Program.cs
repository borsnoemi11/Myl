﻿using Minsk.CodeAnalysis;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Syntax;

var showTree = false;
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
    var binder = new Binder();
    var boundExpression = binder.BindExpression(syntaxTree.Root);
    var diagnostics = syntaxTree.Diagnostics.Concat(binder.Diagnostics).ToArray();

    if (showTree)
    {
        RunWithColoredConsole(() => PrettyPrint(syntaxTree.Root), ConsoleColor.DarkGray);
    }

    if (!diagnostics.Any())
    {
        var e = new Evaluator(boundExpression);
        var result = e.Evaluate();
        Console.WriteLine(result);
    }
    else
    {
        RunWithColoredConsole(() =>
        {
            foreach (var diagnostic in diagnostics)
            {
                Console.WriteLine(diagnostic);
            }
        }, ConsoleColor.DarkRed);
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
﻿using GBX.NET;
using GBX.NET.Exceptions;
using Microsoft.Extensions.Logging;

if (args.Length == 0)
    return;

var fileName = args[0];

var logger = LoggerFactory.Create(builder =>
{
    builder.AddSimpleConsole(options =>
    {
        options.IncludeScopes = true;
        options.SingleLine = true;
    });
    builder.SetMinimumLevel(LogLevel.Debug);
}).CreateLogger<Program>();

var chunkTxt = File.ReadAllText("Chunk.txt");
var classTxt = File.ReadAllText("Node.txt");

while (true)
{
    try
    {
        var gbx = GameBox.ParseNode(fileName, logger: logger);
    }
    catch (ChunkParseException ex)
    {
        if (ex.ClassName != "unknown class")
        {
            var splitClassName = ex.ClassName.Split("::");
            var className = splitClassName[1];

            var objs = new string[]
            {
                (ex.ChunkId & 0xFFF).ToString("X3"),
                className,
                ex.ChunkId.ToString("X8")
            };

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Possible chunk class structure:\n\n" + string.Format(chunkTxt, objs));
            Console.WriteLine();
            Console.WriteLine();
        }
        
        logger.LogError(ex, "Exception during parse: ");
    }
    catch (NodeNotImplementedException ex)
    {
        if (ex.ClassName != "unknown class")
        {
            var splitClassName = ex.ClassName.Split("::");
            var engineName = splitClassName[0];
            var className = splitClassName[1];

            var objs = new string[]
            {
            engineName,
            ex.ClassId.ToString("X8"),
            className
            };

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Possible class structure:\n\n" + string.Format(classTxt, objs));
            Console.WriteLine();
            Console.WriteLine();
        }
        
        logger.LogError(ex, "Exception during parse: ");
    }

    Console.WriteLine();
    Console.ReadKey(true);
}
using Graphviz4Net.Dot;
using Graphviz4Net.Graphs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FacebookGroupMemberGrapher
{
    class Program
    {
        static readonly Regex ADDED_REGEX = new Regex("^Added by (.*) (Today|Yesterday|on (.*))$");
        static readonly Regex JOINED_REGEX = new Regex("^Joined (.*)$");
        static List<string> AllNames = new List<string>(); // All names ever encountered; used to know when to create vertices
        static Dictionary<string, int> NameNumber = new Dictionary<string, int>();
        const string OUTPUT_EXTENSION = "dot";
        const string MESSAGE_LITERAL = "Message";

        static int Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            if (args.Length != 2)
            {
                Console.Error.WriteLine("Please provide exactly two parameters -- the Facebook name of the founder and the filename to analyze.");
                return 1;
            }

            var fileName = args[1];
            if (!File.Exists(fileName))
            {
                Console.Error.WriteLine("File " + fileName + " was not found.");
                return 2;
            }
            Console.WriteLine("Processing " + fileName);

            var groupFounder = args[0];

            var graph = ProcessFile(fileName, groupFounder);
            var outputFilename = Path.ChangeExtension(fileName, OUTPUT_EXTENSION);
            var writer = File.CreateText(outputFilename);
            new GraphToDotConverter().Convert(writer, graph, new AttributesProvider());
            writer.Close();
            Console.WriteLine("Output written to " + outputFilename);

            return 0;
        }
        private static Graph<string> ProcessFile(string fileName, string groupFounder)
        {
            var graph = new Graph<string>();
            var fp = File.OpenText(fileName);
            int i = 0;
            while (!fp.EndOfStream)
            {
                var name = ReadLine(fp);
                if (!NameNumber.ContainsKey(name))
                {
                    NameNumber.Add(name, 1);
                }
                else
                {
                    Console.Error.WriteLine($"WARNING! Duplicate name: {name}");
                    /*
                     * Unfortunately, Facebook doesn't allow us to distinguish between multiple
                     * people with the same name in this list, even if we were to parse the
                     * page's HTML – we could distinguish between people when they are added
                     * (because Facebook displays their image and link), but we could NOT
                     * distinguish between them when they add someone (because then it only shows the
                     * person's name, nothing else).
                     * 
                     * So we do the best we can: we just add another node for this person, and we're
                     * done with it.
                     */

                    NameNumber[name]++;
                    name = name + "@" + NameNumber[name];
                }
                if (!AllNames.Contains(name))
                {
                    graph.AddVertex(name);
                    AllNames.Add(name);
                }

                Console.Write($"{++i}. {name}");
                while (true)
                {
                    var added = ReadLine(fp);
                    var addedMatch = ADDED_REGEX.Match(added);
                    if (addedMatch.Success)
                    {
                        var addedBy = addedMatch.Groups[1].Value;
                        Console.WriteLine(" was added by " + addedBy);
                        if (!AllNames.Contains(addedBy))
                        {
                            graph.AddVertex(addedBy);
                            AllNames.Add(addedBy);
                        }
                        graph.AddEdge(new Edge<string>(addedBy, name));
                        break;
                    }

                    var joinedMatch = JOINED_REGEX.Match(added);
                    if (joinedMatch.Success)
                    {
                        Console.WriteLine(" has joined");
                        break;
                    }
                }

                while (true)
                {
                    var message = ReadLine(fp);
                    if (MESSAGE_LITERAL.Equals(message))
                    {
                        break;
                    }

                    if (groupFounder.Equals(message))
                    {
                        // We reached the end of the list, the founder is always last
                        Console.WriteLine($"{++i}. {groupFounder} was here first.");
                        return graph;
                    }
                }
            }

            return graph;
        }

        private static string ReadLine(StreamReader fp)
        {
            while (true)
            {
                var line = fp.ReadLine().Trim();
                if (line.Length == 0)
                {
                    continue;
                }
                return line;
            }
        }
    }

    internal class AttributesProvider : IAttributesProvider
    {
        IDictionary<string, string> IAttributesProvider.GetVertexAttributes(object vertex)
        {
            return new Dictionary<string, string>()
            {
                {  "label", vertex as string }
            };
        }
    }

}

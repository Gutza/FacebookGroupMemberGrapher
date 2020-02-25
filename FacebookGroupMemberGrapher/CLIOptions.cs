using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace FacebookGroupMemberGrapher
{
    class CLIOptions
    {
        [Option('a', "anonymize", Required = false, HelpText = "Anonymize the group members' names. Useful if you want to share the DOT file.", Default = false)]
        public bool Anonymize { get; set; }

        [Value(0, Required = true, HelpText = "The file to process.", MetaName = "filename")]
        public string Filename { get; set; }
    }
}

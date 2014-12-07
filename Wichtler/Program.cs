using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Wichtler.Configuration.CommandLine;
using Wichtler.Configuration.Files;

namespace Wichtler
{
    class Program
    {
        static void Main(string[] args)
        {
            var commandLineParser = new CommandLineParser();
            Arguments arguments;
            if (!commandLineParser.TryParse<Arguments>(args, out arguments))
                Abort(commandLineParser.GetSyntax<Arguments>());

            if (!File.Exists(arguments.InputFile))
                Abort("Input file does not exist");

            if (!string.IsNullOrEmpty(arguments.PreviousYear) && !File.Exists(arguments.PreviousYear))
                Abort("Previous year import file does not exist.");


            IList<Person> people;
            var csvParser = new CsvParser();
            if (!csvParser.TryParse<Person>(arguments.InputFile, out people))
                Abort("Input file not in correct format.");

            if (File.Exists(arguments.PreviousYear))
            {
                if (!TryParsePreviousYearAssignments(File.ReadAllLines(arguments.PreviousYear), people))
                    Abort("Previous year import failed.");
            }

            if (!string.IsNullOrEmpty(arguments.SmtpServer)
                && !File.Exists(arguments.EmailTemplateFile))
                Abort("You need to specify an Email template");

            var stopwatch = Stopwatch.StartNew();
            IList<Person> shuffled;
            while (true)
            {
                shuffled = people.Shuffle();
                Console.Write(".");
                if (IsValid(people, shuffled))
                    break;
                if (stopwatch.Elapsed > TimeSpan.FromSeconds(arguments.CalculationTime))
                    Abort("Cound not find a solution in the allotted time. Try increasing CalculationTime");
                Console.Write(".");
            }

            var assignments = new Dictionary<Person, Person>();
            for (int i = 0; i < people.Count; i++)
                assignments.Add(people[i], shuffled[i]);

            Console.WriteLine("\nAssignment completed.");

            int current = 0;
            if (!string.IsNullOrEmpty(arguments.SmtpServer))
            {
                // Read email template
                var templateText = File.ReadAllText(arguments.EmailTemplateFile);
                var subject = templateText.Substring(0, templateText.IndexOf(Environment.NewLine));
                var body = templateText.Substring(templateText.IndexOf(Environment.NewLine));

                foreach (var assignment in assignments)
                {
                    MailMessage message = new MailMessage(new MailAddress("jens@ameskamp.de"), new MailAddress(assignment.Key.Email))
                    {
                        Subject = string.Format(subject, assignment.Key.Name),
                        Body = string.Format(body, assignment.Key.Name, assignment.Value.FullName),
                    };

                    var credentials = new NetworkCredential(arguments.SmtpUsername, arguments.SmtpPassword);
                    var client = new SmtpClient(arguments.SmtpServer)
                    {
                        Credentials = credentials,
                        EnableSsl = true,
                    };

                    try
                    {
                        client.Send(message);
                    }
                    catch (SmtpException ex)
                    {
                        string error = ex.Message;
                        if (ex.InnerException != null)
                            error += "\n" + ex.InnerException.Message;
                        Abort(error);
                    }

                    Console.Write("\rSent Email {0} of {1}.", ++current, assignments.Count);
                }
                Console.WriteLine();
            }

            if (!string.IsNullOrEmpty(arguments.OutputFile))
            {
                using (var writer = new StreamWriter(arguments.OutputFile))
                    foreach (var assignment in assignments)
                    {
                        writer.WriteLine("{0} ({1}) -> {2} ({3})", assignment.Key.FullName, assignment.Key.Email, assignment.Value.FullName, assignment.Value.Email);
                    }
                Console.WriteLine("Wrote {0} assignments to output file.", assignments.Count);
            }

            if (string.IsNullOrEmpty(arguments.OutputFile) && string.IsNullOrEmpty(arguments.SmtpServer))
                for (int i = 0; i < people.Count; i++)
                    Console.WriteLine("{0} -> {1}", people[i].FullName, shuffled[i].FullName);

#if DEBUG
            Console.ReadLine();
#endif
        }

        private static bool TryParsePreviousYearAssignments(string[] assignmentLines, IList<Person> people)
        {
            foreach (var line in assignmentLines)
            {
                var match = Regex.Match(line, @"^([^\(]*) \(([^\)]*)\) -> ([^\(]*) \(([^\)]*)\)$");
                if (!match.Success)
                {
                    Abort(string.Format("Invalid line in previous year file: {0}", line));
                    return false;
                }
                string fromName = match.Groups[1].Value;
                string fromEmail = match.Groups[2].Value;
                string toName = match.Groups[3].Value;
                string toEmail = match.Groups[4].Value;

                Person from = people.SingleOrDefault(p => p.FullName == fromName && p.Email == fromEmail);
                Person to = people.SingleOrDefault(p => p.FullName == toName && p.Email == toEmail);

                if (from == null)
                {
                    Console.WriteLine("Unknown Wichtel: {0} ({1})", fromName, fromEmail);
                    continue;
                }
                if (to == null)
                {
                    Console.WriteLine("Unknown Wichtelee: {0} ({1})", toName, toEmail);
                    continue;
                }

                from.ForbiddenAssignments.Add(to);
            }
            return true;
        }

        private static bool IsValid(IList<Person> people, IList<Person> assignments)
        {
            for (int i = 0; i < people.Count; i++)
            {
                if (people[i].Family == assignments[i].Family)
                    return false;
                if (people[i].ForbiddenAssignments.Contains(assignments[i]))
                    return false;
            }
            return true;
        }

        private static void Abort(string message)
        {
            Console.WriteLine(message);
#if DEBUG
            Console.ReadLine();
#endif
            Environment.Exit(1);
        }

        private static IEnumerable<Person> ParseInputFile(string file)
        {
            if (!File.Exists(file))
                yield break;
        }
    }
}

namespace X12Explorer.Core;

/// <summary>
/// The command line that de-identifies files instead of opening the window:
/// <c>X12Explorer --deidentify INPUT OUTPUT</c>. INPUT is a file or a folder; for a folder, every X12 file in it and
/// its subfolders is written under OUTPUT with the same relative path, all with one key, so the stand-ins match from
/// file to file. It never writes over a file that exists.
/// </summary>
public static class DeidentifyCommand
{
    public static bool Wanted(string[] args) => args.Length > 0 && args[0].Equals("--deidentify", StringComparison.OrdinalIgnoreCase);

    /// <summary>Runs the command; returns the exit code (0 done, 1 a file failed, 2 bad arguments).</summary>
    public static int Run(string[] args, TextWriter output, TextWriter errors, Deidentifier? deidentifier = null)
    {
        if (args.Length != 3)
        {
            errors.WriteLine("Usage: X12Explorer --deidentify INPUT OUTPUT   (INPUT and OUTPUT both files, or both folders)");
            return 2;
        }
        var input = Path.GetFullPath(args[1]);
        var target = Path.GetFullPath(args[2]);
        deidentifier ??= new Deidentifier();

        List<(string From, string To)> work;
        if (Directory.Exists(input))
        {
            if (File.Exists(target))
            {
                errors.WriteLine($"{args[2]} is a file; to de-identify a folder, give a folder to write into.");
                return 2;
            }
            var inside = Path.TrimEndingDirectorySeparator(target).StartsWith(Path.TrimEndingDirectorySeparator(input) + Path.DirectorySeparatorChar, StringComparison.Ordinal);
            work = Directory.EnumerateFiles(input, "*", new EnumerationOptions { RecurseSubdirectories = true, IgnoreInaccessible = true })
                .Where(f => !(inside && f.StartsWith(target + Path.DirectorySeparatorChar, StringComparison.Ordinal)))
                .Where(X12File.LooksLikeX12)
                .Select(f => (f, Path.Combine(target, Path.GetRelativePath(input, f))))
                .ToList();
            if (work.Count == 0)
            {
                errors.WriteLine($"There are no X12 files in {args[1]}.");
                return 1;
            }
        }
        else if (File.Exists(input))
        {
            if (Directory.Exists(target)) target = Path.Combine(target, Path.GetFileName(input));
            work = [(input, target)];
        }
        else
        {
            errors.WriteLine($"There's no file or folder {args[1]}.");
            return 2;
        }

        var failed = 0;
        foreach (var (from, to) in work)
        {
            if (File.Exists(to))
            {
                errors.WriteLine($"{to} exists; left it alone.");
                failed++;
                continue;
            }
            try
            {
                var result = deidentifier.Load(from);
                Directory.CreateDirectory(Path.GetDirectoryName(to)!);
                File.WriteAllText(to, result.Text);
                output.WriteLine($"{to}: {result.Summary}");
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                errors.WriteLine($"{from}: {e.Message}");
                failed++;
            }
        }
        output.WriteLine($"Dates moved back {-deidentifier.ShiftDays} days. Check the results before sharing them.");
        return failed == 0 ? 0 : 1;
    }
}

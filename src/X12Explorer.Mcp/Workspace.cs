using X12Explorer.Core;

namespace X12Explorer.Mcp;

/// <summary>
/// The folder an agent may read X12 files from. Every path an agent gives is taken relative to it, and anything that
/// resolves outside it (through "..", an absolute path or a link) is refused. The server never writes.
/// </summary>
public sealed class Workspace
{
    /// <summary>The most files <see cref="List"/> returns.</summary>
    public const int MaxListed = 500;

    public Workspace(string root)
    {
        Root = Path.TrimEndingDirectorySeparator(Path.GetFullPath(root));
        if (!Directory.Exists(Root)) throw new DirectoryNotFoundException($"The folder {Root} doesn't exist.");
    }

    public string Root { get; }

    /// <summary>The full path of <paramref name="file"/>, which must be inside the root and exist.</summary>
    public string Resolve(string file)
    {
        if (string.IsNullOrWhiteSpace(file)) throw new ArgumentException("Name a file, relative to the folder (list_files shows them).");
        var full = Path.GetFullPath(Path.Combine(Root, file.Trim()));
        if (!Inside(full)) throw new ArgumentException($"\"{file}\" is outside the folder this server reads from. Give a path relative to it.");
        if (!File.Exists(full)) throw new FileNotFoundException($"There's no file \"{file}\". list_files shows the X12 files there are.");
        // A link inside the folder may still point out of it.
        if (new FileInfo(full).ResolveLinkTarget(true) is { } target && !Inside(target.FullName))
            throw new ArgumentException($"\"{file}\" is a link to a file outside the folder this server reads from.");
        return full;
    }

    /// <summary>The file read and parsed.</summary>
    public X12File Load(string file) => X12File.Load(Resolve(file));

    /// <summary>The X12 files under <paramref name="folder"/> (the root when null): those whose text starts with ISA or ST.</summary>
    public (IReadOnlyList<string> Files, bool More) List(string? folder)
    {
        var start = Root;
        if (!string.IsNullOrWhiteSpace(folder))
        {
            start = Path.GetFullPath(Path.Combine(Root, folder.Trim()));
            if (!Inside(start)) throw new ArgumentException($"\"{folder}\" is outside the folder this server reads from.");
            if (!Directory.Exists(start)) throw new DirectoryNotFoundException($"There's no folder \"{folder}\".");
        }

        var options = new EnumerationOptions { RecurseSubdirectories = true, IgnoreInaccessible = true, AttributesToSkip = FileAttributes.Hidden | FileAttributes.System | FileAttributes.ReparsePoint };
        var found = new List<string>();
        foreach (var path in Directory.EnumerateFiles(start, "*", options))
        {
            if (!LooksLikeX12(path)) continue;
            if (found.Count == MaxListed) return (found, true);
            found.Add(Path.GetRelativePath(Root, path).Replace('\\', '/'));
        }
        found.Sort(StringComparer.OrdinalIgnoreCase);
        return (found, false);
    }

    private bool Inside(string full) =>
        full.Equals(Root, StringComparison.Ordinal) || full.StartsWith(Root + Path.DirectorySeparatorChar, StringComparison.Ordinal);

    /// <summary>True when the file's first characters, past white space and a byte order mark, are ISA or ST.</summary>
    private static bool LooksLikeX12(string path)
    {
        try
        {
            using var reader = new StreamReader(path);
            var buffer = new char[64];
            var read = reader.ReadBlock(buffer, 0, buffer.Length);
            var head = new string(buffer, 0, read).TrimStart('﻿', ' ', '\t', '\r', '\n');
            return head.StartsWith("ISA", StringComparison.Ordinal) || head.StartsWith("ST", StringComparison.Ordinal) && head.Length > 2 && !char.IsLetterOrDigit(head[2]);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }
}

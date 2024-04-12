using System.IO.Compression;
using System.Xml.Linq;

namespace Belp.Build.Test.MSBuild.Resources;

/// <summary>
/// Manages packages under test.
/// </summary>
public static class TestPackagesManager
{
    internal static List<TestPackage> PackagesList;

    private static readonly Dictionary<string, TestPackage> InternalPackages;

    /// <summary>
    /// Gets a dictionary which maps package IDs to package data.
    /// </summary>
    public static IReadOnlyDictionary<string, TestPackage> Packages => InternalPackages.AsReadOnly();

    static TestPackagesManager()
    {
        var packagesList = new List<TestPackage>();

        string[] packageFiles = Directory.GetFiles(TestPaths.TestPackages, "*.nupkg");
        foreach (string packageFile in packageFiles)
        {
            string filename = Path.GetFileNameWithoutExtension(packageFile);

            using var zipStream = new FileStream(packageFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var zip = new ZipArchive(zipStream);

            ZipArchiveEntry? nuspecEntry = zip.Entries.FirstOrDefault(e => !e.FullName.Contains('/') && !e.FullName.Contains('\\') && e.FullName.EndsWith(".nuspec")) ?? throw new InvalidOperationException($"{packageFile} must have a .nuspec file.");
            using Stream nuspecStream = nuspecEntry.Open();
            var nuspec = XDocument.Load(nuspecStream);
            if (nuspec.Root is null)
            {
                throw new InvalidOperationException($"{packageFile}@{filename}.nuspec must have a root element.");
            }
            XNamespace? defaultNamespace = nuspec.Root.GetDefaultNamespace();
            XName GetXName(string name)
            {
                return defaultNamespace?.GetName(name) ?? name;
            }
            XElement packageMetadata = nuspec.Root.Element(GetXName("metadata")) ?? throw new InvalidOperationException($"{packageFile}@{filename}.nuspec must have a metadata element inside the root element.");
            XElement packageID = packageMetadata.Element(GetXName("id")) ?? throw new InvalidOperationException($"{packageFile}@{filename}.nuspec must have an id element under the metadata element.");
            XElement packageVersion = packageMetadata.Element(GetXName("version")) ?? throw new InvalidOperationException($"{packageFile}@{filename}.nuspec must have a version element under the metadata element.");

            if (!IsValidValue(packageID.Value))
            {
                throw new InvalidOperationException($"""The package ID of {packageFile} "{packageID.Value}" is not valid.""");
            }

            if (!IsValidValue(packageVersion.Value))
            {
                throw new InvalidOperationException($"""The package version of {packageFile} "{packageVersion.Value}" is not valid.""");
            }

            packagesList.Add(new TestPackage(packageID.Value, packageVersion.Value));

            static bool IsValidValue(string text)
            {
                return !text.Contains('"') && !text.Contains("<![CDATA[");
            }
        }

        PackagesList = packagesList;
        InternalPackages = packagesList.ToDictionary(p => p.ID);
    }
}

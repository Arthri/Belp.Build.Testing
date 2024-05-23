using System.IO.Compression;
using System.Xml.Linq;

namespace Belp.Build.Testing.Resources;

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

            ZipArchiveEntry? nuspecEntry = zip.Entries.FirstOrDefault(e => !e.FullName.Contains('/') && !e.FullName.Contains('\\') && e.FullName.EndsWith(".nuspec")) ?? throw new NuspecNotFoundException(packageFile);
            using Stream nuspecStream = nuspecEntry.Open();
            var nuspec = XDocument.Load(nuspecStream);
            string nuspecPath = $"{packageFile}@{filename}.nuspec";
            if (nuspec.Root is null)
            {
                throw new NuspecRootNotFoundException(nuspecPath);
            }
            XNamespace? defaultNamespace = nuspec.Root.GetDefaultNamespace();
            XName GetXName(string name)
            {
                return defaultNamespace?.GetName(name) ?? name;
            }
            XElement packageMetadata = nuspec.Root.Element(GetXName("metadata")) ?? throw new NuspecElementNotFoundException(nuspecPath, "/package/metadata");
            XElement packageID = packageMetadata.Element(GetXName("id")) ?? throw new NuspecElementNotFoundException(nuspecPath, "/package/metadata/id");
            XElement packageVersion = packageMetadata.Element(GetXName("version")) ?? throw new NuspecElementNotFoundException(nuspecPath, "/package/metadata/version");

            if (!IsValidValue(packageID.Value))
            {
                throw new InvalidPackageIDException(packageFile, packageID.Value);
            }

            if (!IsValidValue(packageVersion.Value))
            {
                throw new InvalidPackageVersionException(packageFile, packageVersion.Value);
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

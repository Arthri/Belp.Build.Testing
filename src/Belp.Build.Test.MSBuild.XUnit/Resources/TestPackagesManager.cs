using System.IO.Compression;
using System.Xml.Linq;

namespace Belp.Build.Test.MSBuild.XUnit.Resources;

/// <summary>
/// Manages packages under test.
/// </summary>
internal static class TestPackagesManager
{
    public static List<TestPackage> Packages;

    static TestPackagesManager()
    {
        Packages = [];

        string[] packageFiles = Directory.GetFiles(TestPaths.TestPackages, "*.nupkg");
        foreach (string packageFile in packageFiles)
        {
            string filename = Path.GetFileNameWithoutExtension(packageFile);

            using var zipStream = new FileStream(packageFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var zip = new ZipArchive(zipStream);

            ZipArchiveEntry? nuspecEntry = zip.Entries.FirstOrDefault(e => !e.FullName.Contains('/') && !e.FullName.Contains('\\') && e.FullName.EndsWith(".nuspec")) ?? throw new InvalidOperationException($"{packageFile} must have a .nuspec file.");
            using Stream nuspecStream = nuspecEntry.Open();
            var nuspec = XDocument.Load(nuspecStream);
            XElement nuspecRoot = nuspec.Root ?? throw new InvalidOperationException($"{packageFile}@{filename}.nuspec must have a root element.");
            XElement packageMetadata = nuspec.Root.Element("metadata") ?? throw new InvalidOperationException($"{packageFile}@{filename}.nuspec must have a metadata element inside the root element.");
            XElement packageID = packageMetadata.Element("id") ?? throw new InvalidOperationException($"{packageFile}@{filename}.nuspec must have an id element under the metadata element.");
            XElement packageVersion = packageMetadata.Element("version") ?? throw new InvalidOperationException($"{packageFile}@{filename}.nuspec must have a version element under the metadata element.");

            if (!IsValidValue(packageID.Value))
            {
                throw new InvalidOperationException($"""The package ID of {packageFile} "{packageID.Value}" is not valid.""");
            }

            if (!IsValidValue(packageVersion.Value))
            {
                throw new InvalidOperationException($"""The package version of {packageFile} "{packageVersion.Value}" is not valid.""");
            }

            Packages.Add(new TestPackage(packageID.Value, packageVersion.Value));

            static bool IsValidValue(string text)
            {
                return !text.Contains('"') && !text.Contains("<![CDATA[");
            }
        }
    }
}

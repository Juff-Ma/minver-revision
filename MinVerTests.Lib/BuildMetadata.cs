using System.Reflection;
using System.Threading.Tasks;
using MinVer.Lib;
using MinVerTests.Infra;
using MinVerTests.Lib.Infra;
using Xunit;
using static MinVerTests.Infra.Git;

namespace MinVerTests.Lib;

public static class BuildMetadata
{
    [Theory]
    [InlineData("", "0.0.0.0-alpha.0")]
    [InlineData("a", "0.0.0.0-alpha.0+a")]
    public static async Task NoCommits(string buildMetadata, string expectedVersion)
    {
        // arrange
        var path = MethodBase.GetCurrentMethod().GetTestDirectory(buildMetadata);
        await EnsureEmptyRepository(path);

        // act
        var actualVersion = Versioner.GetVersion(path, "", MajorMinor.Default, buildMetadata, default, PreReleaseIdentifiers.Default, false, NullLogger.Instance);

        // assert
        Assert.Equal(expectedVersion, actualVersion.ToString());
    }

    [Theory]
    [InlineData("", "0.0.0.0-alpha.0")]
    [InlineData("a", "0.0.0.0-alpha.0+a")]
    public static async Task NoTag(string buildMetadata, string expectedVersion)
    {
        // arrange
        var path = MethodBase.GetCurrentMethod().GetTestDirectory(buildMetadata);
        await EnsureEmptyRepositoryAndCommit(path);

        // act
        var actualVersion = Versioner.GetVersion(path, "", MajorMinor.Default, buildMetadata, default, PreReleaseIdentifiers.Default, false, NullLogger.Instance);

        // assert
        Assert.Equal(expectedVersion, actualVersion.ToString());
    }

    [Theory]
    [InlineData("1.2.3.4+a", "", "1.2.3.4+a")]
    [InlineData("1.2.3.4", "b", "1.2.3.4+b")]
    [InlineData("1.2.3.4+a", "b", "1.2.3.4+a.b")]
    [InlineData("1.2.3.4-pre+a", "", "1.2.3.4-pre+a")]
    [InlineData("1.2.3.4-pre", "b", "1.2.3.4-pre+b")]
    [InlineData("1.2.3.4-pre+a", "b", "1.2.3.4-pre+a.b")]
    [InlineData("1.2.3-pre+a", "b", "1.2.3.0-pre+a.b")]
    [InlineData("1.2.3+a", "b", "1.2.3.0+a.b")]
    [InlineData("1.2.3", "b", "1.2.3.0+b")]
    public static async Task CurrentTag(string tag, string buildMetadata, string expectedVersion)
    {
        // arrange
        var path = MethodBase.GetCurrentMethod().GetTestDirectory((tag, buildMetadata));
        await EnsureEmptyRepositoryAndCommit(path);
        await Tag(path, tag);

        // act
        var actualVersion = Versioner.GetVersion(path, "", MajorMinor.Default, buildMetadata, default, PreReleaseIdentifiers.Default, false, NullLogger.Instance);

        // assert
        Assert.Equal(expectedVersion, actualVersion.ToString());
    }

    [Theory]
    [InlineData("1.2.3.4+a", "", "1.2.3.5-alpha.0.1")]
    [InlineData("1.2.3.4", "b", "1.2.3.5-alpha.0.1+b")]
    [InlineData("1.2.3.4+a", "b", "1.2.3.5-alpha.0.1+b")]
    [InlineData("1.2.3.4-pre+a", "", "1.2.3.4-pre.1")]
    [InlineData("1.2.3.4-pre", "b", "1.2.3.4-pre.1+b")]
    [InlineData("1.2.3.4-pre+a", "b", "1.2.3.4-pre.1+b")]
    [InlineData("1.2.3-pre+a", "b", "1.2.3.0-pre.1+b")]
    [InlineData("1.2.3", "b", "1.2.3.1-alpha.0.1+b")]
    public static async Task PreviousTag(string tag, string buildMetadata, string expectedVersion)
    {
        // arrange
        var path = MethodBase.GetCurrentMethod().GetTestDirectory((tag, buildMetadata));
        await EnsureEmptyRepositoryAndCommit(path);
        await Tag(path, tag);
        await Commit(path);

        // act
        var actualVersion = Versioner.GetVersion(path, "", MajorMinor.Default, buildMetadata, default, PreReleaseIdentifiers.Default, false, NullLogger.Instance);

        // assert
        Assert.Equal(expectedVersion, actualVersion.ToString());
    }
}

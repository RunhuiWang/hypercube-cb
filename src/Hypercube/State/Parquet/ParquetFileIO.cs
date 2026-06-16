namespace Hypercube.State.Parquet;

internal static class ParquetFileIO
{
    public static FileStream OpenReadShared(string filePath) =>
        File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

    public static FileStream CreateExclusive(string filePath) =>
        File.Create(filePath);

    /// <summary>
    /// Name of a scratch file, unique per write, that a writer fills before it replaces <paramref name="filePath"/>.
    /// Two writers of the same path (for example two backend instances) never share a scratch file.
    /// The name does not end in ".parquet", so a directory scan for spill files never sees it.
    /// </summary>
    public static string TempPathFor(string filePath) => filePath + "." + Guid.NewGuid().ToString("N") + ".tmp";

    /// <summary>
    /// Replaces <paramref name="filePath"/> with the finished scratch file in one rename. A reader that opens the
    /// spill file always gets a complete file and is never blocked by the writer's exclusive handle.
    /// The scratch file's stream must be closed before this call.
    /// </summary>
    public static void Publish(string tempPath, string filePath) =>
        File.Move(tempPath, filePath, overwrite: true);
}

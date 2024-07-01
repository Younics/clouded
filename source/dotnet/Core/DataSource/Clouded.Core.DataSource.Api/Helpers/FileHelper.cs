using Clouded.Core.DataSource.Shared;
using Clouded.Shared.Enums;

namespace Clouded.Core.DataSource.Api.Helpers;

public static class FileHelper
{
    public static string GetTmpFilePath(string fileName)
    {
        return Path.Combine(Path.GetTempPath(), "tmp_uploads", fileName);
    }

    public static async Task<List<string>> LoadTmpFiles(
        IEnumerable<ColumnResult> columns,
        DataSourceDictionary entity,
        DataSourceDictionary updateData
    )
    {
        var loadedPaths = new List<string>();
        foreach (var fileColumn in columns.Where(col => col.Type == EColumnType.Bytea))
        {
            if (entity[fileColumn.Name] == null || entity[fileColumn.Name] is not string)
            {
                // null or already byte[]
                continue;
            }

            var filePath = GetTmpFilePath(entity[fileColumn.Name] as string);
            loadedPaths.Add(filePath);
            updateData[fileColumn.Name] = await File.ReadAllBytesAsync(filePath);
        }

        return loadedPaths;
    }

    public static void DeleteFiles(this List<string> loadedPaths)
    {
        foreach (var filePath in loadedPaths)
        {
            File.Delete(filePath);
        }
    }
}

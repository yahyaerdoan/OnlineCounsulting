namespace OnlineConsulting.BusinessLogic.Concretions.StorageServices.Utilities.FileHelpers;

public class FileNameHelper
{
    protected static async Task<string> GenerateUniqueFileNameAsync(
        string fileName,
        string fileExtension,
        string targetFolderPathOrContainerName,
        Func<string, string, bool> hasFile)
    {
        fileName = NameOperation.CharacterRegulatory(fileName);
        return await Task.Run(() =>
        {
            var newFileName = fileName;
            var fullFileName = $"{newFileName}{fileExtension}";
            var counter = 2;

            while (hasFile(targetFolderPathOrContainerName, fullFileName))
            {
                newFileName = $"{fileName}-({counter++})";
                fullFileName = $"{newFileName}{fileExtension}";
            }

            return fullFileName;
        });
    }
}

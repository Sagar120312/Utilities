namespace FileUtility.Operation.Interface
{
    public interface IFileCreation
    {
        byte[] CreateCsvFile(List<dynamic> csvData, string delimiter);
        byte[] CreateXmlFile(string xmlString);
        byte[] CreateDtdFile(string dtdContent);
    }
}

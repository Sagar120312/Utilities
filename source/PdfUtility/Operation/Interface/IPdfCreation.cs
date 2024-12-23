namespace PdfUtility.Operation.Interface
{
    public interface IPdfCreation
    {
        byte[] GeneratePdf(string htmlContent, string fileName);
    }
}

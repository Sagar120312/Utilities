using jsreport.Binary;
using jsreport.Local;
using jsreport.Types;
using PdfUtility.Operation.Interface;

namespace PdfUtility.Operation.Abstract
{
    public class PdfCreation : IPdfCreation
    {
        public byte[] GeneratePdf(string htmlContent, string fileName)
        {
            try
            {
                // Setup jsreport options for local instance
                var rs = new LocalReporting()
                    .UseBinary(JsReportBinary.GetBinary())  // Use the jsreport binary
                    .AsUtility()  // Run jsreport in utility mode (no server)
                    .Create();

                // Create a report based on the HTML content
                var report = rs.RenderAsync(new RenderRequest
                {
                    Template = new Template
                    {
                        Content = htmlContent,  // HTML content to convert
                        Engine = Engine.Handlebars,  // Using Handlebars engine
                        Recipe = Recipe.ChromePdf  // Use Chrome-based PDF recipe
                    }
                }).Result;  // Generate the report
                using (var memoryStream = new MemoryStream())
                {
                    report.Content.CopyTo(memoryStream);
                    return memoryStream.ToArray();  // Convert MemoryStream to byte[]
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating PDF: {ex.Message}");
                throw;  // Rethrow the exception for higher-level handling
            }
        }

    }
}

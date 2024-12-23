using CsvHelper;
using CsvHelper.Configuration;
using FileUtility.Operation.Interface;
using System.Globalization;
using System.Text;
using System.Xml;

namespace FileUtility.Operation.Abstract
{
    public class FileCreation : IFileCreation
    {
        /// <summary>
        /// Method to create a byte[] for a csv file which takes list of dynamic objects.
        /// </summary>
        /// <param name="csvData"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public byte[] CreateCsvFile(List<dynamic> csvData, string delimiter = ";")
        {
            if (csvData == null || !csvData.Any())
                throw new ArgumentException("CSV data cannot be null or empty.", nameof(csvData));

            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = delimiter
                };

                using (var memoryStream = new MemoryStream())
                using (var writer = new StreamWriter(memoryStream, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, config))
                {
                    csv.WriteRecords(csvData);
                    writer.Flush();
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while creating the CSV file.", ex);
            }
        }

        /// <summary>
        /// Creates a byte array from an XML string.
        /// </summary>
        /// <param name="xmlString">The XML string to convert to a file.</param>
        /// <returns>Byte array representing the XML file.</returns>
        /// <exception cref="ArgumentException">Thrown if the input XML string is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the XML processing fails.</exception>
        public byte[] CreateXmlFile(string xmlString)
        {
            if (string.IsNullOrWhiteSpace(xmlString))
                throw new ArgumentException("The XML string cannot be null or empty.", nameof(xmlString));

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlString);

                using (var memoryStream = new MemoryStream())
                {
                    xmlDoc.Save(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create XML file from the provided string.", ex);
            }
        }

        /// <summary>
        /// Creates a byte array from a DTD string.
        /// </summary>
        /// <param name="dtdContent">The DTD content as a string.</param>
        /// <returns>Byte array representing the DTD file.</returns>
        /// <exception cref="ArgumentException">Thrown if the input DTD content is null or empty.</exception>
        public byte[] CreateDtdFile(string dtdContent)
        {
            if (string.IsNullOrWhiteSpace(dtdContent))
                throw new ArgumentException("The DTD content cannot be null or empty.", nameof(dtdContent));

            try
            {
                return Encoding.UTF8.GetBytes(dtdContent);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create DTD file from the provided content.", ex);
            }
        }


    }
}




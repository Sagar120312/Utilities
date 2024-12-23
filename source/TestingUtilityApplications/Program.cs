﻿using PdfUtility.Operation.Abstract;
using PdfUtility.Operation.Interface;
using System;
using System.IO;

namespace PdfUtilityTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Sample HTML content for the PDF
                string htmlContent = @"
                    <html>
                        <head>
                            <title>Sample PDF</title>
                        </head>
                        <body>
                            <div id=""table1"" class=""invoice-body"" style=""position: relative; width: 23cm; height: 100%; margin: 0 auto; color: #001028; background: #FFFFFF; font-family: Arial, sans-serif; font-size: 12px; font-weight: 500; padding-right: 50px; padding-left: 50px; padding-top: 50px;"">    <div class=""invoice-inner"" style=""clear: both;"">      <div class=""invoice-header"">        <table style=""width: 100%; border-collapse: collapse; border-spacing: 0; margin-bottom: 20px;"">          <tbody>            <tr>              <td class=""header-part"" style=""font-size: large; font-weight: 600;width:45%"">Dummy Firm 1</td>              <td class=""header-part"" style=""font-size: large; font-weight: 600;padding-left: 2.5%;"">Stornierungs rechnung</td>            </tr>          </tbody>        </table>        <table style=""width: 100%; border-collapse: collapse; border-spacing: 0; margin-bottom: 20px;"">          <tbody>            <tr>              <td style=""vertical-align: top; padding-top: 0px !important;width:45%"">                <p style=""margin: 0;"">Vijay Nagar<br/> Indore<br/> Madhya Pradesh 452010<br/> Indien</p>                                   <p style=""margin: 0;"">btaxiqa+2205@gmail.com</p>                <p style=""margin: 0;"">Tel.:  07312411374</p>              </td>              <td>                <table class=""invoice-info"" style=""width: 100%; border-collapse: collapse; border-spacing: 0; margin-bottom: 20px;"">                  <tbody>                    <tr style=""line-height: 0px;"">                      <td style=""text-align: left; padding: 10px 20px;""><strong>Rechnungsnummer:</strong></td>                      <td style=""text-align: right; padding: 10px 20px;""><strong>1-6931-03-01-2025</strong></td>                    </tr>                    <tr style=""line-height: 0px;"">                      <td style=""text-align: left; padding: 10px 20px;""><strong>Rechnungsdatum:</strong></td>                      <td style=""text-align: right; padding: 10px 20px;""><strong>12-12-2024</strong></td>                    </tr>                    <tr style=""line-height: 0px;"">                      <td style=""text-align: left; padding: 10px 20px;""><strong>UST ID:</strong></td>                      <td style=""text-align: right; padding: 10px 20px;""><strong>Test123</strong></td>                    </tr>                    <tr style=""line-height: 0px;"">                      <td style=""text-align: left; padding: 10px 20px;""><strong>Bestellnummer:</strong></td>                      <td style=""text-align: right; padding: 10px 20px;""><strong>#KK-20241212-00002</strong></td>                    </tr>                    <tr style=""line-height: 0px;"">                      <td style=""text-align: left; padding: 10px 20px;""><strong>Bestellung vom:</strong></td>                      <td style=""text-align: right; padding: 10px 20px;""><strong>12-12-2024</strong></td>                    </tr>                  </tbody>                </table>              </td>            </tr>          </tbody>        </table>        <table class=""addressheight"" style=""height: 146px; width: 100%; border-collapse: collapse; border-spacing: 0; margin-bottom: 20px;"">          <tbody>            <tr>              <td style=""vertical-align: top; width:45%"">               <p style=""margin: 0;"">sagar</p>               <p style=""margin: 0;"">apalasia</p>               <p style=""margin: 0;"">452001 Indore</p>                <p style=""margin: 0;"">Deutschland</p>                <p style=""margin: 0;"">452001 Indore</p>                <p style=""margin: 0;""></p>              </td>              <td style=""padding-left: 2.5%;vertical-align: top;"">                <p style=""font-weight: bold; margin: 0;""><strong>Lieferadresse:</strong></p>                <p style=""margin: 0;"">Sagar Srivastava</p>                <p style=""margin: 0;"">vijaynagar</p>                <p style=""margin: 0;"">452010 Indore</p>                <p style=""margin: 0;"">Deutschland</p>                <p style=""margin: 0;"">sagar.srivastava@empower-solutions.co.in</p>                <p style=""margin: 0;"">258374528369</p>              </td>            </tr>          </tbody>        </table>      </div>      <main>        <table style=""width: 100%; border-collapse: collapse; border-spacing: 0; margin-bottom: 20px;"">          <thead>            <tr style=""background-color: #E73916;"">              <th class=""product"" style=""color: #FFF; text-align: center; padding: 5px 20px;"">ProductName</th>              <th class=""unit text-center"" style=""color: #FFF; text-align: center; padding: 5px 20px;"">Einzelpreis</th>              <th style=""color: #FFF; text-align: center; padding: 5px 20px;"">Anzahl</th>              <th class=""vat"" style=""color: #FFF; text-align: center; padding: 5px 20px;"">MwSt. Satz</th>              <th class=""text-center"" style=""color: #FFF; text-align: center; padding: 5px 20px;"">Summe</th>            </tr>          </thead>          <div class=""mb-10"" style=""margin-bottom: 10px;"">        <strong>Kundenadresse</strong>        <div>          <span>Sagar</span>          <span> Srivastava -</span>          <span>  </span>          <span>  -</span>          <span>  -</span>          <span> btaxiqa+102@gmail.com -</span>          <span> 258374528369</span>        </div>      </div><tbody>            <tr>              <td class=""product"" style=""text-align: left; padding: 10px 20px;"">Dummy Product 2</td>              <td class=""unit text-center"" style=""text-align: center !important; padding: 10px 20px;"">€1.50</td>              <td style=""text-align: center !important; padding: 10px 20px;"">1</td>              <td class=""vat"" style=""text-align: center !important; padding: 10px 20px;"">0%</td>              <td class=""text-right"" style=""text-align: center !important; padding: 10px 20px;"">€1.50</td>            </tr>          </tbody><tr>              <td class=""product"" style=""text-align: left; padding: 10px 20px;"">Lieferzuschlag</td>              <td class=""unit text-center"" style=""text-align: center !important; padding: 10px 20px;""></td>              <td style=""text-align: center !important; padding: 10px 20px;""></td>              <td class=""vat"" style=""text-align: center !important; padding: 10px 20px;"">19%</td>              <td class=""text-right"" style=""text-align: center !important; padding: 10px 20px;"">€18.50</td>            </tr>          <tfoot>            <tr>              <td class=""product"" colspan=""4"" style=""padding: 5px 20px; font-weight: bold;""><b>Gesamtsumme</b></td>              <td class=""text-right"" style=""text-align: center !important; padding: 5px 20px; font-weight: bold;""><b>-€20</b></td>            </tr>            <tr>              <td class=""product"" style=""text-align: left; padding: 10px 20px;"">Gutschein - test123</td>              <td class=""unit text-center"" style=""text-align: center !important; padding: 10px 20px;""></td>              <td style=""text-align: center !important; padding: 10px 20px;""></td>              <td class=""vat"" style=""text-align: center !important; padding: 10px 20px;""></td>              <td class=""text-right"" style=""text-align: center !important; padding: 10px 20px;"">-€5</td>            </tr><tr>              <td class=""product"" colspan=""4"" style=""padding: 5px 20px;"">Gesamtpreis brutto</td>              <td class=""text-right"" style=""text-align: center !important; padding: 5px 20px;"">-€15</td>            </tr>            <tr>              <td class=""product"" colspan=""4"" style=""padding: 5px 20px;"">Enthaltene MwSt</td>              <td class=""text-right"" style=""text-align: center !important; padding: 5px 20px;"">-€2.95</td>            </tr>            <tr>                <td class=""product gross-price"" colspan=""4"" style=""padding: 5px 20px; font-weight: bold;""><b>Gesamtpreis netto</b></td>                <td class=""text-right gross-price"" style=""text-align: center !important; padding: 5px 20px; font-weight: bold;""><b>-€12.05</b></td>              </tr>          </tfoot>        </table>      </main>      <footer class=""order-invoice-footer-info"" style=""text-align: center; bottom: 0px; width: 100%; color: #666; margin-bottom: 6px;"">        <b>Zahlung: Dummy </b>        <p style=""color: #666; margin-bottom: 6px;"">Bankverbindungen: AXIS - IBAN: DE27100777770209299700 - BIC: NORSDE51XXX</p>        <p style=""color: #666; margin-bottom: 6px;"">Dummy bank - IBAN: DE11520513735120710131 - BIC: HELADEF1BOR </p>        <p style=""color: #666; margin-bottom: 6px;"">Geschäftsführer: Btaxi QA 2 & Btaxi Tester </p>      </footer>    </div>  </div>
                        </body>
                    </html>";

                // Create an instance of PdfCreation
                IPdfCreation pdfUtility = new PdfCreation();

                // Call the GeneratePdf method
                byte[] pdfBytes = pdfUtility.GeneratePdf(htmlContent, "SamplePDF");

                // Specify the path to save the generated PDF (Desktop as output folder)
                string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TestPDF.pdf");

                // Write the byte array to the specified file
                File.WriteAllBytes(outputPath, pdfBytes);

                Console.WriteLine($"PDF generated and saved at: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}

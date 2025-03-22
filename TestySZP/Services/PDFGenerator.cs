using System.Diagnostics;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using TestySZP.Models;

namespace TestySZP.Services
{
    public static class PDFGenerator
    {
        public static void GenerateTestPDF(Person person, List<Question> questions, string filePath)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Document document = new Document(PageSize.A4, 50, 50, 50, 50);
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            document.Open();

            string fontPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Fonts", "arial.ttf");
            BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font titleFont = new Font(baseFont, 16, Font.BOLD);
            Font normalFont = new Font(baseFont, 12, Font.NORMAL);
            Font boldFont = new Font(baseFont, 12, Font.BOLD);
            // Úvod
            document.Add(new Paragraph($"Kontrolní test SZP", titleFont) { Alignment = Element.ALIGN_CENTER });
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph($"Jméno a příjmení: {person.Name}", normalFont));
            document.Add(new Paragraph($"Dosažená třída: {person.KnowledgeClass}", normalFont));
            document.Add(new Paragraph($"Datum:"));
            document.Add(new Paragraph($"Podpis:"));
            document.Add(new Paragraph("\n"));

            int questionNumber = 0;
            Dictionary<int, string> answerKey = new();

            foreach (var question in questions)
            {
                questionNumber++;

                // Tabulka s jedním sloupcem pro otázku + odpovědi
                PdfPTable table = new PdfPTable(1);
                table.WidthPercentage = 100;

                PdfPCell cell = new PdfPCell { Border = Rectangle.NO_BORDER };

                // Text otázky
                cell.AddElement(new Paragraph($"{questionNumber}. {question.Text}", boldFont));
                cell.AddElement(new Paragraph(" "));

                if (!question.IsWritten && question.Answers != null && question.Answers.Count > 0)
                {
                    char optionLabel = 'A';
                    foreach (var answer in question.Answers)
                    {
                        cell.AddElement(new Paragraph($" {optionLabel}) {answer.Text}", normalFont));
                        if (answer.IsCorrect)
                            answerKey[questionNumber] = optionLabel.ToString();
                        optionLabel++;
                    }
                }
                else
                {
                    // Otázka s otevřenou odpovědí nebo bez odpovědí
                    for (int i = 0; i < 5; i++)
                        cell.AddElement(new Paragraph(" "));
                }

                // Přidej otázku do dokumentu
                table.AddCell(cell);
                document.Add(table);
                document.Add(new Paragraph("\n"));
            }

            // Nová stránka: klíč
            document.NewPage();
            document.Add(new Paragraph($"Správné odpovědi test {person.Name}:", titleFont));
            document.Add(new Paragraph("\n"));

            foreach (var entry in answerKey)
            {
                document.Add(new Paragraph($"{entry.Key}. {entry.Value}", normalFont));
            }

            document.Close();
            writer.Close();
        }
    }
}

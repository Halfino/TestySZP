using System.Collections.Generic;
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
            Document document = new Document(PageSize.A4, 50, 50, 50, 50);
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            document.Open();

            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
            Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

            // Úvod
            document.Add(new Paragraph($"Test pro: {person.Name}", titleFont));
            document.Add(new Paragraph($"Třída znalostí: {person.KnowledgeClass}", normalFont));
            document.Add(new Paragraph("\n"));

            int questionNumber = 1;
            Dictionary<int, string> answerKey = new();

            foreach (var question in questions)
            {
                // Blok otázky s odpověďmi – aby se nerozdělil přes dvě stránky
                PdfPTable table = new PdfPTable(1);
                table.WidthPercentage = 100;
                PdfPCell cell = new PdfPCell { Border = Rectangle.NO_BORDER };

                // Otázka
                cell.AddElement(new Paragraph($"{questionNumber}. {question.Text}", normalFont));
                cell.AddElement(new Paragraph(" "));

                if (!question.IsWritten)
                {
                    char optionLabel = 'A';
                    foreach (var answer in question.Answers)
                    {
                        cell.AddElement(new Paragraph($"   {optionLabel}) {answer.Text}", normalFont));

                        if (answer.IsCorrect)
                            answerKey[questionNumber] = optionLabel.ToString();

                        optionLabel++;
                    }
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                        cell.AddElement(new Paragraph(" "));
                }

                table.AddCell(cell);
                document.Add(table);
                document.Add(new Paragraph(" "));
                questionNumber++;
            }

            // Klíč na nové stránce
            document.NewPage();
            document.Add(new Paragraph("Správné odpovědi:", titleFont));
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

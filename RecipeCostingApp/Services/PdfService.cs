using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using RecipeCostingApp.Models;

namespace RecipeCostingApp.Services
{
    public class PdfService
    {
        public void GenerateRecipeCard(Recipe recipe, string filePath)
        {
            using var document = new Document(PageSize.A4, 50, 50, 50, 50);
            using var writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            
            document.Open();
            
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            
            document.Add(new Paragraph($"Recipe Card: {recipe.Name}", titleFont));
            document.Add(new Paragraph($"Category: {recipe.Category}", normalFont));
            document.Add(Chunk.NEWLINE);
            
            var table = new PdfPTable(4) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 3, 1, 1, 1 });
            
            table.AddCell(new PdfPCell(new Phrase("Ingredient", headerFont)));
            table.AddCell(new PdfPCell(new Phrase("Quantity", headerFont)));
            table.AddCell(new PdfPCell(new Phrase("Unit", headerFont)));
            table.AddCell(new PdfPCell(new Phrase("Cost", headerFont)));
            
            foreach (var ingredient in recipe.Ingredients)
            {
                table.AddCell(new Phrase(ingredient.Ingredient.Name, normalFont));
                table.AddCell(new Phrase(ingredient.Quantity.ToString("F1"), normalFont));
                table.AddCell(new Phrase(ingredient.Ingredient.Unit, normalFont));
                table.AddCell(new Phrase(ingredient.TotalCost.ToString("C2"), normalFont));
            }
            
            document.Add(table);
            document.Add(Chunk.NEWLINE);
            
            document.Add(new Paragraph($"Recipe Cost: {recipe.RecipeCost:C2}", headerFont));
            document.Add(new Paragraph($"Final Cost: {recipe.FinalCost:C2}", headerFont));
            document.Add(new Paragraph($"Selling Price: {recipe.SellingPrice:C2}", headerFont));
            document.Add(new Paragraph($"Profit: {recipe.Profit:C2}", headerFont));
            document.Add(new Paragraph($"Profit Margin: {recipe.GrossMargin:F1}%", headerFont));
            
            document.Close();
        }
        
        public void GenerateBulkCostingSheet(Recipe recipe, int multiplier, string filePath)
        {
            using var document = new Document(PageSize.A4, 50, 50, 50, 50);
            using var writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            
            document.Open();
            
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            
            document.Add(new Paragraph($"Bulk Production Sheet", titleFont));
            document.Add(new Paragraph($"Recipe: {recipe.Name} (x{multiplier})", headerFont));
            document.Add(Chunk.NEWLINE);
            
            var table = new PdfPTable(4) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 3, 1, 1, 1 });
            
            table.AddCell(new PdfPCell(new Phrase("Ingredient", headerFont)));
            table.AddCell(new PdfPCell(new Phrase("Unit Qty", headerFont)));
            table.AddCell(new PdfPCell(new Phrase("Total Qty", headerFont)));
            table.AddCell(new PdfPCell(new Phrase("Total Cost", headerFont)));
            
            foreach (var ingredient in recipe.Ingredients)
            {
                var totalQty = ingredient.Quantity * multiplier;
                var bulkCost = ingredient.TotalCost * multiplier;
                
                table.AddCell(new Phrase(ingredient.Ingredient.Name, normalFont));
                table.AddCell(new Phrase(ingredient.Quantity.ToString("F1"), normalFont));
                table.AddCell(new Phrase(totalQty.ToString("F1"), normalFont));
                table.AddCell(new Phrase(bulkCost.ToString("C2"), normalFont));
            }
            
            document.Add(table);
            document.Add(Chunk.NEWLINE);
            
            var totalCost = recipe.FinalCost * multiplier;
            document.Add(new Paragraph($"Total Production Cost: {totalCost:C2}", headerFont));
            
            document.Close();
        }
    }
}
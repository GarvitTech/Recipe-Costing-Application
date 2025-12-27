using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using RecipeCostingApp.Models;

namespace RecipeCostingApp.Services
{
    public class PdfService
    {
        public void GenerateRecipeCard(Recipe recipe, string filePath)
        {
            using var writer = new PdfWriter(filePath);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);
            
            document.Add(new Paragraph($"Recipe Card: {recipe.Name}").SetFontSize(18).SetBold());
            document.Add(new Paragraph($"Category: {recipe.Category}").SetFontSize(10));
            document.Add(new Paragraph(" "));
            
            var table = new Table(4).UseAllAvailableWidth();
            
            table.AddHeaderCell(new Cell().Add(new Paragraph("Ingredient").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Quantity").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Unit").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Cost").SetBold()));
            
            foreach (var ingredient in recipe.Ingredients)
            {
                table.AddCell(new Cell().Add(new Paragraph(ingredient.Ingredient.Name)));
                table.AddCell(new Cell().Add(new Paragraph(ingredient.Quantity.ToString("F1"))));
                table.AddCell(new Cell().Add(new Paragraph(ingredient.Ingredient.Unit)));
                table.AddCell(new Cell().Add(new Paragraph(ingredient.TotalCost.ToString("F3") + " KWD")));
            }
            
            document.Add(table);
            document.Add(new Paragraph(" "));
            
            document.Add(new Paragraph($"Recipe Cost: {recipe.RecipeCost:F3} KWD").SetBold());
            document.Add(new Paragraph($"Final Cost: {recipe.FinalCost:F3} KWD").SetBold());
            document.Add(new Paragraph($"Selling Price: {recipe.SellingPrice:F3} KWD").SetBold());
            document.Add(new Paragraph($"Profit: {recipe.Profit:F3} KWD").SetBold());
            document.Add(new Paragraph($"Profit Margin: {recipe.GrossMargin:F1}%").SetBold());
        }
        
        public void GenerateBulkCostingSheet(Recipe recipe, int multiplier, string filePath)
        {
            using var writer = new PdfWriter(filePath);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);
            
            document.Add(new Paragraph("Bulk Production Sheet").SetFontSize(18).SetBold());
            document.Add(new Paragraph($"Recipe: {recipe.Name} (x{multiplier})").SetFontSize(12).SetBold());
            document.Add(new Paragraph(" "));
            
            var table = new Table(4).UseAllAvailableWidth();
            
            table.AddHeaderCell(new Cell().Add(new Paragraph("Ingredient").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Unit Qty").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Total Qty").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Total Cost").SetBold()));
            
            foreach (var ingredient in recipe.Ingredients)
            {
                var totalQty = ingredient.Quantity * multiplier;
                var bulkCost = ingredient.TotalCost * multiplier;
                
                table.AddCell(new Cell().Add(new Paragraph(ingredient.Ingredient.Name)));
                table.AddCell(new Cell().Add(new Paragraph(ingredient.Quantity.ToString("F1"))));
                table.AddCell(new Cell().Add(new Paragraph(totalQty.ToString("F1"))));
                table.AddCell(new Cell().Add(new Paragraph(bulkCost.ToString("F3") + " KWD")));
            }
            
            document.Add(table);
            document.Add(new Paragraph(" "));
            
            var totalCost = recipe.FinalCost * multiplier;
            document.Add(new Paragraph($"Total Production Cost: {totalCost:F3} KWD").SetBold());
        }
    }
}
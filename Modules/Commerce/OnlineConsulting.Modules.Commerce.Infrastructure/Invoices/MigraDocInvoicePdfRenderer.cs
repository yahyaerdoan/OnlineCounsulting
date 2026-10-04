using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Tenancy;
using System.Globalization;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using PdfSharp.Fonts;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Invoices;

/// <summary>PDFsharp/MigraDoc (MIT). The font is embedded in this assembly, so rendering doesn't depend on fonts installed on the server.</summary>
public sealed class MigraDocInvoicePdfRenderer : IInvoicePdfRenderer
{
    private const string FontName = EmbeddedFontResolver.FamilyName;
    private static readonly CultureInfo Usd = CultureInfo.GetCultureInfo("en-US");
    private static readonly Color Brand = new(0x0F, 0x6C, 0xBD);
    private static readonly Color Muted = new(0x6B, 0x6B, 0x6B);
    private static readonly Color Hairline = new(0xE0, 0xE0, 0xE0);
    private static readonly Lock FontGate = new();

    public byte[] Render(InvoiceResponse invoice, InvoiceBusinessInfo business, TimeZoneInfo timeZone)
    {
        EnsureFontResolver();

        var document = new Document();
        document.Info.Title = invoice.InvoiceNumber;
        var normal = document.Styles[StyleNames.Normal] ?? throw new InvalidOperationException("MigraDoc has no Normal style.");
        normal.Font.Name = FontName;
        normal.Font.Size = 10;

        var section = document.AddSection();
        section.PageSetup.PageFormat = PageFormat.Letter;
        section.PageSetup.LeftMargin = Unit.FromCentimeter(2);
        section.PageSetup.RightMargin = Unit.FromCentimeter(2);
        section.PageSetup.TopMargin = Unit.FromCentimeter(2);

        AddHeader(section, invoice, business, timeZone);
        AddParties(section, invoice, business);
        AddLines(section, invoice);
        AddTotals(section, invoice);
        AddFooter(section, invoice);

        var renderer = new PdfDocumentRenderer { Document = document };
        renderer.RenderDocument();
        using var stream = new MemoryStream();
        renderer.PdfDocument.Save(stream, false);
        return stream.ToArray();
    }

    private static void AddHeader(Section section, InvoiceResponse invoice, InvoiceBusinessInfo business, TimeZoneInfo timeZone)
    {
        var table = section.AddTable();
        _ = table.AddColumn(Unit.FromCentimeter(10.5));
        _ = table.AddColumn(Unit.FromCentimeter(7));
        var row = table.AddRow();

        var brand = row.Cells[0].AddParagraph(business.BusinessName);
        brand.Format.Font.Size = 18;
        brand.Format.Font.Bold = true;
        brand.Format.Font.Color = Brand;

        var title = row.Cells[1].AddParagraph(invoice.Status == InvoiceStatuses.Paid ? "RECEIPT" : "INVOICE");
        title.Format.Alignment = ParagraphAlignment.Right;
        title.Format.Font.Size = 18;
        title.Format.Font.Bold = true;

        var status = row.Cells[1].AddParagraph(invoice.Status switch
        {
            InvoiceStatuses.Paid => $"PAID {invoice.PaidAt?.InZone(timeZone).ToString("MMM d, yyyy", Usd)}",
            InvoiceStatuses.Void => "VOID",
            _ => invoice.DueAt is { } due ? $"DUE {due.InZone(timeZone).ToString("MMM d, yyyy", Usd)}" : "DUE",
        });
        status.Format.Alignment = ParagraphAlignment.Right;
        status.Format.Font.Bold = true;
        status.Format.Font.Color = invoice.Status == InvoiceStatuses.Paid ? new Color(0x10, 0x7C, 0x10) : invoice.Status == InvoiceStatuses.Void ? Muted : new Color(0xC2, 0x5E, 0x00);

        var meta = row.Cells[1].AddParagraph($"{invoice.InvoiceNumber}\nIssued {invoice.IssuedAt.InZone(timeZone).ToString("MMM d, yyyy", Usd)}");
        meta.Format.Alignment = ParagraphAlignment.Right;
        meta.Format.Font.Color = Muted;
        meta.Format.SpaceBefore = Unit.FromPoint(4);

        _ = section.AddParagraph().Format.SpaceAfter = Unit.FromPoint(16);
    }

    private static void AddParties(Section section, InvoiceResponse invoice, InvoiceBusinessInfo business)
    {
        var table = section.AddTable();
        _ = table.AddColumn(Unit.FromCentimeter(8.75));
        _ = table.AddColumn(Unit.FromCentimeter(8.75));
        var row = table.AddRow();

        AddBlock(row.Cells[0], "BILL TO", [invoice.BillToName, invoice.BillToEmail, invoice.BillToAddress]);
        AddBlock(row.Cells[1], "FROM", [business.BusinessName, business.BusinessEmail, business.BusinessPhone, business.BusinessAddress]);

        var about = section.AddParagraph(invoice.Title);
        about.Format.SpaceBefore = Unit.FromPoint(18);
        about.Format.SpaceAfter = Unit.FromPoint(8);
        about.Format.Font.Bold = true;
        about.Format.Font.Size = 12;
    }

    private static void AddBlock(Cell cell, string label, IEnumerable<string?> lines)
    {
        var heading = cell.AddParagraph(label);
        heading.Format.Font.Size = 8;
        heading.Format.Font.Bold = true;
        heading.Format.Font.Color = Muted;
        heading.Format.SpaceAfter = Unit.FromPoint(4);

        foreach (var line in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
        {
            _ = cell.AddParagraph(line ?? "");
        }
    }

    private static void AddLines(Section section, InvoiceResponse invoice)
    {
        var table = section.AddTable();
        table.Borders.Bottom.Color = Hairline;
        _ = table.AddColumn(Unit.FromCentimeter(8.5));
        _ = table.AddColumn(Unit.FromCentimeter(2)).Format.Alignment = ParagraphAlignment.Right;
        _ = table.AddColumn(Unit.FromCentimeter(3.25)).Format.Alignment = ParagraphAlignment.Right;
        _ = table.AddColumn(Unit.FromCentimeter(3.75)).Format.Alignment = ParagraphAlignment.Right;

        var header = table.AddRow();
        header.HeadingFormat = true;
        header.Shading.Color = new Color(0xF3, 0xF6, 0xFA);
        header.Format.Font.Bold = true;
        header.Format.Font.Size = 9;
        header.TopPadding = Unit.FromPoint(6);
        header.BottomPadding = Unit.FromPoint(6);
        _ = header.Cells[0].AddParagraph("Description");
        _ = header.Cells[1].AddParagraph("Qty");
        _ = header.Cells[2].AddParagraph("Unit price");
        _ = header.Cells[3].AddParagraph("Amount");

        foreach (var line in invoice.Lines)
        {
            var row = table.AddRow();
            row.TopPadding = Unit.FromPoint(6);
            row.BottomPadding = Unit.FromPoint(6);
            row.Borders.Bottom.Color = Hairline;
            row.Borders.Bottom.Width = Unit.FromPoint(0.5);
            _ = row.Cells[0].AddParagraph(line.Description);
            _ = row.Cells[1].AddParagraph(line.Quantity.ToString("0.##", Usd));
            _ = row.Cells[2].AddParagraph(Money(line.UnitPrice));
            _ = row.Cells[3].AddParagraph(Money(line.Subtotal));
        }
    }

    private static void AddTotals(Section section, InvoiceResponse invoice)
    {
        _ = section.AddParagraph().Format.SpaceAfter = Unit.FromPoint(6);

        var table = section.AddTable();
        _ = table.AddColumn(Unit.FromCentimeter(7));
        _ = table.AddColumn(Unit.FromCentimeter(7));
        _ = table.AddColumn(Unit.FromCentimeter(3.5)).Format.Alignment = ParagraphAlignment.Right;

        AddTotalRow(table, "Subtotal", Money(invoice.Subtotal));
        if (invoice.DiscountAmount > 0)
        {
            AddTotalRow(table, invoice.DiscountLabel ?? "Discount", $"-{Money(invoice.DiscountAmount)}");
        }

        if (invoice.TaxAmount > 0)
        {
            AddTotalRow(table, "Tax", Money(invoice.TaxAmount));
        }

        var total = AddTotalRow(table, invoice.Status == InvoiceStatuses.Paid ? "Total paid" : "Amount due", Money(invoice.Total));
        total.Format.Font.Bold = true;
        total.Format.Font.Size = 12;
        total.Borders.Top.Color = Brand;
        total.Borders.Top.Width = Unit.FromPoint(1);
    }

    private static Row AddTotalRow(Table table, string label, string value)
    {
        var row = table.AddRow();
        row.TopPadding = Unit.FromPoint(4);
        row.BottomPadding = Unit.FromPoint(4);
        _ = row.Cells[1].AddParagraph(label);
        _ = row.Cells[2].AddParagraph(value);
        return row;
    }

    private static void AddFooter(Section section, InvoiceResponse invoice)
    {
        var note = section.AddParagraph(invoice.Status switch
        {
            InvoiceStatuses.Paid => $"Paid by {(invoice.PaymentMethod ?? "card").ToLowerInvariant()}. Thank you for your business!",
            InvoiceStatuses.Void => $"This invoice was voided.{(string.IsNullOrWhiteSpace(invoice.VoidReason) ? "" : $" Reason: {invoice.VoidReason}")}",
            _ => "Pay online from your account under Invoices. Thank you for your business!",
        });
        note.Format.SpaceBefore = Unit.FromPoint(28);
        note.Format.Font.Color = Muted;
    }

    private static string Money(decimal amount) => amount.ToString("C", Usd);

    private static void EnsureFontResolver()
    {
        lock (FontGate)
        {
            if (GlobalFontSettings.FontResolver is not EmbeddedFontResolver)
            {
                GlobalFontSettings.FontResolver = new EmbeddedFontResolver();
            }
        }
    }
}

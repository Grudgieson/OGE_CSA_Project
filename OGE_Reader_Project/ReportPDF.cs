using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class ReportPDF : IDocument
{
    public ReportModel Model { get; }

    public ReportPDF(ReportModel model)
    {
        Model = model;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {

                page.Size(PageSizes.A4);

                page.Margin(35);
            
                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);

                page.Footer().AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
    }

    void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().PaddingTop(25)
                    .Text($"Security Control System Report")
                    .FontSize(20).SemiBold().FontColor(Colors.Red.Medium);

                column.Item().PaddingBottom(35)
                    .Text($"Date of Report: {DateTime.Now.ToString("MM/dd/yyyy hh:mm tt")}")
                    .FontSize(10);

            });

            row.ConstantItem(150).Image(Image.FromStream(File.OpenRead("wwwroot/OGE_Logo_Color.png")));
        });
    }

    void ComposeContent(IContainer container)
    {

        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().PaddingBottom(10)
                    .Text($"Most Active User")
                    .FontSize(18).Bold();
                column.Item()
                    .Text($"{Model.mostActiveUser}")
                    .FontSize(16);
                column.Item().PaddingBottom(35)
                    .Text($"{Model.mostActiveUserNumberOfScans} Scans")
                    .FontSize(14);

                column.Item().PaddingBottom(10)
                    .Text($"Most Active Reader")
                    .FontSize(18).Bold();
                column.Item()
                    .Text($"{Model.mostActiveReader}")
                    .FontSize(16);
                column.Item().PaddingBottom(35)
                    .Text($"{Model.mostActiveReaderNumberOfScans} Scans")
                    .FontSize(14);

                column.Item().PaddingBottom(10)
                    .Text($"Busiest Day of The Week")
                    .FontSize(18).Bold();
                column.Item()
                    .Text($"{Model.busiestDay}")
                    .FontSize(16);
                column.Item().PaddingBottom(35)
                    .Text($"{Model.busiestDayAverageScans} Average Scans")
                    .FontSize(14);

                column.Item().PaddingBottom(10)
                    .Text($"Average Unique Visitors Per Day")
                    .FontSize(18).Bold();
                column.Item()
                    .Text($"Unique Visitors")
                    .FontSize(16);
                column.Item().PaddingBottom(35)
                    .Text($"{Model.averageUniqueVisitorsPerDay} People")
                    .FontSize(14);

                column.Item().PaddingBottom(10)
                    .Text($"Alerts Detected")
                    .FontSize(18).Bold();
                column.Item().PaddingBottom(35)
                    .Text($"{Model.numberOfAlerts} Alerts")
                    .FontSize(14);

                column.Item()
                    .Table(table =>
                    {

                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(100);
                            columns.RelativeColumn();
                            columns.ConstantColumn(75);
                        });

                        table.Header(header =>
                        {
                            header.Cell().BorderBottom(2).Padding(8).Text("Alert");
                            header.Cell().BorderBottom(2).Padding(8).Text("Description");
                            header.Cell().BorderBottom(2).Padding(8).AlignRight().Text("Severity");
                        });
                        
                        foreach (var alert in AlertSystem.masterAlertList)
                        {
                            
                            table.Cell().Padding(8).Text($"{alert.alertType}");
                            table.Cell().Padding(8).Text($"{alert.alertDescription}");
                            table.Cell().Padding(8).AlignRight().Text($"{alert.severityToString()}");

                        }

                    });


            });

        });
        
    }

}

/*


*/
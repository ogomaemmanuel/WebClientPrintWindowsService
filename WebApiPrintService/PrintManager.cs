using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;

internal class PrintManager
{
    private byte[] fileStream;
    public PrintManager() { }
    public PrintManager(byte[] fileStream)
    {
        this.fileStream = fileStream;
        Printing();
    }

    private void Printing()
    {
        try
        {

            try
            {
                var printFont = new Font("Arial", 10);
                PrintDocument pd = new PrintDocument();
                pd.PrintController = new StandardPrintController();
                pd.PrintPage += (sender, args) =>
                {
                    Image i = Image.FromStream(new MemoryStream(fileStream));
                    args.PageSettings.PaperSize = new PaperSize("custom", i.Width, i.Height);
                    args.Graphics.DrawImage(i, args.PageBounds);
                };

                pd.Print();

            }
            finally
            {
            }


        }
        catch (Exception ex)
        {

        }
}
    public List<String> InstalledPrinters()
    {
        List<string> printers = new List<string>();
        var installedPrinters = PrinterSettings.InstalledPrinters;

        foreach (string printer in installedPrinters)
        {
            printers.Add(printer);
        }
        return printers;
    }
}
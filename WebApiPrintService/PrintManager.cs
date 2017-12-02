using RawPrint;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Management;
using WebApiPrintService;

internal class PrintManager
{
    private byte[] fileStream;
    public PrintManager() { }
    public PrintManager(byte[] fileStream)
    {
        this.fileStream = fileStream;
        Printing();
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
    public static string GetDefaultPrinterName()
    {
        var query = new ObjectQuery("SELECT * FROM Win32_Printer");
        var searcher = new ManagementObjectSearcher(query);

        foreach (ManagementObject mo in searcher.Get())
        {
            if (((bool?)mo["Default"]) ?? false)
            {
                return mo["Name"] as string;
            }
        }

        return null;
    }
    private void Printing()
    {
        try
        {
            try
            {
                PrintDocument pd = new PrintDocument();
                pd.PrintController = new StandardPrintController();
                pd.PrintPage += (sender, args) =>
                {
                    Image i = Image.FromStream(new MemoryStream(fileStream));
                    args.Graphics.DrawImage(i, args.Graphics.VisibleClipBounds);
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

}
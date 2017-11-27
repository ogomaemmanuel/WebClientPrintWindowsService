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

    private void Printing()
    {
        string path = Path.Combine(Path.GetTempPath(),Guid.NewGuid().ToString()+".pdf");
        File.WriteAllBytes(path, fileStream);
        var defaultprinter = GetDefaultPrinterName();
        //Printer.PrintFile(defaultprinter, path);//option1

        //option2
        //if (File.Exists(path))
        //{
        //    var printJob = new Process
        //    {
        //        StartInfo = new ProcessStartInfo
        //        {
        //            FileName = path,
        //            UseShellExecute = true,
        //            Verb = "print",
        //            CreateNoWindow = true,
        //            WindowStyle = ProcessWindowStyle.Hidden,
        //            WorkingDirectory = Path.GetDirectoryName(path)
        //        }
        //    };
        //    printJob.Start();
        //}

        //endoption2

        WindowsRawPrintUtility.SendFileTo(defaultprinter, path);
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
}
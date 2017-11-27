using System;
using System.Collections.Generic;
using System.Web.Http;

public class HomeController : ApiController
{
    public string Get()
    {
        return "Hello World!";
    }

    public string Get(string name)
    {
        return "Hello " + name;
    }

  

    
}
public class PrintController : ApiController
{

    [HttpPost]
    [Route("print_api/print_file")]
    public IHttpActionResult PostPrintFile(PrintModel printModel)
    {
        if (ModelState.IsValid)
        {
            var fileStream = Convert.FromBase64String(printModel.Base64File);
            var printManager = new PrintManager(fileStream);
            return Ok();
        }
        return BadRequest("");
    }
    [HttpGet]
    [Route("printers")]
    public IHttpActionResult GetPrinter()
    {
        var printManager = new PrintManager();
        var printers = printManager.InstalledPrinters();
        if (printers.Count > 0) return Ok<List<string>>(printers); return NotFound();
    }
}

public class PrintModel
{
    public string Base64File { get; set; }
    public string PrinterName { get; set; }
}
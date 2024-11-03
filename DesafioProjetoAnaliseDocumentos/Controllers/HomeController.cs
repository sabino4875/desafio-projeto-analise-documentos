namespace DesafioProjetoAnaliseDocumentos.Controllers
{
    using DesafioProjetoAnaliseDocumentos.Models;
    using DesafioProjetoAnaliseDocumentos.Services;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Serilog;
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;

    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger _logger = Log.ForContext<HomeController>();
        private readonly IAzureStorageService _azureStorageService;

        public HomeController(IWebHostEnvironment environment, IAzureStorageService service)
        {
            ArgumentNullException.ThrowIfNull(environment, nameof(environment));
            ArgumentNullException.ThrowIfNull(service, nameof(service));

            _environment = environment;
            _azureStorageService = service; 
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(FileUpload model)
        {
            ArgumentNullException.ThrowIfNull(model, nameof(model));

            if (ModelState.IsValid)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "Uploads");
                var filename = model.File.FileName;

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, filename);

                // Validar o conteúdo do arquivo pelo stream
                using (var memoryStream = new MemoryStream())
                {
                    await model.File.CopyToAsync(memoryStream).ConfigureAwait(false);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    if (!ValidateFileContent(memoryStream))
                    {
                        ModelState.AddModelError("File", "Conteúdo do arquivo inválido.");
                        return View("Index");
                    }

                    // Salvar o arquivo se passar na validação
                    using var stream = new FileStream(filePath, FileMode.Create);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await memoryStream.CopyToAsync(stream).ConfigureAwait(false);

                    memoryStream.Seek(0, SeekOrigin.Begin);
                    filename = $"{Guid.NewGuid().ToString().Replace("-", "_", StringComparison.OrdinalIgnoreCase).ToUpper(CultureInfo.CurrentCulture)}{GetFileExtension(memoryStream)}";
                    var uri = await _azureStorageService.SaveImage(filename, memoryStream).ConfigureAwait(false);
                    var sample = "";
                }

                ViewBag.Message = "Arquivo enviado com sucesso!";
                return View("Index");
            }

            return View("Index");
        }

        private Boolean ValidateFileContent(MemoryStream stream)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));
            try
            {
                var buffer = new Byte[8];
                stream.Read(buffer, 0, buffer.Length);
                stream.Seek(0, SeekOrigin.Begin);

                // Verify the first bytes for diferent image file type
                if (IsJpeg(buffer) || IsPng(buffer) || IsBmp(buffer))
                {
                    return true;
                }

                return false;
            }
            catch(ApplicationException e)
            {
                _logger.Error(e, "Houve um erro ao tentar validar o conteúdo do arquivo.");
                return false;
            }
        }

        private String GetFileExtension(MemoryStream stream)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));
            try
            {
                var buffer = new Byte[8];
                stream.Read(buffer, 0, buffer.Length);
                stream.Seek(0, SeekOrigin.Begin);

                // Verify the first bytes for diferent image file type
                if (IsJpeg(buffer)) return ".jpg";
                if (IsPng(buffer)) return ".png";
                if (IsBmp(buffer)) return ".bmp";
                return String.Empty;
            }
            catch (ApplicationException e)
            {
                _logger.Error(e, "Houve um erro ao tentar recuperar a extensão do arquivo.");
                return String.Empty;
            }
        }

        private static Boolean IsJpeg(Byte[] buffer)
        {
            // JPEG Signature: FF D8 FF
            return buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF;
        }

        private static Boolean IsPng(Byte[] buffer)
        {
            // PNG Signature: 89 50 4E 47 0D 0A 1A 0A
            return buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47 &&
                   buffer[4] == 0x0D && buffer[5] == 0x0A && buffer[6] == 0x1A && buffer[7] == 0x0A;
        }

        private static Boolean IsBmp(Byte[] buffer)
        {
            // BMP Signature: 42 4D
            return buffer[0] == 0x42 && buffer[1] == 0x4D;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

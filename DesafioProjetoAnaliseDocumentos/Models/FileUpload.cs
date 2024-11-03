namespace DesafioProjetoAnaliseDocumentos.Models
{
    using Microsoft.AspNetCore.Http;
    using System.ComponentModel.DataAnnotations;

    public class FileUpload
    {
        [Required(ErrorMessage = "Por favor, informe um arquivo de imagem nas extensões bmp, jpeg ou png")]
        [Display(Name = "Escolha um arquivo de imagem com a extensão bmp, jpeg ou png")]
        public IFormFile File { get; set; }
    }
}

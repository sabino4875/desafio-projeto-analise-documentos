namespace DesafioProjetoAnaliseDocumentos.Services
{
    using Azure.AI.FormRecognizer.DocumentAnalysis;
    using DesafioProjetoAnaliseDocumentos.Context;
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAzureDocumentInteligenceService : IDisposable
    {
        Task ReadDocument(Uri document);
    }

    public class AzureDocumentInteligenceService : IAzureDocumentInteligenceService
    {
        private Boolean _disposable;
        private readonly IAzureDocumentInteligenceContext _context;

        public AzureDocumentInteligenceService(IAzureDocumentInteligenceContext context) 
        { 
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            _disposable = true;
            _context = context;
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing && _disposable)
            {
                _disposable = false;
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AzureDocumentInteligenceService()
        {
            Dispose(false);
        }

        public async Task ReadDocument(Uri document)
        {
            String modelId = "prebuilt-creditcard";
            AnalyzeDocumentOptions options = new AnalyzeDocumentOptions();
            CancellationToken cancellationToken = new CancellationToken();

            var result = await _context.Client.AnalyzeDocumentFromUriAsync(Azure.WaitUntil.Completed, modelId, document, options, cancellationToken).ConfigureAwait(false);
        }
    }
}

namespace DesafioProjetoAnaliseDocumentos.Services
{
    using Azure.AI.FormRecognizer.DocumentAnalysis;
    using DesafioProjetoAnaliseDocumentos.Context;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAzureDocumentInteligenceService : IDisposable
    {
        Task<Dictionary<String, String>> ReadDocument(Uri document);
    }

    public class AzureDocumentInteligenceService : IAzureDocumentInteligenceService
    {
        private Boolean _disposable;
        private readonly IAzureDocumentInteligenceContext _context;
        private readonly ILogger _logger;

        public AzureDocumentInteligenceService(IAzureDocumentInteligenceContext context) 
        { 
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            _disposable = true;
            _context = context;
            _logger = Log.ForContext<AzureDocumentInteligenceService>();
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

        public async Task<Dictionary<String, String>> ReadDocument(Uri document)
        {
            try
            {
                String modelId = "prebuilt-creditcard";
                AnalyzeDocumentOptions options = new();
                CancellationToken cancellationToken = new();

                var result = new Dictionary<String, String>();

                var data = await _context.Client.AnalyzeDocumentFromUriAsync(Azure.WaitUntil.Completed, modelId, document, options, cancellationToken).ConfigureAwait(false);
                if (data.Value != null)
                {
                    if (data.Value.Documents.Count > 0)
                    {
                        if (data.Value.Documents[0].Fields.Count > 0)
                        {
                            foreach (var field in data.Value.Documents[0].Fields)
                            {
                                result.Add(field.Key, field.Value.Content);
                            }
                        }
                    }
                }
                return result;
            }
            catch (ApplicationException e)
            {
                _logger.Error(e, "Houve um erro ao tentar ler os dados da imagem.");
                throw;
            }
        }
    }
}

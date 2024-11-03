namespace DesafioProjetoAnaliseDocumentos.Context
{
    using Azure;
    using Azure.AI.FormRecognizer.DocumentAnalysis;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Net;

    public interface IAzureDocumentInteligenceContext: IDisposable
    {
        DocumentAnalysisClient Client { get; }
    }

    public class AzureDocumentInteligenceContext : IAzureDocumentInteligenceContext
    {
        private Boolean _disposable;
        private readonly DocumentAnalysisClient _client;

        public DocumentAnalysisClient Client => _client;

        public AzureDocumentInteligenceContext(IConfiguration configuration) 
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

            var endpoint = configuration.GetValue<String>("AzureDocumentInteligence:endpoint");
            var credential = configuration.GetValue<String>("AzureDocumentInteligence:credentials");

            ValidateParameter(endpoint);
            ValidateParameter(credential);

            _disposable = true;
            _client = new DocumentAnalysisClient(new Uri(endpoint), new AzureKeyCredential(credential));
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

        ~AzureDocumentInteligenceContext()
        {
            Dispose(false);
        }

        private static void ValidateParameter(String parameter)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(parameter, nameof(parameter));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(parameter, nameof(parameter));
        }
    }
}

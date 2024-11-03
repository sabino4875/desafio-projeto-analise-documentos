namespace DesafioProjetoAnaliseDocumentos.Context
{
    using Azure.Storage;
    using Azure.Storage.Blobs;
    using Microsoft.Extensions.Configuration;
    using System;

    public interface IAzureStorageContext : IDisposable
    {
        BlobContainerClient Container { get; }
    }

    public class AzureStorageContext : IAzureStorageContext
    {
        private Boolean _disposable;
        private readonly BlobContainerClient _container;

        public AzureStorageContext(IConfiguration configuration) 
        { 
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

            var url = configuration.GetValue<String>("AzureStorage:Url");
            var account = configuration.GetValue<String>("AzureStorage:AccountName");
            var key = configuration.GetValue<String>("AzureStorage:AccountKey");
            var container = configuration.GetValue<String>("AzureStorage:ContainerName");

            ValidateParameter(url);
            ValidateParameter(account);
            ValidateParameter(key);
            ValidateParameter(container);

            _disposable = true;
            _container = new BlobContainerClient(
                            new Uri($"{url}/{account}/{container}"),
                            new StorageSharedKeyCredential(account, key)
                          );
            _container.CreateIfNotExists();
        }

        public BlobContainerClient Container => _container;

        protected virtual void Dispose(Boolean disposing) 
        { 
            if(disposing && _disposable)
            {
                _disposable = false;
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AzureStorageContext()
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

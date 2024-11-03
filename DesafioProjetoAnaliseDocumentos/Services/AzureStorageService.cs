namespace DesafioProjetoAnaliseDocumentos.Services
{
    using Azure;
    using Azure.Storage;
    using Azure.Storage.Blobs.Models;
    using DesafioProjetoAnaliseDocumentos.Context;
    using Serilog;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public interface IAzureStorageService : IDisposable
    {
        Task<Uri> SaveImage(String blobName, MemoryStream stream);
        Task<Stream> LoadImage(String blobName);
    }

    public class AzureStorageService : IAzureStorageService
    {
        private readonly IAzureStorageContext _context;
        private Boolean _disposable;
        private readonly ILogger _logger;

        public AzureStorageService(IAzureStorageContext context) 
        { 
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            _disposable = true;
            _context = context;
            _logger = Log.ForContext<AzureStorageService>();
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

        ~AzureStorageService()
        {
            Dispose(false);
        }

        public async Task<Stream> LoadImage(String blobName)
        {
            try
            {
                var validationOptions = new DownloadTransferValidationOptions
                {
                    AutoValidateChecksum = true,
                    ChecksumAlgorithm = StorageChecksumAlgorithm.Auto
                };

                BlobDownloadToOptions downloadOptions = new BlobDownloadToOptions()
                {
                    TransferValidation = validationOptions
                };

                var stream = new MemoryStream();
                var client = _context.Container.GetBlobClient(blobName);
                await client.DownloadToAsync(stream, downloadOptions).ConfigureAwait(false);
                return stream;
            }
            catch (ApplicationException e)
            {
                _logger.Error(e, "Houve um erro ao tentar carregar a imagem.");
                throw;
            }
        }

        public async Task<Uri> SaveImage(String blobName, MemoryStream stream)
        {
            try
            {
                var transferOptions = new StorageTransferOptions
                {
                    // Set the maximum number of parallel transfer workers
                    MaximumConcurrency = 2,

                    // Set the initial transfer length to 8 MiB
                    InitialTransferSize = 8 * 1024 * 1024,

                    // Set the maximum length of a transfer to 4 MiB
                    MaximumTransferSize = 4 * 1024 * 1024
                };

                var validationOptions = new UploadTransferValidationOptions
                {
                    ChecksumAlgorithm = StorageChecksumAlgorithm.Auto
                };

                var client = _context.Container.GetBlobClient(blobName);
                await client.UploadAsync(stream, new BlobUploadOptions
                {
                    AccessTier = AccessTier.Hot,
                    Conditions = new BlobRequestConditions { IfNoneMatch = new ETag("*") },
                    TransferOptions = transferOptions,
                    TransferValidation = validationOptions
                }).ConfigureAwait(false);

                return client.Uri;
            }
            catch (ApplicationException e) 
            {
                _logger.Error(e, "Houve um erro ao tentar salvar a imagem.");
                throw;
            }
        }
    }
}

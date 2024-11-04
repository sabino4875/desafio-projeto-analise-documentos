# DesafioProjetoAnaliseDocumentos

## Descrição
O `DesafioProjetoAnaliseDocumentos` é uma aplicação que utiliza inteligência artificial para analisar documentos e armazená-los em uma solução de armazenamento na nuvem da Azure. Este projeto é desenvolvido em C# com .NET Core e segue a arquitetura MVC.

## Funcionalidades

### Análise de Documentos
- **AzureDocumentInteligenceService**: Este serviço integra-se com a API de Inteligência de Documentos da Azure para analisar documentos enviados pelos usuários.

### Armazenamento na Nuvem
- **AzureStorageService**: Serviço responsável por armazenar os documentos analisados em uma conta de armazenamento da Azure.

### Interface do Usuário
- **HomeController**: Controlador principal que gerencia as interações com a página inicial da aplicação.
- **Views/Home/Index.cshtml**: View que permite aos usuários fazer upload de documentos para análise.

### Modelos de Dados
- **FileUpload**: Modelo que representa os dados do arquivo que o usuário faz upload.
- **ErrorViewModel**: Modelo utilizado para exibição de mensagens de erro.

## Como Executar o Projeto
1. Clone o repositório.
2. Abra o arquivo `DesafioProjetoAnaliseDocumentos.sln` no Visual Studio.
3. Restaure as dependências do projeto usando o Library Manager (libman).
4. Renmomeie o arquivo `config.template.json` para `config.json`.
5. Configure as credenciais do Azure no arquivo `config.json`.
6. Execute o projeto.
7. Utilize as imagens do diretório Images para os testes.
8. Utilize os arquivsos do diretório Docs para os testes.

## Licença
Este projeto é licenciado sob os termos da licença especificada no arquivo [LICENSE](LICENSE).

## Termos de uso
Este projeto segue os termos de uso especificados em <a href="https://app.dio.me/terms/" target="_blank">Termos de Uso - DIO</a>

## Contribuição
Sinta-se à vontade para contribuir com o projeto. Faça um fork do repositório, crie um branch para suas alterações e submeta um pull request.

## Referência bibliográfica

[Usar o emulador Azurite para o desenvolvimento local do Armazenamento do Microsoft Azure](https://learn.microsoft.com/pt-br/azure/storage/common/storage-use-azurite?tabs=npm%2Cblob-storage#tabpanel_1_docker-hub)
[Document Intelligence Studio](https://documentintelligence.ai.azure.com/studio)
[Document Intelligence credit card model](https://learn.microsoft.com/en-us/azure/ai-services/document-intelligence/prebuilt/credit-card?view=doc-intel-4.0.0#try-credit-card-data-extraction)
[Document Signatures](https://www.garykessler.net/library/file_sigs.html)


### Texto gerado por IA e revisado por humano

# Demo solution for showcasing a serverless architecture using Azure Functions

The demo can be used to show how a serverless architecture can be designed using Azure Functions and storage queues. By decoupling the functions using the queues the functions can be individually developed and deployed without interrupting the flow of the application.

More context is given in [this presentation](https://www.slideshare.net/marcduiker/getting-started-with-serverless-architectures-using-azure-functions-80755768) I gave at TechDays 2017 in Amsterdam.

## Solution files

The ImageAnalysisApp solution contains an Azure Functions App with two functions:

- `ValidateBlobSize`, this function is triggered by messages on the `imagesinput` storage queue and checks the size of the image that is configured as input binding to the `images` blob container. If the blob is too big a message is placed on the `imagestoolarge` queue. The the blob is not too big a message is placed on the `imagestoanalyze` queue. 
- `ImageAnalyzer`, this function is triggered by messages on the `imagestoanalyze` queue and uses the Azure Cognitive Services Computer Vision api to analyze the image that is configured as input binding. The results of the computer vision analysis are then placed on the `analysisresultstostore` queue.

The ImageUploader solution contains a console application that can be used to upload images from a local folder to the `imagesinput` queue.

## Creating the Function App & storage queues in Azure

The ImageAnalysisApp solution needs to run in a Azure Function App. You can either create this manually through the Azure portal or use these commands in the Azure CLI. In addition a separate storage account is created for the blob container and the 4 storage queues.

### Azure CLI: Function App commands

You can use these commands to create the Function App and the related storage account and resource group. 

Before running the commands I recommend verifying in the Azure portal if the names for the Function App, resource groups and storage accounts are available.

`resourceGroup=functionsdemo-rg`

`location=westeurope`

`appStorage=functionappstorage`

`functionAppName=myfirstfunctionsdemo`

`az group create --name $resourceGroup --location $location`

`az storage account create --name $appStorage --location $location --resource-group $resourceGroup --sku Standard_LRS`

`az functionapp create --name $functionAppName --storage-account $appStorage --resource-group $resourceGroup --consumption-plan-location $location`

### Azure CLI: Blob container & Storage queues commands

Use these commands to create a separate storage account for the blob container and the queues:

`dataStorage=functiondatastorage`

`az storage account create --name $dataStorage --location $location --resource-group $resourceGroup --sku Standard_LRS`

`storageKey="$(az storage account keys list --account-name $dataStorage --resource-group $resourceGroup --query [0].value)"`

`az storage container create --name "images" --account-key $storageKey --account-name $dataStorage`

`az storage queue create --name "imagesinput" --account-key $storageKey --account-name $dataStorage`

`az storage queue create --name "imagestoolarge" --account-key $storageKey --account-name $dataStorage`

`az storage queue create --name "imagestoanalyze" --account-key $storageKey --account-name $dataStorage`

`az storage queue create --name "analysisresultstostore" --account-key $storageKey --account-name $dataStorage`

The following command will show the connection string which needs to be put in the Function App application settings:

`az storage account show-connection-string --resource-group $resourceGroup --name $dataStorage`

## Function App settings

Finally add the following application settings to the Function App:

- Key=`StorageConnectionString`, Value=_<ConnectionString of the $dataStorage account>_
- Key=`ComputerVisionApiKey`, Value=_<Api key of the Cognitive Services Computer Vision API>_

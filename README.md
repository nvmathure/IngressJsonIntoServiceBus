# Ingress array of JSON objects from BLOB into ServiceBus Queue using Fucntion App

## What is the application doing?
The application contains two azure functions. First one, named "GenerateBogusDataFile" is HTTP Trigger function. One can call this function using PowerShell or PostMan to generate the data file with bogus data of a specified sample size. The function copies the file to Azure BLOB Collection named "file-drop." The file drop triggers the second function. This second function reads an array of objects, and adds them as a message to Azure ServiceBus Queue named "requests." Also, this function writes the small file with the same name as the incoming file containing performance summary of execution.

## Steps to Deploy/Test

### Step 1 - Create Azure Resources and Deploy Code

[![Deploy](http://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fnvmathure%2FIngressJsonIntoServiceBus%2Fmaster%2Fazuredeploy.json) [![Visualize](http://armviz.io/visualizebutton.png)](http://armviz.io/#/?load=https://raw.githubusercontent.com/nvmathure/IngressJsonIntoServiceBus/master/azuredeploy.json)

### Step 2 - Run test
Use Powershell to invoke function "GenerateBogusDataFile"
``` PowerShell
Invoke-WebRequest `
    -Uri http://localhost:7071/api/GenerateBogusDataFile `
    -Method Post `
    -Body '{ "fileName" : "data5", "sampleSize" : "5" }'
```
In couple of seconds depending on the sample zie file will be generated in Azure Storage BLOB collection "log" with same name as parameter "fileName". List of columns in the file generated in "log" folder are as below
|Position|Column Name           |
|:------:|----------------------|
| 1      | File Name            | 
| 2      | Size of File (bytes) |
| 3      | Number of Objects    |
| 4      | Elapsed Time (msec)  |
| 5      | Elapsed Time (Ticks) |
| 6      | Start Time UTC       |
| 7      | End Time UTC         |
Imports Amazon
Imports Amazon.DynamoDBv2
Imports Amazon.DynamoDBv2.DocumentModel
Imports Newtonsoft.Json.Linq
Imports System

Public Class AwsClient
    Private ReadOnly SimplePrimaryKey As String = "donation_date"
    Private ReadOnly TableName As String = "vitalStatsVB"
    Private ReadOnly _client As IAmazonDynamoDB

    Public Sub New()
        ' Get credentials from GitHub Actions secrets
        Dim accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID")
        Dim secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY")
        Dim regionName = Environment.GetEnvironmentVariable("AWS_REGION")

        If String.IsNullOrEmpty(accessKey) OrElse String.IsNullOrEmpty(secretKey) Then
            Throw New InvalidOperationException("AWS credentials not configured. Please set AWS_ACCESS_KEY_ID and AWS_SECRET_ACCESS_KEY environment variables.")
        End If

        Dim region = RegionEndpoint.GetBySystemName(regionName)
        Dim config As New AmazonDynamoDBConfig With {
            .RegionEndpoint = region
        }

        ' Create client with explicit credentials
        _client = New AmazonDynamoDBClient(accessKey, secretKey, config)
    End Sub

    Public Sub PushDataToDynamoDB(jsonData As JArray)
        Try
            Console.WriteLine($"Preparing to push {jsonData.Count} items to DynamoDB table {TableName}")

            ' Load the DynamoDB table
            Dim table As Table = Table.LoadTable(_client, TableName)

            ' Process each item in the JSON array
            For Each item As JObject In jsonData
                ' Convert JSON object to DynamoDB document
                Dim doc As Document = Document.FromJson(item.ToString())

                ' Add the document to DynamoDB
                table.PutItemAsync(doc).Wait()
                Console.WriteLine($"Successfully added item with key: {item(SimplePrimaryKey)}")
            Next

            Console.WriteLine("All items successfully pushed to DynamoDB")
        Catch ex As AmazonDynamoDBException When ex.ErrorCode = "ResourceNotFoundException"
            Console.WriteLine($"Error: DynamoDB table {TableName} not found in the specified region.")
            Throw
        Catch ex As Exception
            Console.WriteLine($"Error in PushDataToDynamoDB: {ex.Message}")
            Throw
        End Try
    End Sub
End Class
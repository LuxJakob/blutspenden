Imports Newtonsoft.Json.Linq
Imports System.IO

Module Orchestrator
    Sub Main(args As String())
        Console.WriteLine("Starting...")

        Try
            ' --- STEP 1: Create new CSV ---
            Console.WriteLine("Create CSV file...")
            AdditionalEntry.ProcessMarkdownFile()

            ' --- STEP 2: Load CSV ---
            Console.WriteLine("Loading CSV file...")
            Dim healthData = ParseCSV.ReadCSV()

            ' --- STEP 3: ETL Process ---
            Console.WriteLine($"ETL: Processes {healthData.Count} records.")
            ETLforJSON(healthData)

            ' --- STEP 4: AWS Dynamo Database ---
            Console.WriteLine("Initializing AWS client...")
            Dim awsClient = New AwsClient()
            Console.WriteLine("Pushing data to AWS...")

            ' --- STEP 5: Push new Data ---
            Dim jsonData As String = File.ReadAllText("health_data.json")
            Dim jArray As JArray = JArray.Parse(jsonData)

            awsClient.PushDataToDynamoDB(jArray)

        Catch ex As Exception
            Console.WriteLine($"Error in Orchestrator: {ex.Message}")
        End Try
    End Sub
End Module
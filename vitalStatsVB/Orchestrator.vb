Imports Newtonsoft.Json.Linq
Imports System.IO

Module Orchestrator
    Sub Main(args As String())
        Console.WriteLine("Starting...")

        Try
            ' --- STEP 1: Load CSV ---
            Console.WriteLine("Loading CSV file...")

            Dim healthData = ParseCSV.ReadCSV()

            ' --- STEP 2: ETL Process ---
            Console.WriteLine($"ETL: Processes {healthData.Count} records.")
            ETLforJSON(healthData)

            ' --- STEP 3: Supabase Database ---
            Console.WriteLine("Initializing AWS client...")
            Dim awsClient = New AwsClient()
            Console.WriteLine("Pushing data to AWS...")

            ' Read the JSON file that was created by ETL
            Dim jsonData As String = File.ReadAllText("health_data.json")
            Dim jArray As JArray = JArray.Parse(jsonData)

            awsClient.PushDataToDynamoDB(jArray)

        Catch ex As Exception
            Console.WriteLine($"Error in Orchestrator: {ex.Message}")
        End Try
    End Sub
End Module
Imports System.Text.Json
Imports System.IO
Imports System.Text.Json.Serialization

Public Module ETL
    Public Sub ETLforJSON(healthData As List(Of HealthRecord))

        Dim options As New JsonSerializerOptions With {
            .WriteIndented = True,
            .PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            .IgnoreReadOnlyProperties = True,
            .DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        }
        Dim json As String = JsonSerializer.Serialize(healthData, options)

        Dim jsonPath As String = "health_data.json"
        File.WriteAllText(jsonPath, json)
        Console.WriteLine($"Saved JSON to: {Path.GetFullPath(jsonPath)}")

    End Sub

End Module

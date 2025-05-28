Imports System.IO
Imports System.Text.Json

Public Class AdditionalEntry
    Public Shared Sub ProcessMarkdownFile()
        Dim inputFile As String = "README.md"
        Dim outputFile As String = "blood_donations.csv"

        Dim lines As String() = File.ReadAllLines(inputFile)

        If lines.Length < 7 Then
            Console.WriteLine("File doesn't have enough lines to process")
            Return
        End If

        Dim newLines(lines.Length - 7) As String
        Array.Copy(lines, 6, newLines, 0, lines.Length - 7)

        Dim jsonContent As String = String.Join(Environment.NewLine, newLines)

        Try
            Dim options As JsonSerializerOptions = New JsonSerializerOptions With {
                .PropertyNameCaseInsensitive = True
            }
            Dim records As List(Of HealthRecord) = JsonSerializer.Deserialize(Of List(Of HealthRecord))(jsonContent, options)

            Dim csvContent As New System.Text.StringBuilder()

            csvContent.AppendLine("donation_date,donation_type,weight_kg,amount_donated_ml,pulse,temperature,hemoglobin,blood_pressure")

            For Each record In records
                csvContent.AppendLine($"""{record.Timestamp:yyyy-MM-dd}"",""{record.Donation_type}"",{record.Weight},{record.AmountDonated},{record.Pulse},{record.Temperature},{record.Hemoglobin},{record.BloodPressure}")
            Next

            File.WriteAllText(outputFile, csvContent.ToString())

            Console.WriteLine($"Successfully converted JSON to {outputFile}")
        Catch ex As Exception
            Console.WriteLine($"Error processing file: {ex.Message}")
        End Try
    End Sub
End Class
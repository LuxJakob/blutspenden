Imports System.IO
Imports System.Text.Json
Imports System.Text.RegularExpressions
Imports Npgsql

Module Program
    Public Class HealthRecord
        Public Property donation_date As String
        Public Property donation_type As String
        Public Property weight_kg As Decimal
        Public Property amount_donated_ml As Integer
        Public Property blood_pressure As String
        Public Property pulse As Integer
        Public Property temperature As Decimal
        Public Property hemoglobin As Decimal
    End Class

    Sub Main()
        Console.WriteLine("Starting Supabase Sync...")

        ' --- STEP 1:  Read the README.md file
        Dim readmePath = "README.md"
        If Not File.Exists(readmePath) Then
            Console.WriteLine("Error: README.md not found!")
            Return
        End If
        Dim content = File.ReadAllText(readmePath)
        
        ' --- STEP 2: Extract the hidden JSON block using Regex
        Dim pattern = ""
        Dim match = Regex.Match(content, pattern, RegexOptions.Singleline)
        
        If Not match.Success Then
            Console.WriteLine("Error: Could not find HEALTHDATA block in README.")
            Return
        End If
        
        ' --- STEP 3: Parse the JSON
        Dim jsonString = match.Groups(1).Value
        Dim options As New JsonSerializerOptions With {
            .PropertyNameCaseInsensitive = True
        }
        Dim records = JsonSerializer.Deserialize(Of List(Of HealthRecord))(jsonString, options)
        Console.WriteLine($"Found {records.Count} records in README.")
        
        ' --- STEP 4: Connect to Supabase
        Dim connString = Environment.GetEnvironmentVariable("SUPABASE_CONNECTION")
        If String.IsNullOrEmpty(connString) Then
            Console.WriteLine("Error: Missing SUPABASE_CONNECTION environment variable.")
            Return
        End If

        Using conn As New NpgsqlConnection(connString)
            conn.Open()
            Console.WriteLine("Connected to Supabase successfully!")

        ' --- STEP 5: Insert the records
            For Each rec In records
                ' Split the "118/77" string into Upper and Lower values
                Dim bpParts = rec.blood_pressure.Split("/"c)
                Dim sys = Integer.Parse(bpParts(0))
                Dim dia = Integer.Parse(bpParts(1))

                ' Parameterized SQL to prevent injection and formatting errors
                Dim sql = "INSERT INTO vital_stats_main (donation_date, donation_type, weight_kg, amount_donated_ml, blood_pressure_upper, blood_pressure_lower, pulse, temperature, hemoglobin) " &
                          "VALUES (@date, @type, @weight, @amount, @upper, @lower, @pulse, @temp, @hemo)"
                
                Using cmd As New NpgsqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("date", DateTime.Parse(rec.donation_date))
                    cmd.Parameters.AddWithValue("type", rec.donation_type)
                    cmd.Parameters.AddWithValue("weight", rec.weight_kg)
                    cmd.Parameters.AddWithValue("amount", rec.amount_donated_ml)
                    cmd.Parameters.AddWithValue("upper", sys)
                    cmd.Parameters.AddWithValue("lower", dia)
                    cmd.Parameters.AddWithValue("pulse", rec.pulse)
                    cmd.Parameters.AddWithValue("temp", rec.temperature)
                    cmd.Parameters.AddWithValue("hemo", rec.hemoglobin)

                    Try
                        cmd.ExecuteNonQuery()
                        Console.WriteLine($"SUCCESS: Inserted record for {rec.donation_date}")
                    Catch ex As PostgresException When ex.SqlState = "23505"
                        ' 23505 is the Postgres code for "Unique Violation"
                        Console.WriteLine($"SKIPPED: {rec.donation_date} already exists in the database.")
                    Catch ex As Exception
                        Console.WriteLine($"ERROR on {rec.donation_date}: {ex.Message}")
                    End Try
                End Using
            Next
        End Using
        
        Console.WriteLine("Sync Complete!")
    End Sub
End Module
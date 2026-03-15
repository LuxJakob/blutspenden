Imports System.IO
Imports System.Text.Json
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
        Dim readmePath = Path.Combine(AppContext.BaseDirectory, "README.md")
        Console.WriteLine($"Looking for README at: {readmePath}")

        If Not File.Exists(readmePath) Then
            Console.WriteLine("Error: README.md not found in output directory!")
            Return
        End If
        
        Dim content = File.ReadAllText(readmePath)
        
        ' --- STEP 2: Extract the hidden JSON block using Regex
        Dim startMarker = "HEALTHDATA:"
        Dim endMarker = "-->"
        
        Dim startIndex = content.IndexOf(startMarker)
        If startIndex = -1 Then
            Console.WriteLine("Error: Could not find 'HEALTHDATA:' marker.")
            Return
        End If
        
        Dim endIndex = content.IndexOf(endMarker, startIndex)
        If endIndex = -1 Then
            Console.WriteLine("Error: Could not find closing '-->' marker.")
            Return
        End If
        
        ' Find the exact array brackets inside that block
        Dim jsonStart = content.IndexOf("["c, startIndex)
        Dim jsonEnd = content.LastIndexOf("]"c, endIndex)
        
        If jsonStart = -1 Or jsonEnd = -1 Or jsonStart > endIndex Then
            Console.WriteLine("Error: Could not find JSON array brackets [...] inside the HEALTHDATA block.")
            Return
        End If
        
        ' Extract exactly from '[' to ']'
        Dim jsonString = content.Substring(jsonStart, jsonEnd - jsonStart + 1)
        
        Console.WriteLine($"Extracted JSON length: {jsonString.Length} characters. Attempting native parse...")
        
        ' --- STEP 3: Parse the JSON
        Dim options As New JsonSerializerOptions With {
            .PropertyNameCaseInsensitive = True
        }
        
        Dim records As List(Of HealthRecord)
        Try
            records = JsonSerializer.Deserialize(Of List(Of HealthRecord))(jsonString, options)
            Console.WriteLine($"Found {records.Count} records in README.")
        Catch ex As JsonException
            Console.WriteLine("CRITICAL JSON ERROR: System.Text.Json rejected the format.")
            Console.WriteLine($"Raw string we tried to parse: {jsonString}")
            Console.WriteLine($"Exception: {ex.Message}")
            Return
        End Try
        
        ' --- STEP 4: Connect to Supabase
        Dim host = Environment.GetEnvironmentVariable("SUPABASE_HOST")
        Dim user = Environment.GetEnvironmentVariable("SUPABASE_USER")
        Dim pass = Environment.GetEnvironmentVariable("SUPABASE_PASS")

        If String.IsNullOrEmpty(host) OrElse String.IsNullOrEmpty(user) OrElse String.IsNullOrEmpty(pass) Then
            Console.WriteLine("Error: Missing one or more Supabase environment variables.")
            Return
        End If

        Dim builder As New NpgsqlConnectionStringBuilder() With {
            .Host = host,
            .Port = 6543,
            .Database = "postgres",
            .Username = user,
            .Password = pass,
            .SslMode = SslMode.Require
        }

        Using conn As New NpgsqlConnection(builder.ConnectionString)
            conn.Open()
            Console.WriteLine("Connected to Supabase successfully!")

        ' --- STEP 5: Insert the records
            For Each rec In records
                Dim bpParts = rec.blood_pressure.Split("/"c)
                Dim sys = Integer.Parse(bpParts(0))
                Dim dia = Integer.Parse(bpParts(1))

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
                        Console.WriteLine($"SKIPPED: {rec.donation_date} already exists.")
                    Catch ex As Exception
                        Console.WriteLine($"ERROR on {rec.donation_date}: {ex.Message}")
                    End Try
                End Using
            Next
        End Using
        
        Console.WriteLine("Sync Complete!")
    End Sub
End Module
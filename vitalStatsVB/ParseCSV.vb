Imports Microsoft.VisualBasic.FileIO
Imports System.IO

Public Module ParseCSV
    Public Function ReadCSV() As List(Of HealthRecord)

        Dim healthFile As String = "blood_donations.csv"

        If Not File.Exists(healthFile) Then
            Console.WriteLine("ERROR: CSV file not found!")
            Throw New Exception("ERROR: CSV file not found!")
        End If
        Dim records As New List(Of HealthRecord)()

        Using reader As New StreamReader(healthFile)
            While Not reader.EndOfStream
                Console.WriteLine(reader.ReadLine())
            End While
        End Using

        Try
            Using parser As New TextFieldParser(healthFile)
                parser.TextFieldType = FieldType.Delimited
                parser.SetDelimiters(",")
                parser.HasFieldsEnclosedInQuotes = True

                If Not parser.EndOfData Then
                    parser.ReadFields()
                End If

                While Not parser.EndOfData
                    Dim fields = parser.ReadFields()

                    Dim record As New HealthRecord With {
                        .Timestamp = DateTime.Parse(fields(0)),
                        .Donation_type = fields(1),
                        .Weight = Double.Parse(fields(2)),
                        .AmountDonated = Integer.Parse(fields(3)),
                        .Pulse = Integer.Parse(fields(4)),
                        .Temperature = Double.Parse(fields(5)),
                        .Hemoglobin = Double.Parse(fields(6)),
                        .BloodPressureUpper = Integer.Parse(fields(7).Split("/"c)(0)),
                        .BloodPressureLower = Integer.Parse(fields(7).Split("/"c)(1))
                    }
                    records.Add(record)
                End While
            End Using
        Catch ex As Exception
            Console.WriteLine($"Error parsing CSV: {ex.Message}")
            Throw
        End Try

        Return records
    End Function
End Module
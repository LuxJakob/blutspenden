Imports Microsoft.VisualBasic.FileIO
Imports System.IO

Public Module ParseCSV
    Public Function ReadCSV() As List(Of HealthRecord)

        Dim healthFile As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plasma_donation_testdata.csv")

        If Not File.Exists(healthFile) Then
            Console.WriteLine("ERROR: CSV file not found!")
            Throw New Exception("ERROR: CSV file not found!")
        End If
        Dim records As New List(Of HealthRecord)()

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
                        .Weight = Double.Parse(fields(1)),
                        .AmountDonated = Integer.Parse(fields(2)),
                        .BloodPressure = fields(3)
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
Imports System.Text.Json.Serialization
Public Class HealthRecord
    <JsonPropertyName("donation_date")>
    Public Property Timestamp As DateTime

    <JsonPropertyName("donation_type")>
    Public Property Donation_type As String

    <JsonPropertyName("weight_kg")>
    Public Property Weight As Double

    <JsonPropertyName("amount_donated_ml")>
    Public Property AmountDonated As Integer

    <JsonPropertyName("pulse")>
    Public Property Pulse As Integer

    <JsonPropertyName("temperature")>
    Public Property Temperature As Double

    <JsonPropertyName("hemoglobin")>
    Public Property Hemoglobin As Double

    <JsonIgnore>
    Public Property BloodPressure As String
    <JsonPropertyName("blood_pressure_upper")>
    Public ReadOnly Property BloodPressureUpper As Integer
        Get
            If String.IsNullOrEmpty(BloodPressure) Then Return 0
            Return Integer.Parse(BloodPressure.Split("/"c)(0))
        End Get
    End Property
    <JsonPropertyName("blood_pressure_lower")>
    Public ReadOnly Property BloodPressureLower As Integer
        Get
            If String.IsNullOrEmpty(BloodPressure) Then Return 0
            Return Integer.Parse(BloodPressure.Split("/"c)(1))
        End Get
    End Property

End Class
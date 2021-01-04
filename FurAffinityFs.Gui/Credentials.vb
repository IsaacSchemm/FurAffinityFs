Imports System.IO

Public Class Credentials
    Implements IFurAffinityCredentials

    Public Property A As String Implements IFurAffinityCredentials.A

    Public Property B As String Implements IFurAffinityCredentials.B

    Public Shared Function Load(filename As String) As Credentials
        If Not File.Exists(filename) Then
            Return Nothing
        End If

        Dim arr = File.ReadAllLines(filename)
        If arr.Length >= 2 Then
            Return New Credentials With {.A = arr(0), .B = arr(1)}
        Else
            Return Nothing
        End If
    End Function

    Public Sub Save(filename As String)
        File.WriteAllLines(filename, New String() {A, B})
    End Sub
End Class

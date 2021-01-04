Imports System.IO

Public Class Form1
    Private _file As FurAffinityFile = Nothing

    Private Sub OnShow() Handles Me.Shown
        ddlCategory.Items.Clear()
        For Each o In [Enum].GetValues(GetType(FurAffinityCategory))
            ddlCategory.Items.Add(CType(o, FurAffinityCategory))
        Next
        ddlGender.Items.Clear()
        For Each o In [Enum].GetValues(GetType(FurAffinityGender))
            ddlGender.Items.Add(CType(o, FurAffinityGender))
        Next
        ddlSpecies.Items.Clear()
        For Each o In [Enum].GetValues(GetType(FurAffinitySpecies))
            ddlSpecies.Items.Add(CType(o, FurAffinitySpecies))
        Next
        ddlTheme.Items.Clear()
        For Each o In [Enum].GetValues(GetType(FurAffinityType))
            ddlTheme.Items.Add(CType(o, FurAffinityType))
        Next
        ddlRating.Items.Clear()
        For Each o In [Enum].GetValues(GetType(FurAffinityRating))
            ddlRating.Items.Add(CType(o, FurAffinityRating))
        Next

        Dim args = Environment.GetCommandLineArgs()
        If args.Length > 1 Then
            If File.Exists(args(1)) Then
                Open(args(1))
            End If
        End If
    End Sub

    Private Sub Open() Handles OpenToolStripMenuItem.Click
        Using dialog As New OpenFileDialog()
            dialog.Filter = "Image files (*.png; *.jpg; *.jpeg; *.gif)|*.png;*.jpg;*.gif"
            If dialog.ShowDialog() = DialogResult.OK Then
                Open(dialog.FileName)
            End If
        End Using
    End Sub

    Private Sub Open(path As String)
        Dim img = Image.FromFile(path)

        Dim contentType = "application/octet-stream"
        If img.RawFormat.Guid = Imaging.ImageFormat.Png.Guid Then
            contentType = "image/png"
        ElseIf img.RawFormat.Guid = Imaging.ImageFormat.Jpeg.Guid Then
            contentType = "image/jpeg"
        ElseIf img.RawFormat.Guid = Imaging.ImageFormat.Gif.Guid Then
            contentType = "image/gif"
        End If

        _file = New FurAffinityFile(File.ReadAllBytes(path), contentType)

        PictureBox1.Image = img
    End Sub

    Private Sub DoExit() Handles ExitToolStripMenuItem.Click
        Close()
    End Sub

    Private Sub About() Handles AboutToolStripMenuItem.Click
        MsgBox("FurAffinityFs.Gui
(C) 2021 Isaac Schemm

https://github.com/IsaacSchemm/FurAffinityFs")
    End Sub

    Private Async Sub PostAsync() Handles btnPost.Click
        If _file Is Nothing Then
            Open()
            If _file Is Nothing Then
                Return
            End If
        End If

        Dim creds = Credentials.Load("credentials.txt")

        If creds Is Nothing Then
            Dim a = InputBox("Enter the value of the ""a"" cookie:")
            If a Is Nothing Then
                Return
            End If
            Dim b = InputBox("Enter the value of the ""b"" cookie:")
            If b Is Nothing Then
                Return
            End If

            creds = New Credentials With {.A = a, .B = b}
            creds.Save("credentials.txt")
        End If

        If creds IsNot Nothing Then
            btnPost.Enabled = False
            Try
                Dim metadata As New FurAffinitySubmission.ArtworkMetadata(
                    title:=txtTitle.Text,
                    message:=txtDescription.Text,
                    keywords:=txtTags.Text.Split(" "c),
                    cat:=ddlCategory.SelectedItem,
                    scrap:=chkScraps.Checked,
                    atype:=ddlCategory.SelectedItem,
                    species:=ddlSpecies.SelectedItem,
                    gender:=ddlGender.SelectedItem,
                    rating:=ddlRating.SelectedItem,
                    lock_comments:=chkLockComments.Checked)
                Dim url = Await FurAffinitySubmission.PostArtworkAsync(creds, _file, metadata)
                If File.Exists("C:\Program Files (x86)\Internet Explorer\iexplore.exe") Then
                    Process.Start("C:\Program Files (x86)\Internet Explorer\iexplore.exe", url.AbsoluteUri)
                End If
            Catch ex As Exception
                Console.Error.WriteLine(ex)
                MsgBox("An error was encountered. Your credentials might have expired. You might want to delete credentials.txt and try logging in again.")
            End Try
            btnPost.Enabled = True
        End If
    End Sub
End Class

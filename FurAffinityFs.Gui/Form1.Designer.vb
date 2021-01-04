<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.lblDescription = New System.Windows.Forms.Label()
        Me.txtTitle = New System.Windows.Forms.TextBox()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.txtTags = New System.Windows.Forms.TextBox()
        Me.lblKeywords = New System.Windows.Forms.Label()
        Me.txtDescription = New System.Windows.Forms.TextBox()
        Me.ddlGender = New System.Windows.Forms.ComboBox()
        Me.lblGender = New System.Windows.Forms.Label()
        Me.ddlSpecies = New System.Windows.Forms.ComboBox()
        Me.lblSpecies = New System.Windows.Forms.Label()
        Me.ddlTheme = New System.Windows.Forms.ComboBox()
        Me.lblTheme = New System.Windows.Forms.Label()
        Me.ddlCategory = New System.Windows.Forms.ComboBox()
        Me.lblCategory = New System.Windows.Forms.Label()
        Me.chkLockComments = New System.Windows.Forms.CheckBox()
        Me.chkScraps = New System.Windows.Forms.CheckBox()
        Me.btnPost = New System.Windows.Forms.Button()
        Me.ddlRating = New System.Windows.Forms.ComboBox()
        Me.lblRating = New System.Windows.Forms.Label()
        Me.MenuStrip1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.HelpToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(598, 24)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenToolStripMenuItem, Me.toolStripSeparator, Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "&File"
        '
        'OpenToolStripMenuItem
        '
        Me.OpenToolStripMenuItem.Image = CType(resources.GetObject("OpenToolStripMenuItem.Image"), System.Drawing.Image)
        Me.OpenToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem"
        Me.OpenToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
        Me.OpenToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
        Me.OpenToolStripMenuItem.Text = "&Open"
        '
        'toolStripSeparator
        '
        Me.toolStripSeparator.Name = "toolStripSeparator"
        Me.toolStripSeparator.Size = New System.Drawing.Size(143, 6)
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
        Me.ExitToolStripMenuItem.Text = "E&xit"
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AboutToolStripMenuItem})
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.HelpToolStripMenuItem.Text = "&Help"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(116, 22)
        Me.AboutToolStripMenuItem.Text = "&About..."
        '
        'PictureBox1
        '
        Me.PictureBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PictureBox1.Location = New System.Drawing.Point(346, 27)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(240, 206)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 1
        Me.PictureBox1.TabStop = False
        '
        'lblDescription
        '
        Me.lblDescription.AutoSize = True
        Me.lblDescription.Location = New System.Drawing.Point(13, 71)
        Me.lblDescription.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblDescription.Name = "lblDescription"
        Me.lblDescription.Size = New System.Drawing.Size(67, 15)
        Me.lblDescription.TabIndex = 10
        Me.lblDescription.Text = "Description"
        '
        'txtTitle
        '
        Me.txtTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtTitle.Location = New System.Drawing.Point(13, 45)
        Me.txtTitle.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.txtTitle.Name = "txtTitle"
        Me.txtTitle.Size = New System.Drawing.Size(326, 23)
        Me.txtTitle.TabIndex = 9
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Location = New System.Drawing.Point(13, 27)
        Me.lblTitle.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(30, 15)
        Me.lblTitle.TabIndex = 8
        Me.lblTitle.Text = "Title"
        '
        'txtTags
        '
        Me.txtTags.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtTags.Location = New System.Drawing.Point(12, 210)
        Me.txtTags.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.txtTags.Name = "txtTags"
        Me.txtTags.Size = New System.Drawing.Size(327, 23)
        Me.txtTags.TabIndex = 13
        '
        'lblKeywords
        '
        Me.lblKeywords.AutoSize = True
        Me.lblKeywords.Location = New System.Drawing.Point(13, 192)
        Me.lblKeywords.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblKeywords.Name = "lblKeywords"
        Me.lblKeywords.Size = New System.Drawing.Size(153, 15)
        Me.lblKeywords.TabIndex = 12
        Me.lblKeywords.Text = "Keywords (space separated)"
        '
        'txtDescription
        '
        Me.txtDescription.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtDescription.Font = New System.Drawing.Font("Lucida Console", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.txtDescription.Location = New System.Drawing.Point(13, 89)
        Me.txtDescription.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.txtDescription.Multiline = True
        Me.txtDescription.Name = "txtDescription"
        Me.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtDescription.Size = New System.Drawing.Size(326, 100)
        Me.txtDescription.TabIndex = 11
        '
        'ddlGender
        '
        Me.ddlGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlGender.FormattingEnabled = True
        Me.ddlGender.Location = New System.Drawing.Point(76, 329)
        Me.ddlGender.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.ddlGender.Name = "ddlGender"
        Me.ddlGender.Size = New System.Drawing.Size(262, 23)
        Me.ddlGender.TabIndex = 25
        '
        'lblGender
        '
        Me.lblGender.AutoSize = True
        Me.lblGender.Location = New System.Drawing.Point(13, 332)
        Me.lblGender.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblGender.Name = "lblGender"
        Me.lblGender.Size = New System.Drawing.Size(45, 15)
        Me.lblGender.TabIndex = 24
        Me.lblGender.Text = "Gender"
        '
        'ddlSpecies
        '
        Me.ddlSpecies.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlSpecies.FormattingEnabled = True
        Me.ddlSpecies.Location = New System.Drawing.Point(76, 270)
        Me.ddlSpecies.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.ddlSpecies.Name = "ddlSpecies"
        Me.ddlSpecies.Size = New System.Drawing.Size(262, 23)
        Me.ddlSpecies.TabIndex = 23
        '
        'lblSpecies
        '
        Me.lblSpecies.AutoSize = True
        Me.lblSpecies.Location = New System.Drawing.Point(13, 273)
        Me.lblSpecies.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblSpecies.Name = "lblSpecies"
        Me.lblSpecies.Size = New System.Drawing.Size(46, 15)
        Me.lblSpecies.TabIndex = 22
        Me.lblSpecies.Text = "Species"
        '
        'ddlTheme
        '
        Me.ddlTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlTheme.FormattingEnabled = True
        Me.ddlTheme.Location = New System.Drawing.Point(76, 299)
        Me.ddlTheme.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.ddlTheme.Name = "ddlTheme"
        Me.ddlTheme.Size = New System.Drawing.Size(262, 23)
        Me.ddlTheme.TabIndex = 21
        '
        'lblTheme
        '
        Me.lblTheme.AutoSize = True
        Me.lblTheme.Location = New System.Drawing.Point(13, 302)
        Me.lblTheme.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblTheme.Name = "lblTheme"
        Me.lblTheme.Size = New System.Drawing.Size(44, 15)
        Me.lblTheme.TabIndex = 20
        Me.lblTheme.Text = "Theme"
        '
        'ddlCategory
        '
        Me.ddlCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlCategory.FormattingEnabled = True
        Me.ddlCategory.Location = New System.Drawing.Point(76, 241)
        Me.ddlCategory.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.ddlCategory.Name = "ddlCategory"
        Me.ddlCategory.Size = New System.Drawing.Size(262, 23)
        Me.ddlCategory.TabIndex = 19
        '
        'lblCategory
        '
        Me.lblCategory.AutoSize = True
        Me.lblCategory.Location = New System.Drawing.Point(13, 244)
        Me.lblCategory.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblCategory.Name = "lblCategory"
        Me.lblCategory.Size = New System.Drawing.Size(55, 15)
        Me.lblCategory.TabIndex = 18
        Me.lblCategory.Text = "Category"
        '
        'chkLockComments
        '
        Me.chkLockComments.AutoSize = True
        Me.chkLockComments.Location = New System.Drawing.Point(346, 239)
        Me.chkLockComments.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.chkLockComments.Name = "chkLockComments"
        Me.chkLockComments.Size = New System.Drawing.Size(223, 19)
        Me.chkLockComments.TabIndex = 9
        Me.chkLockComments.Text = "Lock comments (minimum 24 hours)"
        Me.chkLockComments.UseVisualStyleBackColor = True
        '
        'chkScraps
        '
        Me.chkScraps.AutoSize = True
        Me.chkScraps.Location = New System.Drawing.Point(346, 264)
        Me.chkScraps.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.chkScraps.Name = "chkScraps"
        Me.chkScraps.Size = New System.Drawing.Size(93, 19)
        Me.chkScraps.TabIndex = 27
        Me.chkScraps.Text = "Put in scraps"
        Me.chkScraps.UseVisualStyleBackColor = True
        '
        'btnPost
        '
        Me.btnPost.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnPost.Location = New System.Drawing.Point(497, 362)
        Me.btnPost.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.btnPost.Name = "btnPost"
        Me.btnPost.Size = New System.Drawing.Size(88, 27)
        Me.btnPost.TabIndex = 28
        Me.btnPost.Text = "Post"
        Me.btnPost.UseVisualStyleBackColor = True
        '
        'ddlRating
        '
        Me.ddlRating.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlRating.FormattingEnabled = True
        Me.ddlRating.Location = New System.Drawing.Point(76, 358)
        Me.ddlRating.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.ddlRating.Name = "ddlRating"
        Me.ddlRating.Size = New System.Drawing.Size(262, 23)
        Me.ddlRating.TabIndex = 30
        '
        'lblRating
        '
        Me.lblRating.AutoSize = True
        Me.lblRating.Location = New System.Drawing.Point(13, 361)
        Me.lblRating.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblRating.Name = "lblRating"
        Me.lblRating.Size = New System.Drawing.Size(41, 15)
        Me.lblRating.TabIndex = 29
        Me.lblRating.Text = "Rating"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(598, 401)
        Me.Controls.Add(Me.ddlRating)
        Me.Controls.Add(Me.lblRating)
        Me.Controls.Add(Me.btnPost)
        Me.Controls.Add(Me.chkScraps)
        Me.Controls.Add(Me.chkLockComments)
        Me.Controls.Add(Me.ddlGender)
        Me.Controls.Add(Me.lblGender)
        Me.Controls.Add(Me.ddlSpecies)
        Me.Controls.Add(Me.lblSpecies)
        Me.Controls.Add(Me.ddlTheme)
        Me.Controls.Add(Me.lblTheme)
        Me.Controls.Add(Me.ddlCategory)
        Me.Controls.Add(Me.lblCategory)
        Me.Controls.Add(Me.lblDescription)
        Me.Controls.Add(Me.txtTitle)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.txtTags)
        Me.Controls.Add(Me.lblKeywords)
        Me.Controls.Add(Me.txtDescription)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents toolStripSeparator As ToolStripSeparator
    Friend WithEvents ExitToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PictureBox1 As PictureBox
    Private WithEvents lblDescription As Label
    Private WithEvents txtTitle As TextBox
    Private WithEvents lblTitle As Label
    Private WithEvents txtTags As TextBox
    Private WithEvents lblKeywords As Label
    Private WithEvents txtDescription As TextBox
    Private WithEvents ddlGender As ComboBox
    Private WithEvents lblGender As Label
    Private WithEvents ddlSpecies As ComboBox
    Private WithEvents lblSpecies As Label
    Private WithEvents ddlTheme As ComboBox
    Private WithEvents lblTheme As Label
    Private WithEvents ddlCategory As ComboBox
    Private WithEvents lblCategory As Label
    Private WithEvents chkLockComments As CheckBox
    Private WithEvents chkScraps As CheckBox
    Private WithEvents btnPost As Button
    Private WithEvents ddlRating As ComboBox
    Private WithEvents lblRating As Label
End Class

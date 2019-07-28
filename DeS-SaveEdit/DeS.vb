Imports DeSEditor.PS3FileSystem
Imports System.IO

Public Class DeS
    Public Shared bytes() As Byte
    Public Shared folder
    Public Shared filename
    Public Shared modified = False
    Public Shared manager As Ps3SaveManager
    Public Shared file As Ps3File
    Public Shared SecureID() As Byte = {&H1, &H23, &H45, &H67, &H89, &HAB, &HCD, &HEF, &HFE, &HDC, &HBA, &H98, &H76, &H54, &H32, &H10}

    Public Shared cllHairstyles() As UInteger
    Public Shared cllRings() As UInteger
    Public Shared cllWeapons() As UInteger
    Public Shared cllArmor() As UInteger
    Public Shared cllStartClass() As UInteger
    Public Shared cllItems() As UInteger
    Public Shared cllSpells() As UInteger
    Public Shared cllSpellStatus() As String

    Public Shared inventory(,) As UInteger




    Private Function FourByteFloat(ByRef bytes, start) As String
        Return HexToSingle(Hex(bytes(start)).PadLeft(2, "0"c).ToString & Hex(bytes(start + 1)).PadLeft(2, "0"c).ToString & Hex(bytes(start + 2)).PadLeft(2, "0"c).ToString & Hex(bytes(start + 3)).PadLeft(2, "0"c).ToString)
    End Function
    Private Function FourByteInt32(ByRef bytes, start) As Int32
        Dim value = 0
        For i = 0 To 3
            value = value + bytes(start + i) * (256 ^ (3 - i))
        Next
        Return value
    End Function
    Private Function FourByteUInt32(ByRef bytes, start) As UInteger
        Dim value As UInteger = 0
        For i = 0 To 3
            value = value + bytes(start + i) * (256 ^ (3 - i))
        Next
        Return value
    End Function
    Private Function TwoByteInt16(ByRef bytes, start) As Int32
        Dim value = 0
        For i = 0 To 1
            value = value + bytes(start + i) * (256 ^ (1 - i))
        Next
        Return value
    End Function

    Private Function Int32ToFourByte(ByVal val As String) As Byte()
        If val.Length > 0 Then
            Return ReverseFourBytes(BitConverter.GetBytes(Convert.ToInt32(val)))
        Else
            Return {0, 0, 0, 0}
        End If
    End Function
    Private Function Int32ToFourByte(ByVal val As Integer) As Byte()
        Return ReverseFourBytes(BitConverter.GetBytes(Convert.ToInt32(val)))
    End Function
    Private Function UInt32ToFourByte(ByVal val As UInteger) As Byte()
        Return ReverseFourBytes(BitConverter.GetBytes(Convert.ToUInt32(val)))
    End Function
    Private Function FloatToFourByte(ByVal val As String) As Byte()
        If IsNumeric(val) Then
            Return ReverseFourBytes(BitConverter.GetBytes(Convert.ToSingle(val)))
        Else
            Return {0, 0, 0, 0}
        End If
    End Function

    Private Function OneByteAnd(ByVal loc As UInteger, ByVal cmp As UInteger) As Boolean
        Return ((bytes(loc) And cmp) > 0)
    End Function
    Private Function GetUnicodeStr(ByVal loc As UInteger, ByVal len As UInteger) As String
        Dim buildstr As String = ""

        For i As UInteger = 0 To len
            If bytes(loc + i) > 0 Then
                buildstr = buildstr & Convert.ToChar(bytes(loc + i))
            Else
                If bytes(loc + i + 1) = 0 Then Exit For
            End If
        Next
        Return buildstr
    End Function

    Private Function HexToSingle(ByVal hexValue As String) As Single
        Dim iInputIndex As Integer = 0
        Dim iOutputIndex As Integer = 0
        Dim bArray(3) As Byte

        For iInputIndex = 0 To hexValue.Length - 1 Step 2
            bArray(iOutputIndex) = Byte.Parse(hexValue.Chars(iInputIndex) & hexValue.Chars(iInputIndex + 1), Globalization.NumberStyles.HexNumber)
            iOutputIndex += 1
        Next

        Array.Reverse(bArray)
        Return BitConverter.ToSingle(bArray, 0)

    End Function
    Private Function ReverseFourBytes(ByVal byt() As Byte)
        Return {byt(3), byt(2), byt(1), byt(0)}
    End Function
    Private Sub InsBytes(ByVal loc As UInteger, ByVal byt As Byte())
        For i = 0 To byt.Length - 1
            bytes(loc + i) = byt(i)
        Next
    End Sub

    Private Sub btnDeSOpen_Click(sender As System.Object, e As System.EventArgs) Handles btnDeSOpen.Click


        ' Try
            filename = "PARAM.SFO"
            bytes = System.IO.File.ReadAllBytes(txtDeSFolder.Text & "\" & filename)

            txtProfNum.Text = bytes(&H570)

            manager = New Ps3SaveManager(txtDeSFolder.Text, SecureID)
            filename = "\USER.DAT"

            ' file = manager.Files.FirstOrDefault(Function(t) t.PFDEntry.file_name = filename)
            file = manager.Files.FirstOrDefault(Function(t) t.FilePath.EndsWith(filename))
            bytes = file.DecryptToBytes

            txtWorld.Text = Convert.ToUInt16(bytes(&H4))
            txtBlock.Text = Convert.ToUInt16(bytes(&H5))

            txtCurrHP.Text = FourByteInt32(bytes, &H50)
            txtMaxHP.Text = FourByteInt32(bytes, &H58)
            txtCurrMP.Text = FourByteInt32(bytes, &H5C)
            txtMaxMP.Text = FourByteInt32(bytes, &H64)
            txtCurrStam.Text = FourByteInt32(bytes, &H6C)
            txtMaxStam.Text = FourByteInt32(bytes, &H74)
            txtVit.Text = FourByteInt32(bytes, &H80)
            txtInt.Text = FourByteInt32(bytes, &H88)
            txtEnd.Text = FourByteInt32(bytes, &H90)
            txtStr.Text = FourByteInt32(bytes, &H98)
            txtDex.Text = FourByteInt32(bytes, &HA0)
            txtMagic.Text = FourByteInt32(bytes, &HA8)
            txtFaith.Text = FourByteInt32(bytes, &HB0)
            txtLuck.Text = FourByteInt32(bytes, &HB8)
            txtSouls.Text = FourByteInt32(bytes, &HC0)
            txtSoulMem.Text = FourByteInt32(bytes, &HC8)
            txtLevelsPurchased.Text = FourByteInt32(bytes, &HCC)

            txtName.Text = GetUnicodeStr(&HD4, &H21)

            cmbGender.SelectedIndex = bytes(&HF6)

            cmbStartClass.SelectedIndex = bytes(&HFB)

            cmbLeftHand1.SelectedIndex = Array.IndexOf(cllWeapons, FourByteUInt32(bytes, &H28C))
            cmbRightHand1.SelectedIndex = Array.IndexOf(cllWeapons, FourByteUInt32(bytes, &H290))
            cmbLeftHand2.SelectedIndex = Array.IndexOf(cllWeapons, FourByteUInt32(bytes, &H294))
            cmbRightHand2.SelectedIndex = Array.IndexOf(cllWeapons, FourByteUInt32(bytes, &H298))

            cmbArrows.SelectedIndex = Array.IndexOf(cllWeapons, FourByteUInt32(bytes, &H29C))
            cmbBolts.SelectedIndex = Array.IndexOf(cllWeapons, FourByteUInt32(bytes, &H2A0))

            cmbHelmet.SelectedIndex = Array.IndexOf(cllArmor, FourByteUInt32(bytes, &H2A4))
            cmbChest.SelectedIndex = Array.IndexOf(cllArmor, FourByteUInt32(bytes, &H2A8))
            cmbGauntlets.SelectedIndex = Array.IndexOf(cllArmor, FourByteUInt32(bytes, &H2AC))
            cmbLeggings.SelectedIndex = Array.IndexOf(cllArmor, FourByteUInt32(bytes, &H2B0))

            cmbHairstyle.SelectedIndex = Array.IndexOf(cllHairstyles, FourByteUInt32(bytes, &H2B4))

            cmbRing1.SelectedIndex = Array.IndexOf(cllRings, FourByteUInt32(bytes, &H2B8))
            cmbRing2.SelectedIndex = Array.IndexOf(cllRings, FourByteUInt32(bytes, &H2BC))

            cmbQuickSlot1.SelectedIndex = Array.IndexOf(cllItems, FourByteUInt32(bytes, &H2C0))
            cmbQuickSlot2.SelectedIndex = Array.IndexOf(cllItems, FourByteUInt32(bytes, &H2C4))
            cmbQuickSlot3.SelectedIndex = Array.IndexOf(cllItems, FourByteUInt32(bytes, &H2C8))
            cmbQuickSlot4.SelectedIndex = Array.IndexOf(cllItems, FourByteUInt32(bytes, &H2CC))
            cmbQuickSlot5.SelectedIndex = Array.IndexOf(cllItems, FourByteUInt32(bytes, &H2D0))


            Dim invCount As UInteger = FourByteUInt32(bytes, &H2D4)
            ReDim inventory(invCount, 8)

            dgvWeapons.Rows.Clear()
            dgvArmor.Rows.Clear()
            dgvRings.Rows.Clear()
            dgvGoods.Rows.Clear()
            dgvSpells.Rows.Clear()

            Dim i As UInteger
            Dim Type As UInteger
            Dim ItemID As UInteger
            Dim ItemCount As UInteger
            Dim Misc1 As UInteger
            Dim Misc2 As UInteger
            Dim Misc3 As UInteger

            For i = 0 To invCount - 1
                Type = FourByteUInt32(bytes, &H2DC + i * &H20)
                ItemID = FourByteUInt32(bytes, &H2E0 + i * &H20)
                ItemCount = FourByteUInt32(bytes, &H2E4 + i * &H20)
                Misc1 = FourByteUInt32(bytes, &H2E8 + i * &H20)
                Misc2 = FourByteUInt32(bytes, &H2EC + i * &H20)
                Misc3 = FourByteUInt32(bytes, &H2F0 + i * &H20)

                Select Case Type
                    Case 0
                        dgvWeapons.Rows.Add(cmbLeftHand1.Items(Array.IndexOf(cllWeapons, ItemID)), ItemCount, Misc1, Misc2, Misc3)
                    Case &H10000000
                        dgvArmor.Rows.Add(cmbChest.Items(Array.IndexOf(cllArmor, ItemID)), ItemCount, Misc1, Misc2, Misc3)
                    Case &H20000000
                        dgvRings.Rows.Add(cmbRing1.Items(Array.IndexOf(cllRings, ItemID)), ItemCount, Misc1, Misc2, Misc3)
                    Case &H40000000
                        dgvGoods.Rows.Add(cmbQuickSlot1.Items(Array.IndexOf(cllItems, ItemID)), ItemCount, Misc1, Misc2, Misc3)
                End Select
            Next

            txtSpellSlots.Text = FourByteUInt32(bytes, &H102E0)
            txtMiracleSlots.Text = FourByteUInt32(bytes, &H1030C)

            txtHairR.Text = Math.Round(Val(FourByteFloat(bytes, &H14368)), 3)
            txtHairG.Text = Math.Round(Val(FourByteFloat(bytes, &H1436C)), 3)
            txtHairB.Text = Math.Round(Val(FourByteFloat(bytes, &H14370)), 3)

            Dim spellCount As UInteger = FourByteUInt32(bytes, &H143E8)
            Dim spellStatus As UInteger
            Dim spellID As UInteger


            For i = 0 To spellCount - 1
                spellStatus = FourByteUInt32(bytes, &H143EC + i * &H10)
                spellID = FourByteUInt32(bytes, &H143F0 + i * &H10)
                Misc1 = FourByteUInt32(bytes, &H143F4 + i * &H10)
                Misc2 = FourByteUInt32(bytes, &H143F8 + i * &H10)

                dgvSpells.Rows.Add(cmbSpellSlot1.Items(Array.IndexOf(cllSpells, spellID)), cllSpellStatus(spellStatus), Misc1, Misc2)
            Next

            txtCharTendency.Text = FourByteFloat(bytes, &H1EBF0)
            txtNexusTendency.Text = FourByteFloat(bytes, &H1EBF8)
            txtW1Tendency.Text = FourByteFloat(bytes, &H1EC00)
            txtW2Tendency.Text = FourByteFloat(bytes, &H1EC20)
            txtW3Tendency.Text = FourByteFloat(bytes, &H1EC10)
            txtW4Tendency.Text = FourByteFloat(bytes, &H1EC08)
            txtW5Tendency.Text = FourByteFloat(bytes, &H1EC18)

            chkArchSealed.Checked = Not OneByteAnd(&H1F965, &H40)

        ' Catch ex As Exception
        '     MsgBox("Failed to open save.  Either you or I did something dumb..." & ex.Message)
        ' End Try

    End Sub
    Private Sub btnDeSSave_Click(sender As System.Object, e As System.EventArgs) Handles btnDeSSave.Click

        ' Try
            ' filename = "PARAM.SFO"
            ' bytes = System.IO.File.ReadAllBytes(txtDeSFolder.Text & "\" & filename)
            ' bytes(&H570) = Val(txtProfNum.Text)
            ' System.IO.File.WriteAllBytes(txtDeSFolder.Text & "\" & filename, bytes)


            filename = "\USER.DAT"
            file = manager.Files.FirstOrDefault(Function(t) t.FilePath.EndsWith(filename))
            bytes = file.DecryptToBytes

            bytes(&H4) = Val(txtWorld.Text)
            bytes(&H5) = Val(txtBlock.Text)

            bytes(&H6) = 0
            bytes(&H7) = 0

            InsBytes(&H50, Int32ToFourByte(txtCurrHP.Text))
            InsBytes(&H58, Int32ToFourByte(txtMaxHP.Text))
            InsBytes(&H5C, Int32ToFourByte(txtCurrMP.Text))
            InsBytes(&H64, Int32ToFourByte(txtMaxMP.Text))
            InsBytes(&H6C, Int32ToFourByte(txtCurrStam.Text))
            InsBytes(&H74, Int32ToFourByte(txtMaxStam.Text))

            InsBytes(&H7C, Int32ToFourByte(txtVit.Text))
            InsBytes(&H80, Int32ToFourByte(txtVit.Text))
            InsBytes(&H84, Int32ToFourByte(txtInt.Text))
            InsBytes(&H88, Int32ToFourByte(txtInt.Text))
            InsBytes(&H8C, Int32ToFourByte(txtEnd.Text))
            InsBytes(&H90, Int32ToFourByte(txtEnd.Text))
            InsBytes(&H94, Int32ToFourByte(txtStr.Text))
            InsBytes(&H98, Int32ToFourByte(txtStr.Text))
            InsBytes(&H9C, Int32ToFourByte(txtDex.Text))
            InsBytes(&HA0, Int32ToFourByte(txtDex.Text))
            InsBytes(&HA4, Int32ToFourByte(txtMagic.Text))
            InsBytes(&HA8, Int32ToFourByte(txtMagic.Text))
            InsBytes(&HAC, Int32ToFourByte(txtFaith.Text))
            InsBytes(&HB0, Int32ToFourByte(txtFaith.Text))
            InsBytes(&HB4, Int32ToFourByte(txtLuck.Text))
            InsBytes(&HB8, Int32ToFourByte(txtLuck.Text))

            InsBytes(&HBC, Int32ToFourByte(txtSouls.Text))
            InsBytes(&HC8, Int32ToFourByte(txtSoulMem.Text))

            InsBytes(&HCC, Int32ToFourByte(txtLevelsPurchased.Text))

            For i = 0 To &H10
                If i < txtName.Text.Length Then
                    bytes(&HD5 + i * 2) = Microsoft.VisualBasic.Asc(txtName.Text(i))
                Else
                    bytes(&HD5 + i * 2) = 0
                End If
                bytes(&HD5 + i * 2 + 1) = 0
            Next

            bytes(&HF6) = cmbGender.SelectedIndex

            bytes(&HFB) = cmbStartClass.SelectedIndex

            InsBytes(&H28C, UInt32ToFourByte(cllWeapons(cmbLeftHand1.SelectedIndex)))
            InsBytes(&H290, UInt32ToFourByte(cllWeapons(cmbRightHand1.SelectedIndex)))
            InsBytes(&H294, UInt32ToFourByte(cllWeapons(cmbLeftHand2.SelectedIndex)))
            InsBytes(&H298, UInt32ToFourByte(cllWeapons(cmbRightHand2.SelectedIndex)))

            InsBytes(&H29C, UInt32ToFourByte(cllWeapons(cmbArrows.SelectedIndex)))
            InsBytes(&H2A0, UInt32ToFourByte(cllWeapons(cmbBolts.SelectedIndex)))

            InsBytes(&H2A4, UInt32ToFourByte(cllArmor(cmbHelmet.SelectedIndex)))
            InsBytes(&H2A8, UInt32ToFourByte(cllArmor(cmbChest.SelectedIndex)))
            InsBytes(&H2AC, UInt32ToFourByte(cllArmor(cmbGauntlets.SelectedIndex)))
            InsBytes(&H2B0, UInt32ToFourByte(cllArmor(cmbLeggings.SelectedIndex)))

            InsBytes(&H2B4, UInt32ToFourByte(cllHairstyles(cmbHairstyle.SelectedIndex)))
            InsBytes(&H2B8, UInt32ToFourByte(cllRings(cmbRing1.SelectedIndex)))
            InsBytes(&H2BC, UInt32ToFourByte(cllRings(cmbRing2.SelectedIndex)))

            REM InsBytes(&H2C0, UInt32ToFourByte(cllItems(cmbQuickSlot1.SelectedIndex)))
            REM InsBytes(&H2C4, UInt32ToFourByte(cllItems(cmbQuickSlot2.SelectedIndex)))
            REM InsBytes(&H2C8, UInt32ToFourByte(cllItems(cmbQuickSlot3.SelectedIndex)))
            REM InsBytes(&H2CC, UInt32ToFourByte(cllItems(cmbQuickSlot4.SelectedIndex)))
            REM InsBytes(&H2D0, UInt32ToFourByte(cllItems(cmbQuickSlot5.SelectedIndex)))
            InsBytes(&H2D4, UInt32ToFourByte(dgvWeapons.Rows.Count + dgvArmor.Rows.Count + dgvRings.Rows.Count + dgvGoods.Rows.Count - 4))

            Dim invslot = 0

            If dgvWeapons.Rows.Count > 0 Then
                For i = 0 To dgvWeapons.Rows.Count - 2
                    InsBytes(&H2DC + invslot * &H20, UInt32ToFourByte(0))
                    InsBytes(&H2E0 + invslot * &H20, UInt32ToFourByte(cllWeapons(cmbLeftHand1.FindStringExact(dgvWeapons.Rows(i).Cells(0).FormattedValue))))
                    InsBytes(&H2E4 + invslot * &H20, UInt32ToFourByte(dgvWeapons.Rows(i).Cells(1).FormattedValue))
                    InsBytes(&H2E8 + invslot * &H20, UInt32ToFourByte(dgvWeapons.Rows(i).Cells(2).FormattedValue))
                    InsBytes(&H2EC + invslot * &H20, UInt32ToFourByte(dgvWeapons.Rows(i).Cells(3).FormattedValue))
                    InsBytes(&H2F0 + invslot * &H20, UInt32ToFourByte(dgvWeapons.Rows(i).Cells(4).FormattedValue))
                    InsBytes(&H2F4 + invslot * &H20, UInt32ToFourByte(0))
                    InsBytes(&H2F8 + invslot * &H20, UInt32ToFourByte(0))
                    invslot += 1
                Next
            End If

            If dgvArmor.Rows.Count > 0 Then
                For i = 0 To dgvArmor.Rows.Count - 2
                    InsBytes(&H2DC + invslot * &H20, UInt32ToFourByte(&H10000000))
                    InsBytes(&H2E0 + invslot * &H20, UInt32ToFourByte(cllArmor(cmbChest.FindStringExact(dgvArmor.Rows(i).Cells(0).FormattedValue))))
                    InsBytes(&H2E4 + invslot * &H20, UInt32ToFourByte(dgvArmor.Rows(i).Cells(1).FormattedValue))
                    InsBytes(&H2E8 + invslot * &H20, UInt32ToFourByte(dgvArmor.Rows(i).Cells(2).FormattedValue))
                    InsBytes(&H2EC + invslot * &H20, UInt32ToFourByte(dgvArmor.Rows(i).Cells(3).FormattedValue))
                    InsBytes(&H2F0 + invslot * &H20, UInt32ToFourByte(dgvArmor.Rows(i).Cells(4).FormattedValue))
                    InsBytes(&H2F4 + invslot * &H20, UInt32ToFourByte(0))
                    InsBytes(&H2F8 + invslot * &H20, UInt32ToFourByte(0))
                    invslot += 1
                Next
            End If

            If dgvRings.Rows.Count > 0 Then
                For i = 0 To dgvRings.Rows.Count - 2
                    InsBytes(&H2DC + invslot * &H20, UInt32ToFourByte(&H20000000))
                    InsBytes(&H2E0 + invslot * &H20, UInt32ToFourByte(cllRings(cmbRing1.FindStringExact(dgvRings.Rows(i).Cells(0).FormattedValue))))
                    InsBytes(&H2E4 + invslot * &H20, UInt32ToFourByte(dgvRings.Rows(i).Cells(1).FormattedValue))
                    InsBytes(&H2E8 + invslot * &H20, UInt32ToFourByte(dgvRings.Rows(i).Cells(2).FormattedValue))
                    InsBytes(&H2EC + invslot * &H20, UInt32ToFourByte(dgvRings.Rows(i).Cells(3).FormattedValue))
                    InsBytes(&H2F0 + invslot * &H20, UInt32ToFourByte(dgvRings.Rows(i).Cells(4).FormattedValue))
                    InsBytes(&H2F4 + invslot * &H20, UInt32ToFourByte(0))
                    InsBytes(&H2F8 + invslot * &H20, UInt32ToFourByte(0))
                    invslot += 1
                Next
            End If

            If dgvGoods.Rows.Count > 0 Then
                For i = 0 To dgvGoods.Rows.Count - 2
                    InsBytes(&H2DC + invslot * &H20, UInt32ToFourByte(&H40000000))
                    InsBytes(&H2E0 + invslot * &H20, UInt32ToFourByte(cllItems(cmbQuickSlot1.FindStringExact(dgvGoods.Rows(i).Cells(0).FormattedValue))))
                    InsBytes(&H2E4 + invslot * &H20, UInt32ToFourByte(dgvGoods.Rows(i).Cells(1).FormattedValue))
                    InsBytes(&H2E8 + invslot * &H20, UInt32ToFourByte(dgvGoods.Rows(i).Cells(2).FormattedValue))
                    InsBytes(&H2EC + invslot * &H20, UInt32ToFourByte(dgvGoods.Rows(i).Cells(3).FormattedValue))
                    InsBytes(&H2F0 + invslot * &H20, UInt32ToFourByte(dgvGoods.Rows(i).Cells(4).FormattedValue))
                    InsBytes(&H2F4 + invslot * &H20, UInt32ToFourByte(0))
                    InsBytes(&H2F8 + invslot * &H20, UInt32ToFourByte(0))
                    invslot += 1
                Next
            End If

            InsBytes(&H102E0, UInt32ToFourByte(Val(txtSpellSlots.Text)))
            InsBytes(&H1030C, UInt32ToFourByte(Val(txtMiracleSlots.Text)))

            InsBytes(&H143E8, UInt32ToFourByte(dgvSpells.Rows.Count - 1))
            invslot = 0
            If dgvSpells.Rows.Count > 0 Then
                For i = 0 To dgvSpells.Rows.Count - 2
                    InsBytes(&H143EC + invslot * &H10, UInt32ToFourByte(Array.IndexOf(cllSpellStatus, dgvSpells.Rows(i).Cells(1).FormattedValue)))
                    InsBytes(&H143F0 + invslot * &H10, UInt32ToFourByte(cllSpells(cmbSpellSlot1.FindStringExact(dgvSpells.Rows(i).Cells(0).FormattedValue))))
                    InsBytes(&H143F4 + invslot * &H10, UInt32ToFourByte(dgvSpells.Rows(i).Cells(2).FormattedValue))
                    InsBytes(&H143F8 + invslot * &H10, UInt32ToFourByte(dgvSpells.Rows(i).Cells(3).FormattedValue))
                    invslot += 1
                Next
            End If

            InsBytes(&H14368, FloatToFourByte(txtHairR.Text))
            InsBytes(&H1436C, FloatToFourByte(txtHairG.Text))
            InsBytes(&H14370, FloatToFourByte(txtHairB.Text))


            InsBytes(&H1EBF0, FloatToFourByte(txtCharTendency.Text))

            InsBytes(&H1EBF8, FloatToFourByte(txtNexusTendency.Text))
            InsBytes(&H1EBFC, FloatToFourByte(txtNexusTendency.Text))
            InsBytes(&H1EC00, FloatToFourByte(txtW1Tendency.Text))
            InsBytes(&H1EC04, FloatToFourByte(txtW1Tendency.Text))
            InsBytes(&H1EC08, FloatToFourByte(txtW4Tendency.Text))
            InsBytes(&H1EC0C, FloatToFourByte(txtW4Tendency.Text))
            InsBytes(&H1EC10, FloatToFourByte(txtW3Tendency.Text))
            InsBytes(&H1EC14, FloatToFourByte(txtW3Tendency.Text))
            InsBytes(&H1EC18, FloatToFourByte(txtW5Tendency.Text))
            InsBytes(&H1EC1C, FloatToFourByte(txtW5Tendency.Text))
            InsBytes(&H1EC20, FloatToFourByte(txtW2Tendency.Text))
            InsBytes(&H1EC24, FloatToFourByte(txtW2Tendency.Text))

            bytes(&H1F965) = (bytes(&H1F965) And &HBF) Or &H40 * ((Not chkArchSealed.Checked) * -1)

            file.Encrypt(bytes)
            ' manager.ReBuildChanges()




            ' filename = "104USER.DAT"
            ' file = manager.Files.FirstOrDefault(Function(t) t.PFDEntry.file_name = filename)
            ' bytes = file.DecryptToBytes

            ' For i = 0 To &H10
            '     If i < txtName.Text.Length Then
            '         bytes(&H21D + i * 2) = Microsoft.VisualBasic.Asc(txtName.Text(i))
            '     Else
            '         bytes(&H21D + i * 2) = 0
            '     End If
            '     bytes(&H21D + i * 2 + 1) = 0
            ' Next

            ' file.Encrypt(bytes)
            ' manager.ReBuildChanges()

            MsgBox("Save Completed")
        ' Catch ex As Exception
        '     MsgBox("Save failed, no specific reason.  Either you or I did something dumb..." & ex.Message)
        ' End Try
    End Sub

    Private Sub txtDeSBrowse_Click(sender As System.Object, e As System.EventArgs) Handles btnDeSBrowse.Click
        Dim openDlg As New OpenFileDialog()
        openDlg.Filter = "DeS Save File|PARAM.SFO"
        openDlg.Title = "Open your save file"

        If openDlg.ShowDialog() = Windows.Forms.DialogResult.OK Then txtDeSFolder.Text = Microsoft.VisualBasic.Left(openDlg.FileName, openDlg.FileName.Length - 9)
    End Sub
    Private Sub txtDeSFile_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtDeSFolder.TextChanged
        folder = UCase(txtDeSFolder.Text)
    End Sub
    Private Sub btnSendMoney_Click(sender As Object, e As EventArgs) Handles btnSendMoney.Click
        System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=D7UD87LN43ERN")
    End Sub

    Sub InitArrays()
        cllSpells = {&H0, &H1, &H2, &H3, &H4, &H64, &H3E8, &H3E9, &H3EA, &H3EB, &H3EC, &H3ED, &H3EE, &H3EF, &H3F0, &H3F1, &H3F2, &H3F3, &H3F4, &H3F5, &H3F6, &H3F7, &H3F8, &H3F9, &H3FA, &H3FB, &H3FC, &H3FD, &H7D0, &H7D1, &H7D2, &H7D3, &H7D4, &H7D5, &H7D6, &H7D7, &H7D8, &H7D9, &H7DA, &H7DB}
        cllRings = {0, &H64, &H65, &H66, &H67, &H68, &H69, &H6A, &H6B, &H6C, &H6D, &H6E, &H6F, &H70, &H71, _
                    &H72, &H73, &H74, &H75, &H76, &H77, &H78, &H79, &H7A, &H7B, &H7C, &H7D, &H7E, &HFFFFFFFF&}
        cllHairstyles = {&H7A120, &H7A184, &H7A1E8, &H7A24C, &H7A2B0, &H7A314, &H7A378, &H7A3DC, &H7A440, &H7A4A4, &H7A508, _
                         &H7A56C, &H7A5D0, &H7A634, &H7A698, &H7A6FC, &H7A760, &H7A7C4, &H7A828, &H7A88C, &H900B0, &HFFFFFFFF&}
        cllWeapons = {&H1, &H2, &H2710, &H2711, &H2712, &H2713, &H2714, &H2715, &H2716, &H2717, _
            &H2718, &H2719, &H271A, &H271B, &H271C, &H271D, &H271E, &H271F, &H2725, &H2726, _
            &H2727, &H2728, &H2729, &H272F, &H2730, &H2731, &H2732, &H2733, &H2739, &H273A, _
            &H273B, &H273C, &H273D, &H2743, &H2744, &H2745, &H2746, &H2747, &H2774, &H2775, _
            &H2776, &H2777, &H2778, &H2779, &H277A, &H277B, &H277C, &H277D, &H277E, &H277F, _
            &H2780, &H2781, &H2782, &H2783, &H2789, &H278A, &H278B, &H278C, &H278D, &H2793, _
            &H2794, &H2795, &H2796, &H2797, &H279D, &H279E, &H279F, &H27A0, &H27A1, &H27A7, _
            &H27A8, &H27A9, &H27AA, &H27AB, &H27D8, &H27D9, &H27DA, &H27DB, &H27DC, &H27DD, _
            &H27DE, &H27DF, &H27E0, &H27E1, &H27E2, &H27E3, &H27E4, &H27E5, &H27E6, &H27E7, _
            &H27ED, &H27EE, &H27EF, &H27F0, &H27F1, &H27F7, &H27F8, &H27F9, &H27FA, &H27FB, _
            &H2801, &H2802, &H2803, &H2804, &H2805, &H280B, &H280C, &H280D, &H280E, &H280F, _
            &H28A0, &H28A1, &H28A2, &H28A3, &H28A4, &H28A5, &H2904, &H2905, &H2906, &H2907, _
            &H2908, &H2909, &H2968, &H2969, &H296A, &H296B, &H296C, &H296D, &H29CC, &H29CD, _
            &H29CE, &H29CF, &H29D0, &H29D1, &H29D2, &H29D3, &H29D4, &H29D5, &H29D6, &H29D7, _
            &H29D8, &H29D9, &H29DA, &H29DB, &H29E1, &H29E2, &H29E3, &H29EB, &H29EC, &H29ED, _
            &H29EE, &H29EF, &H29F5, &H29F6, &H29F7, &H29F8, &H29F9, &H29FF, &H2A00, &H2A01, _
            &H2A02, &H2A03, &H2A09, &H2A0A, &H2A0B, &H2A0C, &H2A0D, &H4A38, &H4E20, &H4E21, _
            &H4E22, &H4E23, &H4E24, &H4E25, &H4E26, &H4E27, &H4E28, &H4E29, &H4E2A, &H4E2B, _
            &H4E2C, &H4E2D, &H4E2E, &H4E2F, &H4E35, &H4E36, &H4E37, &H4E38, &H4E39, &H4E3F, _
            &H4E40, &H4E41, &H4E42, &H4E43, &H4E49, &H4E4A, &H4E4B, &H4E4C, &H4E4D, &H4E53, _
            &H4E54, &H4E55, &H4E56, &H4E57, &H4E84, &H4E85, &H4E86, &H4E87, &H4E88, &H4E89, _
            &H4E8A, &H4E8B, &H4E8C, &H4E8D, &H4E8E, &H4E8F, &H4E90, &H4E91, &H4E92, &H4E93, _
            &H4E99, &H4E9A, &H4E9B, &H4E9C, &H4E9D, &H4EA3, &H4EA4, &H4EA5, &H4EA6, &H4EA7, _
            &H4EAD, &H4EAE, &H4EAF, &H4EB0, &H4EB1, &H4EB7, &H4EB8, &H4EB9, &H4EBA, &H4EBB, _
            &H4EC1, &H4EC2, &H4EC3, &H4EC4, &H4EC5, &H4EE8, &H4EE9, &H4EEA, &H4EEB, &H4EEC, _
            &H4EED, &H4EEE, &H4EEF, &H4EF0, &H4EF1, &H4EF2, &H4EF3, &H4EF4, &H4EF5, &H4EF6, _
            &H4EF7, &H4EFD, &H4EFE, &H4EFF, &H4F00, &H4F01, &H4F07, &H4F08, &H4F09, &H4F0A, _
            &H4F0B, &H4F11, &H4F12, &H4F13, &H4F14, &H4F15, &H4F1B, &H4F1C, &H4F1D, &H4F1E, _
            &H4F1F, &H4F4C, &H4F4D, &H4F4E, &H4F4F, &H4F50, &H4F51, &H4F52, &H4F53, &H4F54, _
            &H4F55, &H4F56, &H4F57, &H4F58, &H4F59, &H4F5A, &H4F5B, &H4F61, &H4F62, &H4F63, _
            &H4F64, &H4F65, &H4F6B, &H4F6C, &H4F6D, &H4F6E, &H4F6F, &H4F75, &H4F76, &H4F77, _
            &H4F78, &H4F79, &H4F7F, &H4F80, &H4F81, &H4F82, &H4F83, &H4F89, &H4F8A, &H4F8B, _
            &H4F8C, &H4F8D, &H4FB0, &H4FB1, &H4FB2, &H4FB3, &H4FB4, &H4FB5, &H4FB6, &H4FB7, _
            &H4FB8, &H4FB9, &H4FBA, &H4FBB, &H4FBC, &H4FBD, &H4FBE, &H4FBF, &H4FC5, &H4FC6, _
            &H4FC7, &H4FC8, &H4FC9, &H4FCF, &H4FD0, &H4FD1, &H4FD2, &H4FD3, &H4FD9, &H4FDA, _
            &H4FDB, &H4FDC, &H4FDD, &H4FE3, &H4FE4, &H4FE5, &H4FE6, &H4FE7, &H5014, &H5015, _
            &H5016, &H5017, &H5018, &H5019, &H501A, &H501B, &H501C, &H501D, &H501E, &H501F, _
            &H5020, &H5021, &H5022, &H5023, &H5029, &H502A, &H502B, &H502C, &H502D, &H5033, _
            &H5034, &H5035, &H5036, &H5037, &H503D, &H503E, &H503F, &H5040, &H5041, &H5047, _
            &H5048, &H5049, &H504A, &H504B, &H5078, &H5079, &H507A, &H507B, &H507C, &H507D, _
            &H507E, &H507F, &H5080, &H5081, &H5082, &H5083, &H5084, &H5085, &H5086, &H5087, _
            &H508D, &H508E, &H508F, &H5090, &H5091, &H5097, &H5098, &H5099, &H509A, &H509B, _
            &H50A1, &H50A2, &H50A3, &H50A4, &H50A5, &H50AB, &H50AC, &H50AD, &H50AE, &H50AF, _
            &H50DC, &H50DD, &H50DE, &H50DF, &H50E0, &H50E1, &H5140, &H5141, &H5142, &H5143, _
            &H5144, &H5145, &H51A4, &H5208, &H526C, &H526D, &H526E, &H526F, &H5270, &H5271, _
            &H5272, &H5273, &H5334, &H5335, &H5336, &H5337, &H5338, &H5339, &H533A, &H533B, _
            &H533C, &H533D, &H533E, &H533F, &H5340, &H5341, &H5342, &H5343, &H5349, &H534A, _
            &H534B, &H534C, &H534D, &H5353, &H5354, &H5355, &H5356, &H5357, &H535D, &H535E, _
            &H535F, &H5360, &H5361, &H5367, &H5368, &H5369, &H536A, &H536B, &H5398, &H53FC, _
            &H53FD, &H5460, &H5461, &H5462, &H5463, &H5464, &H5465, &H54C4, &H5528, &H5529, _
            &H552A, &H552B, &H552C, &H552D, &H558C, &H558D, &H558E, &H558F, &H5590, &H5591, _
            &H7148, &H7530, &H7531, &H7532, &H7533, &H7534, &H7535, &H7536, &H7537, &H7538, _
            &H7539, &H753A, &H753B, &H753C, &H753D, &H753E, &H753F, &H7545, &H7546, &H7547, _
            &H7548, &H7549, &H754F, &H7550, &H7551, &H7552, &H7553, &H7559, &H755A, &H755B, _
            &H755C, &H755D, &H7563, &H7564, &H7565, &H7566, &H7567, &H7594, &H7595, &H7596, _
            &H7597, &H7598, &H7599, &H759A, &H759B, &H759C, &H759D, &H759E, &H759F, &H75A0, _
            &H75A1, &H75A2, &H75A3, &H75A9, &H75AA, &H75AB, &H75AC, &H75AD, &H75B3, &H75B4, _
            &H75B5, &H75B6, &H75B7, &H75BD, &H75BE, &H75BF, &H75C0, &H75C1, &H75C7, &H75C8, _
            &H75C9, &H75CA, &H75CB, &H75F8, &H75F9, &H75FA, &H75FB, &H75FC, &H75FD, &H765C, _
            &H765D, &H765E, &H765F, &H7660, &H7661, &H7662, &H7663, &H7664, &H7665, &H7666, _
            &H7667, &H7668, &H7669, &H766A, &H766B, &H7671, &H7672, &H7673, &H7674, &H7675, _
            &H767B, &H767C, &H767D, &H767E, &H767F, &H7685, &H7686, &H7687, &H7688, &H7689, _
            &H768F, &H7690, &H7691, &H7692, &H7693, &H76C0, &H76C1, &H76C2, &H76C3, &H76C4, _
            &H76C5, &H76C6, &H76C7, &H9858, &H9C40, &H9C41, &H9C42, &H9C43, &H9C44, &H9C45, _
            &H9C46, &H9C47, &H9C48, &H9C49, &H9C4A, &H9C4B, &H9C4C, &H9C4D, &H9C4E, &H9C4F, _
            &H9C55, &H9C56, &H9C57, &H9C58, &H9C59, &H9C5F, &H9C60, &H9C61, &H9C62, &H9C63, _
            &H9C69, &H9C6A, &H9C6B, &H9C6C, &H9C6D, &H9C73, &H9C74, &H9C75, &H9C76, &H9C77, _
            &H9C7D, &H9C7E, &H9C7F, &H9C80, &H9C81, &H9CA4, &H9CA5, &H9CA6, &H9CA7, &H9CA8, _
            &H9CA9, &H9CAA, &H9CAB, &H9CAC, &H9CAD, &H9CAE, &H9CAF, &H9CB0, &H9CB1, &H9CB2, _
            &H9CB3, &H9CB9, &H9CBA, &H9CBB, &H9CBC, &H9CBD, &H9CC3, &H9CC4, &H9CC5, &H9CC6, _
            &H9CC7, &H9CCD, &H9CCE, &H9CCF, &H9CD0, &H9CD1, &H9CD7, &H9CD8, &H9CD9, &H9CDA, _
            &H9CDB, &H9CE1, &H9CE2, &H9CE3, &H9CE4, &H9CE5, &H9D08, &H9D09, &H9D0A, &H9D0B, _
            &H9D0C, &H9D0D, &H9D0E, &H9D0F, &H9D10, &H9D11, &H9D12, &H9D13, &H9D14, &H9D15, _
            &H9D16, &H9D17, &H9D1D, &H9D1E, &H9D1F, &H9D20, &H9D21, &H9D27, &H9D28, &H9D29, _
            &H9D2A, &H9D2B, &H9D31, &H9D32, &H9D33, &H9D34, &H9D35, &H9D3B, &H9D3C, &H9D3D, _
            &H9D3E, &H9D3F, &H9D45, &H9D46, &H9D47, &H9D48, &H9D49, &H9D6C, &H9D6D, &H9D6E, _
            &H9D6F, &H9D70, &H9D71, &H9D72, &H9D73, &H9D74, &H9D75, &H9D76, &H9D77, &H9D78, _
            &H9D79, &H9D7A, &H9D7B, &H9D81, &H9D82, &H9D83, &H9D84, &H9D85, &H9D8B, &H9D8C, _
            &H9D8D, &H9D8E, &H9D8F, &H9D95, &H9D96, &H9D97, &H9D98, &H9D99, &H9D9F, &H9DA0, _
            &H9DA1, &H9DA2, &H9DA3, &H9DA9, &H9DAA, &H9DAB, &H9DAC, &H9DAD, &H9DD0, &H9DD1, _
            &H9DD2, &H9DD3, &H9DD4, &H9DD5, &H9DD6, &H9DD7, &H9DD8, &H9DD9, &H9DDA, &H9DDB, _
            &H9DDC, &H9DDD, &H9DDE, &H9DDF, &H9DE5, &H9DE6, &H9DE7, &H9DE8, &H9DE9, &H9DEF, _
            &H9DF0, &H9DF1, &H9DF2, &H9DF3, &H9DF9, &H9DFA, &H9DFB, &H9DFC, &H9DFD, &H9E03, _
            &H9E04, &H9E05, &H9E06, &H9E07, &H9E0D, &H9E0E, &H9E0F, &H9E10, &H9E11, &H9E34, _
            &H9E35, &H9E36, &H9E37, &H9E38, &H9E39, &H9E98, &H9E99, &H9E9A, &H9E9B, &H9E9C, _
            &H9E9D, &H9EFC, &H9EFD, &H9EFE, &H9EFF, &H9F00, &H9F01, &H9FC4, &H9FC5, &H9FC6, _
            &H9FC7, &H9FC8, &H9FC9, &H9FCA, &H9FCB, &H9FCC, &HBF68, &HC350, &HC351, &HC352, _
            &HC353, &HC354, &HC355, &HC356, &HC357, &HC358, &HC359, &HC35A, &HC35B, &HC35C, _
            &HC35D, &HC35E, &HC35F, &HC365, &HC366, &HC367, &HC368, &HC369, &HC36F, &HC370, _
            &HC371, &HC372, &HC373, &HC379, &HC37A, &HC37B, &HC37C, &HC37D, &HC383, &HC384, _
            &HC385, &HC386, &HC387, &HC418, &HC419, &HC41A, &HC41B, &HC41C, &HC41D, &HC41E, _
            &HC41F, &HC420, &HC421, &HC422, &HC423, &HC424, &HC425, &HC426, &HC427, &HC42D, _
            &HC42E, &HC42F, &HC430, &HC431, &HC437, &HC438, &HC439, &HC43A, &HC43B, &HC441, _
            &HC442, &HC443, &HC444, &HC445, &HC44B, &HC44C, &HC44D, &HC44E, &HC44F, &HC47C, _
            &HC47D, &HC47E, &HC47F, &HC480, &HC481, &HC482, &HC483, &HC484, &HC485, &HC486, _
            &HC487, &HC488, &HC489, &HC48A, &HC48B, &HC491, &HC492, &HC493, &HC494, &HC495, _
            &HC49B, &HC49C, &HC49D, &HC49E, &HC49F, &HC4A5, &HC4A6, &HC4A7, &HC4A8, &HC4A9, _
            &HC4AF, &HC4B0, &HC4B1, &HC4B2, &HC4B3, &HC4E0, &HC4E1, &HC4E2, &HC4E3, &HC4E4, _
            &HC4E5, &HC4E6, &HC4E7, &HC4E8, &HC4E9, &HC4EA, &HC4EB, &HC4EC, &HC4ED, &HC4EE, _
            &HC4EF, &HC4F5, &HC4F6, &HC4F7, &HC4F8, &HC4F9, &HC4FF, &HC500, &HC501, &HC502, _
            &HC503, &HC509, &HC50A, &HC50B, &HC50C, &HC50D, &HC513, &HC514, &HC515, &HC516, _
            &HC517, &HC544, &HC545, &HC546, &HC547, &HC548, &HE678, &HEA60, &HEA61, &HEA62, _
            &HEA63, &HEA64, &HEA65, &HEA6B, &HEA6C, &HEA6D, &HEA6E, &HEA6F, &HEAC4, &HEAC5, _
            &HEAC6, &HEAC7, &HEAC8, &HEAC9, &HEACF, &HEAD0, &HEAD1, &HEAD2, &HEAD3, &HEAD9, _
            &HEADA, &HEADB, &HEADC, &HEADD, &HEADE, &HEADF, &HEAE0, &HEAE1, &HEAE2, &HEAE3, _
            &HEAE4, &HEAE5, &HEAE6, &HEAE7, &HEAED, &HEAEE, &HEAEF, &HEAF0, &HEAF1, &HEAF7, _
            &HEAF8, &HEAF9, &HEAFA, &HEAFB, &HEB28, &HEB29, &HEB2A, &HEB2B, &HEB2C, &HEB2D, _
            &HEB2E, &HEB2F, &HEB30, &HEB31, &HEB32, &HEB33, &HEB34, &HEB35, &HEB36, &HEB37, _
            &HEB3D, &HEB3E, &HEB3F, &HEB40, &HEB41, &HEB47, &HEB48, &HEB49, &HEB4A, &HEB4B, _
            &HEB51, &HEB52, &HEB53, &HEB54, &HEB55, &HEB5B, &HEB5C, &HEB5D, &HEB5E, &HEB5F, _
            &HEB8C, &HEB8D, &HEB8E, &HEB8F, &HEB90, &HEB91, &HEB97, &HEB98, &HEB99, &HEB9A, _
            &HEB9B, &HEBA1, &HEBA2, &HEBA3, &HEBA4, &HEBA5, &HEBA6, &HEBA7, &HEBA8, &HEBA9, _
            &HEBAA, &HEBAB, &HEBAC, &HEBAD, &HEBAE, &HEBAF, &HEBB5, &HEBB6, &HEBB7, &HEBB8, _
            &HEBB9, &HEBBF, &HEBC0, &HEBC1, &HEBC2, &HEBC3, &HEBF0, &HEBF1, &HEBF2, &HEBF3, _
            &HEBF4, &HEBF5, &HEBFB, &HEBFC, &HEBFD, &HEBFE, &HEBFF, &HEC54, &HEC55, &HEC56, _
            &HEC57, &HEC58, &HEC59, &HECB8, &HECB9, &HECBA, &HECBB, &HECBC, &HECBD, &HECBE, _
            &HECBF, &HECC0, &HECC1, &HECC2, &HECC3, &HECC4, &HECC5, &HECC6, &HECC7, &HECCD, _
            &HECCE, &HECCF, &HECD0, &HECD1, &HECD7, &HECD8, &HECD9, &HECDA, &HECDB, &HECE1, _
            &HECE2, &HECE3, &HECE4, &HECE5, &HECEB, &HECEC, &HECED, &HECEE, &HECEF, &HED1C, _
            &HED1D, &HED1E, &HED1F, &HED20, &HED21, &HED22, &HED23, &HED80, &H10D88, &H11170, _
            &H11171, &H11172, &H11173, &H11174, &H11175, &H11176, &H11177, &H11178, &H11179, _
            &H1117A, &H1117B, &H1117C, &H1117D, &H1117E, &H1117F, &H11185, &H11186, &H11187, _
            &H11188, &H11189, &H1118F, &H11190, &H11191, &H11192, &H11193, &H11199, &H1119A, _
            &H1119B, &H1119C, &H1119D, &H111A3, &H111A4, &H111A5, &H111A6, &H111A7, &H111D4, _
            &H111D5, &H111D6, &H111D7, &H111D8, &H111D9, &H111DA, &H111DB, &H111DC, &H111DD, _
            &H111DE, &H111DF, &H111E0, &H111E1, &H111E2, &H111E3, &H111E9, &H111EA, &H111EB, _
            &H111EC, &H111ED, &H111F3, &H111F4, &H111F5, &H111F6, &H111F7, &H111FD, &H111FE, _
            &H111FF, &H11200, &H11201, &H11207, &H11208, &H11209, &H1120A, &H1120B, &H1129C, _
            &H1129D, &H1129E, &H1129F, &H112A0, &H112A1, &H11300, &H11301, &H11302, &H11303, _
            &H11304, &H11305, &H13498, &H13880, &H13881, &H13882, &H13883, &H13884, &H13885, _
            &H13886, &H13887, &H13888, &H13889, &H1388A, &H1388B, &H1388C, &H1388D, &H1388E, _
            &H1388F, &H13895, &H13896, &H13897, &H13898, &H13899, &H1389F, &H138A0, &H138A1, _
            &H138A2, &H138A3, &H138A9, &H138AA, &H138AB, &H138AC, &H138AD, &H138B3, &H138B4, _
            &H138B5, &H138B6, &H138B7, &H138BD, &H138BE, &H138BF, &H138C0, &H138C1, &H138E4, _
            &H138E5, &H138E6, &H138E7, &H138E8, &H138E9, &H138EA, &H138EB, &H138EC, &H138ED, _
            &H138EE, &H138EF, &H138F0, &H138F1, &H138F2, &H138F3, &H138F9, &H138FA, &H138FB, _
            &H13903, &H13904, &H13905, &H13906, &H13907, &H1390D, &H1390E, &H1390F, &H13910, _
            &H13911, &H13917, &H13918, &H13919, &H1391A, &H1391B, &H13921, &H13922, &H13923, _
            &H13924, &H13925, &H13948, &H13949, &H1394A, &H1394B, &H1394C, &H1394D, &H1394E, _
            &H1394F, &H13950, &H13951, &H13952, &H13953, &H13954, &H13955, &H13956, &H13957, _
            &H1395D, &H1395E, &H1395F, &H13967, &H13968, &H13969, &H1396A, &H1396B, &H13971, _
            &H13972, &H13973, &H13974, &H13975, &H1397B, &H1397C, &H1397D, &H1397E, &H1397F, _
            &H13985, &H13986, &H13987, &H13988, &H13989, &H139AC, &H139AD, &H139AE, &H139AF, _
            &H139B0, &H139B1, &H15BA8, &H15F90, &H15F91, &H15F92, &H15F93, &H15F94, &H15F95, _
            &H15FF4, &H15FF5, &H15FF6, &H15FF7, &H15FF8, &H15FF9, &H16058, &H16059, &H16120, _
            &H16184, &H182B8, &H186A0, &H186A1, &H186A2, &H186A3, &H186A4, &H186A5, &H186AB, _
            &H186AC, &H186AD, &H186AE, &H186AF, &H186B5, &H186B6, &H186B7, &H186B8, &H186B9, _
            &H186BA, &H186BB, &H186BC, &H186BD, &H186BE, &H186BF, &H186C0, &H186C1, &H186C2, _
            &H186C3, &H186C9, &H186CA, &H186CB, &H186CC, &H186CD, &H186D3, &H186D4, &H186D5, _
            &H186D6, &H186D7, &H18704, &H18705, &H18706, &H18707, &H18708, &H18709, &H1870A, _
            &H1870B, &H1870C, &H1870D, &H1870E, &H1870F, &H18710, &H18711, &H18712, &H18713, _
            &H18719, &H1871A, &H1871B, &H1871C, &H1871D, &H18723, &H18724, &H18725, &H18726, _
            &H18727, &H1872D, &H1872E, &H1872F, &H18730, &H18731, &H18737, &H18738, &H18739, _
            &H1873A, &H1873B, &H18741, &H18742, &H18743, &H18744, &H18745, &H18768, &H18769, _
            &H1876A, &H1876B, &H1876C, &H1876D, &H187CC, &H1A9C8, &H1FBD0, &H1FBD1, &H1FBD2, _
            &H1FBD3, &H1FBD4, &H1FBD5, &H1FBD6, &H1FBD7, &H1FBD8, &H1FBD9, &H1FBDA, &H1FBDB, _
            &H1FBDC, &H1FBDD, &H1FBDE, &H1FBDF, &H1FBE5, &H1FBE6, &H1FBE7, &H1FBE8, &H1FBE9, _
            &H1FBEF, &H1FBF0, &H1FBF1, &H1FBF2, &H1FBF3, &H1FC34, &H1FC35, &H1FC36, &H1FC37, _
            &H1FC38, &H1FC39, &H1FC3A, &H1FC3B, &H1FC3C, &H1FC3D, &H1FC3E, &H1FC3F, &H1FC40, _
            &H1FC41, &H1FC42, &H1FC43, &H1FC49, &H1FC4A, &H1FC4B, &H1FC4C, &H1FC4D, &H1FC53, _
            &H1FC54, &H1FC55, &H1FC56, &H1FC57, &H1FC98, &H1FC99, &H1FC9A, &H1FC9B, &H1FC9C, _
            &H1FC9D, &H1FC9E, &H1FC9F, &H1FCA0, &H1FCA1, &H1FCA2, &H1FCA3, &H1FCA4, &H1FCA5, _
            &H1FCA6, &H1FCA7, &H1FCAD, &H1FCAE, &H1FCAF, &H1FCB0, &H1FCB1, &H1FCB7, &H1FCB8, _
            &H1FCB9, &H1FCBA, &H1FCBB, &H1FCFC, &H1FCFD, &H1FCFE, &H1FCFF, &H1FD00, &H1FD01, _
            &H1FD02, &H1FD03, &H1FD04, &H1FD05, &H1FD06, &H1FD07, &H1FD08, &H1FD09, &H1FD0A, _
            &H1FD0B, &H1FD11, &H1FD12, &H1FD13, &H1FD14, &H1FD15, &H1FD1B, &H1FD1C, &H1FD1D, _
            &H1FD1E, &H1FD1F, &H1FD60, &H1FD61, &H1FD62, &H1FD63, &H1FD64, &H1FD65, &H1FDC4, _
            &H1FDC5, &H1FDC6, &H1FDC7, &H21EF8, &H222E0, &H22344, &H223A8, &H24608, &H249F0, _
            &H249F1, &H249F2, &H249F3, &H249F4, &H249F5, &H249F6, &H249F7, &H249F8, &H249F9, _
            &H249FA, &H249FB, &H249FC, &H249FD, &H249FE, &H249FF, &H24A05, &H24A06, &H24A07, _
            &H24A08, &H24A09, &H24A54, &H24AB8, &H24AB9, &H24ABA, &H24ABB, &H24ABC, &H24ABD, _
            &H24ABE, &H24ABF, &H24AC0, &H24AC1, &H24AC2, &H24AC3, &H24AC4, &H24AC5, &H24AC6, _
            &H24AC7, &H24ACD, &H24ACE, &H24ACF, &H24AD0, &H24AD1, &H24B1C, &H24B1D, &H24B1E, _
            &H24B1F, &H24B20, &H24B21, &H24B22, &H24B23, &H24B24, &H24B25, &H24B26, &H24B27, _
            &H24B28, &H24B29, &H24B2A, &H24B2B, &H24B31, &H24B32, &H24B33, &H24B34, &H24B35, _
            &H24B80, &H24B81, &H24B82, &H24B83, &H24B84, &H24B85, &H24B8B, &H24B8C, &H24B8D, _
            &H24B8E, &H24B8F, &H24BE4, &H24BE5, &H24BE6, &H24BE7, &H24BE8, &H24BE9, &H24BEA, _
            &H24BEB, &H24BEC, &H24BED, &H24BEE, &H24BEF, &H24BF0, &H24BF1, &H24BF2, &H24BF3, _
            &H24BF9, &H24BFA, &H24BFB, &H24BFC, &H24BFD, &H24C48, &H24C49, &H24C4A, &H24C4B, _
            &H24C4C, &H24C4D, &H24CAC, &H24CAD, &H24CAE, &H24CAF, &H24CB0, &H24CB1, &H24D10, _
            &H24D11, &H24D12, &H24D13, &H24D14, &H24D15, &H24D16, &H24D17, &H24D18, &H24D19, _
            &H24D1A, &H24D25, &H24D26, &H24D27, &H24D28, &H24D29, &H24D74, &H24D75, &H24D76, _
            &H24D77, &H24D78, &H24D79, &H24D7A, &H24D7B, &H24D7C, &H24D7D, &H24D7E, &H24D7F, _
            &H24D80, &H24D81, &H24D82, &H24D83, &H24D89, &H24D8A, &H24D8B, &H24D8C, &H24D8D, _
            &H24DD8, &H24E3C, &H24E3D, &H24E3E, &H24E3F, &H24E40, &H24E41, &H24EA0, &H24EA1, _
            &H24EA2, &H24EA3, &H24EA4, &H24EA5, &H24F04, &H24F05, &H24F06, &H24F07, &H24F08, _
            &H24F09, &H24F0A, &H24F0B, &H24F0C, &H24F0D, &H24F0E, &H24F19, &H24F1A, &H24F1B, _
            &H24F1C, &H24F1D, &H24F68, &H24F69, &H24F6A, &H24F6B, &H24F6C, &H24F6D, &H24F6E, _
            &H24F6F, &H24F70, &H24F71, &H24F72, &H24F7D, &H24F7E, &H24F7F, &H24F80, &H24F81, _
            &H24FCC, &H26D18, &H27100, &H27164, &H271C8, &H2722C, &H27290, &H272F4, &H27358, _
            &H273BC, &H27420, &H29428, &H29810, &H29874, &H298D8, &H2993C, &HFFFFFFFF&}

        cllArmor = {&H186A0, &H18704, &H18768, &H187CC, &H18830, &H18894, &H188F8, &H1895C, &H189C0, _
                    &H18A24, &H18A88, &H18AEC, &H18C18, &H18D44, &H18DA8, &H18E0C, &H18E70, &H18ED4, _
                    &H2BF20, &H2E630, &H2E694, &H2E6F8, &H2E75C, &H30D40, &H30DA4, &H30E08, &H30E6C, _
                    &H30ED0, &H30F34, &H30F98, &H30FFC, &H31060, &H310C4, &H31128, &H3118C, &H311F0, _
                    &H312B8, &H313E4, &H31448, &H314AC, &H31510, &H31574, &H46CD0, &H46D34, &H46D98, _
                    &H46DFC, &H493E0, &H49444, &H494A8, &H4950C, &H49570, &H495D4, &H49638, &H4969C, _
                    &H49700, &H49764, &H497C8, &H4982C, &H49890, &H49958, &H499BC, &H49A84, &H49AE8, _
                    &H49B4C, &H49BB0, &H49C14, &H5F370, &H5F3D4, &H5F438, &H5F49C, &H61A80, &H61AE4, _
                    &H61B48, &H61BAC, &H61C10, &H61C74, &H61CD8, &H61D3C, &H61DA0, &H61E04, &H61E68, _
                    &H61ECC, &H61F30, &H61FF8, &H6205C, &H62188, &H621EC, &H62250, &H622B4, &H77A10, _
                    &H77A74, &H77AD8, &H77B3C, &HFFFFFFFF&}

        cllItems = {&H6, &H7, &H8, &H9, &HA, &HB, &HC, &HD, &HE, &HF, &H10, &H11, &H12, &H13, &H14, _
                    &H15, &H16, &H17, &H18, &H19, &H1A, &H1B, &H1C, &H1D, &H1E, &H1F, &H20, &H21, _
                    &H22, &H23, &H24, &H25, &H26, &H27, &H28, &H29, &H2A, &H2B, &H2C, &H2D, &H63, _
                    &H3E8, &H3E9, &H3EA, &H3EB, &H3EC, &H3ED, &H3EE, &H3EF, &H3F0, &H3F1, &H3F2, _
                    &H3F3, &H3F4, &H3F5, &H3F6, &H3F7, &H3F8, &H3F9, &H3FA, &H3FC, &H3FD, &H3FE, _
                    &H3FF, &H400, &H401, &H402, &H403, &H404, &H405, &H406, &H407, &H408, &H409, _
                    &H7D0, &H7D1, &H7D2, &H7D3, &H7D4, &H7D5, &H7D6, &H7D7, &H7DC, &H7DD, &H7DE, _
                    &H7DF, &H7E0, &H7E1, &H7E5, &H7E6, &H7E7, &H7E8, &H7E9, &H7EA, &H7EB, &H7EC, _
                    &H7ED, &H7EE, &H7EF, &H7F0, &H7F1, &H7F2, &H7F3, &H7F4, &H7F5, &H7F6, &H7F7, _
                    &H7F8, &H7F9, &H7FA, &H7FB, &H7FC, &H7FD, &H7FE, &H7FF, &H800, &H801, &H802, _
                    &H806, &H807, &H808, &H809, &H2328, &H2329, &H232A, &H232B, &H232C, &H270B, _
                    &H270C, &H270D, &H270E, &H270F, &HFFFFFFFF&}
    End Sub
    Sub InitWeaps(cmbbox As ComboBox)
        cmbbox.Items.Clear()
        cmbbox.Items.Add("All-purpose catalyst (For Debugging/For both Magic & Miracles)")
        cmbbox.Items.Add("All-purpose catalyst (For Menu Dummy)")
        cmbbox.Items.Add("Dagger")
        cmbbox.Items.Add("Dagger+1")
        cmbbox.Items.Add("Dagger+2")
        cmbbox.Items.Add("Dagger+3")
        cmbbox.Items.Add("Dagger+4")
        cmbbox.Items.Add("Dagger+5")
        cmbbox.Items.Add("Dagger+6")
        cmbbox.Items.Add("Dagger+7")
        cmbbox.Items.Add("Dagger+8")
        cmbbox.Items.Add("Dagger+9")
        cmbbox.Items.Add("Dagger+10")
        cmbbox.Items.Add("Quality Dagger+1")
        cmbbox.Items.Add("Quality Dagger+2")
        cmbbox.Items.Add("Quality Dagger+3")
        cmbbox.Items.Add("Quality Dagger+4")
        cmbbox.Items.Add("Quality Dagger+5")
        cmbbox.Items.Add("Mercury Dagger+1")
        cmbbox.Items.Add("Mercury Dagger+2")
        cmbbox.Items.Add("Mercury Dagger+3")
        cmbbox.Items.Add("Mercury Dagger+4")
        cmbbox.Items.Add("Mercury Dagger+5")
        cmbbox.Items.Add("Sharp Dagger+1")
        cmbbox.Items.Add("Sharp Dagger+2")
        cmbbox.Items.Add("Sharp Dagger+3")
        cmbbox.Items.Add("Sharp Dagger+4")
        cmbbox.Items.Add("Sharp Dagger+5")
        cmbbox.Items.Add("Fatal Dagger+1")
        cmbbox.Items.Add("Fatal Dagger+2")
        cmbbox.Items.Add("Fatal Dagger+3")
        cmbbox.Items.Add("Fatal Dagger+4")
        cmbbox.Items.Add("Fatal Dagger+5")
        cmbbox.Items.Add("Crescent Dagger+1")
        cmbbox.Items.Add("Crescent Dagger+2")
        cmbbox.Items.Add("Crescent Dagger+3")
        cmbbox.Items.Add("Crescent Dagger+4")
        cmbbox.Items.Add("Crescent Dagger+5")
        cmbbox.Items.Add("Parrying Dagger")
        cmbbox.Items.Add("Parrying Dagger+1")
        cmbbox.Items.Add("Parrying Dagger+2")
        cmbbox.Items.Add("Parrying Dagger+3")
        cmbbox.Items.Add("Parrying Dagger+4")
        cmbbox.Items.Add("Parrying Dagger+5")
        cmbbox.Items.Add("Parrying Dagger+6")
        cmbbox.Items.Add("Parrying Dagger+7")
        cmbbox.Items.Add("Parrying Dagger+8")
        cmbbox.Items.Add("Parrying Dagger+9")
        cmbbox.Items.Add("Parrying Dagger+10")
        cmbbox.Items.Add("Quality Parrying Dagger+1")
        cmbbox.Items.Add("Quality Parrying Dagger+2")
        cmbbox.Items.Add("Quality Parrying Dagger+3")
        cmbbox.Items.Add("Quality Parrying Dagger+4")
        cmbbox.Items.Add("Quality Parrying Dagger+5")
        cmbbox.Items.Add("Mercury Parrying Dagger+1")
        cmbbox.Items.Add("Mercury Parrying Dagger+2")
        cmbbox.Items.Add("Mercury Parrying Dagger+3")
        cmbbox.Items.Add("Mercury Parrying Dagger+4")
        cmbbox.Items.Add("Mercury Parrying Dagger+5")
        cmbbox.Items.Add("Fatal Parrying Dagger+1")
        cmbbox.Items.Add("Fatal Parrying Dagger+2")
        cmbbox.Items.Add("Fatal Parrying Dagger+3")
        cmbbox.Items.Add("Fatal Parrying Dagger+4")
        cmbbox.Items.Add("Fatal Parrying Dagger+5")
        cmbbox.Items.Add("Sharp Parrying Dagger+1")
        cmbbox.Items.Add("Sharp Parrying Dagger+2")
        cmbbox.Items.Add("Sharp Parrying Dagger+3")
        cmbbox.Items.Add("Sharp Parrying Dagger+4")
        cmbbox.Items.Add("Sharp Parrying Dagger+5")
        cmbbox.Items.Add("Crescent Parrying Dagger+1")
        cmbbox.Items.Add("Crescent Parrying Dagger+2")
        cmbbox.Items.Add("Crescent Parrying Dagger+3")
        cmbbox.Items.Add("Crescent Parrying Dagger+4")
        cmbbox.Items.Add("Crescent Parrying Dagger+5")
        cmbbox.Items.Add("Mail Breaker")
        cmbbox.Items.Add("Mail Breaker+1")
        cmbbox.Items.Add("Mail Breaker+2")
        cmbbox.Items.Add("Mail Breaker+3")
        cmbbox.Items.Add("Mail Breaker+4")
        cmbbox.Items.Add("Mail Breaker+5")
        cmbbox.Items.Add("Mail Breaker+6")
        cmbbox.Items.Add("Mail Breaker+7")
        cmbbox.Items.Add("Mail Breaker+8")
        cmbbox.Items.Add("Mail Breaker+9")
        cmbbox.Items.Add("Mail Breaker+10")
        cmbbox.Items.Add("Quality Mail Breaker+1")
        cmbbox.Items.Add("Quality Mail Breaker+2")
        cmbbox.Items.Add("Quality Mail Breaker+3")
        cmbbox.Items.Add("Quality Mail Breaker+4")
        cmbbox.Items.Add("Quality Mail Breaker+5")
        cmbbox.Items.Add("Mercury Mail Breaker+1")
        cmbbox.Items.Add("Mercury Mail Breaker+2")
        cmbbox.Items.Add("Mercury Mail Breaker+3")
        cmbbox.Items.Add("Mercury Mail Breaker+4")
        cmbbox.Items.Add("Mercury Mail Breaker+5")
        cmbbox.Items.Add("Fatal Mail Breaker+1")
        cmbbox.Items.Add("Fatal Mail Breaker+2")
        cmbbox.Items.Add("Fatal Mail Breaker+3")
        cmbbox.Items.Add("Fatal Mail Breaker+4")
        cmbbox.Items.Add("Fatal Mail Breaker+5")
        cmbbox.Items.Add("Sharp Mail Breaker+1")
        cmbbox.Items.Add("Sharp Mail Breaker+2")
        cmbbox.Items.Add("Sharp Mail Breaker+3")
        cmbbox.Items.Add("Sharp Mail Breaker+4")
        cmbbox.Items.Add("Sharp Mail Breaker+5")
        cmbbox.Items.Add("Crescent Mail Breaker+1")
        cmbbox.Items.Add("Crescent Mail Breaker+2")
        cmbbox.Items.Add("Crescent Mail Breaker+3")
        cmbbox.Items.Add("Crescent Mail Breaker+4")
        cmbbox.Items.Add("Crescent Mail Breaker+5")
        cmbbox.Items.Add("Baby's Nail")
        cmbbox.Items.Add("Baby's Nail+1")
        cmbbox.Items.Add("Baby's Nail+2")
        cmbbox.Items.Add("Baby's Nail+3")
        cmbbox.Items.Add("Baby's Nail+4")
        cmbbox.Items.Add("Baby's Nail+5")
        cmbbox.Items.Add("Geri's Stiletto")
        cmbbox.Items.Add("Geri's Stiletto+1")
        cmbbox.Items.Add("Geri's Stiletto+2")
        cmbbox.Items.Add("Geri's Stiletto+3")
        cmbbox.Items.Add("Geri's Stiletto+4")
        cmbbox.Items.Add("Geri's Stiletto+5")
        cmbbox.Items.Add("Kris Blade")
        cmbbox.Items.Add("Kris Blade+1")
        cmbbox.Items.Add("Kris Blade+2")
        cmbbox.Items.Add("Kris Blade+3")
        cmbbox.Items.Add("Kris Blade+4")
        cmbbox.Items.Add("Kris Blade+5")
        cmbbox.Items.Add("Secret Dagger")
        cmbbox.Items.Add("Secret Dagger+1")
        cmbbox.Items.Add("Secret Dagger+2")
        cmbbox.Items.Add("Secret Dagger+3")
        cmbbox.Items.Add("Secret Dagger+4")
        cmbbox.Items.Add("Secret Dagger+5")
        cmbbox.Items.Add("Secret Dagger+6")
        cmbbox.Items.Add("Secret Dagger+7")
        cmbbox.Items.Add("Secret Dagger+8")
        cmbbox.Items.Add("Secret Dagger+9")
        cmbbox.Items.Add("Secret Dagger+10")
        cmbbox.Items.Add("Sharp Secret Dagger+1")
        cmbbox.Items.Add("Sharp Secret Dagger+2")
        cmbbox.Items.Add("Sharp Secret Dagger+3")
        cmbbox.Items.Add("Sharp Secret Dagger+4")
        cmbbox.Items.Add("Sharp Secret Dagger+5")
        cmbbox.Items.Add("Tearing Dagger+1")
        cmbbox.Items.Add("Tearing Dagger+2")
        cmbbox.Items.Add("Tearing Dagger+3")
        cmbbox.Items.Add("Crescent Secret Dagger+1")
        cmbbox.Items.Add("Crescent Secret Dagger+2")
        cmbbox.Items.Add("Crescent Secret Dagger+3")
        cmbbox.Items.Add("Crescent Secret Dagger+4")
        cmbbox.Items.Add("Crescent Secret Dagger+5")
        cmbbox.Items.Add("Quality Secret Dagger+1")
        cmbbox.Items.Add("Quality Secret Dagger+2")
        cmbbox.Items.Add("Quality Secret Dagger+3")
        cmbbox.Items.Add("Quality Secret Dagger+4")
        cmbbox.Items.Add("Quality Secret Dagger+5")
        cmbbox.Items.Add("Mercury Secret Dagger+1")
        cmbbox.Items.Add("Mercury Secret Dagger+2")
        cmbbox.Items.Add("Mercury Secret Dagger+3")
        cmbbox.Items.Add("Mercury Secret Dagger+4")
        cmbbox.Items.Add("Mercury Secret Dagger+5")
        cmbbox.Items.Add("Fatal Secret Dagger+1")
        cmbbox.Items.Add("Fatal Secret Dagger+2")
        cmbbox.Items.Add("Fatal Secret Dagger+3")
        cmbbox.Items.Add("Fatal Secret Dagger+4")
        cmbbox.Items.Add("Fatal Secret Dagger+5")
        cmbbox.Items.Add("_?_?w?Rc   (Ghost dagger)")
        cmbbox.Items.Add("Short Sword")
        cmbbox.Items.Add("Short Sword+1")
        cmbbox.Items.Add("Short Sword+2")
        cmbbox.Items.Add("Short Sword+3")
        cmbbox.Items.Add("Short Sword+4")
        cmbbox.Items.Add("Short Sword+5")
        cmbbox.Items.Add("Short Sword+6")
        cmbbox.Items.Add("Short Sword+7")
        cmbbox.Items.Add("Short Sword+8")
        cmbbox.Items.Add("Short Sword+9")
        cmbbox.Items.Add("Short Sword+10")
        cmbbox.Items.Add("Quality Short Sword+1")
        cmbbox.Items.Add("Quality Short Sword+2")
        cmbbox.Items.Add("Quality Short Sword+3")
        cmbbox.Items.Add("Quality Short Sword+4")
        cmbbox.Items.Add("Quality Short Sword+5")
        cmbbox.Items.Add("Dragon Short Sword+1")
        cmbbox.Items.Add("Dragon Short Sword+2")
        cmbbox.Items.Add("Dragon Short Sword+3")
        cmbbox.Items.Add("Dragon Short Sword+4")
        cmbbox.Items.Add("Dragon Short Sword+5")
        cmbbox.Items.Add("Moon Short Sword+1")
        cmbbox.Items.Add("Moon Short Sword+2")
        cmbbox.Items.Add("Moon Short Sword+3")
        cmbbox.Items.Add("Moon Short Sword+4")
        cmbbox.Items.Add("Moon Short Sword+5")
        cmbbox.Items.Add("Crushing Short Sword+1")
        cmbbox.Items.Add("Crushing Short Sword+2")
        cmbbox.Items.Add("Crushing Short Sword+3")
        cmbbox.Items.Add("Crushing Short Sword+4")
        cmbbox.Items.Add("Crushing Short Sword+5")
        cmbbox.Items.Add("Blessed Short Sword+1")
        cmbbox.Items.Add("Blessed Short Sword+2")
        cmbbox.Items.Add("Blessed Short Sword+3")
        cmbbox.Items.Add("Blessed Short Sword+4")
        cmbbox.Items.Add("Blessed Short Sword+5")
        cmbbox.Items.Add("Broadsword")
        cmbbox.Items.Add("Broadsword+1")
        cmbbox.Items.Add("Broadsword+2")
        cmbbox.Items.Add("Broadsword+3")
        cmbbox.Items.Add("Broadsword+4")
        cmbbox.Items.Add("Broadsword+5")
        cmbbox.Items.Add("Broadsword+6")
        cmbbox.Items.Add("Broadsword+7")
        cmbbox.Items.Add("Broadsword+8")
        cmbbox.Items.Add("Broadsword+9")
        cmbbox.Items.Add("Broadsword+10")
        cmbbox.Items.Add("Sharp Broadsword+1")
        cmbbox.Items.Add("Sharp Broadsword+2")
        cmbbox.Items.Add("Sharp Broadsword+3")
        cmbbox.Items.Add("Sharp Broadsword+4")
        cmbbox.Items.Add("Sharp Broadsword+5")
        cmbbox.Items.Add("Tearing Broadsword+1")
        cmbbox.Items.Add("Tearing Broadsword+2")
        cmbbox.Items.Add("Tearing Broadsword+3")
        cmbbox.Items.Add("Tearing Broadsword+4")
        cmbbox.Items.Add("Tearing Broadsword+5")
        cmbbox.Items.Add("Crescent Broadsword+1")
        cmbbox.Items.Add("Crescent Broadsword+2")
        cmbbox.Items.Add("Crescent Broadsword+3")
        cmbbox.Items.Add("Crescent Broadsword+4")
        cmbbox.Items.Add("Crescent Broadsword+5")
        cmbbox.Items.Add("Quality Broadsword+1")
        cmbbox.Items.Add("Quality Broadsword+2")
        cmbbox.Items.Add("Quality Broadsword+3")
        cmbbox.Items.Add("Quality Broadsword+4")
        cmbbox.Items.Add("Quality Broadsword+5")
        cmbbox.Items.Add("Mercury Broadsword+1")
        cmbbox.Items.Add("Mercury Broadsword+2")
        cmbbox.Items.Add("Mercury Broadsword+3")
        cmbbox.Items.Add("Mercury Broadsword+4")
        cmbbox.Items.Add("Mercury Broadsword+5")
        cmbbox.Items.Add("Moon Broadsword+1")
        cmbbox.Items.Add("Moon Broadsword+2")
        cmbbox.Items.Add("Moon Broadsword+3")
        cmbbox.Items.Add("Moon Broadsword+4")
        cmbbox.Items.Add("Moon Broadsword+5")
        cmbbox.Items.Add("Long Sword")
        cmbbox.Items.Add("Long Sword+1")
        cmbbox.Items.Add("Long Sword+2")
        cmbbox.Items.Add("Long Sword+3")
        cmbbox.Items.Add("Long Sword+4")
        cmbbox.Items.Add("Long Sword+5")
        cmbbox.Items.Add("Long Sword+6")
        cmbbox.Items.Add("Long Sword+7")
        cmbbox.Items.Add("Long Sword+8")
        cmbbox.Items.Add("Long Sword+9")
        cmbbox.Items.Add("Long Sword+10")
        cmbbox.Items.Add("Quality Long Sword+1")
        cmbbox.Items.Add("Quality Long Sword+2")
        cmbbox.Items.Add("Quality Long Sword+3")
        cmbbox.Items.Add("Quality Long Sword+4")
        cmbbox.Items.Add("Quality Long Sword+5")
        cmbbox.Items.Add("Dragon Long Sword+1")
        cmbbox.Items.Add("Dragon Long Sword+2")
        cmbbox.Items.Add("Dragon Long Sword+3")
        cmbbox.Items.Add("Dragon Long Sword+4")
        cmbbox.Items.Add("Dragon Long Sword+5")
        cmbbox.Items.Add("Moon Long Sword+1")
        cmbbox.Items.Add("Moon Long Sword+2")
        cmbbox.Items.Add("Moon Long Sword+3")
        cmbbox.Items.Add("Moon Long Sword+4")
        cmbbox.Items.Add("Moon Long Sword+5")
        cmbbox.Items.Add("Crushing Long Sword+1")
        cmbbox.Items.Add("Crushing Long Sword+2")
        cmbbox.Items.Add("Crushing Long Sword+3")
        cmbbox.Items.Add("Crushing Long Sword+4")
        cmbbox.Items.Add("Crushing Long Sword+5")
        cmbbox.Items.Add("Blessed Long Sword+1")
        cmbbox.Items.Add("Blessed Long Sword+2")
        cmbbox.Items.Add("Blessed Long Sword+3")
        cmbbox.Items.Add("Blessed Long Sword+4")
        cmbbox.Items.Add("Blessed Long Sword+5")
        cmbbox.Items.Add("Flamberge")
        cmbbox.Items.Add("Flamberge+1")
        cmbbox.Items.Add("Flamberge+2")
        cmbbox.Items.Add("Flamberge+3")
        cmbbox.Items.Add("Flamberge+4")
        cmbbox.Items.Add("Flamberge+5")
        cmbbox.Items.Add("Flamberge+6")
        cmbbox.Items.Add("Flamberge+7")
        cmbbox.Items.Add("Flamberge+8")
        cmbbox.Items.Add("Flamberge+9")
        cmbbox.Items.Add("Flamberge+10")
        cmbbox.Items.Add("Sharp Flamberge+1")
        cmbbox.Items.Add("Sharp Flamberge+2")
        cmbbox.Items.Add("Sharp Flamberge+3")
        cmbbox.Items.Add("Sharp Flamberge+4")
        cmbbox.Items.Add("Sharp Flamberge+5")
        cmbbox.Items.Add("Tearing Flamberge+1")
        cmbbox.Items.Add("Tearing Flamberge+2")
        cmbbox.Items.Add("Tearing Flamberge+3")
        cmbbox.Items.Add("Tearing Flamberge+4")
        cmbbox.Items.Add("Tearing Flamberge+5")
        cmbbox.Items.Add("Crescent Flamberge+1")
        cmbbox.Items.Add("Crescent Flamberge+2")
        cmbbox.Items.Add("Crescent Flamberge+3")
        cmbbox.Items.Add("Crescent Flamberge+4")
        cmbbox.Items.Add("Crescent Flamberge+5")
        cmbbox.Items.Add("Quality Flamberge+1")
        cmbbox.Items.Add("Quality Flamberge+2")
        cmbbox.Items.Add("Quality Flamberge+3")
        cmbbox.Items.Add("Quality Flamberge+4")
        cmbbox.Items.Add("Quality Flamberge+5")
        cmbbox.Items.Add("Mercury Flamberge+1")
        cmbbox.Items.Add("Mercury Flamberge+2")
        cmbbox.Items.Add("Mercury Flamberge+3")
        cmbbox.Items.Add("Mercury Flamberge+4")
        cmbbox.Items.Add("Mercury Flamberge+5")
        cmbbox.Items.Add("Moon Flamberge+1")
        cmbbox.Items.Add("Moon Flamberge+2")
        cmbbox.Items.Add("Moon Flamberge+3")
        cmbbox.Items.Add("Moon Flamberge+4")
        cmbbox.Items.Add("Moon Flamberge+5")
        cmbbox.Items.Add("Bastard Sword")
        cmbbox.Items.Add("Bastard Sword+1")
        cmbbox.Items.Add("Bastard Sword+2")
        cmbbox.Items.Add("Bastard Sword+3")
        cmbbox.Items.Add("Bastard Sword+4")
        cmbbox.Items.Add("Bastard Sword+5")
        cmbbox.Items.Add("Bastard Sword+6")
        cmbbox.Items.Add("Bastard Sword+7")
        cmbbox.Items.Add("Bastard Sword+8")
        cmbbox.Items.Add("Bastard Sword+9")
        cmbbox.Items.Add("Bastard Sword+10")
        cmbbox.Items.Add("Quality Bastard Sword+1")
        cmbbox.Items.Add("Quality Bastard Sword+2")
        cmbbox.Items.Add("Quality Bastard Sword+3")
        cmbbox.Items.Add("Quality Bastard Sword+4")
        cmbbox.Items.Add("Quality Bastard Sword+5")
        cmbbox.Items.Add("Dragon Bastard Sword+1")
        cmbbox.Items.Add("Dragon Bastard Sword+2")
        cmbbox.Items.Add("Dragon Bastard Sword+3")
        cmbbox.Items.Add("Dragon Bastard Sword+4")
        cmbbox.Items.Add("Dragon Bastard Sword+5")
        cmbbox.Items.Add("Moon Bastard Sword+1")
        cmbbox.Items.Add("Moon Bastard Sword+2")
        cmbbox.Items.Add("Moon Bastard Sword+3")
        cmbbox.Items.Add("Moon Bastard Sword+4")
        cmbbox.Items.Add("Moon Bastard Sword+5")
        cmbbox.Items.Add("Crushing Bastard Sword+1")
        cmbbox.Items.Add("Crushing Bastard Sword+2")
        cmbbox.Items.Add("Crushing Bastard Sword+3")
        cmbbox.Items.Add("Crushing Bastard Sword+4")
        cmbbox.Items.Add("Crushing Bastard Sword+5")
        cmbbox.Items.Add("Blessed Bastard Sword+1")
        cmbbox.Items.Add("Blessed Bastard Sword+2")
        cmbbox.Items.Add("Blessed Bastard Sword+3")
        cmbbox.Items.Add("Blessed Bastard Sword+4")
        cmbbox.Items.Add("Blessed Bastard Sword+5")
        cmbbox.Items.Add("Claymore")
        cmbbox.Items.Add("Claymore+1")
        cmbbox.Items.Add("Claymore+2")
        cmbbox.Items.Add("Claymore+3")
        cmbbox.Items.Add("Claymore+4")
        cmbbox.Items.Add("Claymore+5")
        cmbbox.Items.Add("Claymore+6")
        cmbbox.Items.Add("Claymore+7")
        cmbbox.Items.Add("Claymore+8")
        cmbbox.Items.Add("Claymore+9")
        cmbbox.Items.Add("Claymore+10")
        cmbbox.Items.Add("Quality Claymore+1")
        cmbbox.Items.Add("Quality Claymore+2")
        cmbbox.Items.Add("Quality Claymore+3")
        cmbbox.Items.Add("Quality Claymore+4")
        cmbbox.Items.Add("Quality Claymore+5")
        cmbbox.Items.Add("Dragon Claymore+1")
        cmbbox.Items.Add("Dragon Claymore+2")
        cmbbox.Items.Add("Dragon Claymore+3")
        cmbbox.Items.Add("Dragon Claymore+4")
        cmbbox.Items.Add("Dragon Claymore+5")
        cmbbox.Items.Add("Moon Claymore+1")
        cmbbox.Items.Add("Moon Claymore+2")
        cmbbox.Items.Add("Moon Claymore+3")
        cmbbox.Items.Add("Moon Claymore+4")
        cmbbox.Items.Add("Moon Claymore+5")
        cmbbox.Items.Add("Crushing Claymore+1")
        cmbbox.Items.Add("Crushing Claymore+2")
        cmbbox.Items.Add("Crushing Claymore+3")
        cmbbox.Items.Add("Crushing Claymore+4")
        cmbbox.Items.Add("Crushing Claymore+5")
        cmbbox.Items.Add("Blessed Claymore+1")
        cmbbox.Items.Add("Blessed Claymore+2")
        cmbbox.Items.Add("Blessed Claymore+3")
        cmbbox.Items.Add("Blessed Claymore+4")
        cmbbox.Items.Add("Blessed Claymore+5")
        cmbbox.Items.Add("Great Sword")
        cmbbox.Items.Add("Great Sword+1")
        cmbbox.Items.Add("Great Sword+2")
        cmbbox.Items.Add("Great Sword+3")
        cmbbox.Items.Add("Great Sword+4")
        cmbbox.Items.Add("Great Sword+5")
        cmbbox.Items.Add("Great Sword+6")
        cmbbox.Items.Add("Great Sword+7")
        cmbbox.Items.Add("Great Sword+8")
        cmbbox.Items.Add("Great Sword+9")
        cmbbox.Items.Add("Great Sword+10")
        cmbbox.Items.Add("Quality Great Sword+1")
        cmbbox.Items.Add("Quality Great Sword+2")
        cmbbox.Items.Add("Quality Great Sword+3")
        cmbbox.Items.Add("Quality Great Sword+4")
        cmbbox.Items.Add("Quality Great Sword+5")
        cmbbox.Items.Add("Dragon Great Sword+1")
        cmbbox.Items.Add("Dragon Great Sword+2")
        cmbbox.Items.Add("Dragon Great Sword+3")
        cmbbox.Items.Add("Dragon Great Sword+4")
        cmbbox.Items.Add("Dragon Great Sword+5")
        cmbbox.Items.Add("Moon Great Sword+1")
        cmbbox.Items.Add("Moon Great Sword+2")
        cmbbox.Items.Add("Moon Great Sword+3")
        cmbbox.Items.Add("Moon Great Sword+4")
        cmbbox.Items.Add("Moon Great Sword+5")
        cmbbox.Items.Add("Crushing Great Sword+1")
        cmbbox.Items.Add("Crushing Great Sword+2")
        cmbbox.Items.Add("Crushing Great Sword+3")
        cmbbox.Items.Add("Crushing Great Sword+4")
        cmbbox.Items.Add("Crushing Great Sword+5")
        cmbbox.Items.Add("Blessed Great Sword+1")
        cmbbox.Items.Add("Blessed Great Sword+2")
        cmbbox.Items.Add("Blessed Great Sword+3")
        cmbbox.Items.Add("Blessed Great Sword+4")
        cmbbox.Items.Add("Blessed Great Sword+5")
        cmbbox.Items.Add("Dragon Bone Smasher")
        cmbbox.Items.Add("Dragon Bone Smasher+1")
        cmbbox.Items.Add("Dragon Bone Smasher+2")
        cmbbox.Items.Add("Dragon Bone Smasher+3")
        cmbbox.Items.Add("Dragon Bone Smasher+4")
        cmbbox.Items.Add("Dragon Bone Smasher+5")
        cmbbox.Items.Add("Rune Sword")
        cmbbox.Items.Add("Rune Sword+1")
        cmbbox.Items.Add("Rune Sword+2")
        cmbbox.Items.Add("Rune Sword+3")
        cmbbox.Items.Add("Rune Sword+4")
        cmbbox.Items.Add("Rune Sword+5")
        cmbbox.Items.Add("Soulbrandt")
        cmbbox.Items.Add("Demonbrandt")
        cmbbox.Items.Add("Storm Ruler")
        cmbbox.Items.Add("Storm Ruler")
        cmbbox.Items.Add("Storm Ruler")
        cmbbox.Items.Add("Storm Ruler")
        cmbbox.Items.Add("Storm Ruler")
        cmbbox.Items.Add("Storm Ruler")
        cmbbox.Items.Add("Storm Ruler")
        cmbbox.Items.Add("Storm Ruler")
        cmbbox.Items.Add("Knight Sword")
        cmbbox.Items.Add("Knight Sword+1")
        cmbbox.Items.Add("Knight Sword+2")
        cmbbox.Items.Add("Knight Sword+3")
        cmbbox.Items.Add("Knight Sword+4")
        cmbbox.Items.Add("Knight Sword+5")
        cmbbox.Items.Add("Knight Sword+6")
        cmbbox.Items.Add("Knight Sword+7")
        cmbbox.Items.Add("Knight Sword+8")
        cmbbox.Items.Add("Knight Sword+9")
        cmbbox.Items.Add("Knight Sword+10")
        cmbbox.Items.Add("Quality Knight Sword+1")
        cmbbox.Items.Add("Quality Knight Sword+2")
        cmbbox.Items.Add("Quality Knight Sword+3")
        cmbbox.Items.Add("Quality Knight Sword+4")
        cmbbox.Items.Add("Quality Knight Sword+5")
        cmbbox.Items.Add("Dragon Knight Sword+1")
        cmbbox.Items.Add("Dragon Knight Sword+2")
        cmbbox.Items.Add("Dragon Knight Sword+3")
        cmbbox.Items.Add("Dragon Knight Sword+4")
        cmbbox.Items.Add("Dragon Knight Sword+5")
        cmbbox.Items.Add("Moon Knight Sword+1")
        cmbbox.Items.Add("Moon Knight Sword+2")
        cmbbox.Items.Add("Moon Knight Sword+3")
        cmbbox.Items.Add("Moon Knight Sword+4")
        cmbbox.Items.Add("Moon Knight Sword+5")
        cmbbox.Items.Add("Crushing Knight Sword+1")
        cmbbox.Items.Add("Crushing Knight Sword+2")
        cmbbox.Items.Add("Crushing Knight Sword+3")
        cmbbox.Items.Add("Crushing Knight Sword+4")
        cmbbox.Items.Add("Crushing Knight Sword+5")
        cmbbox.Items.Add("Blessed Knight Sword+1")
        cmbbox.Items.Add("Blessed Knight Sword+2")
        cmbbox.Items.Add("Blessed Knight Sword+3")
        cmbbox.Items.Add("Blessed Knight Sword+4")
        cmbbox.Items.Add("Blessed Knight Sword+5")
        cmbbox.Items.Add("Broken Sword")
        cmbbox.Items.Add("Northern Regalia")
        cmbbox.Items.Add("Northern Regalia")
        cmbbox.Items.Add("Large Sword of Moonlight")
        cmbbox.Items.Add("Large Sword of Moonlight+1")
        cmbbox.Items.Add("Large Sword of Moonlight+2")
        cmbbox.Items.Add("Large Sword of Moonlight+3")
        cmbbox.Items.Add("Large Sword of Moonlight+4")
        cmbbox.Items.Add("Large Sword of Moonlight+5")
        cmbbox.Items.Add("Blueblood Sword")
        cmbbox.Items.Add("Penetrating Sword")
        cmbbox.Items.Add("Penetrating Sword+1")
        cmbbox.Items.Add("Penetrating Sword+2")
        cmbbox.Items.Add("Penetrating Sword+3")
        cmbbox.Items.Add("Penetrating Sword+4")
        cmbbox.Items.Add("Penetrating Sword+5")
        cmbbox.Items.Add("Morion Blade")
        cmbbox.Items.Add("Morion Blade")
        cmbbox.Items.Add("Morion Blade")
        cmbbox.Items.Add("Morion Blade")
        cmbbox.Items.Add("Morion Blade")
        cmbbox.Items.Add("Morion Blade")
        cmbbox.Items.Add("_?_?v?Rc    (Ghost Sword)")
        cmbbox.Items.Add("Rapier")
        cmbbox.Items.Add("Rapier+1")
        cmbbox.Items.Add("Rapier+2")
        cmbbox.Items.Add("Rapier+3")
        cmbbox.Items.Add("Rapier+4")
        cmbbox.Items.Add("Rapier+5")
        cmbbox.Items.Add("Rapier+6")
        cmbbox.Items.Add("Rapier+7")
        cmbbox.Items.Add("Rapier+8")
        cmbbox.Items.Add("Rapier+9")
        cmbbox.Items.Add("Rapier+10")
        cmbbox.Items.Add("Quality Rapier+1")
        cmbbox.Items.Add("Quality Rapier+2")
        cmbbox.Items.Add("Quality Rapier+3")
        cmbbox.Items.Add("Quality Rapier+4")
        cmbbox.Items.Add("Quality Rapier+5")
        cmbbox.Items.Add("Mercury Rapier+1")
        cmbbox.Items.Add("Mercury Rapier+2")
        cmbbox.Items.Add("Mercury Rapier+3")
        cmbbox.Items.Add("Mercury Rapier+4")
        cmbbox.Items.Add("Mercury Rapier+5")
        cmbbox.Items.Add("Fatal Rapier+1")
        cmbbox.Items.Add("Fatal Rapier+2")
        cmbbox.Items.Add("Fatal Rapier+3")
        cmbbox.Items.Add("Fatal Rapier+4")
        cmbbox.Items.Add("Fatal Rapier+5")
        cmbbox.Items.Add("Sharp Rapier+1")
        cmbbox.Items.Add("Sharp Rapier+2")
        cmbbox.Items.Add("Sharp Rapier+3")
        cmbbox.Items.Add("Sharp Rapier+4")
        cmbbox.Items.Add("Sharp Rapier+5")
        cmbbox.Items.Add("Crescent Rapier+1")
        cmbbox.Items.Add("Crescent Rapier+2")
        cmbbox.Items.Add("Crescent Rapier+3")
        cmbbox.Items.Add("Crescent Rapier+4")
        cmbbox.Items.Add("Crescent Rapier+5")
        cmbbox.Items.Add("Estoc")
        cmbbox.Items.Add("Estoc+1")
        cmbbox.Items.Add("Estoc+2")
        cmbbox.Items.Add("Estoc+3")
        cmbbox.Items.Add("Estoc+4")
        cmbbox.Items.Add("Estoc+5")
        cmbbox.Items.Add("Estoc+6")
        cmbbox.Items.Add("Estoc+7")
        cmbbox.Items.Add("Estoc+8")
        cmbbox.Items.Add("Estoc+9")
        cmbbox.Items.Add("Estoc+10")
        cmbbox.Items.Add("Quality Estoc+1")
        cmbbox.Items.Add("Quality Estoc+2")
        cmbbox.Items.Add("Quality Estoc+3")
        cmbbox.Items.Add("Quality Estoc+4")
        cmbbox.Items.Add("Quality Estoc+5")
        cmbbox.Items.Add("Mercury Estoc+1")
        cmbbox.Items.Add("Mercury Estoc+2")
        cmbbox.Items.Add("Mercury Estoc+3")
        cmbbox.Items.Add("Mercury Estoc+4")
        cmbbox.Items.Add("Mercury Estoc+5")
        cmbbox.Items.Add("Fatal Estoc+1")
        cmbbox.Items.Add("Fatal Estoc+2")
        cmbbox.Items.Add("Fatal Estoc+3")
        cmbbox.Items.Add("Fatal Estoc+4")
        cmbbox.Items.Add("Fatal Estoc+5")
        cmbbox.Items.Add("Sharp Estoc+1")
        cmbbox.Items.Add("Sharp Estoc+2")
        cmbbox.Items.Add("Sharp Estoc+3")
        cmbbox.Items.Add("Sharp Estoc+4")
        cmbbox.Items.Add("Sharp Estoc+5")
        cmbbox.Items.Add("Crescent Estoc+1")
        cmbbox.Items.Add("Crescent Estoc+2")
        cmbbox.Items.Add("Crescent Estoc+3")
        cmbbox.Items.Add("Crescent Estoc+4")
        cmbbox.Items.Add("Crescent Estoc+5")
        cmbbox.Items.Add("Epee Rapier")
        cmbbox.Items.Add("Epee Rapier+1")
        cmbbox.Items.Add("Epee Rapier+2")
        cmbbox.Items.Add("Epee Rapier+3")
        cmbbox.Items.Add("Epee Rapier+4")
        cmbbox.Items.Add("Epee Rapier+5")
        cmbbox.Items.Add("Spiral Rapier")
        cmbbox.Items.Add("Spiral Rapier+1")
        cmbbox.Items.Add("Spiral Rapier+2")
        cmbbox.Items.Add("Spiral Rapier+3")
        cmbbox.Items.Add("Spiral Rapier+4")
        cmbbox.Items.Add("Spiral Rapier+5")
        cmbbox.Items.Add("Spiral Rapier+6")
        cmbbox.Items.Add("Spiral Rapier+7")
        cmbbox.Items.Add("Spiral Rapier+8")
        cmbbox.Items.Add("Spiral Rapier+9")
        cmbbox.Items.Add("Spiral Rapier+10")
        cmbbox.Items.Add("Quality Spiral Rapier+1")
        cmbbox.Items.Add("Quality Spiral Rapier+2")
        cmbbox.Items.Add("Quality Spiral Rapier+3")
        cmbbox.Items.Add("Quality Spiral Rapier+4")
        cmbbox.Items.Add("Quality Spiral Rapier+5")
        cmbbox.Items.Add("Mercury Spiral Rapier+1")
        cmbbox.Items.Add("Mercury Spiral Rapier+2")
        cmbbox.Items.Add("Mercury Spiral Rapier+3")
        cmbbox.Items.Add("Mercury Spiral Rapier+4")
        cmbbox.Items.Add("Mercury Spiral Rapier+5")
        cmbbox.Items.Add("Fatal Spiral Rapier+1")
        cmbbox.Items.Add("Fatal Spiral Rapier+2")
        cmbbox.Items.Add("Fatal Spiral Rapier+3")
        cmbbox.Items.Add("Fatal Spiral Rapier+4")
        cmbbox.Items.Add("Fatal Spiral Rapier+5")
        cmbbox.Items.Add("Sharp Spiral Rapier+1")
        cmbbox.Items.Add("Sharp Spiral Rapier+2")
        cmbbox.Items.Add("Sharp Spiral Rapier+3")
        cmbbox.Items.Add("Sharp Spiral Rapier+4")
        cmbbox.Items.Add("Sharp Spiral Rapier+5")
        cmbbox.Items.Add("Crescent Spiral Rapier+1")
        cmbbox.Items.Add("Crescent Spiral Rapier+2")
        cmbbox.Items.Add("Crescent Spiral Rapier+3")
        cmbbox.Items.Add("Crescent Spiral Rapier+4")
        cmbbox.Items.Add("Crescent Spiral Rapier+5")
        cmbbox.Items.Add("Needle of Eternal Agony")
        cmbbox.Items.Add("Needle of Eternal Agony")
        cmbbox.Items.Add("Needle of Eternal Agony")
        cmbbox.Items.Add("Needle of Eternal Agony")
        cmbbox.Items.Add("Needle of Eternal Agony")
        cmbbox.Items.Add("Needle of Eternal Agony")
        cmbbox.Items.Add("Needle of Eternal Agony")
        cmbbox.Items.Add("Needle of Eternal Agony")
        cmbbox.Items.Add("_?_?R:Rc    (Ghost Rapier)")
        cmbbox.Items.Add("Scimitar")
        cmbbox.Items.Add("Scimitar+1")
        cmbbox.Items.Add("Scimitar+2")
        cmbbox.Items.Add("Scimitar+3")
        cmbbox.Items.Add("Scimitar+4")
        cmbbox.Items.Add("Scimitar+5")
        cmbbox.Items.Add("Scimitar+6")
        cmbbox.Items.Add("Scimitar+7")
        cmbbox.Items.Add("Scimitar+8")
        cmbbox.Items.Add("Scimitar+9")
        cmbbox.Items.Add("Scimitar+10")
        cmbbox.Items.Add("Sharp Scimitar+1")
        cmbbox.Items.Add("Sharp Scimitar+2")
        cmbbox.Items.Add("Sharp Scimitar+3")
        cmbbox.Items.Add("Sharp Scimitar+4")
        cmbbox.Items.Add("Sharp Scimitar+5")
        cmbbox.Items.Add("Tearing Scimitar+1")
        cmbbox.Items.Add("Tearing Scimitar+2")
        cmbbox.Items.Add("Tearing Scimitar+3")
        cmbbox.Items.Add("Tearing Scimitar+4")
        cmbbox.Items.Add("Tearing Scimitar+5")
        cmbbox.Items.Add("Crescent Scimitar+1")
        cmbbox.Items.Add("Crescent Scimitar+2")
        cmbbox.Items.Add("Crescent Scimitar+3")
        cmbbox.Items.Add("Crescent Scimitar+4")
        cmbbox.Items.Add("Crescent Scimitar+5")
        cmbbox.Items.Add("Quality Scimitar+1")
        cmbbox.Items.Add("Quality Scimitar+2")
        cmbbox.Items.Add("Quality Scimitar+3")
        cmbbox.Items.Add("Quality Scimitar+4")
        cmbbox.Items.Add("Quality Scimitar+5")
        cmbbox.Items.Add("Mercury Scimitar+1")
        cmbbox.Items.Add("Mercury Scimitar+2")
        cmbbox.Items.Add("Mercury Scimitar+3")
        cmbbox.Items.Add("Mercury Scimitar+4")
        cmbbox.Items.Add("Mercury Scimitar+5")
        cmbbox.Items.Add("Moon Scimitar+1")
        cmbbox.Items.Add("Moon Scimitar+2")
        cmbbox.Items.Add("Moon Scimitar+3")
        cmbbox.Items.Add("Moon Scimitar+4")
        cmbbox.Items.Add("Moon Scimitar+5")
        cmbbox.Items.Add("Kilij")
        cmbbox.Items.Add("Kilij+1")
        cmbbox.Items.Add("Kilij+2")
        cmbbox.Items.Add("Kilij+3")
        cmbbox.Items.Add("Kilij+4")
        cmbbox.Items.Add("Kilij+5")
        cmbbox.Items.Add("Kilij+6")
        cmbbox.Items.Add("Kilij+7")
        cmbbox.Items.Add("Kilij+8")
        cmbbox.Items.Add("Kilij+9")
        cmbbox.Items.Add("Kilij+10")
        cmbbox.Items.Add("Sharp Kilij+1")
        cmbbox.Items.Add("Sharp Kilij+2")
        cmbbox.Items.Add("Sharp Kilij+3")
        cmbbox.Items.Add("Sharp Kilij+4")
        cmbbox.Items.Add("Sharp Kilij+5")
        cmbbox.Items.Add("Tearing Kilij+1")
        cmbbox.Items.Add("Tearing Kilij+2")
        cmbbox.Items.Add("Tearing Kilij+3")
        cmbbox.Items.Add("Tearing Kilij+4")
        cmbbox.Items.Add("Tearing Kilij+5")
        cmbbox.Items.Add("Crescent Kilij+1")
        cmbbox.Items.Add("Crescent Kilij+2")
        cmbbox.Items.Add("Crescent Kilij+3")
        cmbbox.Items.Add("Crescent Kilij+4")
        cmbbox.Items.Add("Crescent Kilij+5")
        cmbbox.Items.Add("Quality Kilij+1")
        cmbbox.Items.Add("Quality Kilij+2")
        cmbbox.Items.Add("Quality Kilij+3")
        cmbbox.Items.Add("Quality Kilij+4")
        cmbbox.Items.Add("Quality Kilij+5")
        cmbbox.Items.Add("Mercury Kilij+1")
        cmbbox.Items.Add("Mercury Kilij+2")
        cmbbox.Items.Add("Mercury Kilij+3")
        cmbbox.Items.Add("Mercury Kilij+4")
        cmbbox.Items.Add("Mercury Kilij+5")
        cmbbox.Items.Add("Moon Kilij+1")
        cmbbox.Items.Add("Moon Kilij+2")
        cmbbox.Items.Add("Moon Kilij+3")
        cmbbox.Items.Add("Moon Kilij+4")
        cmbbox.Items.Add("Moon Kilij+5")
        cmbbox.Items.Add("Shotel")
        cmbbox.Items.Add("Shotel+1")
        cmbbox.Items.Add("Shotel+2")
        cmbbox.Items.Add("Shotel+3")
        cmbbox.Items.Add("Shotel+4")
        cmbbox.Items.Add("Shotel+5")
        cmbbox.Items.Add("Shotel+6")
        cmbbox.Items.Add("Shotel+7")
        cmbbox.Items.Add("Shotel+8")
        cmbbox.Items.Add("Shotel+9")
        cmbbox.Items.Add("Shotel+10")
        cmbbox.Items.Add("Sharp Shotel+1")
        cmbbox.Items.Add("Sharp Shotel+2")
        cmbbox.Items.Add("Sharp Shotel+3")
        cmbbox.Items.Add("Sharp Shotel+4")
        cmbbox.Items.Add("Sharp Shotel+5")
        cmbbox.Items.Add("Tearing Shotel+1")
        cmbbox.Items.Add("Tearing Shotel+2")
        cmbbox.Items.Add("Tearing Shotel+3")
        cmbbox.Items.Add("Tearing Shotel+4")
        cmbbox.Items.Add("Tearing Shotel+5")
        cmbbox.Items.Add("Quality Shotel+1")
        cmbbox.Items.Add("Quality Shotel+2")
        cmbbox.Items.Add("Quality Shotel+3")
        cmbbox.Items.Add("Quality Shotel+4")
        cmbbox.Items.Add("Quality Shotel+5")
        cmbbox.Items.Add("Mercury Shotel+1")
        cmbbox.Items.Add("Mercury Shotel+2")
        cmbbox.Items.Add("Mercury Shotel+3")
        cmbbox.Items.Add("Mercury Shotel+4")
        cmbbox.Items.Add("Mercury Shotel+5")
        cmbbox.Items.Add("Moon Shotel+1")
        cmbbox.Items.Add("Moon Shotel+2")
        cmbbox.Items.Add("Moon Shotel+3")
        cmbbox.Items.Add("Moon Shotel+4")
        cmbbox.Items.Add("Moon Shotel+5")
        cmbbox.Items.Add("Crescent Shotel+1")
        cmbbox.Items.Add("Crescent Shotel+2")
        cmbbox.Items.Add("Crescent Shotel+3")
        cmbbox.Items.Add("Crescent Shotel+4")
        cmbbox.Items.Add("Crescent Shotel+5")
        cmbbox.Items.Add("Falchion")
        cmbbox.Items.Add("Falchion+1")
        cmbbox.Items.Add("Falchion+2")
        cmbbox.Items.Add("Falchion+3")
        cmbbox.Items.Add("Falchion+4")
        cmbbox.Items.Add("Falchion+5")
        cmbbox.Items.Add("Falchion+6")
        cmbbox.Items.Add("Falchion+7")
        cmbbox.Items.Add("Falchion+8")
        cmbbox.Items.Add("Falchion+9")
        cmbbox.Items.Add("Falchion+10")
        cmbbox.Items.Add("Sharp Falchion+1")
        cmbbox.Items.Add("Sharp Falchion+2")
        cmbbox.Items.Add("Sharp Falchion+3")
        cmbbox.Items.Add("Sharp Falchion+4")
        cmbbox.Items.Add("Sharp Falchion+5")
        cmbbox.Items.Add("Tearing Falchion+1")
        cmbbox.Items.Add("Tearing Falchion+2")
        cmbbox.Items.Add("Tearing Falchion+3")
        cmbbox.Items.Add("Tearing Falchion+4")
        cmbbox.Items.Add("Tearing Falchion+5")
        cmbbox.Items.Add("Crescent Falchion+1")
        cmbbox.Items.Add("Crescent Falchion+2")
        cmbbox.Items.Add("Crescent Falchion+3")
        cmbbox.Items.Add("Crescent Falchion+4")
        cmbbox.Items.Add("Crescent Falchion+5")
        cmbbox.Items.Add("Quality Falchion+1")
        cmbbox.Items.Add("Quality Falchion+2")
        cmbbox.Items.Add("Quality Falchion+3")
        cmbbox.Items.Add("Quality Falchion+4")
        cmbbox.Items.Add("Quality Falchion+5")
        cmbbox.Items.Add("Mercury Falchion+1")
        cmbbox.Items.Add("Mercury Falchion+2")
        cmbbox.Items.Add("Mercury Falchion+3")
        cmbbox.Items.Add("Mercury Falchion+4")
        cmbbox.Items.Add("Mercury Falchion+5")
        cmbbox.Items.Add("Moon Falchion+1")
        cmbbox.Items.Add("Moon Falchion+2")
        cmbbox.Items.Add("Moon Falchion+3")
        cmbbox.Items.Add("Moon Falchion+4")
        cmbbox.Items.Add("Moon Falchion+5")
        cmbbox.Items.Add("Uchigatana")
        cmbbox.Items.Add("Uchigatana+1")
        cmbbox.Items.Add("Uchigatana+2")
        cmbbox.Items.Add("Uchigatana+3")
        cmbbox.Items.Add("Uchigatana+4")
        cmbbox.Items.Add("Uchigatana+5")
        cmbbox.Items.Add("Uchigatana+6")
        cmbbox.Items.Add("Uchigatana+7")
        cmbbox.Items.Add("Uchigatana+8")
        cmbbox.Items.Add("Uchigatana+9")
        cmbbox.Items.Add("Uchigatana+10")
        cmbbox.Items.Add("Sharp Uchigatana+1")
        cmbbox.Items.Add("Sharp Uchigatana+2")
        cmbbox.Items.Add("Sharp Uchigatana+3")
        cmbbox.Items.Add("Sharp Uchigatana+4")
        cmbbox.Items.Add("Sharp Uchigatana+5")
        cmbbox.Items.Add("Tearing Uchigatana+1")
        cmbbox.Items.Add("Tearing Uchigatana+2")
        cmbbox.Items.Add("Tearing Uchigatana+3")
        cmbbox.Items.Add("Tearing Uchigatana+4")
        cmbbox.Items.Add("Tearing Uchigatana+5")
        cmbbox.Items.Add("Crescent Uchigatana+1")
        cmbbox.Items.Add("Crescent Uchigatana+2")
        cmbbox.Items.Add("Crescent Uchigatana+3")
        cmbbox.Items.Add("Crescent Uchigatana+4")
        cmbbox.Items.Add("Crescent Uchigatana+5")
        cmbbox.Items.Add("Quality Uchigatana+1")
        cmbbox.Items.Add("Quality Uchigatana+2")
        cmbbox.Items.Add("Quality Uchigatana+3")
        cmbbox.Items.Add("Quality Uchigatana+4")
        cmbbox.Items.Add("Quality Uchigatana+5")
        cmbbox.Items.Add("Mercury Uchigatana+1")
        cmbbox.Items.Add("Mercury Uchigatana+2")
        cmbbox.Items.Add("Mercury Uchigatana+3")
        cmbbox.Items.Add("Mercury Uchigatana+4")
        cmbbox.Items.Add("Mercury Uchigatana+5")
        cmbbox.Items.Add("Moon Uchigatana+1")
        cmbbox.Items.Add("Moon Uchigatana+2")
        cmbbox.Items.Add("Moon Uchigatana+3")
        cmbbox.Items.Add("Moon Uchigatana+4")
        cmbbox.Items.Add("Moon Uchigatana+5")
        cmbbox.Items.Add("Hiltless")
        cmbbox.Items.Add("Hiltless+1")
        cmbbox.Items.Add("Hiltless+2")
        cmbbox.Items.Add("Hiltless+3")
        cmbbox.Items.Add("Hiltless+4")
        cmbbox.Items.Add("Hiltless+5")
        cmbbox.Items.Add("Blind")
        cmbbox.Items.Add("Blind+1")
        cmbbox.Items.Add("Blind+2")
        cmbbox.Items.Add("Blind+3")
        cmbbox.Items.Add("Blind+4")
        cmbbox.Items.Add("Blind+5")
        cmbbox.Items.Add("Magic Sword 'Makoto'")
        cmbbox.Items.Add("Magic Sword 'Makoto'+1")
        cmbbox.Items.Add("Magic Sword 'Makoto'+2")
        cmbbox.Items.Add("Magic Sword 'Makoto'+3")
        cmbbox.Items.Add("Magic Sword 'Makoto'+4")
        cmbbox.Items.Add("Magic Sword 'Makoto'+5")
        cmbbox.Items.Add("Large Sword of Searching")
        cmbbox.Items.Add("Large Sword of Searching")
        cmbbox.Items.Add("Large Sword of Searching")
        cmbbox.Items.Add("Large Sword of Searching")
        cmbbox.Items.Add("Large Sword of Searching")
        cmbbox.Items.Add("Large Sword of Searching")
        cmbbox.Items.Add("Large Sword of Searching")
        cmbbox.Items.Add("Large Sword of Searching")
        cmbbox.Items.Add("Large Sword of Searching")
        cmbbox.Items.Add("_?_?f?Rc    (Ghost Falchion)")
        cmbbox.Items.Add("Battle Axe")
        cmbbox.Items.Add("Battle Axe+1")
        cmbbox.Items.Add("Battle Axe+2")
        cmbbox.Items.Add("Battle Axe+3")
        cmbbox.Items.Add("Battle Axe+4")
        cmbbox.Items.Add("Battle Axe+5")
        cmbbox.Items.Add("Battle Axe+6")
        cmbbox.Items.Add("Battle Axe+7")
        cmbbox.Items.Add("Battle Axe+8")
        cmbbox.Items.Add("Battle Axe+9")
        cmbbox.Items.Add("Battle Axe+10")
        cmbbox.Items.Add("Crushing Battle Axe+1")
        cmbbox.Items.Add("Crushing Battle Axe+2")
        cmbbox.Items.Add("Crushing Battle Axe+3")
        cmbbox.Items.Add("Crushing Battle Axe+4")
        cmbbox.Items.Add("Crushing Battle Axe+5")
        cmbbox.Items.Add("Dragon Battle Axe+1")
        cmbbox.Items.Add("Dragon Battle Axe+2")
        cmbbox.Items.Add("Dragon Battle Axe+3")
        cmbbox.Items.Add("Dragon Battle Axe+4")
        cmbbox.Items.Add("Dragon Battle Axe+5")
        cmbbox.Items.Add("Moon Battle Axe+1")
        cmbbox.Items.Add("Moon Battle Axe+2")
        cmbbox.Items.Add("Moon Battle Axe+3")
        cmbbox.Items.Add("Moon Battle Axe+4")
        cmbbox.Items.Add("Moon Battle Axe+5")
        cmbbox.Items.Add("Quality Battle Axe+1")
        cmbbox.Items.Add("Quality Battle Axe+2")
        cmbbox.Items.Add("Quality Battle Axe+3")
        cmbbox.Items.Add("Quality Battle Axe+4")
        cmbbox.Items.Add("Quality Battle Axe+5")
        cmbbox.Items.Add("Blessed Battle Axe+1")
        cmbbox.Items.Add("Blessed Battle Axe+2")
        cmbbox.Items.Add("Blessed Battle Axe+3")
        cmbbox.Items.Add("Blessed Battle Axe+4")
        cmbbox.Items.Add("Blessed Battle Axe+5")
        cmbbox.Items.Add("Great Axe")
        cmbbox.Items.Add("Great Axe+1")
        cmbbox.Items.Add("Great Axe+2")
        cmbbox.Items.Add("Great Axe+3")
        cmbbox.Items.Add("Great Axe+4")
        cmbbox.Items.Add("Great Axe+5")
        cmbbox.Items.Add("Great Axe+6")
        cmbbox.Items.Add("Great Axe+7")
        cmbbox.Items.Add("Great Axe+8")
        cmbbox.Items.Add("Great Axe+9")
        cmbbox.Items.Add("Great Axe+10")
        cmbbox.Items.Add("Crushing Great Axe+1")
        cmbbox.Items.Add("Crushing Great Axe+2")
        cmbbox.Items.Add("Crushing Great Axe+3")
        cmbbox.Items.Add("Crushing Great Axe+4")
        cmbbox.Items.Add("Crushing Great Axe+5")
        cmbbox.Items.Add("Dragon Great Axe+1")
        cmbbox.Items.Add("Dragon Great Axe+2")
        cmbbox.Items.Add("Dragon Great Axe+3")
        cmbbox.Items.Add("Dragon Great Axe+4")
        cmbbox.Items.Add("Dragon Great Axe+5")
        cmbbox.Items.Add("Moon Great Axe+1")
        cmbbox.Items.Add("Moon Great Axe+2")
        cmbbox.Items.Add("Moon Great Axe+3")
        cmbbox.Items.Add("Moon Great Axe+4")
        cmbbox.Items.Add("Moon Great Axe+5")
        cmbbox.Items.Add("Quality Great Axe+1")
        cmbbox.Items.Add("Quality Great Axe+2")
        cmbbox.Items.Add("Quality Great Axe+3")
        cmbbox.Items.Add("Quality Great Axe+4")
        cmbbox.Items.Add("Quality Great Axe+5")
        cmbbox.Items.Add("Blessed Great Axe+1")
        cmbbox.Items.Add("Blessed Great Axe+2")
        cmbbox.Items.Add("Blessed Great Axe+3")
        cmbbox.Items.Add("Blessed Great Axe+4")
        cmbbox.Items.Add("Blessed Great Axe+5")
        cmbbox.Items.Add("Crescent Axe")
        cmbbox.Items.Add("Crescent Axe+1")
        cmbbox.Items.Add("Crescent Axe+2")
        cmbbox.Items.Add("Crescent Axe+3")
        cmbbox.Items.Add("Crescent Axe+4")
        cmbbox.Items.Add("Crescent Axe+5")
        cmbbox.Items.Add("Crescent Axe+6")
        cmbbox.Items.Add("Crescent Axe+7")
        cmbbox.Items.Add("Crescent Axe+8")
        cmbbox.Items.Add("Crescent Axe+9")
        cmbbox.Items.Add("Crescent Axe+10")
        cmbbox.Items.Add("Crushing Crescent Axe+1")
        cmbbox.Items.Add("Crushing Crescent Axe+2")
        cmbbox.Items.Add("Crushing Crescent Axe+3")
        cmbbox.Items.Add("Crushing Crescent Axe+4")
        cmbbox.Items.Add("Crushing Crescent Axe+5")
        cmbbox.Items.Add("Dragon Crescent Axe+1")
        cmbbox.Items.Add("Dragon Crescent Axe+2")
        cmbbox.Items.Add("Dragon Crescent Axe+3")
        cmbbox.Items.Add("Dragon Crescent Axe+4")
        cmbbox.Items.Add("Dragon Crescent Axe+5")
        cmbbox.Items.Add("Moon Crescent Axe+1")
        cmbbox.Items.Add("Moon Crescent Axe+2")
        cmbbox.Items.Add("Moon Crescent Axe+3")
        cmbbox.Items.Add("Moon Crescent Axe+4")
        cmbbox.Items.Add("Moon Crescent Axe+5")
        cmbbox.Items.Add("Quality Crescent Axe+1")
        cmbbox.Items.Add("Quality Crescent Axe+2")
        cmbbox.Items.Add("Quality Crescent Axe+3")
        cmbbox.Items.Add("Quality Crescent Axe+4")
        cmbbox.Items.Add("Quality Crescent Axe+5")
        cmbbox.Items.Add("Blessed Crescent Axe+1")
        cmbbox.Items.Add("Blessed Crescent Axe+2")
        cmbbox.Items.Add("Blessed Crescent Axe+3")
        cmbbox.Items.Add("Blessed Crescent Axe+4")
        cmbbox.Items.Add("Blessed Crescent Axe+5")
        cmbbox.Items.Add("Guillotine Axe")
        cmbbox.Items.Add("Guillotine Axe+1")
        cmbbox.Items.Add("Guillotine Axe+2")
        cmbbox.Items.Add("Guillotine Axe+3")
        cmbbox.Items.Add("Guillotine Axe+4")
        cmbbox.Items.Add("Guillotine Axe+5")
        cmbbox.Items.Add("Guillotine Axe+6")
        cmbbox.Items.Add("Guillotine Axe+7")
        cmbbox.Items.Add("Guillotine Axe+8")
        cmbbox.Items.Add("Guillotine Axe+9")
        cmbbox.Items.Add("Guillotine Axe+10")
        cmbbox.Items.Add("Crushing Guillotine Axe+1")
        cmbbox.Items.Add("Crushing Guillotine Axe+2")
        cmbbox.Items.Add("Crushing Guillotine Axe+3")
        cmbbox.Items.Add("Crushing Guillotine Axe+4")
        cmbbox.Items.Add("Crushing Guillotine Axe+5")
        cmbbox.Items.Add("Dragon Guillotine Axe+1")
        cmbbox.Items.Add("Dragon Guillotine Axe+2")
        cmbbox.Items.Add("Dragon Guillotine Axe+3")
        cmbbox.Items.Add("Dragon Guillotine Axe+4")
        cmbbox.Items.Add("Dragon Guillotine Axe+5")
        cmbbox.Items.Add("Moon Guillotine Axe+1")
        cmbbox.Items.Add("Moon Guillotine Axe+2")
        cmbbox.Items.Add("Moon Guillotine Axe+3")
        cmbbox.Items.Add("Moon Guillotine Axe+4")
        cmbbox.Items.Add("Moon Guillotine Axe+5")
        cmbbox.Items.Add("Quality Guillotine Axe+1")
        cmbbox.Items.Add("Quality Guillotine Axe+2")
        cmbbox.Items.Add("Quality Guillotine Axe+3")
        cmbbox.Items.Add("Quality Guillotine Axe+4")
        cmbbox.Items.Add("Quality Guillotine Axe+5")
        cmbbox.Items.Add("Blessed Guillotine Axe+1")
        cmbbox.Items.Add("Blessed Guillotine Axe+2")
        cmbbox.Items.Add("Blessed Guillotine Axe+3")
        cmbbox.Items.Add("Blessed Guillotine Axe+4")
        cmbbox.Items.Add("Blessed Guillotine Axe+5")
        cmbbox.Items.Add("Dozer Axe")
        cmbbox.Items.Add("Dozer Axe")
        cmbbox.Items.Add("Dozer Axe")
        cmbbox.Items.Add("Dozer Axe")
        cmbbox.Items.Add("Dozer Axe")
        cmbbox.Items.Add("_?_?e?    (Ghost Hand Axe)")
        cmbbox.Items.Add("Club")
        cmbbox.Items.Add("Crushing Club+1")
        cmbbox.Items.Add("Crushing Club+2")
        cmbbox.Items.Add("Crushing Club+3")
        cmbbox.Items.Add("Crushing Club+4")
        cmbbox.Items.Add("Crushing Club+5")
        cmbbox.Items.Add("Blessed Club+1")
        cmbbox.Items.Add("Blessed Club+2")
        cmbbox.Items.Add("Blessed Club+3")
        cmbbox.Items.Add("Blessed Club+4")
        cmbbox.Items.Add("Blessed Club+5")
        cmbbox.Items.Add("Mace")
        cmbbox.Items.Add("Crushing Mace+1")
        cmbbox.Items.Add("Crushing Mace+2")
        cmbbox.Items.Add("Crushing Mace+3")
        cmbbox.Items.Add("Crushing Mace+4")
        cmbbox.Items.Add("Crushing Mace+5")
        cmbbox.Items.Add("Blessed Mace+1")
        cmbbox.Items.Add("Blessed Mace+2")
        cmbbox.Items.Add("Blessed Mace+3")
        cmbbox.Items.Add("Blessed Mace+4")
        cmbbox.Items.Add("Blessed Mace+5")
        cmbbox.Items.Add("Mace+1")
        cmbbox.Items.Add("Mace+2")
        cmbbox.Items.Add("Mace+3")
        cmbbox.Items.Add("Mace+4")
        cmbbox.Items.Add("Mace+5")
        cmbbox.Items.Add("Mace+6")
        cmbbox.Items.Add("Mace+7")
        cmbbox.Items.Add("Mace+8")
        cmbbox.Items.Add("Mace+9")
        cmbbox.Items.Add("Mace+10")
        cmbbox.Items.Add("Quality Mace+1")
        cmbbox.Items.Add("Quality Mace+2")
        cmbbox.Items.Add("Quality Mace+3")
        cmbbox.Items.Add("Quality Mace+4")
        cmbbox.Items.Add("Quality Mace+5")
        cmbbox.Items.Add("Dragon Mace+1")
        cmbbox.Items.Add("Dragon Mace+2")
        cmbbox.Items.Add("Dragon Mace+3")
        cmbbox.Items.Add("Dragon Mace+4")
        cmbbox.Items.Add("Dragon Mace+5")
        cmbbox.Items.Add("Moon Mace+1")
        cmbbox.Items.Add("Moon Mace+2")
        cmbbox.Items.Add("Moon Mace+3")
        cmbbox.Items.Add("Moon Mace+4")
        cmbbox.Items.Add("Moon Mace+5")
        cmbbox.Items.Add("War Pick")
        cmbbox.Items.Add("War Pick+1")
        cmbbox.Items.Add("War Pick+2")
        cmbbox.Items.Add("War Pick+3")
        cmbbox.Items.Add("War Pick+4")
        cmbbox.Items.Add("War Pick+5")
        cmbbox.Items.Add("War Pick+6")
        cmbbox.Items.Add("War Pick+7")
        cmbbox.Items.Add("War Pick+8")
        cmbbox.Items.Add("War Pick+9")
        cmbbox.Items.Add("War Pick+10")
        cmbbox.Items.Add("Quality War Pick+1")
        cmbbox.Items.Add("Quality War Pick+2")
        cmbbox.Items.Add("Quality War Pick+3")
        cmbbox.Items.Add("Quality War Pick+4")
        cmbbox.Items.Add("Quality War Pick+5")
        cmbbox.Items.Add("Mercury War Pick+1")
        cmbbox.Items.Add("Mercury War Pick+2")
        cmbbox.Items.Add("Mercury War Pick+3")
        cmbbox.Items.Add("Mercury War Pick+4")
        cmbbox.Items.Add("Mercury War Pick+5")
        cmbbox.Items.Add("Sharp War Pick+1")
        cmbbox.Items.Add("Sharp War Pick+2")
        cmbbox.Items.Add("Sharp War Pick+3")
        cmbbox.Items.Add("Sharp War Pick+4")
        cmbbox.Items.Add("Sharp War Pick+5")
        cmbbox.Items.Add("Tearing War Pick+1")
        cmbbox.Items.Add("Tearing War Pick+2")
        cmbbox.Items.Add("Tearing War Pick+3")
        cmbbox.Items.Add("Tearing War Pick+4")
        cmbbox.Items.Add("Tearing War Pick+5")
        cmbbox.Items.Add("Moon War Pick+1")
        cmbbox.Items.Add("Moon War Pick+2")
        cmbbox.Items.Add("Moon War Pick+3")
        cmbbox.Items.Add("Moon War Pick+4")
        cmbbox.Items.Add("Moon War Pick+5")
        cmbbox.Items.Add("Morning Star")
        cmbbox.Items.Add("Crushing Morning Star+1")
        cmbbox.Items.Add("Crushing Morning Star+2")
        cmbbox.Items.Add("Crushing Morning Star+3")
        cmbbox.Items.Add("Crushing Morning Star+4")
        cmbbox.Items.Add("Crushing Morning Star+5")
        cmbbox.Items.Add("Blessed Morning Star+1")
        cmbbox.Items.Add("Blessed Morning Star+2")
        cmbbox.Items.Add("Blessed Morning Star+3")
        cmbbox.Items.Add("Blessed Morning Star+4")
        cmbbox.Items.Add("Blessed Morning Star+5")
        cmbbox.Items.Add("Morning Star+1")
        cmbbox.Items.Add("Morning Star+2")
        cmbbox.Items.Add("Morning Star+3")
        cmbbox.Items.Add("Morning Star+4")
        cmbbox.Items.Add("Morning Star+5")
        cmbbox.Items.Add("Morning Star+6")
        cmbbox.Items.Add("Morning Star+7")
        cmbbox.Items.Add("Morning Star+8")
        cmbbox.Items.Add("Morning Star+9")
        cmbbox.Items.Add("Morning Star+10")
        cmbbox.Items.Add("Quality Morning Star+1")
        cmbbox.Items.Add("Quality Morning Star+2")
        cmbbox.Items.Add("Quality Morning Star+3")
        cmbbox.Items.Add("Quality Morning Star+4")
        cmbbox.Items.Add("Quality Morning Star+5")
        cmbbox.Items.Add("Dragon Morning Star+1")
        cmbbox.Items.Add("Dragon Morning Star+2")
        cmbbox.Items.Add("Dragon Morning Star+3")
        cmbbox.Items.Add("Dragon Morning Star+4")
        cmbbox.Items.Add("Dragon Morning Star+5")
        cmbbox.Items.Add("Moon Morning Star+1")
        cmbbox.Items.Add("Moon Morning Star+2")
        cmbbox.Items.Add("Moon Morning Star+3")
        cmbbox.Items.Add("Moon Morning Star+4")
        cmbbox.Items.Add("Moon Morning Star+5")
        cmbbox.Items.Add("Great Club")
        cmbbox.Items.Add("Crushing Great Club+1")
        cmbbox.Items.Add("Crushing Great Club+2")
        cmbbox.Items.Add("Crushing Great Club+3")
        cmbbox.Items.Add("Crushing Great Club+4")
        cmbbox.Items.Add("Crushing Great Club+5")
        cmbbox.Items.Add("Blessed Great Club+1")
        cmbbox.Items.Add("Blessed Great Club+2")
        cmbbox.Items.Add("Blessed Great Club+3")
        cmbbox.Items.Add("Blessed Great Club+4")
        cmbbox.Items.Add("Blessed Great Club+5")
        cmbbox.Items.Add("Bramd")
        cmbbox.Items.Add("Bramd+1")
        cmbbox.Items.Add("Bramd+2")
        cmbbox.Items.Add("Bramd+3")
        cmbbox.Items.Add("Bramd+4")
        cmbbox.Items.Add("Bramd+5")
        cmbbox.Items.Add("Pickaxe")
        cmbbox.Items.Add("Pickaxe+1")
        cmbbox.Items.Add("Pickaxe+2")
        cmbbox.Items.Add("Pickaxe+3")
        cmbbox.Items.Add("Pickaxe+4")
        cmbbox.Items.Add("Pickaxe+5")
        cmbbox.Items.Add("Pickaxe+6")
        cmbbox.Items.Add("Pickaxe+7")
        cmbbox.Items.Add("Pickaxe+8")
        cmbbox.Items.Add("Pickaxe+9")
        cmbbox.Items.Add("Pickaxe+10")
        cmbbox.Items.Add("Quality Pickaxe+1")
        cmbbox.Items.Add("Quality Pickaxe+2")
        cmbbox.Items.Add("Quality Pickaxe+3")
        cmbbox.Items.Add("Quality Pickaxe+4")
        cmbbox.Items.Add("Quality Pickaxe+5")
        cmbbox.Items.Add("Mercury Pickaxe+1")
        cmbbox.Items.Add("Mercury Pickaxe+2")
        cmbbox.Items.Add("Mercury Pickaxe+3")
        cmbbox.Items.Add("Mercury Pickaxe+4")
        cmbbox.Items.Add("Mercury Pickaxe+5")
        cmbbox.Items.Add("Sharp Pickaxe+1")
        cmbbox.Items.Add("Sharp Pickaxe+2")
        cmbbox.Items.Add("Sharp Pickaxe+3")
        cmbbox.Items.Add("Sharp Pickaxe+4")
        cmbbox.Items.Add("Sharp Pickaxe+5")
        cmbbox.Items.Add("Tearing Pickaxe+1")
        cmbbox.Items.Add("Tearing Pickaxe+2")
        cmbbox.Items.Add("Tearing Pickaxe+3")
        cmbbox.Items.Add("Tearing Pickaxe+4")
        cmbbox.Items.Add("Tearing Pickaxe+5")
        cmbbox.Items.Add("Moon Pickaxe+1")
        cmbbox.Items.Add("Moon Pickaxe+2")
        cmbbox.Items.Add("Moon Pickaxe+3")
        cmbbox.Items.Add("Moon Pickaxe+4")
        cmbbox.Items.Add("Moon Pickaxe+5")
        cmbbox.Items.Add("Meat Cleaver")
        cmbbox.Items.Add("Meat Cleaver")
        cmbbox.Items.Add("Meat Cleaver")
        cmbbox.Items.Add("Meat Cleaver")
        cmbbox.Items.Add("Meat Cleaver")
        cmbbox.Items.Add("Meat Cleaver")
        cmbbox.Items.Add("Meat Cleaver")
        cmbbox.Items.Add("Meat Cleaver")
        cmbbox.Items.Add("Torch    (Non-functioning)")
        cmbbox.Items.Add("_?_?i?    (Ghost Club)")
        cmbbox.Items.Add("Short Spear")
        cmbbox.Items.Add("Short Spear+1")
        cmbbox.Items.Add("Short Spear+2")
        cmbbox.Items.Add("Short Spear+3")
        cmbbox.Items.Add("Short Spear+4")
        cmbbox.Items.Add("Short Spear+5")
        cmbbox.Items.Add("Short Spear+6")
        cmbbox.Items.Add("Short Spear+7")
        cmbbox.Items.Add("Short Spear+8")
        cmbbox.Items.Add("Short Spear+9")
        cmbbox.Items.Add("Short Spear+10")
        cmbbox.Items.Add("Quality Short Spear+1")
        cmbbox.Items.Add("Quality Short Spear+2")
        cmbbox.Items.Add("Quality Short Spear+3")
        cmbbox.Items.Add("Quality Short Spear+4")
        cmbbox.Items.Add("Quality Short Spear+5")
        cmbbox.Items.Add("Mercury Short Spear+1")
        cmbbox.Items.Add("Mercury Short Spear+2")
        cmbbox.Items.Add("Mercury Short Spear+3")
        cmbbox.Items.Add("Mercury Short Spear+4")
        cmbbox.Items.Add("Mercury Short Spear+5")
        cmbbox.Items.Add("Sharp Short Spear+1")
        cmbbox.Items.Add("Sharp Short Spear+2")
        cmbbox.Items.Add("Sharp Short Spear+3")
        cmbbox.Items.Add("Sharp Short Spear+4")
        cmbbox.Items.Add("Sharp Short Spear+5")
        cmbbox.Items.Add("Fatal Short Spear+1")
        cmbbox.Items.Add("Fatal Short Spear+2")
        cmbbox.Items.Add("Fatal Short Spear+3")
        cmbbox.Items.Add("Fatal Short Spear+4")
        cmbbox.Items.Add("Fatal Short Spear+5")
        cmbbox.Items.Add("Moon Short Spear+1")
        cmbbox.Items.Add("Moon Short Spear+2")
        cmbbox.Items.Add("Moon Short Spear+3")
        cmbbox.Items.Add("Moon Short Spear+4")
        cmbbox.Items.Add("Moon Short Spear+5")
        cmbbox.Items.Add("Winged Spear")
        cmbbox.Items.Add("Winged Spear+1")
        cmbbox.Items.Add("Winged Spear+2")
        cmbbox.Items.Add("Winged Spear+3")
        cmbbox.Items.Add("Winged Spear+4")
        cmbbox.Items.Add("Winged Spear+5")
        cmbbox.Items.Add("Winged Spear+6")
        cmbbox.Items.Add("Winged Spear+7")
        cmbbox.Items.Add("Winged Spear+8")
        cmbbox.Items.Add("Winged Spear+9")
        cmbbox.Items.Add("Winged Spear+10")
        cmbbox.Items.Add("Quality Winged Spear+1")
        cmbbox.Items.Add("Quality Winged Spear+2")
        cmbbox.Items.Add("Quality Winged Spear+3")
        cmbbox.Items.Add("Quality Winged Spear+4")
        cmbbox.Items.Add("Quality Winged Spear+5")
        cmbbox.Items.Add("Mercury Winged Spear+1")
        cmbbox.Items.Add("Mercury Winged Spear+2")
        cmbbox.Items.Add("Mercury Winged Spear+3")
        cmbbox.Items.Add("Mercury Winged Spear+4")
        cmbbox.Items.Add("Mercury Winged Spear+5")
        cmbbox.Items.Add("Sharp Winged Spear+1")
        cmbbox.Items.Add("Sharp Winged Spear+2")
        cmbbox.Items.Add("Sharp Winged Spear+3")
        cmbbox.Items.Add("Sharp Winged Spear+4")
        cmbbox.Items.Add("Sharp Winged Spear+5")
        cmbbox.Items.Add("Fatal Winged Spear+1")
        cmbbox.Items.Add("Fatal Winged Spear+2")
        cmbbox.Items.Add("Fatal Winged Spear+3")
        cmbbox.Items.Add("Fatal Winged Spear+4")
        cmbbox.Items.Add("Fatal Winged Spear+5")
        cmbbox.Items.Add("Moon Winged Spear+1")
        cmbbox.Items.Add("Moon Winged Spear+2")
        cmbbox.Items.Add("Moon Winged Spear+3")
        cmbbox.Items.Add("Moon Winged Spear+4")
        cmbbox.Items.Add("Moon Winged Spear+5")
        cmbbox.Items.Add("Istarelle")
        cmbbox.Items.Add("Istarelle+1")
        cmbbox.Items.Add("Istarelle+2")
        cmbbox.Items.Add("Istarelle+3")
        cmbbox.Items.Add("Istarelle+4")
        cmbbox.Items.Add("Istarelle+5")
        cmbbox.Items.Add("Scraping Spear")
        cmbbox.Items.Add("Scraping Spear")
        cmbbox.Items.Add("Scraping Spear")
        cmbbox.Items.Add("Scraping Spear")
        cmbbox.Items.Add("Scraping Spear")
        cmbbox.Items.Add("Scraping Spear")
        cmbbox.Items.Add("_?_?i?    (Ghost Spear)")
        cmbbox.Items.Add("War Scythe")
        cmbbox.Items.Add("War Scythe+1")
        cmbbox.Items.Add("War Scythe+2")
        cmbbox.Items.Add("War Scythe+3")
        cmbbox.Items.Add("War Scythe+4")
        cmbbox.Items.Add("War Scythe+5")
        cmbbox.Items.Add("War Scythe+6")
        cmbbox.Items.Add("War Scythe+7")
        cmbbox.Items.Add("War Scythe+8")
        cmbbox.Items.Add("War Scythe+9")
        cmbbox.Items.Add("War Scythe+10")
        cmbbox.Items.Add("Sharp War Scythe+1")
        cmbbox.Items.Add("Sharp War Scythe+2")
        cmbbox.Items.Add("Sharp War Scythe+3")
        cmbbox.Items.Add("Sharp War Scythe+4")
        cmbbox.Items.Add("Sharp War Scythe+5")
        cmbbox.Items.Add("Tearing War Scythe+1")
        cmbbox.Items.Add("Tearing War Scythe+2")
        cmbbox.Items.Add("Tearing War Scythe+3")
        cmbbox.Items.Add("Tearing War Scythe+4")
        cmbbox.Items.Add("Tearing War Scythe+5")
        cmbbox.Items.Add("Crescent War Scythe+1")
        cmbbox.Items.Add("Crescent War Scythe+2")
        cmbbox.Items.Add("Crescent War Scythe+3")
        cmbbox.Items.Add("Crescent War Scythe+4")
        cmbbox.Items.Add("Crescent War Scythe+5")
        cmbbox.Items.Add("Quality War Scythe+1")
        cmbbox.Items.Add("Quality War Scythe+2")
        cmbbox.Items.Add("Quality War Scythe+3")
        cmbbox.Items.Add("Quality War Scythe+4")
        cmbbox.Items.Add("Quality War Scythe+5")
        cmbbox.Items.Add("Mercury War Scythe+1")
        cmbbox.Items.Add("Mercury War Scythe+2")
        cmbbox.Items.Add("Mercury War Scythe+3")
        cmbbox.Items.Add("Mercury War Scythe+4")
        cmbbox.Items.Add("Mercury War Scythe+5")
        cmbbox.Items.Add("Moon War Scythe+1")
        cmbbox.Items.Add("Moon War Scythe+2")
        cmbbox.Items.Add("Moon War Scythe+3")
        cmbbox.Items.Add("Moon War Scythe+4")
        cmbbox.Items.Add("Moon War Scythe+5")
        cmbbox.Items.Add("Mirdan Hammer")
        cmbbox.Items.Add("Mirdan Hammer+1")
        cmbbox.Items.Add("Mirdan Hammer+2")
        cmbbox.Items.Add("Mirdan Hammer+3")
        cmbbox.Items.Add("Mirdan Hammer+4")
        cmbbox.Items.Add("Mirdan Hammer+5")
        cmbbox.Items.Add("Mirdan Hammer+6")
        cmbbox.Items.Add("Mirdan Hammer+7")
        cmbbox.Items.Add("Mirdan Hammer+8")
        cmbbox.Items.Add("Mirdan Hammer+9")
        cmbbox.Items.Add("Mirdan Hammer+10")
        cmbbox.Items.Add("Quality Mirdan Hammer+1")
        cmbbox.Items.Add("Quality Mirdan Hammer+2")
        cmbbox.Items.Add("Quality Mirdan Hammer+3")
        cmbbox.Items.Add("Quality Mirdan Hammer+4")
        cmbbox.Items.Add("Quality Mirdan Hammer+5")
        cmbbox.Items.Add("Mercury Mirdan Hammer+1")
        cmbbox.Items.Add("Mercury Mirdan Hammer+2")
        cmbbox.Items.Add("Mercury Mirdan Hammer+3")
        cmbbox.Items.Add("Crushing Mirdan Hammer+1")
        cmbbox.Items.Add("Crushing Mirdan Hammer+2")
        cmbbox.Items.Add("Crushing Mirdan Hammer+3")
        cmbbox.Items.Add("Crushing Mirdan Hammer+4")
        cmbbox.Items.Add("Crushing Mirdan Hammer+5")
        cmbbox.Items.Add("Dragon Mirdan Hammer+1")
        cmbbox.Items.Add("Dragon Mirdan Hammer+2")
        cmbbox.Items.Add("Dragon Mirdan Hammer+3")
        cmbbox.Items.Add("Dragon Mirdan Hammer+4")
        cmbbox.Items.Add("Dragon Mirdan Hammer+5")
        cmbbox.Items.Add("Moon Mirdan Hammer+1")
        cmbbox.Items.Add("Moon Mirdan Hammer+2")
        cmbbox.Items.Add("Moon Mirdan Hammer+3")
        cmbbox.Items.Add("Moon Mirdan Hammer+4")
        cmbbox.Items.Add("Moon Mirdan Hammer+5")
        cmbbox.Items.Add("Blessed Mirdan Hammer+1")
        cmbbox.Items.Add("Blessed Mirdan Hammer+2")
        cmbbox.Items.Add("Blessed Mirdan Hammer+3")
        cmbbox.Items.Add("Blessed Mirdan Hammer+4")
        cmbbox.Items.Add("Blessed Mirdan Hammer+5")
        cmbbox.Items.Add("Halberd")
        cmbbox.Items.Add("Halberd+1")
        cmbbox.Items.Add("Halberd+2")
        cmbbox.Items.Add("Halberd+3")
        cmbbox.Items.Add("Halberd+4")
        cmbbox.Items.Add("Halberd+5")
        cmbbox.Items.Add("Halberd+6")
        cmbbox.Items.Add("Halberd+7")
        cmbbox.Items.Add("Halberd+8")
        cmbbox.Items.Add("Halberd+9")
        cmbbox.Items.Add("Halberd+10")
        cmbbox.Items.Add("Quality Halberd+1")
        cmbbox.Items.Add("Quality Halberd+2")
        cmbbox.Items.Add("Quality Halberd+3")
        cmbbox.Items.Add("Quality Halberd+4")
        cmbbox.Items.Add("Quality Halberd+5")
        cmbbox.Items.Add("Mercury Halberd+1")
        cmbbox.Items.Add("Mercury Halberd+2")
        cmbbox.Items.Add("Mercury Halberd+3")
        cmbbox.Items.Add("Crushing Halberd+1")
        cmbbox.Items.Add("Crushing Halberd+2")
        cmbbox.Items.Add("Crushing Halberd+3")
        cmbbox.Items.Add("Crushing Halberd+4")
        cmbbox.Items.Add("Crushing Halberd+5")
        cmbbox.Items.Add("Dragon Halberd+1")
        cmbbox.Items.Add("Dragon Halberd+2")
        cmbbox.Items.Add("Dragon Halberd+3")
        cmbbox.Items.Add("Dragon Halberd+4")
        cmbbox.Items.Add("Dragon Halberd+5")
        cmbbox.Items.Add("Moon Halberd+1")
        cmbbox.Items.Add("Moon Halberd+2")
        cmbbox.Items.Add("Moon Halberd+3")
        cmbbox.Items.Add("Moon Halberd+4")
        cmbbox.Items.Add("Moon Halberd+5")
        cmbbox.Items.Add("Blessed Halberd+1")
        cmbbox.Items.Add("Blessed Halberd+2")
        cmbbox.Items.Add("Blessed Halberd+3")
        cmbbox.Items.Add("Blessed Halberd+4")
        cmbbox.Items.Add("Blessed Halberd+5")
        cmbbox.Items.Add("Phosphorescent Pole")
        cmbbox.Items.Add("Phosphorescent Pole+1")
        cmbbox.Items.Add("Phosphorescent Pole+2")
        cmbbox.Items.Add("Phosphorescent Pole+3")
        cmbbox.Items.Add("Phosphorescent Pole+4")
        cmbbox.Items.Add("Phosphorescent Pole+5")
        cmbbox.Items.Add("_?_?z?r?    (Ghost Spear #2)")
        cmbbox.Items.Add("Wooden Catalyst")
        cmbbox.Items.Add("Crushing Wooden Catalyst+1")
        cmbbox.Items.Add("Crushing Wooden Catalyst+2")
        cmbbox.Items.Add("Crushing Wooden Catalyst+3")
        cmbbox.Items.Add("Crushing Wooden Catalyst+4")
        cmbbox.Items.Add("Crushing Wooden Catalyst+5")
        cmbbox.Items.Add("Silver Catalyst")
        cmbbox.Items.Add("Crushing Silver Catalyst+1")
        cmbbox.Items.Add("Crushing Silver Catalyst+2")
        cmbbox.Items.Add("Crushing Silver Catalyst+3")
        cmbbox.Items.Add("Crushing Silver Catalyst+4")
        cmbbox.Items.Add("Crushing Silver Catalyst+5")
        cmbbox.Items.Add("Insanity Catalyst")
        cmbbox.Items.Add("Insanity Catalyst")
        cmbbox.Items.Add("Talisman of God")
        cmbbox.Items.Add("Talisman of Beasts")
        cmbbox.Items.Add("_?_???Z?    (Ghost Catalyst)")
        cmbbox.Items.Add("Iron Knuckles")
        cmbbox.Items.Add("Crushing Iron Knuckles+1")
        cmbbox.Items.Add("Crushing Iron Knuckles+2")
        cmbbox.Items.Add("Crushing Iron Knuckles+3")
        cmbbox.Items.Add("Crushing Iron Knuckles+4")
        cmbbox.Items.Add("Crushing Iron Knuckles+5")
        cmbbox.Items.Add("Blessed Iron Knuckles+1")
        cmbbox.Items.Add("Blessed Iron Knuckles+2")
        cmbbox.Items.Add("Blessed Iron Knuckles+3")
        cmbbox.Items.Add("Blessed Iron Knuckles+4")
        cmbbox.Items.Add("Blessed Iron Knuckles+5")
        cmbbox.Items.Add("Iron Knuckles+1")
        cmbbox.Items.Add("Iron Knuckles+2")
        cmbbox.Items.Add("Iron Knuckles+3")
        cmbbox.Items.Add("Iron Knuckles+4")
        cmbbox.Items.Add("Iron Knuckles+5")
        cmbbox.Items.Add("Iron Knuckles+6")
        cmbbox.Items.Add("Iron Knuckles+7")
        cmbbox.Items.Add("Iron Knuckles+8")
        cmbbox.Items.Add("Iron Knuckles+9")
        cmbbox.Items.Add("Iron Knuckles+10")
        cmbbox.Items.Add("Quality Iron Knuckles+1")
        cmbbox.Items.Add("Quality Iron Knuckles+2")
        cmbbox.Items.Add("Quality Iron Knuckles+3")
        cmbbox.Items.Add("Quality Iron Knuckles+4")
        cmbbox.Items.Add("Quality Iron Knuckles+5")
        cmbbox.Items.Add("Dragon Iron Knuckles+1")
        cmbbox.Items.Add("Dragon Iron Knuckles+2")
        cmbbox.Items.Add("Dragon Iron Knuckles+3")
        cmbbox.Items.Add("Dragon Iron Knuckles+4")
        cmbbox.Items.Add("Dragon Iron Knuckles+5")
        cmbbox.Items.Add("Moon Iron Knuckles+1")
        cmbbox.Items.Add("Moon Iron Knuckles+2")
        cmbbox.Items.Add("Moon Iron Knuckles+3")
        cmbbox.Items.Add("Moon Iron Knuckles+4")
        cmbbox.Items.Add("Moon Iron Knuckles+5")
        cmbbox.Items.Add("Claws")
        cmbbox.Items.Add("Claws+1")
        cmbbox.Items.Add("Claws+2")
        cmbbox.Items.Add("Claws+3")
        cmbbox.Items.Add("Claws+4")
        cmbbox.Items.Add("Claws+5")
        cmbbox.Items.Add("Claws+6")
        cmbbox.Items.Add("Claws+7")
        cmbbox.Items.Add("Claws+8")
        cmbbox.Items.Add("Claws+9")
        cmbbox.Items.Add("Claws+10")
        cmbbox.Items.Add("Sharp Claws+1")
        cmbbox.Items.Add("Sharp Claws+2")
        cmbbox.Items.Add("Sharp Claws+3")
        cmbbox.Items.Add("Sharp Claws+4")
        cmbbox.Items.Add("Sharp Claws+5")
        cmbbox.Items.Add("Tearing Claws+1")
        cmbbox.Items.Add("Tearing Claws+2")
        cmbbox.Items.Add("Tearing Claws+3")
        cmbbox.Items.Add("Tearing Claws+4")
        cmbbox.Items.Add("Tearing Claws+5")
        cmbbox.Items.Add("Crescent Claws+1")
        cmbbox.Items.Add("Crescent Claws+2")
        cmbbox.Items.Add("Crescent Claws+3")
        cmbbox.Items.Add("Crescent Claws+4")
        cmbbox.Items.Add("Crescent Claws+5")
        cmbbox.Items.Add("Quality Claws+1")
        cmbbox.Items.Add("Quality Claws+2")
        cmbbox.Items.Add("Quality Claws+3")
        cmbbox.Items.Add("Quality Claws+4")
        cmbbox.Items.Add("Quality Claws+5")
        cmbbox.Items.Add("Mercury Claws+1")
        cmbbox.Items.Add("Mercury Claws+2")
        cmbbox.Items.Add("Mercury Claws+3")
        cmbbox.Items.Add("Mercury Claws+4")
        cmbbox.Items.Add("Mercury Claws+5")
        cmbbox.Items.Add("Moon Claws+1")
        cmbbox.Items.Add("Moon Claws+2")
        cmbbox.Items.Add("Moon Claws+3")
        cmbbox.Items.Add("Moon Claws+4")
        cmbbox.Items.Add("Moon Claws+5")
        cmbbox.Items.Add("Hands of God")
        cmbbox.Items.Add("Hands of God+1")
        cmbbox.Items.Add("Hands of God+2")
        cmbbox.Items.Add("Hands of God+3")
        cmbbox.Items.Add("Hands of God+4")
        cmbbox.Items.Add("Hands of God+5")
        cmbbox.Items.Add("Bare Fists")
        cmbbox.Items.Add("_?_?b?    (Ghost fists)")
        cmbbox.Items.Add("Short Bow")
        cmbbox.Items.Add("Short Bow+1")
        cmbbox.Items.Add("Short Bow+2")
        cmbbox.Items.Add("Short Bow+3")
        cmbbox.Items.Add("Short Bow+4")
        cmbbox.Items.Add("Short Bow+5")
        cmbbox.Items.Add("Short Bow+6")
        cmbbox.Items.Add("Short Bow+7")
        cmbbox.Items.Add("Short Bow+8")
        cmbbox.Items.Add("Short Bow+9")
        cmbbox.Items.Add("Short Bow+10")
        cmbbox.Items.Add("R[??0nShort Bow+1")
        cmbbox.Items.Add("R[??0nShort Bow+2")
        cmbbox.Items.Add("R[??0nShort Bow+3")
        cmbbox.Items.Add("R[??0nShort Bow+4")
        cmbbox.Items.Add("R[??0nShort Bow+5")
        cmbbox.Items.Add("Sticky Short Bow+1")
        cmbbox.Items.Add("Sticky Short Bow+2")
        cmbbox.Items.Add("Sticky Short Bow+3")
        cmbbox.Items.Add("Sticky Short Bow+4")
        cmbbox.Items.Add("Sticky Short Bow+5")
        cmbbox.Items.Add("Quality Short Bow+1")
        cmbbox.Items.Add("Quality Short Bow+2")
        cmbbox.Items.Add("Quality Short Bow+3")
        cmbbox.Items.Add("Quality Short Bow+4")
        cmbbox.Items.Add("Quality Short Bow+5")
        cmbbox.Items.Add("Compound Short Bow")
        cmbbox.Items.Add("Compound Short Bow+1")
        cmbbox.Items.Add("Compound Short Bow+2")
        cmbbox.Items.Add("Compound Short Bow+3")
        cmbbox.Items.Add("Compound Short Bow+4")
        cmbbox.Items.Add("Compound Short Bow+5")
        cmbbox.Items.Add("Compound Short Bow+6")
        cmbbox.Items.Add("Compound Short Bow+7")
        cmbbox.Items.Add("Compound Short Bow+8")
        cmbbox.Items.Add("Compound Short Bow+9")
        cmbbox.Items.Add("Compound Short Bow+10")
        cmbbox.Items.Add("R[??0nCompound Short Bow+1")
        cmbbox.Items.Add("R[??0nCompound Short Bow+2")
        cmbbox.Items.Add("R[??0nCompound Short Bow+3")
        cmbbox.Items.Add("R[??0nCompound Short Bow+4")
        cmbbox.Items.Add("R[??0nCompound Short Bow+5")
        cmbbox.Items.Add("Sticky Compound Short Bow+1")
        cmbbox.Items.Add("Sticky Compound Short Bow+2")
        cmbbox.Items.Add("Sticky Compound Short Bow+3")
        cmbbox.Items.Add("Sticky Compound Short Bow+4")
        cmbbox.Items.Add("Sticky Compound Short Bow+5")
        cmbbox.Items.Add("Quality Compound Short Bow+1")
        cmbbox.Items.Add("Quality Compound Short Bow+2")
        cmbbox.Items.Add("Quality Compound Short Bow+3")
        cmbbox.Items.Add("Quality Compound Short Bow+4")
        cmbbox.Items.Add("Quality Compound Short Bow+5")
        cmbbox.Items.Add("Long Bow")
        cmbbox.Items.Add("Long Bow+1")
        cmbbox.Items.Add("Long Bow+2")
        cmbbox.Items.Add("Long Bow+3")
        cmbbox.Items.Add("Long Bow+4")
        cmbbox.Items.Add("Long Bow+5")
        cmbbox.Items.Add("Long Bow+6")
        cmbbox.Items.Add("Long Bow+7")
        cmbbox.Items.Add("Long Bow+8")
        cmbbox.Items.Add("Long Bow+9")
        cmbbox.Items.Add("Long Bow+10")
        cmbbox.Items.Add("R[??0nLong Bow+1")
        cmbbox.Items.Add("R[??0nLong Bow+2")
        cmbbox.Items.Add("R[??0nLong Bow+3")
        cmbbox.Items.Add("R[??0nLong Bow+4")
        cmbbox.Items.Add("R[??0nLong Bow+5")
        cmbbox.Items.Add("Sticky Long Bow+1")
        cmbbox.Items.Add("Sticky Long Bow+2")
        cmbbox.Items.Add("Sticky Long Bow+3")
        cmbbox.Items.Add("Sticky Long Bow+4")
        cmbbox.Items.Add("Sticky Long Bow+5")
        cmbbox.Items.Add("Quality Long Bow+1")
        cmbbox.Items.Add("Quality Long Bow+2")
        cmbbox.Items.Add("Quality Long Bow+3")
        cmbbox.Items.Add("Quality Long Bow+4")
        cmbbox.Items.Add("Quality Long Bow+5")
        cmbbox.Items.Add("Compound Long Bow")
        cmbbox.Items.Add("Compound Long Bow+1")
        cmbbox.Items.Add("Compound Long Bow+2")
        cmbbox.Items.Add("Compound Long Bow+3")
        cmbbox.Items.Add("Compound Long Bow+4")
        cmbbox.Items.Add("Compound Long Bow+5")
        cmbbox.Items.Add("Compound Long Bow+6")
        cmbbox.Items.Add("Compound Long Bow+7")
        cmbbox.Items.Add("Compound Long Bow+8")
        cmbbox.Items.Add("Compound Long Bow+9")
        cmbbox.Items.Add("Compound Long Bow+10")
        cmbbox.Items.Add("R[??0nCompound Long Bow+1")
        cmbbox.Items.Add("R[??0nCompound Long Bow+2")
        cmbbox.Items.Add("R[??0nCompound Long Bow+3")
        cmbbox.Items.Add("R[??0nCompound Long Bow+4")
        cmbbox.Items.Add("R[??0nCompound Long Bow+5")
        cmbbox.Items.Add("Sticky Compound Long Bow+1")
        cmbbox.Items.Add("Sticky Compound Long Bow+2")
        cmbbox.Items.Add("Sticky Compound Long Bow+3")
        cmbbox.Items.Add("Sticky Compound Long Bow+4")
        cmbbox.Items.Add("Sticky Compound Long Bow+5")
        cmbbox.Items.Add("Quality Compound Long Bow+1")
        cmbbox.Items.Add("Quality Compound Long Bow+2")
        cmbbox.Items.Add("Quality Compound Long Bow+3")
        cmbbox.Items.Add("Quality Compound Long Bow+4")
        cmbbox.Items.Add("Quality Compound Long Bow+5")
        cmbbox.Items.Add("White Bow")
        cmbbox.Items.Add("White Bow+1")
        cmbbox.Items.Add("White Bow+2")
        cmbbox.Items.Add("White Bow+3")
        cmbbox.Items.Add("White Bow+4")
        cmbbox.Items.Add("White Bow+5")
        cmbbox.Items.Add("Lava Bow")
        cmbbox.Items.Add("Lava Bow")
        cmbbox.Items.Add("Lava Bow")
        cmbbox.Items.Add("Lava Bow")
        cmbbox.Items.Add("_?_?_    (Ghost Short Bow)")
        cmbbox.Items.Add("Light Crossbow")
        cmbbox.Items.Add("Heavy Crossbow")
        cmbbox.Items.Add("Gargoyle Crossbow")
        cmbbox.Items.Add("_?_?_)    (Ghost Crossbow)")
        cmbbox.Items.Add("Buckler")
        cmbbox.Items.Add("Buckler+1")
        cmbbox.Items.Add("Buckler+2")
        cmbbox.Items.Add("Buckler+3")
        cmbbox.Items.Add("Buckler+4")
        cmbbox.Items.Add("Buckler+5")
        cmbbox.Items.Add("Buckler+6")
        cmbbox.Items.Add("Buckler+7")
        cmbbox.Items.Add("Buckler+8")
        cmbbox.Items.Add("Buckler+9")
        cmbbox.Items.Add("Buckler+10")
        cmbbox.Items.Add("0o0X0OBuckler+1")
        cmbbox.Items.Add("0o0X0OBuckler+2")
        cmbbox.Items.Add("0o0X0OBuckler+3")
        cmbbox.Items.Add("0o0X0OBuckler+4")
        cmbbox.Items.Add("0o0X0OBuckler+5")
        cmbbox.Items.Add("Dark Buckler+1")
        cmbbox.Items.Add("Dark Buckler+2")
        cmbbox.Items.Add("Dark Buckler+3")
        cmbbox.Items.Add("Dark Buckler+4")
        cmbbox.Items.Add("Dark Buckler+5")
        cmbbox.Items.Add("Wooden Shield")
        cmbbox.Items.Add("Kite Shield")
        cmbbox.Items.Add("Kite Shield+1")
        cmbbox.Items.Add("Kite Shield+2")
        cmbbox.Items.Add("Kite Shield+3")
        cmbbox.Items.Add("Kite Shield+4")
        cmbbox.Items.Add("Kite Shield+5")
        cmbbox.Items.Add("Kite Shield+6")
        cmbbox.Items.Add("Kite Shield+7")
        cmbbox.Items.Add("Kite Shield+8")
        cmbbox.Items.Add("Kite Shield+9")
        cmbbox.Items.Add("Kite Shield+10")
        cmbbox.Items.Add("0o0X0OKite Shield+1")
        cmbbox.Items.Add("0o0X0OKite Shield+2")
        cmbbox.Items.Add("0o0X0OKite Shield+3")
        cmbbox.Items.Add("0o0X0OKite Shield+4")
        cmbbox.Items.Add("0o0X0OKite Shield+5")
        cmbbox.Items.Add("Dark Kite Shield+1")
        cmbbox.Items.Add("Dark Kite Shield+2")
        cmbbox.Items.Add("Dark Kite Shield+3")
        cmbbox.Items.Add("Dark Kite Shield+4")
        cmbbox.Items.Add("Dark Kite Shield+5")
        cmbbox.Items.Add("Heater Shield")
        cmbbox.Items.Add("Heater Shield+1")
        cmbbox.Items.Add("Heater Shield+2")
        cmbbox.Items.Add("Heater Shield+3")
        cmbbox.Items.Add("Heater Shield+4")
        cmbbox.Items.Add("Heater Shield+5")
        cmbbox.Items.Add("Heater Shield+6")
        cmbbox.Items.Add("Heater Shield+7")
        cmbbox.Items.Add("Heater Shield+8")
        cmbbox.Items.Add("Heater Shield+9")
        cmbbox.Items.Add("Heater Shield+10")
        cmbbox.Items.Add("0o0X0OHeater Shield+1")
        cmbbox.Items.Add("0o0X0OHeater Shield+2")
        cmbbox.Items.Add("0o0X0OHeater Shield+3")
        cmbbox.Items.Add("0o0X0OHeater Shield+4")
        cmbbox.Items.Add("0o0X0OHeater Shield+5")
        cmbbox.Items.Add("Dark Heater Shield+1")
        cmbbox.Items.Add("Dark Heater Shield+2")
        cmbbox.Items.Add("Dark Heater Shield+3")
        cmbbox.Items.Add("Dark Heater Shield+4")
        cmbbox.Items.Add("Dark Heater Shield+5")
        cmbbox.Items.Add("Adjudicator's Shield")
        cmbbox.Items.Add("Adjudicator's Shield+1")
        cmbbox.Items.Add("Adjudicator's Shield+2")
        cmbbox.Items.Add("Adjudicator's Shield+3")
        cmbbox.Items.Add("Adjudicator's Shield+4")
        cmbbox.Items.Add("Adjudicator's Shield+5")
        cmbbox.Items.Add("0o0X0OAdjudicator's Shield+1")
        cmbbox.Items.Add("0o0X0OAdjudicator's Shield+2")
        cmbbox.Items.Add("0o0X0OAdjudicator's Shield+3")
        cmbbox.Items.Add("0o0X0OAdjudicator's Shield+4")
        cmbbox.Items.Add("0o0X0OAdjudicator's Shield+5")
        cmbbox.Items.Add("Spiked Shield")
        cmbbox.Items.Add("Spiked Shield+1")
        cmbbox.Items.Add("Spiked Shield+2")
        cmbbox.Items.Add("Spiked Shield+3")
        cmbbox.Items.Add("Spiked Shield+4")
        cmbbox.Items.Add("Spiked Shield+5")
        cmbbox.Items.Add("Spiked Shield+6")
        cmbbox.Items.Add("Spiked Shield+7")
        cmbbox.Items.Add("Spiked Shield+8")
        cmbbox.Items.Add("Spiked Shield+9")
        cmbbox.Items.Add("Spiked Shield+10")
        cmbbox.Items.Add("0o0X0OSpiked Shield+1")
        cmbbox.Items.Add("0o0X0OSpiked Shield+2")
        cmbbox.Items.Add("0o0X0OSpiked Shield+3")
        cmbbox.Items.Add("0o0X0OSpiked Shield+4")
        cmbbox.Items.Add("0o0X0OSpiked Shield+5")
        cmbbox.Items.Add("Sharp Spiked Shield+1")
        cmbbox.Items.Add("Sharp Spiked Shield+2")
        cmbbox.Items.Add("Sharp Spiked Shield+3")
        cmbbox.Items.Add("Sharp Spiked Shield+4")
        cmbbox.Items.Add("Sharp Spiked Shield+5")
        cmbbox.Items.Add("Tower Shield")
        cmbbox.Items.Add("Tower Shield+1")
        cmbbox.Items.Add("Tower Shield+2")
        cmbbox.Items.Add("Tower Shield+3")
        cmbbox.Items.Add("Tower Shield+4")
        cmbbox.Items.Add("Tower Shield+5")
        cmbbox.Items.Add("Dark Silver Shield")
        cmbbox.Items.Add("Dark Silver Shield+1")
        cmbbox.Items.Add("Dark Silver Shield+2")
        cmbbox.Items.Add("Dark Silver Shield+3")
        cmbbox.Items.Add("Dark Silver Shield+4")
        cmbbox.Items.Add("Dark Silver Shield+5")
        cmbbox.Items.Add("Soldier's Shield")
        cmbbox.Items.Add("Soldier's Shield+1")
        cmbbox.Items.Add("Soldier's Shield+2")
        cmbbox.Items.Add("Soldier's Shield+3")
        cmbbox.Items.Add("Soldier's Shield+4")
        cmbbox.Items.Add("Soldier's Shield+5")
        cmbbox.Items.Add("Soldier's Shield+6")
        cmbbox.Items.Add("Soldier's Shield+7")
        cmbbox.Items.Add("Soldier's Shield+8")
        cmbbox.Items.Add("Soldier's Shield+9")
        cmbbox.Items.Add("Soldier's Shield+10")
        cmbbox.Items.Add("Dark Soldier's Shield+1")
        cmbbox.Items.Add("Dark Soldier's Shield+2")
        cmbbox.Items.Add("Dark Soldier's Shield+3")
        cmbbox.Items.Add("Dark Soldier's Shield+4")
        cmbbox.Items.Add("Dark Soldier's Shield+5")
        cmbbox.Items.Add("Knight's Shield")
        cmbbox.Items.Add("Knight's Shield+1")
        cmbbox.Items.Add("Knight's Shield+2")
        cmbbox.Items.Add("Knight's Shield+3")
        cmbbox.Items.Add("Knight's Shield+4")
        cmbbox.Items.Add("Knight's Shield+5")
        cmbbox.Items.Add("Knight's Shield+6")
        cmbbox.Items.Add("Knight's Shield+7")
        cmbbox.Items.Add("Knight's Shield+8")
        cmbbox.Items.Add("Knight's Shield+9")
        cmbbox.Items.Add("Knight's Shield+10")
        cmbbox.Items.Add("0o0X0OKnight's Shield+1")
        cmbbox.Items.Add("0o0X0OKnight's Shield+2")
        cmbbox.Items.Add("0o0X0OKnight's Shield+3")
        cmbbox.Items.Add("0o0X0OKnight's Shield+4")
        cmbbox.Items.Add("0o0X0OKnight's Shield+5")
        cmbbox.Items.Add("Dark Knight's Shield+1")
        cmbbox.Items.Add("Dark Knight's Shield+2")
        cmbbox.Items.Add("Dark Knight's Shield+3")
        cmbbox.Items.Add("Dark Knight's Shield+4")
        cmbbox.Items.Add("Dark Knight's Shield+5")
        cmbbox.Items.Add("Slave's Shield")
        cmbbox.Items.Add("Rune Shield")
        cmbbox.Items.Add("Rune Shield+1")
        cmbbox.Items.Add("Rune Shield+2")
        cmbbox.Items.Add("Rune Shield+3")
        cmbbox.Items.Add("Rune Shield+4")
        cmbbox.Items.Add("Rune Shield+5")
        cmbbox.Items.Add("Large Brushwood Shield")
        cmbbox.Items.Add("Large Brushwood Shield+1")
        cmbbox.Items.Add("Large Brushwood Shield+2")
        cmbbox.Items.Add("Large Brushwood Shield+3")
        cmbbox.Items.Add("Large Brushwood Shield+4")
        cmbbox.Items.Add("Large Brushwood Shield+5")
        cmbbox.Items.Add("Steel Shield")
        cmbbox.Items.Add("Steel Shield+1")
        cmbbox.Items.Add("Steel Shield+2")
        cmbbox.Items.Add("Steel Shield+3")
        cmbbox.Items.Add("Steel Shield+4")
        cmbbox.Items.Add("Steel Shield+5")
        cmbbox.Items.Add("Steel Shield+6")
        cmbbox.Items.Add("Steel Shield+7")
        cmbbox.Items.Add("Steel Shield+8")
        cmbbox.Items.Add("Steel Shield+9")
        cmbbox.Items.Add("Steel Shield+10")
        cmbbox.Items.Add("Dark Steel Shield+1")
        cmbbox.Items.Add("Dark Steel Shield+2")
        cmbbox.Items.Add("Dark Steel Shield+3")
        cmbbox.Items.Add("Dark Steel Shield+4")
        cmbbox.Items.Add("Dark Steel Shield+5")
        cmbbox.Items.Add("Purple Flame Shield")
        cmbbox.Items.Add("Purple Flame Shield+1")
        cmbbox.Items.Add("Purple Flame Shield+2")
        cmbbox.Items.Add("Purple Flame Shield+3")
        cmbbox.Items.Add("Purple Flame Shield+4")
        cmbbox.Items.Add("Purple Flame Shield+5")
        cmbbox.Items.Add("Purple Flame Shield+6")
        cmbbox.Items.Add("Purple Flame Shield+7")
        cmbbox.Items.Add("Purple Flame Shield+8")
        cmbbox.Items.Add("Purple Flame Shield+9")
        cmbbox.Items.Add("Purple Flame Shield+10")
        cmbbox.Items.Add("Dark Purple Flame Shield+1")
        cmbbox.Items.Add("Dark Purple Flame Shield+2")
        cmbbox.Items.Add("Dark Purple Flame Shield+3")
        cmbbox.Items.Add("Dark Purple Flame Shield+4")
        cmbbox.Items.Add("Dark Purple Flame Shield+5")
        cmbbox.Items.Add("Leather Shield")
        cmbbox.Items.Add("_?_?v?    (Ghost Shield)")
        cmbbox.Items.Add("Arrow")
        cmbbox.Items.Add("Heavy Arrow")
        cmbbox.Items.Add("Light Arrow")
        cmbbox.Items.Add("0?0?0?0?0?0?0?    (Nothing loads)")
        cmbbox.Items.Add("Fire Arrow")
        cmbbox.Items.Add("Rotten Arrow")
        cmbbox.Items.Add("Holy Arrow")
        cmbbox.Items.Add("White Arrow")
        cmbbox.Items.Add("Wooden Arrow")
        cmbbox.Items.Add("_?_?w?{R    (Ghost Quiver)")
        cmbbox.Items.Add("Bolt")
        cmbbox.Items.Add("Heavy Bolt")
        cmbbox.Items.Add("Black Bolt")
        cmbbox.Items.Add("Wooden Bolt")
        cmbbox.Items.Add("---No Weapon---")
    End Sub
    Sub InitSpells(cmbbox As ComboBox)
        cmbbox.Items.Clear()
        cmbbox.Items.Add("Test")
        cmbbox.Items.Add("test_firebolt")
        cmbbox.Items.Add("test_enchantweapon")
        cmbbox.Items.Add("test_cure")
        cmbbox.Items.Add("test_SOSsign")
        cmbbox.Items.Add("Invoke Magic Sq.")
        cmbbox.Items.Add("Soul Arrow")
        cmbbox.Items.Add("Flame Toss")
        cmbbox.Items.Add("Relief")
        cmbbox.Items.Add("Enchant Weapon")
        cmbbox.Items.Add("Curse Weapon")
        cmbbox.Items.Add("Soul Thirst")
        cmbbox.Items.Add("Poison Cloud")
        cmbbox.Items.Add("Demon's Prank")
        cmbbox.Items.Add("Fireball")
        cmbbox.Items.Add("Ignite")
        cmbbox.Items.Add("Soul Ray")
        cmbbox.Items.Add("Homing Soul Arrow")
        cmbbox.Items.Add("Cloak")
        cmbbox.Items.Add("Protection")
        cmbbox.Items.Add("Light Weapon")
        cmbbox.Items.Add("Water Veil")
        cmbbox.Items.Add("Death Cloud")
        cmbbox.Items.Add("Fire Spray")
        cmbbox.Items.Add("Soulsucker")
        cmbbox.Items.Add("Acid Cloud")
        cmbbox.Items.Add("Warding")
        cmbbox.Items.Add("Firestorm")
        cmbbox.Items.Add("God's Wrath")
        cmbbox.Items.Add("Anti-Magic Field")
        cmbbox.Items.Add("Recovery")
        cmbbox.Items.Add("Second Chance")
        cmbbox.Items.Add("Regeneration")
        cmbbox.Items.Add("Resurrection")
        cmbbox.Items.Add("Cure")
        cmbbox.Items.Add("Hidden Soul")
        cmbbox.Items.Add("Evacuate")
        cmbbox.Items.Add("Banish")
        cmbbox.Items.Add("Heal")
        cmbbox.Items.Add("Antidote")
    End Sub

    Sub dgvBuild(ByRef dgv As DataGridView, cmb As ComboBox, name As String)

        Dim dgvcmb As New DataGridViewComboBoxColumn

        dgvcmb.HeaderText = name
        dgvcmb.Name = name

        For i = 0 To cmb.Items.Count - 1
            dgvcmb.Items.Add(cmb.Items(i))
        Next

        dgv.Columns.Add(dgvcmb)

        dgv.Columns(0).Width = 250
        dgv.Columns.Add("Count", "Count")
        dgv.Columns(1).Width = 50
        dgv.Columns.Add("Misc1", "Misc1")
        dgv.Columns(2).Width = 100
        dgv.Columns.Add("Misc2", "Misc2")
        dgv.Columns(3).Width = 100
        dgv.Columns.Add("Misc3", "Misc3")
        dgv.Columns(4).Width = 100
    End Sub
    Sub dgvSpellBuild(ByRef dgv As DataGridView, cmb As ComboBox, name As String)

        Dim dgvcmb As New DataGridViewComboBoxColumn
        Dim dgvcmb2 As New DataGridViewComboBoxColumn

        dgvcmb.HeaderText = name
        dgvcmb.Name = name

        dgvcmb2.HeaderText = "Status"
        dgvcmb2.Name = "Status"

        dgvcmb2.Items.Add("Unavailable")
        dgvcmb2.Items.Add("Unknown")
        dgvcmb2.Items.Add("Known")
        dgvcmb2.Items.Add("Memorized")

        For i = 0 To cmb.Items.Count - 1
            dgvcmb.Items.Add(cmb.Items(i))
        Next

        dgv.Columns.Add(dgvcmb)
        dgv.Columns(0).Width = 250
        dgv.Columns.Add(dgvcmb2)
        dgv.Columns(1).Width = 150
        dgv.Columns.Add("Misc1", "Misc1")
        dgv.Columns(2).Width = 100
        dgv.Columns.Add("Misc2", "Misc2")
        dgv.Columns(3).Width = 100
    End Sub

    Private Sub DeS_Load(sender As Object, e As EventArgs) Handles MyBase.Load


        InitArrays()

        InitWeaps(cmbLeftHand1)
        InitWeaps(cmbLeftHand2)
        InitWeaps(cmbRightHand1)
        InitWeaps(cmbRightHand2)
        InitWeaps(cmbArrows)
        InitWeaps(cmbBolts)
        InitSpells(cmbSpellSlot1)

        cllSpellStatus = {"Unavailable", "Unknown", "Known", "Memorized"}


        dgvBuild(dgvWeapons, cmbLeftHand1, "Weapons")
        dgvBuild(dgvArmor, cmbChest, "Armor")
        dgvBuild(dgvRings, cmbRing1, "Rings")
        dgvBuild(dgvGoods, cmbQuickSlot1, "Goods")
        dgvSpellBuild(dgvSpells, cmbSpellSlot1, "Spells/Miracles")

    End Sub

    Private Sub dgv_DefaultValuesNeeded(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowEventArgs) _
    Handles dgvWeapons.DefaultValuesNeeded, dgvArmor.DefaultValuesNeeded, dgvRings.DefaultValuesNeeded, _
    dgvGoods.DefaultValuesNeeded


        With e.Row
            .Cells("Count").Value = 1
            .Cells("Misc1").Value = 0
            .Cells("Misc2").Value = 0
            .Cells("Misc3").Value = &H1000000
        End With

    End Sub
    Private Sub dgvSpells_DefaultValuesNeeded(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowEventArgs) _
Handles dgvSpells.DefaultValuesNeeded
        With e.Row
            .Cells("Status").Value = "Unavailable"
            .Cells("Misc1").Value = 0
            .Cells("Misc2").Value = 0
        End With

    End Sub


    Private Sub txt_Drop(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles txtDeSFolder.DragDrop
        Dim file() As String = e.Data.GetData(DataFormats.FileDrop)
        sender.Text = Microsoft.VisualBasic.Left(file(0), InStrRev(file(0), "\"))
    End Sub
    Private Sub txt_DragEnter(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles txtDeSFolder.DragEnter
        e.Effect = DragDropEffects.Copy
    End Sub
End Class


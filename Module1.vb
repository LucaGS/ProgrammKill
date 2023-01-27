Imports System.Runtime.InteropServices
Imports Newtonsoft.Json
Imports System.IO
Imports Newtonsoft.Json.Linq
Imports System.Security.Principal
Imports System.Diagnostics
Module Program


    <DllImport("advapi32.dll", SetLastError:=True)>
    Public Function LogonUser(ByVal lpszUsername As String, ByVal lpszDomain As String, ByVal lpszPassword As String, ByVal dwLogonType As Integer, ByVal dwLogonProvider As Integer, ByRef phToken As IntPtr) As Boolean
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Public Function CloseHandle(ByVal hObject As IntPtr) As Boolean
    End Function

    Public Const LOGON32_LOGON_INTERACTIVE As Integer = 2
    Public Const LOGON32_PROVIDER_DEFAULT As Integer = 0

    Sub Main()
        Dim jsonString As String = File.ReadAllText("userData.json")
        Dim jsonData As JObject = JObject.Parse(jsonString)
        ' Benutzername, Passwort und Domain des Administrators
        Dim username As String = jsonData("username").ToString()
        Dim password As String = jsonData("password").ToString()
        Dim domain As String = jsonData("domain").ToString()

        Dim token As IntPtr = IntPtr.Zero
        ' Administrator-Anmeldung
        If LogonUser(username, domain, password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, token) Then
            'Code, der mit Administratorrechten ausgeführt werden soll
            Console.WriteLine("Hello Admin")
            Dim jsonProcessData As String = File.ReadAllText("processes.json")
            Dim processNames As List(Of String) = JsonConvert.DeserializeObject(Of List(Of String))(jsonProcessData)
            For Each prcessName In processNames
                KillProcessByName(prcessName)
            Next

            'Abmelden
            CloseHandle(token)
        Else
            'Fehlerbehandlung
            Console.WriteLine("LogonUser failed with error code: {0}", Marshal.GetLastWin32Error())
        End If

        Console.ReadKey()
        'impersonationContext.Dispose()
    End Sub
    Private Sub KillProcessByName(name As String)
        For Each p In Process.GetProcesses
            Console.WriteLine(p)
            If p.ProcessName.ToLower().Contains(name.ToLower()) Then
                If Not p.HasExited Then
                    Console.WriteLine(p)
                    p.Kill()
                End If
            End If
        Next
    End Sub



End Module


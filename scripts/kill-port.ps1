<#
.SYNOPSIS
    Kills whatever process is listening on the given port(s), or by process name (wildcard supported).
    Run with no arguments to list every listening port so you can find the right one.

.EXAMPLE
    ./scripts/kill-port.ps1                          # list all listening ports
    ./scripts/kill-port.ps1 -Port 7012
    ./scripts/kill-port.ps1 -Port 7012,5095,7196
    ./scripts/kill-port.ps1 -Name OnlineConsulting*   # wildcard, matches Api/AppHost/UserInterface
    ./scripts/kill-port.ps1 -Aspire                   # kills all known Aspire-related processes
#>
param(
    [int[]]$Port,
    [string[]]$Name,
    [switch]$Aspire
)

if (-not $Port -and -not $Name -and -not $Aspire) {
    Get-NetTCPConnection -State Listen -ErrorAction SilentlyContinue |
        Select-Object LocalPort, OwningProcess,
            @{Name = "ProcessName"; Expression = { (Get-Process -Id $_.OwningProcess -ErrorAction SilentlyContinue).ProcessName } } |
        Sort-Object LocalPort |
        Format-Table -AutoSize
    Write-Host "`nRun again with -Port <number>, -Name <pattern>, or -Aspire to kill."
    return
}

if ($Aspire) {
    $Name = @("dcp", "aspire-managed", "OnlineConsulting*")
}

if ($Name) {
    foreach ($pattern in $Name) {
        $procs = Get-Process -Name $pattern -ErrorAction SilentlyContinue
        if (-not $procs) {
            Write-Host "No process matching '$pattern'"
            continue
        }
        foreach ($proc in $procs) {
            Write-Host "Killing $($proc.ProcessName) (PID $($proc.Id))"
            Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
        }
    }
}

foreach ($p in $Port) {
    $pids = Get-NetTCPConnection -LocalPort $p -ErrorAction SilentlyContinue |
        Select-Object -ExpandProperty OwningProcess -Unique

    if (-not $pids) {
        Write-Host "Port $p - nothing listening"
        continue
    }

    foreach ($processId in $pids) {
        $proc = Get-Process -Id $processId -ErrorAction SilentlyContinue
        if ($proc) {
            Write-Host "Port $p - killing $($proc.ProcessName) (PID $processId)"
            Stop-Process -Id $processId -Force
        }
    }
}

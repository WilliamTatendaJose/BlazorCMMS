# Apply Migration Fix to Azure SQL Database
# This script executes the QUICK_FIX_MIGRATIONS.sql file against your Azure database

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Azure SQL Migration Fix Script" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

$ServerName = "techrehub-sql.database.windows.net"
$DatabaseName = "blazor-rbcmms"
$Username = "wjose@techrehub.co.zw@techrehub-sql"
$Password = "@Jose211704117"
$SqlFile = ".\Migrations\QUICK_FIX_MIGRATIONS.sql"

Write-Host "Connecting to:" -ForegroundColor Yellow
Write-Host "  Server: $ServerName" -ForegroundColor Gray
Write-Host "  Database: $DatabaseName" -ForegroundColor Gray
Write-Host ""

# Read the SQL file
if (-not (Test-Path $SqlFile)) {
    Write-Host "ERROR: SQL file not found at $SqlFile" -ForegroundColor Red
    exit 1
}

$sqlContent = Get-Content $SqlFile -Raw

Write-Host "Executing migration fix..." -ForegroundColor Yellow
Write-Host ""

try {
    # Using SqlClient to execute the script
    Add-Type -AssemblyName "System.Data"
    
    $connectionString = "Server=tcp:$ServerName,1433;Initial Catalog=$DatabaseName;Persist Security Info=False;User ID=$Username;Password=$Password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
    
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    Write-Host "✓ Connected to database" -ForegroundColor Green
    
    # Split SQL file by GO statements and execute each batch
    $batches = $sqlContent -split '\r?\nGO\r?\n'
    
    $batchNumber = 0
    foreach ($batch in $batches) {
        if ($batch.Trim() -ne "") {
            $batchNumber++
            
            $command = New-Object System.Data.SqlClient.SqlCommand($batch, $connection)
            $command.CommandTimeout = 120
            
            $reader = $command.ExecuteReader()
            
            # Output any messages
            while ($reader.Read()) {
                for ($i = 0; $i -lt $reader.FieldCount; $i++) {
                    Write-Host $reader.GetValue($i)
                }
            }
            
            # Output any info messages
            $connection.FireInfoMessageEventOnUserErrors = $true
            $connection.add_InfoMessage({
                param($sender, $event)
                Write-Host $event.Message -ForegroundColor Cyan
            })
            
            $reader.Close()
        }
    }
    
    $connection.Close()
    
    Write-Host ""
    Write-Host "=====================================" -ForegroundColor Green
    Write-Host "✓ Migration fix completed successfully!" -ForegroundColor Green
    Write-Host "=====================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "1. Run: dotnet ef database update" -ForegroundColor White
    Write-Host "2. Start your Blazor application" -ForegroundColor White
    Write-Host "3. The error should be resolved!" -ForegroundColor White
    Write-Host ""
    
} catch {
    Write-Host ""
    Write-Host "ERROR: Failed to execute migration fix" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Write-Host "You can manually run the SQL file:" -ForegroundColor Yellow
    Write-Host "1. Open Azure Data Studio or SQL Server Management Studio" -ForegroundColor White
    Write-Host "2. Connect to: $ServerName" -ForegroundColor White
    Write-Host "3. Open file: $SqlFile" -ForegroundColor White
    Write-Host "4. Execute the script" -ForegroundColor White
    Write-Host ""
    exit 1
}

# Seeds the platform's own content through the Api: module offerings, bundles, landing copy, landing blocks and sector categories.
# Re-runnable: existing offerings (by key), bundles (by name) and blocks (by section + title) are skipped; sector categories (by title) are upserted;
# Landing copy and About Us are created only when missing, so edits made in the admin panel are never overwritten.
# Usage: .\scripts\seed-content\Seed-PlatformContent.ps1 -ApiBaseUrl https://comfortxpro-api.up.railway.app -Email platformowner@onlineconsulting.test
param(
    [Parameter(Mandatory = $true)][string]$ApiBaseUrl,
    [Parameter(Mandatory = $true)][string]$Email,
    [string]$CatalogPath = (Join-Path $PSScriptRoot 'platform-catalog.json'),
    [string]$LandingPath = (Join-Path $PSScriptRoot 'platform-landing.json'),
    [string]$CategoriesPath = (Join-Path $PSScriptRoot 'platform-categories.json'),
    [securestring]$Password
)

$ErrorActionPreference = 'Stop'
$api = "$($ApiBaseUrl.TrimEnd('/'))/api/v1"
$catalog = Get-Content $CatalogPath -Raw -Encoding UTF8 | ConvertFrom-Json
$landing = Get-Content $LandingPath -Raw -Encoding UTF8 | ConvertFrom-Json
$sectors = Get-Content $CategoriesPath -Raw -Encoding UTF8 | ConvertFrom-Json

# Password is read at runtime only, never stored.
$securePassword = if ($Password) { $Password } else { Read-Host "Password for $Email" -AsSecureString }
$plainPassword = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword))

$login = Invoke-RestMethod -Method Post -Uri "$api/auth/login" -ContentType 'application/json' `
    -Body (@{ userNameOrEmail = $Email; password = $plainPassword } | ConvertTo-Json)
$headers = @{ Authorization = "Bearer $($login.resultData.accessToken)" }

function Get-AdminItems([string]$path) {
    (Invoke-RestMethod -Uri "$api/$path`?index=0&size=100" -Headers $headers).resultData.items
}

function Send-Json([string]$method, [string]$path, $body) {
    $json = $body | ConvertTo-Json -Depth 5
    Invoke-RestMethod -Method $method -Uri "$api/$path" -Headers $headers -ContentType 'application/json; charset=utf-8' `
        -Body ([Text.Encoding]::UTF8.GetBytes($json)) | Out-Null
}

$existingKeys = @(Get-AdminItems 'tenancy/admin/module-offerings' | ForEach-Object { $_.key })
foreach ($offering in $catalog.moduleOfferings) {
    if ($existingKeys -contains $offering.key) { Write-Host "skip  offering $($offering.key)"; continue }
    Send-Json Post 'tenancy/admin/module-offerings' $offering
    Write-Host "added offering $($offering.key) ($($offering.price) / $($offering.billingCycle))"
}

$existingBundles = @(Get-AdminItems 'tenancy/admin/bundles' | ForEach-Object { $_.name })
foreach ($bundle in $catalog.bundles) {
    if ($existingBundles -contains $bundle.name) { Write-Host "skip  bundle $($bundle.name)"; continue }
    Send-Json Post 'tenancy/admin/bundles' $bundle
    Write-Host "added bundle $($bundle.name)"
}

# Landing copy is created once; later edits (and pictures) belong to the admin panel.
$landingExists = $true
try { Invoke-RestMethod -Uri "$api/site-content/landing-page" | Out-Null } catch { $landingExists = $false }
if ($landingExists) { Write-Host "skip  landing page (already set)" }
else {
    Send-Json Put 'site-content/landing-page' $landing.landingPage
    Write-Host "added landing page"
}

$existingBlocks = @((Invoke-RestMethod -Uri "$api/site-content/landing-blocks" -Headers $headers).resultData | ForEach-Object { "$($_.section)|$($_.title)" })
foreach ($block in $landing.landingBlocks) {
    if ($existingBlocks -contains "$($block.section)|$($block.title)") { Write-Host "skip  block $($block.section): $($block.title)"; continue }
    Send-Json Post 'site-content/landing-blocks' $block
    Write-Host "added block $($block.section): $($block.title)"
}

$existingAbout = @((Invoke-RestMethod -Uri "$api/site-content/about-us" -Headers $headers).resultData)
if ($existingAbout.Count -gt 0) { Write-Host "skip  about us (already set)" }
else {
    Send-Json Post 'site-content/about-us' $landing.aboutUs
    Write-Host "added about us"
}

# Sector categories are upserted by title, so their description and icon follow the json.
$existingCategories = @{}
Get-AdminItems 'categories' | ForEach-Object { $existingCategories[$_.title] = $_.id }
foreach ($category in $sectors.categories) {
    if ($existingCategories.ContainsKey($category.title)) {
        Send-Json Put "categories/$($existingCategories[$category.title])" $category
        Write-Host "saved category $($category.title)"
        continue
    }
    Send-Json Post 'categories' $category
    Write-Host "added category $($category.title)"
}
